
using UnityEngine;
using UnityEngine.UI;

public class TutorialButtonActive : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    
    private void Start()
    {
        _startButton.interactable = false;
    }

    public void ActivateStartButton()
    {
        _startButton.interactable = true;
    }
}
