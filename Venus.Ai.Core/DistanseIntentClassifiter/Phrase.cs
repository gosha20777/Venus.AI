using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.Core.DistanseIntentClassifiter
{
    class Phrase
    {
        public string Text { get; set; }
        public List<Word> Words { get; set; } = new List<Word>();
    }
}
