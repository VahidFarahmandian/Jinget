using Jinget.Core.Tests._BaseData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Jinget.Core.Utilities.Tests
{
    [TestClass()]
    public class StringUtilityTests
    {
        [TestMethod()]
        public void should_create_random_string_using_given_characterset()
        {
            int length = 5;
            IEnumerable<char> charset = new char[] { 'a', 'b', 'c', '1' };

            var result = StringUtility.GetRandomString(length, charset);

            Assert.IsTrue(result.Length == length);
            Assert.IsTrue(result.ToCharArray().All(x => charset.Contains(x)));
        }
    }
}