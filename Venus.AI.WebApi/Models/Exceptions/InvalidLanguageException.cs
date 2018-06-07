using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Exceptions
{
    public class InvalidLanguageException : Exception
    {
        public InvalidLanguageException(string language) : base($"invalid language: {language}") { }
    }
}
