using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject underline;

    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    public void SetActiveState(bool active)
    {
        icon.color = active ? activeColor : inactiveColor;
        label.color = active ? activeColor : inactiveColor;
        underline.SetActive(active);
    }
}