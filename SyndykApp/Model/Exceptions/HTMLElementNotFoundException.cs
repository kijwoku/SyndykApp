namespace SyndykApp.Model.Exceptions
{
    public class HTMLElementNotFoundException : Exception
    {
        public HTMLElementNotFoundException() : base() { }
        public HTMLElementNotFoundException(string message) : base(message) { }
        public HTMLElementNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
