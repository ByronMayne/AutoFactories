using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Sandbox
{
    [GenerateFactory(FactoryFullyQualifiedName = "AnimalFactory", MethodName = "CreateDog")]
    public class Dog
    {
        public Dog()
        { }
    }

    [GenerateFactory(FactoryFullyQualifiedName = "AnimalFactory", MethodName = "CreateCat")]
    public class Cat
    {
        public StringComparison Comparsion { get; }

        public Cat([FromFactory] StringComparison comparison)
        {
            Comparsion = comparison;
        }
    }

    [GenerateFactory(FactoryFullyQualifiedName = "AnimalFactory", MethodName = "CreateFrog")]
    public class Frog
    {
        public int Age { get; }

        public Frog(int age)
        {
            Age = age;
        }
    }
}
