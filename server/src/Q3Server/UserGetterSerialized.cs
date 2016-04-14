namespace Q3Server
{
    public class UserGetterSerialized
    {
        public virtual User Get(string serializedUser)
        {
            string[] parts = serializedUser.Split(new[] { ';' }, 3);

            return parts.Length == 3
                ? new User(parts[0], parts[1], parts[2])
                : null;
        }
    }
}