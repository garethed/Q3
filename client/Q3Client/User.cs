namespace Q3Client
{
    public class User
    {
        public string UserName;
        public string FullName;
        public string EmailAddress;

        public override string ToString()
        {
            return StripSemis(UserName) + ";" + StripSemis(FullName) + ";" + EmailAddress;
        }

        private string StripSemis(string input)
        {
            return input.Replace(";", "");
        }

        public override bool Equals(object obj)
        {
            return obj is User && UserName.Equals(((User) obj).UserName);
        }

        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
    }
}