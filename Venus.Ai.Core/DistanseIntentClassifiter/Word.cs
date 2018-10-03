using System;
using System.Collections.Generic;

namespace Venus.AI.Core.DistanseIntentClassifiter
{
    class Word
    {
        public string Text { get; set; }
        public List<int> Codes { get; set; } = new List<int>();
    }
}
