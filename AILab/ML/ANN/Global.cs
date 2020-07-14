using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN
{
    [Serializable]
	public class Global
    {
        public static int Err
        {
            get { return 1; }
        }

        public static void NextIntArray(int min, int max, int[] t, params int[] exc)
        {
            if (t == null)
                throw new Exception();
            if ((max < min) || ((max - min) < t.Length))
                throw new Exception();
            IList<int> r = new List<int>();
            for (int i = min; i < max; i++)
                r.Add(i);
            int x = 0;
            Random random = new Random();
            for (int i = 0; i < t.Length; i++)
            {
                x = random.Next(r.Count());
                //x = Math.Daemon.Random.Next(r.Count());
                t[i] = r[x];
                r.RemoveAt(x);
            }
        }

        public static int Sig
        {
            get { return 0; }
        }

        [Serializable]
	public class Parser
        {
            public enum Option
            {
                StripDefaultToken,
                None
            }

            public static string Build(string[] tokens, int startIndex, string separator)
            {
                string t = "";

                for (int i = startIndex; i < tokens.Length - 1; i++)
                    t += tokens[i] + separator;

                t += tokens.Last();

                return t;
            }

            public static T Extract<T>(string[] tokens, string[] keyOptions, Option option, out string[] r)
            {
                IList<string> t = new List<string>();
                string[] u, v = new string[] { "" };
                bool found;

                for (int i = 0; i < tokens.Length; i++)
                {
                    u = Split(tokens[i], "=");
                    found = false;
                    for (int j = 0; j < keyOptions.Length; j++)
                    {
                        if (u[0].CompareTo(keyOptions[j]) == 0)
                        {
                            v[0] = Build(u, 1, "=");
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        t.Add(tokens[i]);
                }

                r = t.ToArray();

                if (v[0].CompareTo("") == 0)
                    return default;

                switch (option)
                {
                    case Option.None:
                        break;

                    case Option.StripDefaultToken:
                        v[0] = StripDefaultToken(v[0]);
                        break;
                }

                object o = null;

                switch (typeof(T).ToString())
                {
                    case "System.Double":
                    case "System.Nullable`1[System.Double]":
                        o = double.Parse(v[0]);
                        break;

                    case "System.Int32":
                    case "System.Nullable`1[System.Int32]":
                        o = int.Parse(v[0]);
                        break;

                    case "System.String":
                        o = v[0];
                        break;
                }

                return (T)o;
            }

            public static string RemoveWhiteSpaces(string text)
            {
                string[] s = Split(text, " ");

                return Build(s, 0, "");
            }

            public static string[] Split(string text, params string[] delimiters)
            {
                return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            }

            public static string StripDefaultToken(string text)
            {
                string[] t = Split(text, "(", ")");
                return t[0];
            }
        }
    }
}