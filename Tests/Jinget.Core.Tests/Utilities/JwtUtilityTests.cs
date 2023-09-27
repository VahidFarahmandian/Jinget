using Jinget.Core.Utilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jinget.Core.Utilities.Tests
{
    [TestClass()]
    public class JwtUtilityTests
    {
        [TestMethod()]
        public void should_return_token_parts()
        {
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiaXNzIjoiSmluZ2V0IiwiaWF0IjoxNTE2MjM5MDIyfQ.Ushn140BB6h_G4rEnZuM2VWSKmatFc4DVrvJGWlRRfE";
            var result = JwtUtility.Read(token);
            Assert.IsNotNull(result.Subject);
            Assert.IsNotNull(result.Issuer);
            Assert.IsNotNull(result.IssuedAt);
        }

        [TestMethod()]
        public async Task should_validate_token_with_lifetime_sigingkey()
        {
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiaXNzIjoiSmluZ2V0IiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjIwMTYyMzkwMjIsImF1ZCI6IkppbmdldC5UZXN0In0.e-GVmjCsuP6sv7csybQZbVp5HenQ1UT5AhzafYSlMFU";
            var result = await JwtUtility.IsValidAsync(token);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task should_validate_token_with_lifetime_sigingkey_audience_issuer()
        {
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiaXNzIjoiSmluZ2V0IiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjIwMTYyMzkwMjIsImF1ZCI6IkppbmdldC5UZXN0In0.e-GVmjCsuP6sv7csybQZbVp5HenQ1UT5AhzafYSlMFU";
            string validIssuer = "Jinget";
            IEnumerable<string> validAudiences = new string[] { "Jinget.Test" };

            var result = await JwtUtility.IsValidAsync(token, validAudiences, validIssuer);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        [ExpectedException(typeof(SecurityTokenMalformedException))]
        public async Task should_return_false_for_invalid_tokenAsync()
        {
            string token = "InvalidJwtToken";
            var result = await JwtUtility.IsValidAsync(token);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void should_create_a_valid_jwt_token()
        {
            var result = JwtUtility.Generate("vahid", new[] { "role1,role2" }, new Types.JwtModel { SecretKey = "12345678901234567890123456789012" });
            var tokenInfo = JwtUtility.Read(result);
            Assert.AreEqual(tokenInfo.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value, "vahid");
            Assert.AreEqual(tokenInfo.Claims.First(x => x.Type == ClaimTypes.Role).Value, "role1,role2");
        }
    }
}