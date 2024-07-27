using AutoFactories;

namespace Ninject.Extension.AutoFactories.Sandbox
{
    [AutoFactory(typeof(AnimalFactory),"CreateDog")]
    public class Dog
    {
        public string Name { get; }

        public Dog(string name)
        {
            this.Name = name;
        }
    }


    [AutoFactory(typeof(AnimalFactory), "CreateCat")]
    public class Cat
    { 
    }

    // typeof(AnimalFactory) should be same as this 
    [AutoFactory(typeof(AnimalFactory), "CreateBird")]
    public class Bird
    {
        
    }

    public class Other
    {
        public Other(
            [FromFactory] string fileName)
        {
            
        }
    }

    public partial class AnimalFactory
    {
        public void DO()
        {
        }
    }
}
