using System.Reflection;

namespace AssetUsageDetectorNamespace
{
    public struct VariableGetterHolder
    {
        public readonly string name;
        public readonly bool isProperty;
        public readonly bool isSerializable;
        private readonly VariableGetVal getter;

        public VariableGetterHolder( FieldInfo fieldInfo, VariableGetVal getter, bool isSerializable )
        {
            name = fieldInfo.Name;
            isProperty = false;
            this.isSerializable = isSerializable;
            this.getter = getter;
        }

        public VariableGetterHolder( PropertyInfo propertyInfo, VariableGetVal getter, bool isSerializable )
        {
            name = propertyInfo.Name;
            isProperty = true;
            this.isSerializable = isSerializable;
            this.getter = getter;
        }

        public object Get( object obj )
        {
            return getter( obj );
        }
    }
}