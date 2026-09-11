using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestCasesPanel : MonoBehaviour
{
    [SerializeField] private Transform _testCaseContainer;
    [SerializeField] private RectTransform _testInfoContainer;
    [SerializeField] private TestCaseButton _testCaseButtonPrefab;
    [SerializeField] private TMP_Text _testCasesTitle;
    [SerializeField] private TMP_Text _testCaseTitle;
    [SerializeField] private TMP_Text _input;
    [SerializeField] private TMP_Text _expectedOutput;
    [SerializeField] private TMP_Text _currentOutput;
    [SerializeField] private TMP_Text _currentOutputLabel;
    [SerializeField] private Image _currentOutputBackground;
    [SerializeField] private Color _textColorPassed;
    [SerializeField] private Color _textColorFailed;
    [SerializeField] private Color _backgroundColorPassed;
    [SerializeField] private Color _backgroundColorFailed;

    private TaskManager _taskManager;
    private string _currentTaskID = "";
    private string _lastTestResults = "";
    private int passedTestCount = 0;
    private int _selectedTestIndex = -1;

    private void Start()
    {
        _taskManager = FindObjectOfType<TaskManager>();
        _taskManager.OnTaskStatusChange += OnTaskStatusChange;
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        bool isNewTask = taskStatus.task.id != _currentTaskID;

        if (isNewTask)
        {
            _currentTaskID = taskStatus.task.id;
            _selectedTestIndex = -1;
            _lastTestResults = "";
        }

        if (taskStatus.testResults == null)
            return;

        string newTestResults = GetTestResultsSignature(taskStatus);

        if (newTestResults != _lastTestResults)
        {
            _lastTestResults = newTestResults;

            PopulateTestCases(taskStatus);

            if (taskStatus.testResults.Count > 0)
            {
                if (_selectedTestIndex < 0 ||
                    _selectedTestIndex >= taskStatus.testResults.Count)
                {
                    _selectedTestIndex = 0;
                }

                UpdateTestInfo(
                    taskStatus.testResults[_selectedTestIndex],
                    _selectedTestIndex + 1
                );
            }
        }
    }

    private void PopulateTestCases(TaskStatusResponse taskStatus)
    {
        foreach (Transform child in _testCaseContainer)
        {
            Destroy(child.gameObject);
        }

        passedTestCount = 0;

        for (int i = 0; i < taskStatus.testResults.Count; i++)
        {
            TestCaseButton button = Instantiate(
                _testCaseButtonPrefab,
                _testCaseContainer
            );

            button.Setup(
                i + 1,
                i,
                taskStatus.testResults[i],
                this
            );

            if (taskStatus.testResults[i].passed)
            {
                passedTestCount++;
            }
        }

        _testCasesTitle.text =
            $"Test Cases ({passedTestCount}/{taskStatus.testResults.Count})";
    }

    public void SelectTest(TestCaseResultResponse testResult, int testIndex)
    {
        _selectedTestIndex = testIndex;

        UpdateTestInfo(
            testResult,
            testIndex + 1
        );
    }

    public void UpdateTestInfo(
        TestCaseResultResponse testResult,
        int testNumber)
    {
        _testCaseTitle.text = $"Test Case {testNumber}";
        _input.text = testResult.inputs;
        _expectedOutput.text = testResult.expectedOutput;
        _currentOutput.text =
            testResult.currentOutput == null
                ? "Null"
                : testResult.currentOutput;

        if (testResult.passed)
        {
            _currentOutput.color = _textColorPassed;
            _currentOutputLabel.color = _textColorPassed;
            _currentOutputBackground.color = _backgroundColorPassed;
        }
        else
        {
            _currentOutput.color = _textColorFailed;
            _currentOutputLabel.color = _textColorFailed;
            _currentOutputBackground.color = _backgroundColorFailed;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_testInfoContainer);
    }

    private string GetTestResultsSignature(TaskStatusResponse taskStatus)
    {
        string result = "";

        foreach (var testResult in taskStatus.testResults)
        {
            result += testResult.inputs;
            result += testResult.expectedOutput;
            result += testResult.currentOutput;
            result += testResult.passed.ToString();
        }

        return result;
    }
}