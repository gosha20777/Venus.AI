using System;
using System.Collections.Generic;
using System.Text;
using Venus.Ai.SDK.Core.Structures;

namespace Venus.AI.SDK.Core.Enums
{
    class EnumsConvertor
    {
        private static readonly BiMap<Language, string> biMapLang = new BiMap<Language, string>
            {
                { Language.English, "eng" },
                { Language.Russian, "rus" }
            };
        private static readonly BiMap<RequestType, string> biMapReqType = new BiMap<RequestType, string>
            {
                { RequestType.Text, "text" },
                { RequestType.Voice, "voice" }
            };
        public static string LanguageToString(Language language)
        {
            return biMapLang[language];
        }
        public static string RequestTypeToString(RequestType requestType)
        {
            return biMapReqType[requestType];
        }
        public static RequestType StringToRequestType(string str)
        {
            return biMapReqType[str];
        }
        public static Language StringToLanguage(string str)
        {
            return biMapLang[str];
        }
    }
}
