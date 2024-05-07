using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuiN.Movement
{
    public static class Helpers
    {
        public static Rigidbody GetRidibodyInHierarchy(Transform obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) rb = obj.GetComponentInParent<Rigidbody>();
            else return rb;
            if (rb == null) rb = obj.GetComponentInChildren<Rigidbody>();
            return rb;
        }
    }
}