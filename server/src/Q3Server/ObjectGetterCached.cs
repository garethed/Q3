using System;
using System.Runtime.Caching;
using Q3Server.Interfaces;

namespace Q3Server
{
    public class CachedObjectGetter<T> : IObjectGetter<T>
    {
        private ObjectCache Cache { get; set; }
        private IObjectGetter<T> ObjectGetter { get; set; }
        private double ExpirationInHours { get; set; }

        public CachedObjectGetter(ObjectCache cache, IObjectGetter<T> objectGetter, double expirationInHours = 1.0)
        {
            Cache = cache;
            ObjectGetter = objectGetter;
            ExpirationInHours = expirationInHours;
        }

        public T Get(string id)
        {
            var obj = (T)Cache.Get(id);
            if (obj == null)
            {
                obj = ObjectGetter.Get(id);
                Cache.Set(id, obj, DateTimeOffset.Now.AddHours(ExpirationInHours));
            }
            return obj;
        }
    }
}