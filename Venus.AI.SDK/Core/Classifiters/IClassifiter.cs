using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.SDK.Core.Classifiters
{
    interface IClassifiter
    {
        void SetData(IEnumerable<KeyValuePair<string, string>> data);
        IEnumerable<KeyValuePair<string, double>> Classify(string phrase);
    }
}
