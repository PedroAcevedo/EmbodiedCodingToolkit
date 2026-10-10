using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;



public class StudyConfiguration : MonoBehaviour
{
    [SerializeField] private string _taskOrderUrl =
    "http://localhost:8999/api/task-order";
    [SerializeField] private TextAsset _csvFile;
    [SerializeField] private int _studentId = 1;

    [Serializable]
    private class TaskOrderRequest
    {
        public string[] taskIds;
    }

    private List<string> _taskIds = new List<string>();
    private Dictionary<string, BehaviorType> _behaviorTypes =
        new Dictionary<string, BehaviorType>();

    private void Awake()
    {
        LoadCSV();
    }

    private void Start()
    {
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
                Debug.Log("Task order sent to website: " + json);
        }   
    }
}
