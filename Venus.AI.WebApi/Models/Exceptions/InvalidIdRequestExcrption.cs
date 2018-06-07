using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Exceptions
{
    public class InvalidIdRequestExcrption : RequestException
    {
        public InvalidIdRequestExcrption(long? id) : base(id, "invalid Id")
        {
        }
    }
}
