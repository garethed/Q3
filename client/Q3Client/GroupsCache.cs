using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Q3Client
{
    public class GroupsCache
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<string> Groups { get; private set; }

        public GroupsCache()
        {
            var list = DataCache.Load<DataCache.ListContainer<string>>("Groups");
            Groups = list == null ? new List<string>() : list.items;
            Task.Run(() => UpdateGroupList());
        }

        public bool UserIsInGroup(string groupName)
        {            
            return Groups.Any(g => string.Equals(g, groupName, StringComparison.InvariantCultureIgnoreCase));
        }

        private void UpdateGroupList()
        {
            try
            {
                logger.Info("Retrieving group list");

                var user = UserPrincipal.Current;

                var context = new PrincipalContext(ContextType.Domain);
                var searcher = new DirectorySearcher(context.ConnectedServer);

                // the OID is for recursive memberof search
                searcher.Filter = "(member:1.2.840.113556.1.4.1941:=" + user.DistinguishedName + ")";
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("distinguishedName");
                searcher.PropertiesToLoad.Add("mail");


                var newGroups =
                    searcher.FindAll()
                        .Cast<SearchResult>()
                        .Where(r => r.Properties["mail"].Count > 0)
                        .Select(r => new { Name = (string)r.Properties["cn"][0], Path = (string)r.Properties["distinguishedName"][0] })
                        .Where(r => !r.Path.ToLowerInvariant().Contains("security groups") || r.Name.StartsWith("Softwire - ") || r.Name.StartsWith("Office - "))
                        .Select(s => s.Name)
                        .OrderBy(s => s)
                        .ToList();
                this.Groups = newGroups;
                DataCache.Save(new DataCache.ListContainer<string>(Groups), "Groups");
            }
            catch (Exception e)
            {
                logger.Warn(e, "Failed to read groups");   
                Groups = new List<string>();
            }
        }
    }
}
