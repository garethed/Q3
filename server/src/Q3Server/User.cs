using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;


namespace Q3Server
{
    public class User
    {
        public string UserName { get; }
        public string FullName { get; }
        public string EmailAddress { get; }
        public string DistinguishedName { get; }

        public User(string userName, string fullName, string emailAddress, string distinguishedName = null)
        {
            UserName = userName;
            FullName = fullName;
            EmailAddress = emailAddress;
            DistinguishedName = distinguishedName ?? userName;
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

        public List<string> GetUserGroups()
        {
            try
            {
                var context = new PrincipalContext(ContextType.Domain);
                var searcher = new DirectorySearcher(context.ConnectedServer);

                // the OID is for recursive memberof search
                searcher.Filter = "(member:1.2.840.113556.1.4.1941:=" + StripSemis(DistinguishedName) + ")";
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.PropertiesToLoad.Add("mail");

                var userGroups =
                    searcher.FindAll()
                        .Cast<SearchResult>()
                        .Where(r => r.Properties["mail"].Count > 0)
                        .Select(r => new { Name = (string)r.Properties["cn"][0], Path = (string)r.Properties["distinguishedName"][0] })
                        .Where(r => !r.Path.ToLowerInvariant().Contains("security groups") || r.Name.StartsWith("Softwire - ") || r.Name.StartsWith("Office - "))
                        .Select(s => s.Name)
                        .OrderBy(s => s)
                        .ToList();
                return userGroups;
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
    }
}
