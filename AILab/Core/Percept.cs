using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    [Serializable]
	public class Percept : ICloneable
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();
        
        public Percept(IList<KeyValuePair<string, object>> kvplist)
        {
            for (int i = 0; i < kvplist.Count; i++)
                dict.Add(kvplist[i].Key, kvplist[i].Value);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public object Clone()
        {
            IList<KeyValuePair<string, object>> l = new List<KeyValuePair<string, object>>();

            for (int i = 0; i < dict.Count; i++)
                l.Add(dict.ElementAt(i));

            return new Percept(l);
        }

        public virtual object Get(string key)
        {
            return dict[key];
        }

        public virtual void Set(KeyValuePair<string, object> kvp)
        {
            try
            {
                dict[kvp.Key] = kvp.Value;
            }
            catch (Exception)
            {

            }
        }

        public object this[string key]
        {
            get { return Get(key); }
            set { Set(new KeyValuePair<string, object>(key, value)); }
        }
    }
}
