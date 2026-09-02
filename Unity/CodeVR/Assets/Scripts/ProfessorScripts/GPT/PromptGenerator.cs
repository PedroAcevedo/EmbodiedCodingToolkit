using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptGenerator : MonoBehaviour
{
    public bool RequestInProgress = false;

     [Header("AI Context")]
    [SerializeField] private ChatGPTManager chatGPTManager;
    [SerializeField] private BlocklyCodeManager blocklyCodeManager;
    [SerializeField] private HeldCodeBlockTracker heldCodeBlockTracker;

    public IEnumerator BuildAndSendPrompt(string userPrompt)
    {
        if (RequestInProgress)
            yield break;

        RequestInProgress = true;
        TaskStatusResponse taskStatus = null;
        bool requestFailed = false;

        IEnumerator taskRequest = null;

        try
        {
            taskRequest = WebsiteConnection.GetTaskStatus(
                result => taskStatus = result,
                () => requestFailed = true
            );
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(
                $"Could not create task-status request: {e.Message}"
            );

            requestFailed = true;
        }

        if (taskRequest != null)
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                try
                {
                    keepRunning = taskRequest.MoveNext();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(
                        $"Task-status request failed: {e.Message}"
                    );

                    requestFailed = true;
                    break;
                }

                if (keepRunning)
                {
                    yield return taskRequest.Current;
                }
            }
        }

        string blocklyCode =
            blocklyCodeManager != null
                ? blocklyCodeManager.GenerateBlocklyCode()
                : "No Blockly code available.";

        string heldBlocks =
            heldCodeBlockTracker != null
                ? heldCodeBlockTracker.GetHeldBlocksDescription()
                : "The student is not currently holding a block.";

        string taskContext;

        if (requestFailed || taskStatus == null || taskStatus.task == null)
        {
            taskContext =
                "The current task could not be retrieved.";
        }
        else
        {
            taskContext = BuildTaskContext(taskStatus);
        }

        string prompt =
            $"Student Question: {userPrompt}\n" +
            $"Current Task:\n{taskContext}\n" +
            $"Current Blockly code:\n{blocklyCode}\n" +
            $"Currently held blocks:\n{heldBlocks}\n";

        Debug.Log(prompt);
        chatGPTManager.AskChatGPT(prompt);
    }

    private string BuildTaskContext(TaskStatusResponse response)
    {
        TaskStatus task = response.task;

        string variables = task.variables != null
            ? string.Join(", ", task.variables)
            : "None";

        return
            $"Title: {task.title}\n" +
            $"Description: {task.description}\n" +
            $"Function: {task.functionName}\n" +
            $"Variables: {variables}\n" +
            $"Completed: {response.isCompleted}\n" +
            $"Current Output: {response.currentOutput}";
    }

    private string GetHeldBlockDescription()
    {
        if (heldCodeBlockTracker == null)
        {
            return "The student is not currently holding a block.";
        }

        return heldCodeBlockTracker.GetHeldBlocksDescription();
    }
}
