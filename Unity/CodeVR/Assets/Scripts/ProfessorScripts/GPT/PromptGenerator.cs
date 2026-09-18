using System.Collections;
using UnityEngine;

public class PromptGenerator : MonoBehaviour
{
    public bool RequestInProgress = false;

    [SerializeField] private TranscriptManager _transcriptManager;

    [Header("AI Context")]
    [SerializeField] private ChatGPTManager chatGPTManager;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private HeldCodeBlockTracker heldCodeBlockTracker;

    public IEnumerator BuildAndSendPrompt(string userPrompt)
    {
        if (RequestInProgress)
            yield break;

        RequestInProgress = true;

        TaskStatusResponse taskStatus = taskManager.CurrentTaskStatus;

        string heldBlocks =
            heldCodeBlockTracker != null
                ? heldCodeBlockTracker.GetHeldBlocksDescription()
                : "The student is not currently holding a block.";

        string taskContext;

        if (taskStatus == null || taskStatus.task == null)
        {
            taskContext =
                "The current task could not be retrieved.";
        }
        else
        {
            taskContext = BuildTaskContext(taskStatus);
        }

        string currentCode =
            taskStatus != null && !string.IsNullOrEmpty(taskStatus.currentCode)
                ? taskStatus.currentCode
                : "No code available.";

        string prompt =
            $"Student Question: {userPrompt}\n\n" +
            $"Current Task:\n{taskContext}\n\n" +
            $"Current Code:\n{currentCode}\n\n" +
            $"Currently held blocks:\n{heldBlocks}\n";

        float questionTimestamp =
            _transcriptManager.GetCurrentTaskTime();

        chatGPTManager.AskChatGPT(
            prompt,
            userPrompt,
            currentCode,
            questionTimestamp
        );

        yield break;
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