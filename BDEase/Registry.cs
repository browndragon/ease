using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BDEase
{
    /// Extensions, statics, etc.
    public static class Registry
    {
        /// Marks a type as Registrable
        /// (though specific registries still need to RegisterByAttribute or RegisterAssemblyContaining somewhere relevant).
        /// Requires subclasses specific to your domain to improve the odds you don't confuse your registries.
        [AttributeUsage(
            AttributeTargets.Class | AttributeTargets.Struct
            , AllowMultiple = true)]
        public abstract class ProvidesAttribute : Attribute
        {
            /// This would be some `TK` instead of `object` but generic attributes are a future feature.
            /// Still, it's on you: be consistent about your registry-to-keytypes.
            internal readonly object Key;
            public ProvidesAttribute(object key) => Key = key;
        }
    }
    /// A dictionary TK->TV optimized for reflective population of specific (static)
    /// values to (single) instances of some TV (usually identified by class + 0-arg ctor).
    public class Registry<TK, TV> : IEnumerable<KeyValuePair<TK, TV>>
    {
        readonly Dictionary<Type, int> typeCount = new();
        readonly Dictionary<TK, TV> registry = new();
        public TV Get(TK k, TV @default = default) => registry.TryGetValue(k, out var arith) ? arith : @default;
        /// Populates map from e.g. float->FloatArith.
        /// This is explicit; it doesn't check the typesAndAssemblies protective map.
        public void Register(TK k, TV instance) => registry[k] = instance;

        /// Constraint: TV must be assignablefrom T.
        public int RegisterByAttribute<TProvides>(Type t) where TProvides : Registry.ProvidesAttribute
        {
            if (!typeof(TV).IsAssignableFrom(t)) return 0;
            if (typeCount.TryGetValue(t, out int count)) return count;
            count = 0;
            TV cached = default;
            foreach (Attribute attribute in Attribute.GetCustomAttributes(t, typeof(TProvides)))
            {
                TProvides provides = (TProvides)attribute;
                if (provides == null) throw new ArgumentException($"{t} not compatible with {typeof(TProvides)}");
                TK key = (TK)provides.Key;
                if (cached == null) cached = (TV)Activator.CreateInstance(t);
                Register(key, cached);
            }
            typeCount[t] = count;
            return count;
        }

        public int RegisterAssemblyContaining<TProvides>(Type rootType) where TProvides : Registry.ProvidesAttribute
        {
            /// No new ones added; we already had this element:
            if (typeCount.TryGetValue(rootType, out int count)) return count;
            Assembly assembly = rootType.Assembly;
            count = 0;
            foreach (var t in assembly.GetTypes()) count += RegisterByAttribute<TProvides>(t);
            typeCount[rootType] = count;
            return count;
        }
        public int Count => registry.Count;
        public void Clear()
        {
            typeCount.Clear();
            registry.Clear();
        }
        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator() => registry.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}