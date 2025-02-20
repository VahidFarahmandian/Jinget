using Jinget.Core.Utilities.IP;

namespace Jinget.Core.Tests.Utilities.IP;

[TestClass()]
public class IPUtilityTests
{
    [TestMethod]
    public void IsIPInRange_IPv4_ValidRange_ReturnsTrue()
    {
        Assert.IsTrue(IPUtility.IsIPInRange("192.168.1.10", "192.168.1.0/24"));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_OutOfRange_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("192.168.2.10", "192.168.1.0/24"));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_ExactMatch_ReturnsTrue()
    {
        Assert.IsTrue(IPUtility.IsIPInRange("192.168.1.1", "192.168.1.1/32"));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_InvalidCIDR_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("192.168.1.10", "invalid_cidr"));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_InvalidIP_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("invalid_ip", "192.168.1.0/24"));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_NullCIDR_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("192.168.1.10", null));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_EmptyCIDR_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("192.168.1.10", ""));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_NoSlashCIDR_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("192.168.1.10", "192.168.1.0"));
    }

    [TestMethod]
    public void IsIPInRange_IPv4_InvalidPrefixLength_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("192.168.1.10", "192.168.1.0/33")); // > 32
    }

    // IPv6 Tests
    [TestMethod]
    public void IsIPInRange_IPv6_ValidRange_ReturnsTrue()
    {
        Assert.IsTrue(IPUtility.IsIPInRange("2001:0db8:85a3:0000:0000:8a2e:0370:7334", "2001:0db8:85a3::/48"));
    }

    [TestMethod]
    public void IsIPInRange_IPv6_OutOfRange_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("2001:0db8:85a4:0000:0000:8a2e:0370:7334", "2001:0db8:85a3::/48"));
    }

    [TestMethod]
    public void IsIPInRange_IPv6_ExactMatch_ReturnsTrue()
    {
        Assert.IsTrue(IPUtility.IsIPInRange("2001:0db8:85a3:0000:0000:8a2e:0370:7334", "2001:0db8:85a3:0000:0000:8a2e:0370:7334/128"));
    }

    [TestMethod]
    public void IsIPInRange_IPv6_InvalidCIDR_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("2001:0db8:85a3::1", "invalid_cidr"));
    }

    [TestMethod]
    public void IsIPInRange_IPv6_InvalidIP_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("invalid_ip", "2001:0db8:85a3::/48"));
    }

    [TestMethod]
    public void IsIPInRange_IPv6_InvalidPrefixLength_ReturnsFalse()
    {
        Assert.IsFalse(IPUtility.IsIPInRange("2001:0db8:85a3::1", "2001:0db8:85a3::/129")); // > 128
    }

}