namespace SyndykApp.Model.Exceptions
{
    public class CalculationFailureException : Exception
    {
        public CalculationFailureException() : base() { }
        public CalculationFailureException(string message) : base(message) { }
        public CalculationFailureException(string message, Exception innerException) : base(message, innerException) { }
    }
}
