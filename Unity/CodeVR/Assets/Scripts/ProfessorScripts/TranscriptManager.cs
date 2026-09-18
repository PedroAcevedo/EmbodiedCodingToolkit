using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TranscriptManager : MonoBehaviour
{
    [SerializeField] private int userId;
    private string _currentTaskId;
    private float _taskStartTime;
    private string _filePath;


    void Start()
    {
        string path = Path.Combine(Application.dataPath, "Output/");
        Directory.CreateDirectory(path);

        _filePath = Path.Combine(path, userId.ToString() + "_" + SceneManager.GetActiveScene().name + ".csv");

        if (!File.Exists(_filePath))
        {
            File.AppendAllText(_filePath, "UserId,TaskId,Timestamp,Prompt,CurrentCode,Response,ResponseTime,BehaviorType\n");
        }
    }
    public void OnNewTask(string taskId)
    {
        _currentTaskId = taskId;
        _taskStartTime = Time.realtimeSinceStartup;
    }

    public float GetCurrentTaskTime()
    {
        return Time.realtimeSinceStartup - _taskStartTime;
    }

    public void RecordInteraction(
        float timestamp,
        string prompt,
        string currentCode,
        string response,
        float responseTime,
        BehaviorType behaviorType)
    {
        TranscriptEntry entry = new TranscriptEntry
        {
            userId = userId,
            taskId = _currentTaskId,
            timestamp = timestamp,
            prompt = prompt,
            currentCode = currentCode,
            response = response,
            responseTime = responseTime,
            behaviorType = behaviorType
        };

        WriteEntry(entry);
    }

    
    private void WriteEntry(TranscriptEntry entry)
    {
        string line =
            entry.userId + "," +
            EscapeCsv(entry.taskId) + "," +
            entry.timestamp + "," +
            EscapeCsv(entry.prompt) + "," +
            EscapeCsv(entry.currentCode) + "," +
            EscapeCsv(entry.response) + "," +
            entry.responseTime + "," +
            EscapeCsv(entry.behaviorType.ToString());

        File.AppendAllText(_filePath, line + "\n");
    }
    


    private string EscapeCsv(string value)
    {
        if (value == null)
            return "";

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}



[Serializable]
public class TranscriptEntry
{
    public int userId;
    public string taskId;
    public float timestamp;
    public string prompt;
    public string currentCode;
    public string response;
    public float responseTime;
    public BehaviorType behaviorType;
}