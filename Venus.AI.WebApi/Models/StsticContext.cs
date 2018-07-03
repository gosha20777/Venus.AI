using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models
{
    public static class StsticContext
    {
        private static Dictionary<long, ApiAiSDK.RequestExtras> _requestExtras = new Dictionary<long, ApiAiSDK.RequestExtras>();
        public static ApiAiSDK.RequestExtras GetContext(long id)
        {
            if (_requestExtras.TryGetValue(id, out ApiAiSDK.RequestExtras val))
                return val;
            else
            {
                val = new ApiAiSDK.RequestExtras(); 
                _requestExtras.Add(id, val);
                return val;
            }
                
        }
        public static void SetContext(ApiAiSDK.RequestExtras requestExtras, long id)
        {
            if (_requestExtras.ContainsKey(id))
                _requestExtras[id] = requestExtras;
            else
                _requestExtras.Add(id, requestExtras);
        }
    }
}
