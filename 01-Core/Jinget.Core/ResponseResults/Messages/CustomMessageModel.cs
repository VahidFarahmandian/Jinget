namespace Jinget.Core.ResponseResults.Messages
{
    public class CustomMessageModel(string message)
    {
        public string Error { get; } = message;
    }
}