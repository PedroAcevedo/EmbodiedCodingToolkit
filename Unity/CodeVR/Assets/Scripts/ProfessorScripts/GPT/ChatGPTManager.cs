using ElevenLabs.Demo;
using ElevenLabs.Models;
using OpenAI;
using OpenAI.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.OpenXR;

public enum LLMAPI
{
    OpenAI,
    AzureOpenAI
}

public enum TTSAPI
{
    ElevenLabsTTS
}

public enum BehaviorType
{
    Encouraging,
    Explanation,
    DirectSolution
}

public class ChatGPTManager : MonoBehaviour
{
    public GameObject TextToSpeech;
    public TranscriptManager TranscriptManager;
    public ConversationAnimation ConversationAnimation;
    [SerializeField] private GameObject Thoughtbubble;

    [Header("LLM API")]
    public LLMAPI LLMapi = LLMAPI.OpenAI;

    [Tooltip("Model used when LLMapi is set to OpenAI.")]
    public string OpenAIModel = "gpt-4o";

    [Tooltip("Model/deployment used when LLMapi is set to AzureOpenAI.")]
    public string AzureModel = "gpt-5-nano";

    [Tooltip("Azure auth file path used when LLMapi is set to AzureOpenAI.")]
    public string AzureAuthPath;

    [Tooltip("Azure OpenAI endpoint used when LLMapi is set to AzureOpenAI.")]
    public string AzureEndpoint = "acevedo-llm-avatars.openai.azure.com/openai/";

    public TTSAPI TTSapi;

    public BehaviorType BehaviorOption = BehaviorType.Encouraging;

    [TextArea(5, 20)] public string Role;
    [TextArea(5, 20)] public string Environment;
    [TextArea(5, 20)] public string Behavior;
    [TextArea(5, 20)] public string ResponseStyle;
    [TextArea(5, 20)] public string Slides;
    [TextArea(5, 20)] public string Other;

    public OnResponseEvent OnResponse;
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIClient openAI;
    private string previousResponseId;
    private string systemMessage;

    private void Start()
    {
        InitializeSelectedAPI();
        SetBehavior();
        BuildSystemMessage();
    }

    public OpenAIClient GetOpenAIInstance()
    {
        return openAI;
    }

    private void InitializeSelectedAPI()
    {
        if (LLMapi == LLMAPI.OpenAI)
        {
            OpenaiAPI apiKeyComponent = GetComponent<OpenaiAPI>();
            if (apiKeyComponent == null || string.IsNullOrWhiteSpace(apiKeyComponent.API_key))
            {
                Debug.LogError("Missing OpenaiAPI component or API_key for OpenAI mode.");
                return;
            }

            OpenAIAuthentication auth = new OpenAIAuthentication(apiKeyComponent.API_key);
            openAI = new OpenAIClient(auth);
        }
        else if (LLMapi == LLMAPI.AzureOpenAI)
        {
            OpenAIAuthentication auth = new OpenAIAuthentication(System.Environment.GetEnvironmentVariable("AZURE_KEY"));
            OpenAISettings settings = new OpenAISettings(AzureEndpoint);
            openAI = new OpenAIClient(auth, settings);
        }
        else
        {
            Debug.LogError("Select an LLM API in LLMAPI.");
        }
    }

    private void BuildSystemMessage()
    {
        systemMessage = Role + Environment + Behavior + ResponseStyle + Slides + Other;

        if (GPTConversationLog.instance != null)
        {
            GPTConversationLog.instance.AppendPromtpMessage(systemMessage);
        }
    }

    // Send inquiries to ChatGPT.
    public async void AskChatGPT(string prompt, string userQuestion, string currentCode, float questionTimestamp)
    {
        float responseStartTime = Time.realtimeSinceStartup;
        if (openAI == null)
        {
            InitializeSelectedAPI();
        }

        if (openAI == null)
        {
            Debug.LogError("LLM client was not initialized.");
            return;
        }

        if (string.IsNullOrWhiteSpace(systemMessage))
        {
            BuildSystemMessage();
        }

        string responseText = await AskLLM(prompt);

        if (!string.IsNullOrWhiteSpace(responseText))
        {
            
            HandleLLMResponse(prompt, userQuestion, responseText, questionTimestamp, currentCode, responseStartTime);
        }
    }

    private async Task<string> AskLLM(string userText)
    {
        string model = LLMapi == LLMAPI.AzureOpenAI ? AzureModel : OpenAIModel;

        CreateResponseRequest request = new CreateResponseRequest(
            userText,
            model: model,
            instructions: systemMessage,
            previousResponseId: previousResponseId
        );

        Thoughtbubble.SetActive(true);
        Response response = await openAI.ResponsesEndpoint.CreateModelResponseAsync(request);
        response.PrintUsage();
         
        previousResponseId = response.Id;

        string responseOutput = response.Output[LLMapi == LLMAPI.AzureOpenAI ? 1 : 0].ToString();

        if (responseOutput != null)
        {
            return responseOutput;
        }

        return response.ToString();
    }

    private void HandleLLMResponse(string prompt, string userQuestion, string responseText, float questionTimestamp, string currentCode, float responseStartTime)
    {
        Debug.Log(responseText);

        string processedResponse = ProcessResponseText(responseText);
        float responseTime = Time.realtimeSinceStartup - responseStartTime;
        

        if (TranscriptManager != null)
        {
            TranscriptManager.RecordInteraction(questionTimestamp, userQuestion, currentCode, processedResponse, responseTime, BehaviorOption);
        }

        if (ConversationAnimation != null)
        {
            var random = new System.Random();
            ConversationAnimation.SetSpeakingAnimation(random.Next(1, 4));
        }

        if (TTSapi == TTSAPI.ElevenLabsTTS)
        {
            if (TextToSpeech != null)
            {
                TextToSpeech.GetComponent<ElevenLabsTTS>().ElevenLabGenerateSpeech(responseText);
            }
        }
        else
        {
            Debug.LogError("Select a Text To Speech API in TTSAPI.");
        }

        OnResponse.Invoke(responseText);
    }

    private string ProcessResponseText(string str)
    {
        str = str.Replace(',', '.');
        str = str.Replace("C#", "C sharp");
        str = str.Replace("Metal Gear Solid V", "Metal Gear Solid five");

        string[] charsToRemove = { "*", "@", ":" };
        foreach (string c in charsToRemove)
        {
            str = str.Replace(c, string.Empty);
        }

        return str;
    }

    private void SetBehavior()
    {
        if (BehaviorOption == BehaviorType.Encouraging)
        {
            Behavior = @"Assistance Level: Encouragement Only

Your purpose is to encourage the student without helping solve the programming problem.

You may:
- Acknowledge progress.
- Point out that the student is making progress.
- Encourage experimentation.
- Ask the student to inspect or test their program.
- Acknowledge that something appears incorrect without explaining the solution.

You must NOT:
- Tell the student which block to use.
- Tell the student what algorithmic step comes next.
- Explain how to solve the current problem.
- Identify the correct condition, operation, formula, loop, or variable.
- Give pseudocode or a solution.
- Reveal an answer indirectly through a leading hint.

If the student directly asks for the answer, encourage them to continue working instead of providing it.";
        }
        else if (BehaviorOption == BehaviorType.Explanation)
        {
            Behavior = @"Assistance Level: Guided Assistance

Help the student make progress without giving them the complete solution.
First identify the specific obstacle in the student's current approach. Give one useful hint or explanation addressing that obstacle.

You may:
- Explain relevant programming concepts.
- Point out an incorrect assumption or block.
- Suggest the type of programming operation the student should consider.
- Ask a focused question that guides the student toward the next step.
- Explain why part of their current approach is not working.

Prefer hints based on the student's existing program.

Do NOT:
- Give the complete algorithm.
- Describe every remaining step.
- Construct the full Blockly solution for them.
- Give a complete sequence of blocks.
- Solve parts of the problem the student has not reached yet.

Reveal only enough information to help with the student's current obstacle. Let the student perform the next reasoning step.";
        }
        else if (BehaviorOption == BehaviorType.DirectSolution)
        {
            Behavior = @"Assistance Level: Direct Solution

Answer the student's question directly and provide the information needed to solve their current problem.

You may:
- Tell the student which blocks or operations to use.
- Explain the correct algorithm.
- Identify mistakes and state exactly how to correct them.
- Give the next steps directly.
- Describe a complete solution when the student asks for one.

Prefer solutions that use blocks available in the student's environment.
Even when giving the solution, explain the key reason it works so the student can learn from it.
Do not add unrelated information or continue teaching beyond what is necessary to answer the student's question.";
        }
    }

    //private void Update()
    //{
        // DEBUG ONLY
        //if (OVRInput.GetDown(OVRInput.Button.One)) // A button
        //{
            //loadingAnimatorObject.SetActive(true);
            //AskChatGPT("Hello!");
        //}
    //}
}
