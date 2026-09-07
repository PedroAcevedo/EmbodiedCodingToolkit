using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskStatusResponse
{
    public TaskStatus task;
    public bool isCompleted;
    public bool isLastTask;
    public FailedTest failedTest;
    public string currentOutput;

    public List<TestCaseResultResponse> testResults;
}

[Serializable]
public class FailedTest
{
    public string inputs;
    public string output;
}

[Serializable]
public class TaskStatus
{
    public string id;
    public string title;
    public string description;
    public string functionName;
    public List<string> variables;
    public List<TaskStatusTest> testCases;
}

[Serializable]
public class TaskStatusTest
{
    public List<string> inputs;
    public string output;
}

[Serializable]
public class TestCaseResultResponse
{
    public string inputs;
    public string expectedOutput;
    public string currentOutput;
    public bool passed;
}