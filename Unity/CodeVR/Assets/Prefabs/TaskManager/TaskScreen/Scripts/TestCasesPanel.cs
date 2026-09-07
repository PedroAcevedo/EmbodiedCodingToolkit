using UnityEngine;

public class TestCasesPanel : MonoBehaviour
{
    [SerializeField] private Transform _testCaseContainer;
    [SerializeField] private TestCaseButton _testCaseButtonPrefab;

    private TaskManager _taskManager;

    private void Start()
    {
        _taskManager = FindObjectOfType<TaskManager>();
        _taskManager.OnTaskStatusChange += OnTaskStatusChange;
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        PopulateTestCases(taskStatus);
    }

    private void PopulateTestCases(TaskStatusResponse taskStatus)
    {
        Debug.Log(taskStatus.testResults.Count + " test results found.");
        foreach (Transform child in _testCaseContainer)
        {
            Destroy(child.gameObject);
        }

        if (taskStatus.testResults == null)
        {
            Debug.LogWarning("Test results are null. Cannot populate test cases.");
            return;
        }

        for (int i = 0; i < taskStatus.testResults.Count; i++)
        {
            TestCaseButton button = Instantiate(
                _testCaseButtonPrefab,
                _testCaseContainer
            );

            button.Setup(
                i + 1,
                taskStatus.testResults[i]
            );
        }
    }
}