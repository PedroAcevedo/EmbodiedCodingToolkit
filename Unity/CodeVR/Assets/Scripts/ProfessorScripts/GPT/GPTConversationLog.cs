using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GPTConversationLog : MonoBehaviour
{
    public static GPTConversationLog instance;

    public int userID;
    public List<Conversation> promptToGPT;
    public List<Conversation> responseFromGPT;
    public int currentPrompt;

    private Stopwatch sessionWatch;
    public DateTime sessionStartUtc;
    private DateTime sessionEndUtc;
    [Tooltip("Use unscaled time if you ever pause timeScale.")]
    public bool useUnscaledTime = true;
    private double gazeSeconds = 0.0;
    private int gazeActiveCount = 0; // ref-count for overlapping gazes

    private void Awake()
    {
        instance = this;

        promptToGPT = new List<Conversation>();
        responseFromGPT = new List<Conversation>();

        currentPrompt = 0;

        sessionWatch = new Stopwatch();
        sessionStartUtc = DateTime.UtcNow;
        sessionWatch.Start();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gazeActiveCount > 0)
        {
            gazeSeconds += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }
    }
    void OnApplicationQuit()
    {
        if (sessionWatch != null && sessionWatch.IsRunning)
            sessionWatch.Stop();
        sessionEndUtc = DateTime.UtcNow;

        //saveData(); // Once you stop the application, a new log will be saved!
    }

    public void BeginGaze()
    {
        gazeActiveCount = Math.Max(0, gazeActiveCount + 1);
    }

    public void EndGaze()
    {
        gazeActiveCount = Math.Max(0, gazeActiveCount - 1);
    }

    public void AppendPromtpMessage(string message)
    {
        if(promptToGPT.Count == currentPrompt)
        {
            promptToGPT.Add(new Conversation(""));
        }

        promptToGPT[currentPrompt].UpdateMessage(message);
    }

    public void AddResponse(string message)
    {
        currentPrompt++;
        responseFromGPT.Add(new Conversation(message));
    }

    private static string IsoUtc(DateTime dt) => dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); // NEW

    private void saveData()
    {
        //save transcript to file
        string path = Path.Combine(Application.dataPath, "Output/");
        string filePath = Path.Combine(path, userID.ToString() + "_" + SceneManager.GetActiveScene().name + "_" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + ".txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("==============SESSION TIMING==============");
            var end = (sessionEndUtc == default) ? DateTime.UtcNow : sessionEndUtc;
            var elapsed = (sessionWatch != null) ? sessionWatch.Elapsed : TimeSpan.Zero;
            writer.WriteLine($"SessionStartUTC: {IsoUtc(sessionStartUtc)}");
            writer.WriteLine($"SessionEndUTC:   {IsoUtc(end)}");
            writer.WriteLine($"SessionDurationSeconds: {elapsed.TotalSeconds:F3}");
            writer.WriteLine($"GazeAccumulatedSeconds: {gazeSeconds:F3}");
            writer.WriteLine();
        }

    }
}

[System.Serializable]
public class Conversation
{
    public string message;
    public string time;

    public Conversation(string message)
    {
        this.message = message;
        this.time = DateTime.Now.ToLongTimeString();
    }

    public void UpdateMessage(string message)
    {
        this.message += "\n" + message;
        this.time = DateTime.Now.ToLongTimeString();

    }
}
