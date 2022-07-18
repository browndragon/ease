using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BDEase
{
    public class Registry
    {
        [AttributeUsage(
            AttributeTargets.Class | AttributeTargets.Struct
            , AllowMultiple = true)]
        public abstract class ProvidesAttribute : Attribute
        {
            internal readonly Type Key;
            public ProvidesAttribute(Type key) => Key = key;
        }
        readonly Dictionary<Type, object> registry = new();
        public object Get(Type t) => registry.TryGetValue(t, out var arith) ? arith : default;
        /// Populates map from e.g. float->FloatArith.
        /// We require IDefaultArith's instance ALSO implement IArith<that type>.
        public void Register(Type t, object instance) => registry[t] = instance;

        public int RegisterByAttribute<T>(Type t) where T : ProvidesAttribute
        {
            int count = 0;
            object cached = null;
            foreach (Attribute attribute in Attribute.GetCustomAttributes(t, typeof(T)))
            {
                T provides = (T)attribute;
                if (provides == null) throw new ArgumentException($"{t} not compatible with {typeof(T)}");
                Type key = provides.Key;
                if (cached == null) cached = Activator.CreateInstance(t);
                Register(key, cached);
            }
            return count;
        }
        public int RegisterAssemblyContaining<T>(Type rootType) where T : ProvidesAttribute
        {
            /// No new ones added; we already had this element:
            if (registry.TryGetValue(rootType, out var had) && ReferenceEquals(had, rootType)) return 0;
            Assembly assembly = rootType.Assembly;
            int count = 0;
            foreach (var t in assembly.GetTypes()) count += RegisterByAttribute<T>(t);
            return count;
        }
    }
}