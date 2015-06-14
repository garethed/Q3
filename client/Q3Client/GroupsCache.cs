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
        public IEnumerable<string> Groups { get; private set; }

        public GroupsCache()
        {
            var list = DataCache.Load<DataCache.ListContainer<string>>("Groups");
            Groups = list == null ? new List<string>() : list.items;
            Task.Run(() => UpdateGroupList());
        }

        public bool UserIsInGroup(string groupName)
        {
            groupName = groupName.ToLower();
            return Groups.Contains(groupName);
        }

        private void UpdateGroupList()
        {
            try
            {
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
                        .Where(r => !((string)r.Properties["distinguishedName"][0]).ToLower().Contains("security groups"))
                        .Select(r => r.Properties["cn"][0] as string)
                        .Select(s => s.ToLower())
                        .OrderBy(s => s)
                        .ToList();
                this.Groups = newGroups;
                DataCache.Save(new DataCache.ListContainer<string>(Groups), "Groups");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Failed to read groups " + e);
                Groups = new List<string>();
            }
        }
    }
}
