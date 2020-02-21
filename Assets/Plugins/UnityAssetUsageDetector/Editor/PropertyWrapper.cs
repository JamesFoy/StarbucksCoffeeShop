using System;
using System.Reflection;

namespace AssetUsageDetectorNamespace
{
    /// A wrapper class for properties to get their values more efficiently
    public class PropertyWrapper<TObject, TValue> : IPropertyAccessor where TObject : class
    {
        private readonly Func<TObject, TValue> getter;

        public PropertyWrapper( MethodInfo getterMethod )
        {
            getter = (Func<TObject, TValue>) Delegate.CreateDelegate( typeof( Func<TObject, TValue> ), getterMethod );
        }

        public object GetValue( object obj )
        {
            try
            {
                return getter( (TObject) obj );
            }
            catch
            {
                // Property getters may return various kinds of exceptions
                // if their backing fields are not initialized (yet)
                return null;
            }
        }
    }
}