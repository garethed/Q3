using System.DirectoryServices.AccountManagement;

namespace Q3Server
{
    public class UserGetterDomain
    {
        public virtual User Get(string initials)
        {
            var context = new PrincipalContext(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(context, initials);
            return user == null
                ? null
                : new User(user.SamAccountName, user.Name, user.EmailAddress, user.DistinguishedName);
        }
    }
}