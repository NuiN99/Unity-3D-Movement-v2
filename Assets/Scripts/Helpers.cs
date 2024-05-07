using UnityEngine;

namespace NuiN.Movement
{
    public static class Helpers
    {
        public static T GetInHierarchy<T>(this Component obj)
        {
            T t = obj.GetComponent<T>();
            if (t == null) t = obj.GetComponentInParent<T>();
            else return t;
            if (t == null) t = obj.GetComponentInChildren<T>();
            return t;
        }
    }
}