using System.Collections;
using UnityEngine;

public class ProfessorGazeTrigger : MonoBehaviour
{
    public bool requestInProgress = false;

    [Header("Gaze")]
    [SerializeField] private Transform xrCamera;
    [SerializeField] private Collider professorCollider;
    [SerializeField] private float gazeDuration = 2f;
    [SerializeField] private float maxDistance = 10f;

    [Header("AI Context")]
    [SerializeField] private ChatGPTManager chatGPTManager;
    [SerializeField] private BlocklyCodeManager blocklyCodeManager;
    [SerializeField] private HeldCodeBlockTracker heldCodeBlockTracker;

    private float gazeTimer;
    private bool hasTriggered;

    private void Update()
    {
        if (IsLookingAtProfessor())
        {
            gazeTimer += Time.deltaTime;

            if (gazeTimer >= gazeDuration && !hasTriggered && !requestInProgress)
            {
                hasTriggered = true;
                StartCoroutine(BuildAndSendPrompt());
            }
        }
        else
        {
            gazeTimer = 0f;
            if (!requestInProgress)
            {
                hasTriggered = false;
            }
        }
    }

    private bool IsLookingAtProfessor()
    {
        if (xrCamera == null || professorCollider == null)
            return false;

        Ray ray = new Ray(xrCamera.position, xrCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            return hit.collider == professorCollider;
        }

        return false;
    }

    private IEnumerator BuildAndSendPrompt()
    {
        requestInProgress = true;
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
            "The student looked at the professor for help.\n\n" +
            $"Current task:\n{taskContext}\n\n" +
            $"Current Blockly code:\n{blocklyCode}\n\n" +
            $"Currently held blocks:\n{heldBlocks}\n\n";

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