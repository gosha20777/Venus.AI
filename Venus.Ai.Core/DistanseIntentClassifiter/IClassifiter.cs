using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.Core.DistanseIntentClassifiter
{
    interface IClassifiter
    {
        void SetData(IEnumerable<KeyValuePair<string, string>> data);
        IEnumerable<KeyValuePair<string, double>> Classify(string phrase);
    }
}
