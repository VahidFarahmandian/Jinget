using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Jinget.Core.Collections;

namespace Jinget.Core.Tests.Collections
{
    [TestClass()]
    public class IDictionaryExtensionsTests
    {
        public IDictionary<string, string> First { get; private set; }
        public IDictionary<string, string> Second { get; private set; }

        [TestInitialize()]
        public void Initialize()
        {
            First = new Dictionary<string, string>()
            {
                { "name", "vahid" },
                { "lastname", "farahmandian" }
            };

            Second = new Dictionary<string, string>()
            {
                { "gender", "male" },
                { "name", "alex" },
                { "country", "Iran" }
            };
        }

        [TestMethod()]
        public void Should_merge_two_dictionaries_and_ignore_duplicates()
        {
            Dictionary<string, string> expected = new()
            {
                { "name", "vahid" },
                { "lastname", "farahmandian" },
                { "gender", "male" },
                { "country", "Iran" }
            };
            
            var result = First.Merge(Second);

            Assert.IsFalse(result.Keys.Except(expected.Keys).Any());
            Assert.IsFalse(expected.Keys.Except(result.Keys).Any());

            Assert.IsTrue(expected.Keys.SequenceEqual(result.Keys));
            Assert.IsTrue(expected.Values.SequenceEqual(result.Values));

            Assert.IsTrue(result["name"] == "vahid");
        }

        [TestMethod()]
        public void Should_merge_two_dictionaries_and_overwrite_the_duplicates()
        {
            Dictionary<string, string> expected = new()
            {
                { "name", "alex" },
                { "lastname", "farahmandian" },
                { "gender", "male" },
                { "country", "Iran" }
            };

            var result = First.Merge(Second, overwrite: true);

            Assert.IsFalse(result.Keys.Except(expected.Keys).Any());
            Assert.IsFalse(expected.Keys.Except(result.Keys).Any());

            Assert.IsTrue(expected.Keys.SequenceEqual(result.Keys));
            Assert.IsTrue(expected.Values.SequenceEqual(result.Values)); 
            
            Assert.IsTrue(result["name"] == "alex");
        }
    }
}