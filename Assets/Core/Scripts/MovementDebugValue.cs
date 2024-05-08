using TMPro;
using UnityEngine;

public class MovementDebugValue : MonoBehaviour
{
    [SerializeField] TMP_Text labelText;
    [SerializeField] TMP_Text valueText;

    public void SetValueText(string text) => valueText.SetText(text);
    public void SetlabelText(string text) => labelText.SetText(text);
}