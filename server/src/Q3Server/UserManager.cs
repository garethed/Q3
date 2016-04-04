using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Caching;

namespace Q3Server
{
    public interface IUserManager
    {
        User GetUser(string idString);
    }

    public class UserManager : IUserManager
    {
        ObjectCache cache;
        public UserManager(ObjectCache cache)
        {
            this.cache = cache;
        }

        public User GetUser(string idString)
        {
            var user = (User) cache.Get(idString);
            if (user == null)
            {
                user = CreateUser(idString);
                cache.Set(idString, user, DateTimeOffset.Now.AddHours(1.0));
            }
            return user;
        }

        private User CreateUser(string idString)
        {
            string[] parts = idString.Split(new[] { ';' }, 3);
            var userName = parts[0];

            var user = GetUserFromDomain(userName);
            if (user != null) return user;

            return parts.Length == 3
                ? new User(userName, parts[1], parts[2], userName)
                : new User(userName, userName, userName + "@placeholder", userName);
        }

        private User GetUserFromDomain(string userName)
        {
            var context = new PrincipalContext(ContextType.Domain);
            var userPrincipal = UserPrincipal.FindByIdentity(context, userName);
            return userPrincipal == null
                ? null
                : new User(userName, userPrincipal.Name, userPrincipal.EmailAddress, userPrincipal.DistinguishedName);
        }
    }
}