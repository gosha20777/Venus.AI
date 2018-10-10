using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.SDK.Services
{
    interface IService
    {
        ServiceMessage Invork(ServiceMessage message);
    }
}
