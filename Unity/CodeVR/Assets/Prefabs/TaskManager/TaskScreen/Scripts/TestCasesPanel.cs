using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TestCasesPanel : MonoBehaviour
{
    [SerializeField] TaskScreenColors _colors;
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
    private TaskManager _taskManager;
    private List<TestCaseButton> _testCaseButtons = new List<TestCaseButton>();
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
                
                UpdateButtonSelection();
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
            _testCaseButtons.Add(button);
        }

        _testCasesTitle.text = $"Test Cases ({passedTestCount}/{taskStatus.testResults.Count})";
    }

    public void SelectTest(TestCaseResultResponse testResult, int testIndex)
    {
        _selectedTestIndex = testIndex;
        UpdateButtonSelection();

        UpdateTestInfo(
            testResult,
            testIndex + 1
        );
    }

    public void UpdateTestInfo(TestCaseResultResponse testResult, int testNumber)
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
            _currentOutput.color = _colors.TextColorPassed;
            _currentOutputLabel.color = _colors.TextColorPassed;
            _currentOutputBackground.color = _colors.BackgroundColorPassed;
        }
        else
        {
            _currentOutput.color = _colors.TextColorFailed;
            _currentOutputLabel.color = _colors.TextColorFailed;
            _currentOutputBackground.color = _colors.BackgroundColorFailed;
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

    private void UpdateButtonSelection()
    {
        for (int i = 0; i < _testCaseButtons.Count; i++)
        {
            if (i == _selectedTestIndex)
                _testCaseButtons[i].SetColor(_colors.ActiveButtonColor);
            else
                _testCaseButtons[i].SetColor(_colors.InactiveButtonColor);
        }
    }
}
