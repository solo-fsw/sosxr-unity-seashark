using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.Editor
{
    public static class ReflectionExtensions
    {
        private static readonly Type[] UnityBaseTypes =
        {
            typeof(MonoBehaviour),
            typeof(ScriptableObject),
            typeof(Object),
            typeof(Component),
            typeof(Behaviour)
        };

        private static readonly Type[] PropFieldTypes =
        {
            typeof(Color),
            // typeof(float),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(AnimationCurve)
        };


        public static bool IsUnityBaseMember(this MemberInfo member)
        {
            return UnityBaseTypes.Contains(member.DeclaringType);
        }


        public static bool IsWrongType(this MemberInfo member)
        {
            var memberType = member switch
                             {
                                 PropertyInfo propertyInfo => propertyInfo.PropertyType,
                                 FieldInfo fieldInfo => fieldInfo.FieldType,
                                 _ => null
                             };

            return memberType != null && PropFieldTypes.Contains(memberType);
        }
    }
}