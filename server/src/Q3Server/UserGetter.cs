using Q3Server.Interfaces;

namespace Q3Server
{
    public class UserGetter : IObjectGetter<User>
    {
        private UserGetterSerialized UserGetterSerialized { get; } 
        private UserGetterDomain UserGetterDomain { get; }

        public UserGetter(UserGetterSerialized userGetterSerialized,
                          UserGetterDomain userGetterDomain)
        {
            UserGetterSerialized = userGetterSerialized;
            UserGetterDomain = userGetterDomain;
        }

        public User Get(string id)
        {
            var user = UserGetterSerialized.Get(id);
            if (user != null)
            {
                return user;
            }

            var initials = id.Split(';')[0];
            user = UserGetterDomain.Get(initials);
            if (user != null)
            {
                return user;
            }

            var emailPlaceholder = id.Replace(";", "") + "@email";
            return new User(id, id, emailPlaceholder);
        }
    }
}