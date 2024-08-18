using System;
using System.Reflection;
using UnityEngine;

namespace vwgamedev.GameCreator{
    public static class InteractiveReflectionUtility {
                
        public static void InvokeMethod(object target, ref MethodInfo cachedMethod, string methodName, params object[] parameters)
        {
            if (cachedMethod == null)
            {
                cachedMethod = GetMethodInHierarchy(target, methodName);
            }

            cachedMethod?.Invoke(target, parameters);
        }

        public static MethodInfo GetMethodInHierarchy(object target, string methodName)
        {
            Type type = target.GetType();
            MethodInfo method = null;

            while (type != null)
            {
                method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                if (method != null && method.DeclaringType != typeof(InteractiveMonoBehaviour))
                {
                    return method;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}

