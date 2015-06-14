namespace Q3Server
{
    public class User
    {
        public string UserName;
        public string FullName;
        public string EmailAddress;

        public User(string serialized)
        {
            string[] parts = serialized.Split(new[] { ';' }, 3);
            UserName = parts[0];
            FullName = parts[1];
            EmailAddress = parts[2];
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