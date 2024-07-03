using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extension.AutoFactories.Sandbox
{
    [GenerateFactory(typeof(AnimalFactory),"CreateDog")]
    public class Dog
    {
        public string Name { get; }

        public Dog(string name)
        {
            this.Name = name;
        }
    }

    [GenerateFactory(typeof(AnimalFactory), "CreateCat")]
    public class Cat
    { 
    }

    // typeof(AnimalFactory) should be same as this 
    [GenerateFactory("Ninject.Extension.AutoFactories.Sandbox.AnimalFactory", "CreateBird")]
    public class Bird
    {
        
    }

    public partial class AnimalFactory
    {
        public void DO()
        {
        }
    }
}
