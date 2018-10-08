using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.SDK.Components.Exceptions
{
    class InvalidMessageException : Exception
    {
        public InvalidMessageException(long id, string errorMsg) : base($"Id: {id}, Error: {errorMsg}")
        {
        }
    }
}
