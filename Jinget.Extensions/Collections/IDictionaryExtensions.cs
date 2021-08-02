using System.Collections.Generic;

namespace Jinget.Extensions.Collections.Dictionary
{
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// By default(force=false), only keys in second collection which do not exist in first collection will be merged and duplicate keys will be ignored
        /// </summary>
        /// <param name="overwrite">if set to true, then duplicate keys from second collection will be overwritten to the first collection</param>
        /// <returns></returns>
        public static IDictionary<string, TValue> Merge<TValue>(this IDictionary<string, TValue> first, IDictionary<string, TValue> second, bool overwrite = false)
        {
            foreach (var member in second)
            {
                if (overwrite)
                {
                    if (first.ContainsKey(member.Key))
                    {
                        first[member.Key] = member.Value;
                    }
                    else
                    {
                        first.Add(member.Key, member.Value);
                    }
                }
                else if (!first.ContainsKey(member.Key))
                {
                    first.Add(member.Key, member.Value);
                }
            }

            return first;
        }
    }
}
