using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;



public class StudyConfiguration : MonoBehaviour
{
    [SerializeField] private string _taskOrderUrl = "http://localhost:8999/api/task-order";
    [SerializeField] private TextAsset _csvFile;
    [SerializeField] private int _studentId = 1;
    [SerializeField] private ChatGPTManager _chatGPTManager;
    private List<string> _taskIds = new List<string>();
    private Dictionary<string, BehaviorType> _behaviorTypes = new Dictionary<string, BehaviorType>();
    [Serializable]
    private class TaskOrderRequest
    {
        public string[] taskIds;
    }
    private TaskManager _taskManager;
    private string _currentBehaviorTaskId = "";
    private bool _taskOrderSent = false;


    private void Awake()
    {
        LoadCSV();
    }

    private void Start()
    {
        _taskManager = FindObjectOfType<TaskManager>();
        _taskManager.OnTaskStatusChange += OnTaskStatusChange;
        StartCoroutine(SendTaskOrder());
    }

    private void LoadCSV()
    {
        var lines = _csvFile.text.Split('\n');
        var columns = lines[_studentId - 1].Split(',');

        for (int j = 0; j < columns.Length; j += 2)
        {
            var taskId = columns[j].Trim();
            var behaviorType = (BehaviorType)Enum.Parse(
                typeof(BehaviorType), columns[j + 1].Trim()
            );

            _taskIds.Add(taskId);
            _behaviorTypes[taskId] = behaviorType;
        }

        Debug.Log("Loaded study configuration for student " + _studentId);
        Debug.Log("Task order: " + string.Join(", ", _taskIds));
    }
    
    public string[] GetTaskIds()
    {
        return _taskIds.ToArray();
    }

    public BehaviorType GetBehaviorType(string taskId)
    {
        return _behaviorTypes[taskId];
    }

    private IEnumerator SendTaskOrder()
    {
        var requestData = new TaskOrderRequest
        {
            taskIds = _taskIds.ToArray()
        };

        var json = JsonUtility.ToJson(requestData);

        using (var request = new UnityWebRequest(_taskOrderUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Failed to send task order: " + request.error);
            else
            {
                Debug.Log("Task order sent to website: " + json);
                _taskOrderSent = true;
                UpdateBehavior(_taskIds[0]);
            }
        }   
    }

    private void OnTaskStatusChange(TaskStatusResponse taskStatus)
    {
        if (!_taskOrderSent)
            return;

        UpdateBehavior(taskStatus.task.id);
    }

    private void UpdateBehavior(string taskId)
    {
        if (taskId == _currentBehaviorTaskId)
            return;

        if (!_behaviorTypes.TryGetValue(taskId, out BehaviorType behaviorType))
            return;

        _currentBehaviorTaskId = taskId;

        _chatGPTManager.SetBehaviorForTask(behaviorType);

        Debug.Log("Task: " + taskId + " | Behavior: " + behaviorType);
    }
}
