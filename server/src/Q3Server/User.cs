using System.Runtime.Serialization;


namespace Q3Server
{
    public class User
    {
        public string UserName { get; }
        public string FullName { get; }
        public string EmailAddress { get; }

        [IgnoreDataMember] //Don't send this field to clients by cancelling serialization
        public string DistinguishedName { get; }

        public User(string userName, string fullName, string emailAddress, string distinguishedName = null)
        {
            UserName = userName;
            FullName = fullName;
            EmailAddress = emailAddress;
            DistinguishedName = StripSemis(distinguishedName ?? userName);
        }

        public override string ToString()
        {
            return StripSemis(UserName) + ";" + StripSemis(FullName) + ";" + StripSemis(EmailAddress);
        }

        private string StripSemis(string input)
        {
            return input.Replace(";", "");
        }

        public override bool Equals(object obj)
        {
            return obj is User && UserName.Equals(((User)obj).UserName);
        }

        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
    }
}
