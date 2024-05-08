using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public static void Enable(this InputActionProperty inputActionProperty)
        {
            inputActionProperty.action.Enable();
        }

        public static void AddHandler(this InputActionProperty inputActionProperty, Action<InputAction.CallbackContext> action)
        {
            inputActionProperty.action.performed += action;
        }
        
        public static void RemoveHandler(this InputActionProperty inputActionProperty, Action<InputAction.CallbackContext> action)
        {
            inputActionProperty.action.performed -= action;
        }
    }
}