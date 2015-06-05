using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace Q3Server
{
    public interface IGroupCache
    {
        IEnumerable<string> GetGroups();
    }

    public class GroupCache : IGroupCache
    {
        private readonly object lockable = new { };
        private DateTime lastUpdated = DateTime.MinValue;
        private volatile IEnumerable<string> groups;

        public GroupCache()
        {
            Task.Run(() => GetGroups());
        }

        public IEnumerable<string> GetGroups()
        {
            lock (lockable)
            {
                if (lastUpdated.AddHours(1) < DateTime.Now)
                {
                    lastUpdated = DateTime.Now;
                    Task.Run(() => UpdateGroups());

                }

                return this.groups;
            }

        }

        private void UpdateGroups()
        {
            try
            {
                var context = new PrincipalContext(ContextType.Domain);
                var searcher = new DirectorySearcher(context.ConnectedServer);

                searcher.Filter = "(&(objectclass=group)(|(cn=Social *)(cn=Team *)(cn= Office *)))";

                var newGroups = searcher.FindAll().Cast<SearchResult>().Select(r => r.Properties["cn"][0] as string).ToList();

                this.groups = newGroups;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}