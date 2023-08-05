using Jinget.Core.Utilities.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jinget.Core.Tests.Utilities.Parser.DatabaseParser
{
    [TestClass]
    public class JsonUtilityTests
    {
        [TestMethod()]
        public void should_return_true_for_valid_json_object_string()
        {
            string sampleJson = "{ id: 1, name: \"Leanne Graham\", username: \"Bret\", address: { street: \"Kulas Light\", city: \"Gwenborough\", geo: { lat: \"-37.3159\", lng: \"81.1496\" } }}";
            var result = JsonUtility.IsValid(sampleJson);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void should_return_true_for_valid_json_array_string()
        {
            string sampleJson = "[ { id: 1, name: \"Leanne Graham\", address: { city: \"Gwenborough\" }},{ id: 2, name: \"Vahid Farahmandian\", address: { city: \"Urmia\" }} ]";
            var result = JsonUtility.IsValid(sampleJson);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void should_return_false_for_invalid_json()
        {
            string invalidJsonString = "InvalidJsonString";
            var result = JsonUtility.IsValid(invalidJsonString);
            Assert.IsFalse(result);

            string emptyJsonString = "";
            result = JsonUtility.IsValid(emptyJsonString);
            Assert.IsFalse(result);

            string nullJsonString = "";
            result = JsonUtility.IsValid(nullJsonString);
            Assert.IsFalse(result);
        }
    }
}
