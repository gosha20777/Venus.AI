using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Exceptions
{
    public abstract class RequestException : Exception
    {
        public RequestException(long? id, string errorMsg) : base($"Id: {id}, Error: {errorMsg}")
        {
        }
    }
}
