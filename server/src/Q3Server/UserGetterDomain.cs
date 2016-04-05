using System.DirectoryServices.AccountManagement;
using Q3Server.Interfaces;

namespace Q3Server
{
    public class UserGetterDomain : IObjectGetter<User>
    {
        public User Get(string id)
        {
            var userName = id.Split(';')[0];

            var context = new PrincipalContext(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(context, userName);
            return user == null
                ? null
                : new User(user.SamAccountName, user.Name, user.EmailAddress, user.DistinguishedName);
        }
    }
}