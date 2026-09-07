using TMPro;
using UnityEngine;

public class TestCaseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _testName;

    [SerializeField] private GameObject _passedIcon;
    [SerializeField] private GameObject _failedIcon;

    [SerializeField] private Color _passedColor;
    [SerializeField] private Color _failedColor;

    private TestCaseResultResponse _testResult;

    public void Setup(int testNumber, TestCaseResultResponse testResult)
    {
        _testResult = testResult;

        _testName.text = "Test Case " + testNumber;

        if (testResult.passed)
        {
            _testName.color = _passedColor;

            _passedIcon.SetActive(true);
            _failedIcon.SetActive(false);
        }
        else
        {
            _testName.color = _failedColor;

            _passedIcon.SetActive(false);
            _failedIcon.SetActive(true);
        }
    }
}