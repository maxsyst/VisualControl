using System;
namespace VueExample.Exceptions
{
    public class ValidationErrorException : Exception
    {
        public ValidationErrorException()
        {
            
        }

        public ValidationErrorException(string message) : base(message)
        {
            
        }
    }
}