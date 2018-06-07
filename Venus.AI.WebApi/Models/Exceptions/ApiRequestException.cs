using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Exceptions
{
    public class ApiRequestException : RequestException
    {
        public ApiRequestException(long? id, Exception innerException) : base(id, innerException.Message)
        {
        }
    }
}
