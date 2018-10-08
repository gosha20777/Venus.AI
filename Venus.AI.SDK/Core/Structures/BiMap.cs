using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Venus.Ai.SDK.Core.Structures
{
    public class BiMap<TKey1, TKey2> : IEnumerable
    {
        private Dictionary<TKey1, TKey2> forwardDic;
        private Dictionary<TKey2, TKey1> reversedDic;

        public BiMap()
        {
            forwardDic = new Dictionary<TKey1, TKey2>();
            reversedDic = new Dictionary<TKey2, TKey1>();
        }

        public BiMap(int capacity)
        {
            forwardDic = new Dictionary<TKey1, TKey2>(capacity);
            reversedDic = new Dictionary<TKey2, TKey1>(capacity);
        }

        public BiMap(IDictionary<TKey1, TKey2> dic)
        {
            forwardDic = new Dictionary<TKey1, TKey2>(dic);
            reversedDic = new Dictionary<TKey2, TKey1>();
            foreach(var item in forwardDic)
            {
                reversedDic.Add(item.Value, item.Key);
            }
        }

        public BiMap(IDictionary<TKey2, TKey1> dic)
        {
            forwardDic = new Dictionary<TKey1, TKey2>();
            reversedDic = new Dictionary<TKey2, TKey1>(dic);
            foreach (var item in reversedDic)
            {
                forwardDic.Add(item.Value, item.Key);
            }
        }

        public TKey2 this[TKey1 key1]
        {
            get
            {
                if (forwardDic.TryGetValue(key1, out var val))
                    return val;
                else
                    throw new InvalidOperationException("No such key1");
            }
            set
            {
                if (forwardDic.ContainsKey(key1))
                {
                    var oldKey2 = forwardDic[key1];
                    if (!reversedDic.ContainsKey(value))
                    {
                        forwardDic[key1] = value;
                        reversedDic.Remove(oldKey2);
                        reversedDic.Add(value, key1);
                    }
                    else
                    {
                        throw new InvalidOperationException("Such key2 exists");
                    }
                }
                else
                    throw new InvalidOperationException("No such key1");
            }
        }
        public TKey1 this[TKey2 key2]
        {
            get
            {
                if (reversedDic.TryGetValue(key2, out var val))
                    return val;
                else
                    throw new InvalidOperationException("No such key2");
            }
            set
            {
                if (reversedDic.ContainsKey(key2))
                {
                    var oldKey1 = reversedDic[key2];
                    if (!forwardDic.ContainsKey(value))
                    {
                        reversedDic[key2] = value;
                        forwardDic.Remove(oldKey1);
                        forwardDic.Add(value, key2);
                    }
                    else
                    {
                        throw new InvalidOperationException("Such key1 exists");
                    }
                }
                else
                    throw new InvalidOperationException("No such key2");
            }
        }

        public ICollection Keys1 => forwardDic.Keys;

        public ICollection Keys2 => reversedDic.Keys;

        public int Count => forwardDic.Count;

        public void Add(TKey1 key1, TKey2 key2)
        {
            if (forwardDic.ContainsKey(key1) || reversedDic.ContainsKey(key2))
                throw new InvalidOperationException("Duplicate key1 or key2");
            forwardDic.Add(key1, key2);
            reversedDic.Add(key2, key1);
        }

        public void Clear()
        {
            forwardDic.Clear();
            reversedDic.Clear();
        }

        public bool ContainsKey(TKey1 key1)
        {
            return forwardDic.ContainsKey(key1);
        }
        public bool ContainsKey(TKey2 key2)
        {
            return reversedDic.ContainsKey(key2);
        }

        public IEnumerator GetEnumerator()
        {
            return Keys1.GetEnumerator();
        }

        public void Remove(TKey1 key1)
        {
            if (forwardDic.TryGetValue(key1, out var key2))
            {
                forwardDic.Remove(key1);
                reversedDic.Remove(key2);
            }
            else
                throw new InvalidOperationException("No such key1");
        }
        public void Remove(TKey2 key2)
        {
            if (reversedDic.TryGetValue(key2, out var key1))
            {
                forwardDic.Remove(key1);
                reversedDic.Remove(key2);
            }
            else
                throw new InvalidOperationException("No such key2");
        }
    }
}