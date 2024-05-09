using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Jinget.Core.Exceptions.Tests;

[TestClass()]
public class JingetExceptionTests
{
    [TestMethod()]
    public void Shsould_throw_jinget_expection_with_default_params()
    {
        string exceptionMessage = "This is test exception";
        int exceptionCode = -1;

        try
        {
            throw new JingetException(exceptionMessage);
        }
        catch (JingetException ex)
        when (
        ex.Message == exceptionMessage &&
        ex.Code == exceptionCode &&
        ex.Type == Enumerations.ExceptionType.JingetInternal)
        {
            Assert.IsTrue(true);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [TestMethod()]
    public void Shsould_throw_jinget_expection_with_inner_exception()
    {
        Exception innerException = new AccessViolationException();
        string exceptionMessage = "This is test exception";
        int exceptionCode = -1;

        try
        {
            throw new JingetException(exceptionMessage, innerException, exceptionCode, Enumerations.ExceptionType.Custom);
        }
        catch (JingetException ex)
        when (
        ex.Message == exceptionMessage &&
        ex.Code == exceptionCode &&
        ex.Type == Enumerations.ExceptionType.Custom &&
        ex.InnerException is AccessViolationException)
        {
            Assert.IsTrue(true);
        }
        catch
        {
            Assert.Fail();
        }
    }
}