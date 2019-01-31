using System;

namespace HearthStoneSimCore.Exceptions
{
    /// <summary>
    /// General exception used within the library.
    /// </summary>
    /// <seealso cref="Exception" />
    public abstract class HearthStoneSimExceptions : Exception
    {
        protected HearthStoneSimExceptions() { }

        protected HearthStoneSimExceptions(string message) : base(message) { }

        protected HearthStoneSimExceptions(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ZoneException : HearthStoneSimExceptions
    {
        public ZoneException() { }

        public ZoneException(string message) : base(message) { }

        public ZoneException(string message, Exception innerException) : base(message, innerException) { }
    }
}