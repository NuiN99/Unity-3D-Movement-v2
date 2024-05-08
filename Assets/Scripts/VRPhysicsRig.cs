using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPhysicsRig : MonoBehaviour
{
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    [SerializeField] ConfigurableJoint leftHandJoint;
    [SerializeField] ConfigurableJoint rightHandJoint;

    void FixedUpdate()
    {
        if (leftHand.localPosition != Vector3.zero)
        {
            leftHandJoint.targetPosition = leftHand.localPosition;
            leftHandJoint.targetRotation = leftHand.localRotation;
        }

        if (rightHand.localPosition != Vector3.zero)
        {
            rightHandJoint.targetPosition = rightHand.localPosition;
            rightHandJoint.targetRotation = rightHand.localRotation;
        }
    }
}
