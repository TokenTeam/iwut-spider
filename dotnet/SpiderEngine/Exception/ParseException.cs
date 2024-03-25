namespace SpiderEngine.Exception
{
    using System;

    public class ParseException : AggregateException
    {
        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParseException(string message, params Exception[] innerExceptions) : base(message, innerExceptions)
        {
        }
    }
}
