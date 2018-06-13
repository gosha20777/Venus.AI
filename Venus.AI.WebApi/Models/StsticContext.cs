using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models
{
    public static class StsticContext
    {
        private static ApiAiSDK.RequestExtras _requestExtras;
        public static ApiAiSDK.RequestExtras GetContext()
        {
            if (_requestExtras == null)
                _requestExtras = new ApiAiSDK.RequestExtras();
            return _requestExtras;
        }
        public static void SetContext(ApiAiSDK.RequestExtras requestExtras)
        {
            _requestExtras = requestExtras;
        }
    }
}
