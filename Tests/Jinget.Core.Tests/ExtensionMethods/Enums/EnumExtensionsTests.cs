using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Jinget.Core.ExtensionMethods.Enums;
using Jinget.Core.Tests._BaseData;
using System.Collections.Generic;
using System.Linq;
using static Jinget.Core.Tests._BaseData.SampleEnum;
using System.ComponentModel;
using Jinget.Core.Utilities.Enum;

namespace Jinget.Core.Tests.ExtensionMethods.Enums;

[TestClass()]
public class EnumExtensionsTests
{
    #region DisplayName

    [TestMethod()]
    public void should_return_default_display_name()
    {
        ProgrammingLanguage langauge = default;
        string expected = "";
        string result = langauge.GetDisplayName();

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_display_name_for_member_with_display_attribute()
    {
        ProgrammingLanguage langauge = ProgrammingLanguage.FSharp;
        string expected = "F#";
        string result = langauge.GetDisplayName();

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_display_name_for_member_without_display_attribute()
    {
        ProgrammingLanguage langauge = ProgrammingLanguage.Golang;
        string expected = "Golang";
        string result = langauge.GetDisplayName();

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_enum_value_where_display_name_provided()
    {
        string enumDisplayName = "C#";
        List<ProgrammingLanguage> expected = [ProgrammingLanguage.CSharp, ProgrammingLanguage.VB];

        List<ProgrammingLanguage> result = EnumUtility.GetValueFromDisplayName<ProgrammingLanguage>(enumDisplayName);

        Assert.IsTrue(expected.SequenceEqual(result));
    }

    [TestMethod()]
    public void should_return_enum_value_where_display_name_not_provided()
    {
        string enumDisplayName = "Golang";
        List<ProgrammingLanguage> expected = [ProgrammingLanguage.Golang];

        List<ProgrammingLanguage> result = EnumUtility.GetValueFromDisplayName<ProgrammingLanguage>(enumDisplayName);

        Assert.IsTrue(expected.SequenceEqual(result));
    }

    [TestMethod()]
    [ExpectedException(typeof(System.ComponentModel.InvalidEnumArgumentException))]
    public void should_throw_exception_where_display_name_not_found()
    {
        string enumDescription = "Java";
        EnumUtility.GetValueFromDisplayName<ProgrammingLanguage>(enumDescription);
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void should_throw_exception_where_enum_type_is_invalid_in_display_name()
    {
        string enumDescription = "Java";
        EnumUtility.GetValueFromDisplayName<InvalidStruct>(enumDescription);
    }

    #endregion

    #region Description

    [TestMethod()]
    public void should_return_default_description()
    {
        ProgrammingLanguage langauge = default;
        string expected = "";
        string result = langauge.GetDescription();

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_description_for_member_with_description_attribute()
    {
        ProgrammingLanguage langauge = ProgrammingLanguage.FSharp;
        string expected = "F#.Net";
        string result = langauge.GetDescription();

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_description_for_member_without_description_attribute()
    {
        ProgrammingLanguage langauge = ProgrammingLanguage.CSharp;
        string expected = "CSharp";
        var result = langauge.GetDescription();

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_enum_value_where_description_provided()
    {
        string enumDescription = "F#.Net";
        ProgrammingLanguage expected = ProgrammingLanguage.FSharp;

        ProgrammingLanguage result = EnumUtility.GetValueFromDescription<ProgrammingLanguage>(enumDescription);

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void should_return_enum_value_where_description_not_provided()
    {
        string enumDescription = "VB";
        ProgrammingLanguage expected = ProgrammingLanguage.VB;

        ProgrammingLanguage result = EnumUtility.GetValueFromDescription<ProgrammingLanguage>(enumDescription);

        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    [ExpectedException(typeof(System.ComponentModel.InvalidEnumArgumentException))]
    public void should_throw_exception_where_description_not_found()
    {
        string enumDescription = "Java";
        EnumUtility.GetValueFromDescription<ProgrammingLanguage>(enumDescription);
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void should_throw_exception_where_enum_type_is_invalid_in_description()
    {
        string enumDescription = "Java";
        EnumUtility.GetValueFromDescription<InvalidStruct>(enumDescription);
    }

    #endregion region

    #region Min & Max Values

    [TestMethod()]
    public void should_return_min_val_in_enum()
    {
        int expectedMinVal = 1;
        var result = EnumUtility.GetMinValue<ProgrammingLanguage, int>();
        Assert.AreEqual(expectedMinVal, result);
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidEnumArgumentException))]
    public void should_throw_exception_for_empty_enum_min()
    {
        EnumUtility.GetMinValue<EmptyEnum, int>();
    }

    [TestMethod()]
    public void should_return_max_val_in_enum()
    {
        int expectedMinVal = 4;
        var result = EnumUtility.GetMaxValue<ProgrammingLanguage, int>();
        Assert.AreEqual(expectedMinVal, result);
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidEnumArgumentException))]
    public void should_throw_exception_for_empty_enum_max()
    {
        EnumUtility.GetMaxValue<EmptyEnum, int>();
    }

    #endregion
}
