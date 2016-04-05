using System;
using Q3Server.Interfaces;

namespace Q3Server
{
    public class ObjectGetterTernary<T> : IObjectGetter<T>
    {
        private IObjectGetter<T> Getter1 { get; set; } 
        private IObjectGetter<T> Getter2 { get; set; }
        private Func<T, bool> ConditionOnFirstObject { get; set; }

        public ObjectGetterTernary(IObjectGetter<T> getter1,
                                   IObjectGetter<T> getter2,
                                   Func<T, bool> conditionOnFirstObject)
        {
            Getter1 = getter1;
            Getter2 = getter2;
            ConditionOnFirstObject = conditionOnFirstObject;
        }

        public T Get(string id)
        {
            var obj1 = Getter1.Get(id);
            return ConditionOnFirstObject(obj1) ? obj1 : Getter2.Get(id);
        }
    }
}