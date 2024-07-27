using Ninject;

namespace Ninject.Extension.AutoFactories.Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StandardKernel kernel = new StandardKernel();
                //.LoadFactories();


            //IAnimalFactory animalFacotry = kernel.Get<IAnimalFactory>();
            //Bird bird = animalFacotry.CreateBird();
            //Cat cat = animalFacotry.CreateCat();
            //Dog dog = animalFacotry.CreateDog("Rusty");


            Console.WriteLine("Hello, World!");
        }
    }
}
