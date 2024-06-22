using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;


namespace Ninject.Sandbox
{
    [GenerateFactory]
    public class InputSystem
    {
        public InputSystem(
            string name,
            [FromFactoryAttribute] Stream fileStream,
            [FromFactory] IUserInput userInput)
        {

        }

        public InputSystem(
            int age,
            [FromFactory] Stream stream,
            bool isEnabled)
        {

        }

    }
}
