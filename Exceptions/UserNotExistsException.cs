using System;

namespace Vytals.Exceptions
{
    public class UserNotExistsException : Exception
    {
        public UserNotExistsException(string message) : base(message) { }
    }
}
