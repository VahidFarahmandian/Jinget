namespace Jinget.Core.ResponseResults.Messages
{
    public class CustomMessageModel
    {
        public string Error { get; }

        public CustomMessageModel(string message) => Error = message;
    }
}