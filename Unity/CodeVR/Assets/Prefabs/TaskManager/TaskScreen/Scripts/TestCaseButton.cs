using TMPro;
using UnityEngine;

public class TestCaseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _testName;
    [SerializeField] private GameObject _passedIcon;
    [SerializeField] private GameObject _failedIcon;

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
}