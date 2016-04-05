using Q3Server.Interfaces;

namespace Q3Server
{
    public class UserGetterSerialized : IObjectGetter<User>
    {
        public User Get(string id)
        {
            string[] parts = id.Split(new[] { ';' }, 3);

            return parts.Length == 3
                ? new User(parts[0], parts[1], parts[2])
                : new User(parts[0], id, id.Replace(";", "") + "@placeholder", id);
        }
    }
}