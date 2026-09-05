using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScreen : MonoBehaviour
{
    [SerializeField] private GameObject _taskInfoContainer;
    [SerializeField] private GameObject _taskPanel;
    [SerializeField] private GameObject _testsPanel;
    [SerializeField] private GameObject _taskCompletedContainer;
    [SerializeField] private GameObject _taskLoadingContainer;

    [Header("Header")]
    [SerializeField] private ButtonColorChange _taskTab;
    [SerializeField] private ButtonColorChange _testsTab;
    
    [Header("Task Info")]
    [SerializeField] private TMPro.TMP_Text _title;
    [SerializeField] private TMPro.TMP_Text _description;
    [SerializeField] private TMPro.TMP_Text _testStatus;
    [SerializeField] private TMPro.TMP_Text _inputs;
    [SerializeField] private TMPro.TMP_Text _expectedOutput;
    [SerializeField] private TMPro.TMP_Text _currentOutput;
    [SerializeField] private AudioSource _audioSource;

    private TaskManager _taskManager;

    private bool _taskCompletedHasHappened = false;

    // Start is called before the first frame update
    void Start()
    {
        _taskManager = FindObjectOfType<TaskManager>();
        _taskManager.OnTaskStatusChange += OnTaskStatusChange;
        ShowTask();
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        CheckForTaskComplated(taskStatus);
        
        _title.text = taskStatus.task.title;
        _description.text = taskStatus.task.description;

        _taskCompletedContainer.SetActive(
            taskStatus.isCompleted && _taskManager.CurrentState == TaskManager.State.Ready
        );
        _taskLoadingContainer.SetActive(
            _taskManager.CurrentState == TaskManager.State.Loading
        );
        _taskInfoContainer.SetActive(
            !taskStatus.isCompleted && _taskManager.CurrentState == TaskManager.State.Ready
        );  
        
        if (_testStatus != null)
            _testStatus.text = "Tests failed when:";
            
        _inputs.text = taskStatus.failedTest?.inputs ?? "";
        _expectedOutput.text = taskStatus.failedTest.output;
        if (_currentOutput != null)
            _currentOutput.text = taskStatus.currentOutput;
    }

    private void CheckForTaskComplated(TaskStatusResponse taskStatus)
    {
        if (!_taskCompletedHasHappened && taskStatus.isCompleted)
        {
            _taskCompletedHasHappened = true;
            OnTaskCompleted();
        }
        if (!taskStatus.isCompleted)
            _taskCompletedHasHappened = false;
    }

    private void OnTaskCompleted()
    {
        _audioSource.Play();
        ShowTask();
    }

    public void ShowTask()
    {
        _taskTab.SetActiveState(true);
        _testsTab.SetActiveState(false);

        _taskPanel.SetActive(true);
        _testsPanel.SetActive(false);
    }

    public void ShowTests()
    {
        _taskTab.SetActiveState(false);
        _testsTab.SetActiveState(true);

        _taskPanel.SetActive(false);
        _testsPanel.SetActive(true);
    }
}
