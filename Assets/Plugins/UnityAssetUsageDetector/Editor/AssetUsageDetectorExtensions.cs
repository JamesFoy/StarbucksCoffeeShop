using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetUsageDetectorNamespace
{
    public static class AssetUsageDetectorExtensions
    {
        // A set of commonly used Unity types
        private static HashSet<Type> primitiveUnityTypes = new HashSet<Type>() { typeof( string ), typeof( Vector3 ), typeof( Vector2 ), typeof( Rect ),
            typeof( Quaternion ), typeof( Color ), typeof( Color32 ), typeof( LayerMask ), typeof( Vector4 ),
            typeof( Matrix4x4 ), typeof( AnimationCurve ), typeof( Gradient ), typeof( RectOffset ) };

        // Get a unique-ish string hash code for an object
        public static string Hash( this object obj )
        {
            if( obj is Object )
                return obj.GetHashCode().ToString();

            return obj.GetHashCode() + obj.GetType().Name;
        }

        // Check if object is an asset or a Scene object
        public static bool IsAsset( this object obj )
        {
            return obj is Object && !string.IsNullOrEmpty( AssetDatabase.GetAssetPath( (Object) obj ) );
        }

        // Select an object in the editor
        public static void SelectInEditor( this Object obj )
        {
            if( obj == null )
                return;

            Event e = Event.current;

            // If CTRL key is pressed, do a multi-select;
            // otherwise select only the clicked object and ping it in editor
            if( !e.control )
            {
                obj.PingInEditor();
                Selection.activeObject = obj;
            }
            else
            {
                Component objAsComp = obj as Component;
                GameObject objAsGO = obj as GameObject;
                int selectionIndex = -1;

                Object[] selection = Selection.objects;
                for( int i = 0; i < selection.Length; i++ )
                {
                    Object selected = selection[i];

                    // Don't allow including both a gameobject and one of its components in the selection
                    if( selected == obj || ( objAsComp != null && selected == objAsComp.gameObject ) || 
                        (objAsGO != null && selected is Component && ( (Component) selected ).gameObject == objAsGO ) )
                    {
                        selectionIndex = i;
                        break;
                    }
                }

                Object[] newSelection;
                if( selectionIndex == -1 )
                {
                    // Include object in selection
                    newSelection = new Object[selection.Length + 1];
                    selection.CopyTo( newSelection, 0 );
                    newSelection[selection.Length] = obj;
                }
                else
                {
                    // Remove object from selection
                    newSelection = new Object[selection.Length - 1];
                    int j = 0;
                    for( int i = 0; i < selectionIndex; i++, j++ )
                        newSelection[j] = selection[i];
                    for( int i = selectionIndex + 1; i < selection.Length; i++, j++ )
                        newSelection[j] = selection[i];
                }

                Selection.objects = newSelection;
            }
        }

        // Ping an object in either Project view or Hierarchy view
        public static void PingInEditor( this Object obj )
        {
            if( obj is Component )
                obj = ( (Component) obj ).gameObject;

            // Pinging a prefab only works if the pinged object is the root of the prefab
            // or a direct child of it. Pinging any grandchildren of the prefab
            // does not work; in which case, traverse the parent hierarchy until
            // a pingable parent is reached
            if( obj.IsAsset() && obj is GameObject )
            {
                Transform objTR = ( (GameObject) obj ).transform;
                while( objTR.parent != null && objTR.parent.parent != null )
                    objTR = objTR.parent;

                obj = objTR.gameObject;
            }

            EditorGUIUtility.PingObject( obj );
        }

        // Check if object depends on any of the references
        public static bool HasAnyReferenceTo( this Object obj, HashSet<Object> references )
        {
            Object[] dependencies = EditorUtility.CollectDependencies( new Object[] { obj } );
            for( int i = 0; i < dependencies.Length; i++ )
            {
                if( references.Contains( dependencies[i] ) )
                    return true;
            }

            return false;
        }

        // Check if the field is serializable
        public static bool IsSerializable( this FieldInfo fieldInfo )
        {
            // see Serialization Rules: https://docs.unity3d.com/Manual/script-Serialization.html
            if( fieldInfo.IsInitOnly || ( ( !fieldInfo.IsPublic || fieldInfo.IsNotSerialized ) &&
                                          !Attribute.IsDefined( fieldInfo, typeof( SerializeField ) ) ) )
                return false;

            return IsTypeSerializable( fieldInfo.FieldType );
        }

        // Check if the property is serializable
        public static bool IsSerializable( this PropertyInfo propertyInfo )
        {
            return IsTypeSerializable( propertyInfo.PropertyType );
        }

        // Check if type is serializable
        private static bool IsTypeSerializable( Type type )
        {
            // see Serialization Rules: https://docs.unity3d.com/Manual/script-Serialization.html
            if( typeof( Object ).IsAssignableFrom( type ) )
                return true;

            if( type.IsArray )
            {
                if( type.GetArrayRank() != 1 )
                    return false;

                type = type.GetElementType();

                if( typeof( Object ).IsAssignableFrom( type ) )
                    return true;
            }
            else if( type.IsGenericType )
            {
                if( type.GetGenericTypeDefinition() != typeof( List<> ) )
                    return false;

                type = type.GetGenericArguments()[0];

                if( typeof( Object ).IsAssignableFrom( type ) )
                    return true;
            }

            if( type.IsGenericType )
                return false;

            return Attribute.IsDefined( type, typeof( SerializableAttribute ), false );
        }

        // Check if the type is a common Unity type (let's call them primitives)
        public static bool IsPrimitiveUnityType( this Type type )
        {
            return type.IsPrimitive || primitiveUnityTypes.Contains( type );
        }
		
        // Get <get> function for a field
        public static VariableGetVal CreateGetter( this FieldInfo fieldInfo, Type type )
        {
            // Commented the IL generator code below because it might actually be slower than simply using reflection
            // Credit: https://www.codeproject.com/Articles/14560/Fast-Dynamic-Property-Field-Accessors
            //DynamicMethod dm = new DynamicMethod( "Get" + fieldInfo.Name, fieldInfo.FieldType, new Type[] { typeof( object ) }, type );
            //ILGenerator il = dm.GetILGenerator();
            //// Load the instance of the object (argument 0) onto the stack
            //il.Emit( OpCodes.Ldarg_0 );
            //// Load the value of the object's field (fi) onto the stack
            //il.Emit( OpCodes.Ldfld, fieldInfo );
            //// return the value on the top of the stack
            //il.Emit( OpCodes.Ret );

            //return (VariableGetVal) dm.CreateDelegate( typeof( VariableGetVal ) );

            return fieldInfo.GetValue;
        }

        // Get <get> function for a property
        public static VariableGetVal CreateGetter( this PropertyInfo propertyInfo )
        {
            // Ignore indexer properties
            if( propertyInfo.GetIndexParameters().Length > 0 )
                return null;

            MethodInfo mi = propertyInfo.GetGetMethod( true );
            if( mi != null )
            {
                Type GenType = typeof( PropertyWrapper<,> ).MakeGenericType( propertyInfo.DeclaringType, propertyInfo.PropertyType );
                return ( (IPropertyAccessor) Activator.CreateInstance( GenType, mi ) ).GetValue;
            }

            return null;
        }
    }
}