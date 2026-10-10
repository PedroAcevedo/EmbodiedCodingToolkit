using System;
using System.Collections.Generic;
using UnityEngine;

public class StudyConfiguration : MonoBehaviour
{
    [SerializeField] private TextAsset _csvFile;
    [SerializeField] private int _studentId = 1;

    private List<string> _taskIds = new List<string>();
    private Dictionary<string, BehaviorType> _behaviorTypes =
        new Dictionary<string, BehaviorType>();

    private void Awake()
    {
        LoadCSV();
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
}