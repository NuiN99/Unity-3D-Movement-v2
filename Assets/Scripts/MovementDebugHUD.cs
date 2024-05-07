using System.Collections.Generic;
using NuiN.Movement;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.UI;

public class MovementDebugHUD : MonoBehaviour
{
    [SerializeField] VerticalLayoutGroup verticalLayout;
    [SerializeField] MovementDebugValue debugValuePrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] GroundFloater groundFloater;

    Dictionary<string, MovementDebugValue> _debugValues = new();

    float _jumpStartHeight;
    float _jumpEndHeight;
    bool _jumping;

    void OnEnable() => groundFloater.OnFinishedJump += FinishedJump;
    void OnDisable() => groundFloater.OnFinishedJump -= FinishedJump;

    void FixedUpdate()
    {
        string speedXZ = rb.velocity.With(y: 0).magnitude.ToString("0.00");
        UpdateOrCreateDebugValue("SPEED XZ", speedXZ);

        string speedY = rb.velocity.y.ToString("0.00");
        UpdateOrCreateDebugValue("SPEED Y", speedY);

        float curHeight = rb.position.y;
        
        if (!_jumping && rb.velocity.y > 0)
        {
            _jumpStartHeight = curHeight;
            _jumpEndHeight = curHeight;
            _jumping = true;
        }

        if (!_jumping) return;
        
        float maxHeight = Mathf.Max(rb.position.y - _jumpStartHeight, _jumpEndHeight - _jumpStartHeight);

        if (curHeight <= _jumpEndHeight) return;
        _jumpEndHeight = curHeight;
        UpdateOrCreateDebugValue("MAX Y", maxHeight.ToString("0.00"));
    }

    void UpdateOrCreateDebugValue(string label, string value)
    {
        if (_debugValues.TryGetValue(label, out MovementDebugValue debugValue))
        {
            debugValue.SetValueText(value);
            return;
        }
        _debugValues.Add(label, InitializeDebugValue(label, value));
    }
    
    MovementDebugValue InitializeDebugValue(string label, string value)
    {
        MovementDebugValue debugValue = Instantiate(debugValuePrefab, verticalLayout.transform);
        debugValue.SetlabelText(label);
        debugValue.SetValueText(value);

        return debugValue;
    }

    void FinishedJump()
    {
        UpdateOrCreateDebugValue("MAX Y", (_jumpEndHeight - _jumpStartHeight).ToString("0.00"));
        _jumping = false;
    }
}