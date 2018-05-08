using System;

namespace Vytals.Exceptions
{
    public class InvalidFacebookTokenException : Exception
    {
        public InvalidFacebookTokenException(string message) : base(message)
        {
        }
    }
}
