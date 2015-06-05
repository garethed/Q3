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
        private readonly object lockable = new {};
        private DateTime lastUpdated = DateTime.MinValue;
        private IEnumerable<string> serverGroups = new List<string>();
        private IEnumerable<string> userGroups = new List<string>(); 


        public IEnumerable<string> ServerGroups { get { return serverGroups; } }

        public GroupsCache()
        {
            UpdateUserGroupList();
        }

        public bool UserIsInGroup(string groupName)
        {
            groupName = groupName.ToLower();
            return userGroups.Contains(groupName);
        }

        public void UpdateGroupList(IEnumerable<string> serverGroups)
        {
            this.serverGroups = serverGroups.ToList();
        }

        private void UpdateUserGroupList()
        {
            var user = UserPrincipal.Current;

            var context = new PrincipalContext(ContextType.Domain);
            var searcher = new DirectorySearcher(context.ConnectedServer);

            // the OID is for recursive memberof search
            searcher.Filter = "(member:1.2.840.113556.1.4.1941:=" + user.DistinguishedName + ")";


            var newGroups = searcher.FindAll().Cast<SearchResult>().Select(r => r.Properties["cn"][0] as string).Select(s => s.ToLower()).OrderBy(s => s).ToList();
            this.userGroups = newGroups;
        }
    }
}
