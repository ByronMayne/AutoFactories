using AutoFactories;


public class HumanFactory
{

}


[AutoFactory(typeof(HumanFactory), "Create")]
internal class Human
{

}