using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;

namespace Venus.AI.WebApi.Models.Requests
{
    public abstract class Request
    {
        private long _id;
        public long? Id
        {
            get { return _id; }
            set
            {
                if (value != null && value >= 0)
                    _id = value.Value;
                else
                    throw new InvalidIdRequestExcrption(value);
            }
        }
    }
}
