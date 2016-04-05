using Moq;
using Q3Server.Interfaces;
using Xunit;

namespace Q3Server.Tests
{
    public class GettersTests
    {
        [Fact]
        public void CorrectlySerializedUserRetrievedFully()
        {
            var serializedGetter = new UserGetterSerialized();
            var id = "username;Full Name;email@address.com";

            var user = serializedGetter.Get(id);

            Assert.Equal("username", user.UserName);
            Assert.Equal("Full Name", user.FullName);
            Assert.Equal("email@address.com", user.EmailAddress);
            Assert.Equal("username", user.DistinguishedName);
        }

        [Fact]
        public void IncorrectlySerializedUserRetrievedAsDefault()
        {
            var serializedGetter = new UserGetterSerialized();
            var id = "username";

            var user = serializedGetter.Get(id);

            Assert.Equal("username", user.UserName);
            Assert.Equal("username", user.FullName);
            Assert.Equal("username@placeholder", user.EmailAddress);
            Assert.Equal("username", user.DistinguishedName);
        }

        [Fact]
        public void TernaryGetterReturnsFirstIfConditionValid()
        {
            var getter1 = new Mock<IObjectGetter<string>>();
            getter1.Setup(g => g.Get("id")).Returns("value1");
            var getter2 = new Mock<IObjectGetter<string>>();
            getter1.Setup(g => g.Get("id")).Returns("value2");
            var ternaryGetter = new ObjectGetterTernary<string>(getter1.Object, getter2.Object, s => s != null);

            var value = ternaryGetter.Get("id");

            Assert.Equal("value1", value);
        }

        [Fact]
        public void TernaryGetterReturnsSecondIfConditionInvalid()
        {
            var getter1 = new Mock<IObjectGetter<string>>();
            getter1.Setup(g => g.Get("id")).Returns((string)null);
            var getter2 = new Mock<IObjectGetter<string>>();
            getter2.Setup(g => g.Get("id")).Returns("value2");
            var ternaryGetter = new ObjectGetterTernary<string>(getter1.Object, getter2.Object, s => s != null);

            var value = ternaryGetter.Get("id");

            Assert.Equal("value2", value);
        }
    }
}