using Moq;
using Xunit;

namespace Q3Server.Tests
{
    public class UserGetterTests
    {
        [Fact]
        public void SerializedUserReturnedIfCorrectlySerialized()
        {
            var userGetterSerialized = new Mock<UserGetterSerialized>();
            userGetterSerialized.Setup(g => g.Get("id")).Returns(new User("id", "Full Name", "email@test.com"));
            var userGetterDomain = new Mock<UserGetterDomain>();
            var userGetter = new UserGetter(userGetterSerialized.Object, userGetterDomain.Object);

            var user = userGetter.Get("id");

            Assert.Equal("id", user.UserName);
        }

        [Fact]
        public void DomainUserReturnedIfIncorrectlySerialized()
        {
            var userGetterSerialized = new Mock<UserGetterSerialized>();
            userGetterSerialized.Setup(g => g.Get("id")).Returns((User)null);
            var userGetterDomain = new Mock<UserGetterDomain>();
            userGetterDomain.Setup(g => g.Get("id")).Returns(new User("id", "Full Name", "email@test.com"));
            var userGetter = new UserGetter(userGetterSerialized.Object, userGetterDomain.Object);

            var user = userGetter.Get("id");

            Assert.Equal("id", user.UserName);
        }

        [Fact]
        public void PlaceholderUserReturnedIfEveryhthingFails()
        {
            var userGetterSerialized = new Mock<UserGetterSerialized>();
            userGetterSerialized.Setup(g => g.Get("id")).Returns((User)null);
            var userGetterDomain = new Mock<UserGetterDomain>();
            userGetterDomain.Setup(g => g.Get("id")).Returns((User)null);
            var userGetter = new UserGetter(userGetterSerialized.Object, userGetterDomain.Object);

            var user = userGetter.Get("id");

            Assert.Equal("id", user.UserName);
            Assert.Equal("id", user.FullName);
            Assert.Equal("id@email", user.EmailAddress);
            Assert.Equal("id", user.DistinguishedName);
        }
    }
}