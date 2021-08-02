using Jinget.Extensions.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Jinget.Extensions.Tests.Enums
{
    [TestClass()]
    public class EnumExtensionsTests
    {
        enum ProgrammingLanguage
        {
            CSharp,

            [System.ComponentModel.Description("F#.Net")]
            FSharp,
            VB
        }

        [TestMethod()]
        public void should_return_enum_field_value()
        {
            ProgrammingLanguage langauge = ProgrammingLanguage.CSharp;
            string expected = "CSharp";
            var result = langauge.GetDescription();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void should_return_enum_field_description_value()
        {
            ProgrammingLanguage langauge = ProgrammingLanguage.FSharp;
            string expected = "F#.Net";
            string result = langauge.GetDescription();

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void should_return_enum_field_using_description()
        {
            string enumDescription = "F#.Net";
            ProgrammingLanguage expected = ProgrammingLanguage.FSharp;

            ProgrammingLanguage result = EnumExtensions.GetValueFromDescription<ProgrammingLanguage>(enumDescription);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void should_return_enum_field_using_field_name()
        {
            string enumDescription = "VB";
            ProgrammingLanguage expected = ProgrammingLanguage.VB;

            ProgrammingLanguage result = EnumExtensions.GetValueFromDescription<ProgrammingLanguage>(enumDescription);

            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ComponentModel.InvalidEnumArgumentException))]
        public void should_generate_InvalidEnumArgumentException()
        {
            string enumDescription = "Java";
            EnumExtensions.GetValueFromDescription<ProgrammingLanguage>(enumDescription);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void should_generate_InvalidOperationException()
        {
            string enumDescription = "Java";
            EnumExtensions.GetValueFromDescription<Tests.BaseData.InvalidStruct>(enumDescription);
        }
    }
}
