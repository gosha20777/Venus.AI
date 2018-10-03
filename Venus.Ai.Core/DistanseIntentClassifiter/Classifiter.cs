using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Venus.AI.Core.DistanseIntentClassifiter
{
    public class Classifiter : IClassifiter
    {
        Languages language;
        List<KeyValuePair<string, Phrase>> data;
        public Classifiter(Languages language)
        {
            this.language = language;
            data = new List<KeyValuePair<string, Phrase>>();

            SetPhoneticGroups(PhoneticGroupsRus, new List<string>() { "ыий", "эе", "ая", "оёе", "ую", "шщ", "оа" });
            SetPhoneticGroups(PhoneticGroupsEng, new List<string>() { "aeiouy", "bp", "ckq", "dt", "lr", "mn", "gj", "fpv", "sxz", "csz" });
        }

        public IEnumerable<KeyValuePair<string, double>> Classify(string inputStr)
        {
            return Classify(inputStr, 0, 0.7, 0.2);
        }

        public IEnumerable<KeyValuePair<string, double>> Classify(string inputStr, int topN, double minAccur, double minDiff)
        {
            var resultNormal = new List<KeyValuePair<string, double>>();
            if (data.Count == 0)
            {
                resultNormal.Add(new KeyValuePair<string, double>("failback", 1));
                return resultNormal;
            }

            var codeKeys = Helpers.CodeKeysEng.ToList();
            if (language == Languages.Russian)
                codeKeys = codeKeys.Concat(Helpers.CodeKeysRus).ToList();

            Phrase originalPhrase = new Phrase();
            if (inputStr.Length > 0)
            {
                originalPhrase.Words = inputStr.Split(' ').Select(w => new Word()
                {
                    Text = w.ToLower(),
                    Codes = GetKeyCodes(codeKeys, w)
                }).ToList();
            }

            var resultDic = new Dictionary<string, double>();
            foreach (var item in data)
            {
                double cost = GetRangePhrase(item.Value, originalPhrase);
                if (!resultDic.ContainsKey(item.Key))
                    resultDic.Add(item.Key, cost);
                else
                    resultDic[item.Key] = Math.Min(resultDic[item.Key], cost);
            }

            var result = resultDic.OrderBy(x => x.Value).ToList();
            var maxCost = result.Last().Value;
            
            if (topN == 0)
                topN = result.Count;

            double maxNormalVal = 0;
            for (int i = 0; i < topN; i++)
            {
                var normalVal = 1 - result[i].Value / maxCost;
                if (normalVal > 1)
                    normalVal = 1;
                if (normalVal < 0)
                    normalVal = 0;
                resultNormal.Add(new KeyValuePair<string, double>(result[i].Key, normalVal));

                if (maxNormalVal < normalVal)
                    maxNormalVal = normalVal;
                else if (maxNormalVal - normalVal < 0.2)
                {
                    resultNormal.Insert(0, new KeyValuePair<string, double>("failback", 1));
                    break;
                }
            }
            if(resultNormal.First().Value < 0.7)
            {
                resultNormal.Insert(0, new KeyValuePair<string, double>("failback", 1));
            }
            return resultNormal;
        }

        public void SetData(IEnumerable<KeyValuePair<string, string>> data)
        {
            var codeKeys = Helpers.CodeKeysEng.ToList();
            if (language == Languages.Russian)
                codeKeys = codeKeys.Concat(Helpers.CodeKeysRus).ToList();

            foreach (var item in data)
            {
                this.data.Add(new KeyValuePair<string, Phrase>(item.Key, new Phrase()
                {
                    Text = item.Value,
                    Words = item.Value.Split(' ').Select(w => new Word()
                    {
                        Text = w.ToLower(),
                        Codes = GetKeyCodes(codeKeys, w)
                    }).ToList()
                }));
            }
        }

        private double GetRangePhrase(Phrase source, Phrase search)
        {
            if (!source.Words.Any())
            {
                if (!search.Words.Any())
                    return 0;
                return search.Words.Sum(w => w.Text.Length) * 2 * 100;
            }
            if (!search.Words.Any())
            {
                return source.Words.Sum(w => w.Text.Length) * 2 * 100;
            }
            double result = 0;
            for (int i = 0; i < search.Words.Count; i++)
            {
                double minRangeWord = double.MaxValue;
                int minIndex = 0;
                for (int j = 0; j < source.Words.Count; j++)
                {
                    double currentRangeWord = GetRangeWord(source.Words[j], search.Words[i]);
                    if (currentRangeWord < minRangeWord)
                    {
                        minRangeWord = currentRangeWord;
                        minIndex = j;
                    }
                }
                result += minRangeWord * 100 + (Math.Abs(i - minIndex) / 10.0);
            }
            return result;
        }
        private double GetRangeWord(Word source, Word target)
        {
            double minDistance = double.MaxValue;
            Word croppedSource = new Word();
            int length = Math.Min(source.Text.Length, target.Text.Length + 1);
            for (int i = 0; i <= source.Text.Length - length; i++)
            {
                croppedSource.Text = source.Text.Substring(i, length);
                croppedSource.Codes = source.Codes.Skip(i).Take(length).ToList();
                minDistance = Math.Min(minDistance, LevenshteinDistance(croppedSource, target, croppedSource.Text.Length == source.Text.Length) + (i * 2 / 10.0));
            }
            return minDistance;
        }
        private int LevenshteinDistance(Word source, Word target, bool fullWord)
        {
            if (String.IsNullOrEmpty(source.Text))
            {
                if (String.IsNullOrEmpty(target.Text))
                    return 0;
                return target.Text.Length * 2;
            }
            if (String.IsNullOrEmpty(target.Text))
                return source.Text.Length * 2;
            int n = source.Text.Length;
            int m = target.Text.Length;
            //TODO Убрать в параметры (для оптимизации)
            int[,] distance = new int[3, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++)
                distance[0, j] = j * 2;
            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i % 3;
                var previousRow = (i - 1) % 3;
                distance[currentRow, 0] = i * 2;
                for (var j = 1; j <= m; j++)
                {
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + ((!fullWord && i == n) ? 2 - 1 : 2),
                                distance[currentRow, j - 1] + ((!fullWord && i == n) ? 2 - 1 : 2)),
                                distance[previousRow, j - 1] + CostDistanceSymbol(source, i - 1, target, j - 1));

                    if (i > 1 && j > 1 && source.Text[i - 1] == target.Text[j - 2]
                                       && source.Text[i - 2] == target.Text[j - 1])
                    {
                        distance[currentRow, j] = Math.Min(distance[currentRow, j], distance[(i - 2) % 3, j - 2] + 2);
                    }
                }
            }
            return distance[currentRow, m];
        }
        private int CostDistanceSymbol(Word source, int sourcePosition, Word search, int searchPosition)
        {
            if (source.Text[sourcePosition] == search.Text[searchPosition])
                return 0;
            if (source.Codes[sourcePosition] != 0 && source.Codes[sourcePosition] == search.Codes[searchPosition])
                return 0;
            int resultWeight = 0;
            List<int> nearKeys;
            if (!Helpers.DistanceCodeKey.TryGetValue(source.Codes[sourcePosition], out nearKeys))
                resultWeight = 2;
            else
                resultWeight = nearKeys.Contains(search.Codes[searchPosition]) ? 1 : 2;
            List<char> phoneticGroups;
            if (PhoneticGroupsRus.TryGetValue(search.Text[searchPosition], out phoneticGroups))
                resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
            if (PhoneticGroupsEng.TryGetValue(search.Text[searchPosition], out phoneticGroups))
                resultWeight = Math.Min(resultWeight, phoneticGroups.Contains(source.Text[sourcePosition]) ? 1 : 2);
            return resultWeight;
        }

        private List<int> GetKeyCodes(List<KeyValuePair<char, int>> codeKeys, string word)
        {
            return word.ToLower().Select(ch => codeKeys.FirstOrDefault(ck => ck.Key == ch).Value).ToList();
        }

        #region Блок Фонетических групп
        static Dictionary<char, List<char>> PhoneticGroupsRus = new Dictionary<char, List<char>>();
        static Dictionary<char, List<char>> PhoneticGroupsEng = new Dictionary<char, List<char>>();
        #endregion
        private static void SetPhoneticGroups(Dictionary<char, List<char>> resultPhoneticGroups, List<string> phoneticGroups)
        {
            foreach (string group in phoneticGroups)
                foreach (char symbol in group)
                    if (!resultPhoneticGroups.ContainsKey(symbol))
                        resultPhoneticGroups.Add(symbol, phoneticGroups.Where(pg => pg.Contains(symbol)).SelectMany(pg => pg).Distinct().Where(ch => ch != symbol).ToList());
        }
    }
}
