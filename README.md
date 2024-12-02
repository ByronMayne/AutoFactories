[![NuGet Version](http://img.shields.io/nuget/v/Ninject.Extension.AutoFactories.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extension.AutoFactories/) 
[![NuGet Downloads](http://img.shields.io/nuget/dt/Ninject.Extension.AutoFactories.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extension.AutoFactories/)
![Issues](https://img.shields.io/github/issues-closed/ByronMayne/Ninject.Extensions.AutoFactories)


## AutoFactories

This library is a [Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) that generates factory for types during the compilation  process. This removes the need to have to write uninteresting boilerplate code. You can use the standard version `Autofactories` or create factories for one of the supported third party libraries like [Ninject](https://github.com/ninject/Ninject) or [Microsoft.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection). Do you want support for another framwork, create a request. 

## Quick Example

This code below uses this library to auto generate the `IShippingOrderFactory` and it's `Get` method to create new instances of `ShippingOrder` with both user provider and DI provided parameters.

```cs
namespace Operations 
{
    [AutoFactory] 
    public class ShippingOrder 
    {
        public ShippingOrder(
            string orderId,
            [FromFactory] IShippingProvider provider)
        {}
    }
    public class Program 
    {
        public static void Main(string[] args)
        {
            IShippingProvider provider = new ShippingProvider();
            // The factory is generated, the constructor will take all
            //  parameters that are marked with 'FromFactory' 
            IShippingOrderFactory factory = new ShippingOrderFactory(provider);
            // The `Get` method will take all parameters not marked with 'FromFactory'
            ShippingOrder shippingOrder = factory.Get(orderId:"A2123F")
        }
    }
}
```

You can also use a third party library instead of the generic factory. 

### Ninject
```cs
public static void Main(string[] args)
{
    IKernel kernel = new StandardKernel()
        .AddAutoFactories(); // Adds the required view
    IShippingOrderFactory factory = kernel.Get<ShippingOrderFactory>();
    ShippingOrder shippingOrder = factory.Get(orderId:"A2123F")
}
```

### Microsoft.DependencyInjection
```cs
using Microsoft.Extensions.DependencyInjection;

public static void Main(string[] args)
{
    IServiceProvider serviceProvider = new ServiceCollection()
        .AddAutoFactories() // Adds the generated factories
        .BuildServiceProvider();

    IShippingOrderFactory factory =  serviceProvider.GetRequiredService<ShippingOrderFactory>();
    ShippingOrder shippingOrder = factory.Get(orderId:"A2123F")
}
```

## Introduction
The best practice for using a Dependency Injection (DI) container in a software system is to restrict direct interaction with the container to the Composition Root.

However, many applications face the challenge of not being able to instantiate all dependencies at startup or at the beginning of each  request, due to incomplete information at those times. These applications require a method to create new instances later using the Kernel. This should be done without referencing the container's types outside of the Composition Root. This is where factories come in.


Lets say we have this the following class: 

```csharp
public class Coffee
{
    private IMilkService m_milkService;

    public Coffee(IMilkService milkService)
    {
        m_milkService = milkService;
    }
}
```
If we wanted to have the ability to create new `Coffee` we would create a factory:

```csharp 
public class CoffeeFactory
{
    private IKernel m_kernel;

    public CoffeeFactory(IKernel kernel)
    {
        m_kernel = kernel;
    }

    public Foo Create()
        => m_kernel.Get<Foo>():
}
```

Having this factory means that the `IMilkService` would be resolved by the `Kernel` and we don't have to worry about passing it in. 

If we wanted to add a user parameter like `Size` we could update the factory to reflect this.

```csharp 
using Ninject.Parameters;

public class Coffee
{
    public Size Size { get; }
    private IMilkService m_milkService;

    public Coffee(IMilkService milkService, Size size)
    {
        Size = size;
        m_milkService = milkService;
    }
}


public class CoffeeFactory
{
    private IKernel m_kernel;

    public CoffeeFactory(IKernel kernel)
    {
        m_kernel = kernel;
    }

    public Foo Create(Size size)
    {
        return m_kernel.Get<Foo>(new IParameter[] {
            new ConstructorParameter("size", size)
        }):
    }
}
```
There is a problem here and is the hard coded parameter `size`. With Ninject to provide external parameters you need the exact parameter, `size`. If someone were to go in in change `size` to `coffeeSize` this call would throw a runtime exception. This is what this library tries to do by making this a compile time error instead.   

## How It Works

To generate factories for your types you apply the `[AutoFactory]` attribute. A new factory will be generated for ever class that has this attribute. For every single constructor a `Create{ClassName}` method will be generated as well. Any parameters that don't have `[FromFactory]` attribute applied will be arguments of the generated `Create` methods. For example 

```csharp
[AutoFactory]
public class Coffee
{
    public Size Size { get; }
    public IMilkService Milk { get; }

    public Coffee(
        Size size,
        [FromFactory] IMilkService milkService)
    {}
}
```
Would generate:
```csharp
internal partial class CoffeeFactory : ICoffeeFactory 
{
    private readonly IResolutionRoot m_resolutionRoot;

    public CoffeeFactory(IResolutionRoot resolutionRoot)
    {
        m_resolutionRoot = resolutionRoot;
    }

    public CreateCoffee(Size size)
    {
        IParamater[] parameters = new IParameter[] 
        {
            new ConstructorParameter(nameof(size), size)
        };
        return m_resolutionRoot.Get<Coffee>();
    }
}
```

All the factories are registered in the generate `Ninject.FactoriesModule` which you need to register to your `IKernel`. To do this use the extension method.

```csharp
IKernel kernel = new StandardKernel();
kernel.LoadFactories();
ICoffeeFactory coffeeFactory = kernel.Get<ICoffeeFactory>();
```

