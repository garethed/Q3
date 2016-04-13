using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Q3Server.Interfaces;

namespace Q3Server
{
    public class GroupGetterDomain : IObjectGetter<List<string>>
    {
        public List<string> Get(string distinguishedUserName)
        {
            try
            {
                var context = new PrincipalContext(ContextType.Domain);
                var searcher = new DirectorySearcher(context.ConnectedServer);

                // the OID is for recursive memberof search
                searcher.Filter = "(member:1.2.840.113556.1.4.1941:=" + distinguishedUserName + ")";
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
            catch
            {
                return new List<string>();
            }
        }
    }
}