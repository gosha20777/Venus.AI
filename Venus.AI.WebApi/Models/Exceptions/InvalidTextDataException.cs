using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Exceptions
{
    public class InvalidTextDataException : Exception
    {
        public InvalidTextDataException() : base("Invalid text data")
        {
        }
    }
}
