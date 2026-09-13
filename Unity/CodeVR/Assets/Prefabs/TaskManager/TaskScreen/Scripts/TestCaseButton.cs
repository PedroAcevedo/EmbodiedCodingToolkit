using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestCaseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _testName;
    [SerializeField] private GameObject _passedIcon;
    [SerializeField] private GameObject _failedIcon;
    [SerializeField] private Image _background;
    private TestCaseResultResponse _testResult;
    private TestCasesPanel _testCasesPanel;
    private int _testIndex;

    public void Setup(
        int testNumber,
        int testIndex,
        TestCaseResultResponse testResult,
        TestCasesPanel testCasesPanel)
    {
        _testResult = testResult;
        _testCasesPanel = testCasesPanel;
        _testIndex = testIndex;

        _testName.text = "Test Case " + testNumber;

        if (testResult.passed)
        {
            _passedIcon.SetActive(true);
            _failedIcon.SetActive(false);
        }
        else
        {
            _passedIcon.SetActive(false);
            _failedIcon.SetActive(true);
        }
    }

    public void OnClick()
    {
        if (_testCasesPanel != null)
        {
            _testCasesPanel.SelectTest(
                _testResult,
                _testIndex
            );
        }
    }

    public void SetColor(Color color)
    {
        _background.color = color;
    }
   
}