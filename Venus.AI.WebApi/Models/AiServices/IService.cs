using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Venus.AI.WebApi.Models.Enums;

namespace Venus.AI.WebApi.Models.AiServices
{
    interface IService
    {
        void Initialize(Language language);
    }
}
