using Xunit;

namespace Q3Server.Tests
{
    public class UserGetterSerializedTests
    {
        [Fact]
        public void CorrectlySerializedUserRetrievedFully()
        {
            var serializedId = "username;Full Name;email@address.com";
            var serializedUser = new User("username", "Full Name", "email@address.com", "username");
            var serializedGetter = new UserGetterSerialized();

            var actual = serializedGetter.Get(serializedId);

            Assert.Equal(serializedUser.UserName, actual.UserName);
            Assert.Equal(serializedUser.FullName, actual.FullName);
            Assert.Equal(serializedUser.EmailAddress, actual.EmailAddress);
            Assert.Equal(serializedUser.DistinguishedName, actual.DistinguishedName);
        }

        [Fact]
        public void IncorrectlySerializedUserIsNotRetrieved()
        {
            var serializedGetter = new UserGetterSerialized();
            var id = "username";

            var user = serializedGetter.Get(id);

            Assert.Equal(null, user);
        }
    }
}
