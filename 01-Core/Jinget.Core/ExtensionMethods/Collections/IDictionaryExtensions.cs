using Jinget.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Jinget.Core.ExtensionMethods.Collections
{
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// By default(<paramref name="overwrite"/>=false), only keys in second collection which do not exist 
        /// in the first collection will be merged and duplicate keys will be ignored
        /// </summary>
        /// <param name="overwrite">if set to true, then duplicate keys from second collection will be overwritten to the first collection</param>
        public static IDictionary<string, TValue>? Merge<TValue>(this IDictionary<string, TValue>? first, IDictionary<string, TValue>? second, bool overwrite = false)
        {
            if (first == null && second == null)
                return null;
            if (first == null)
                return second;
            if (second == null)
                return first;

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

        /// <summary>
        /// convert dictionary to <see cref="FilterCriteria"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<FilterCriteria> ToFilterCriteria(this IDictionary<string, string> input)
            => input.Select(x => new FilterCriteria
            {
                Operand = x.Key,
                Value = x.Value
            }).ToList();
    }
}
