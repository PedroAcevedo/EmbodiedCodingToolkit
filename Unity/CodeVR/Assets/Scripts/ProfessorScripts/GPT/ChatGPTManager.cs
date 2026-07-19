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

public class ChatGPTManager : MonoBehaviour
{
    public GameObject TextToSpeech;
    public GameObject OutputTranscript;
    public ConversationAnimation ConversationAnimation;
    public GameObject loadingAnimatorObject;


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
        BuildSystemMessage();
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
    public async void AskChatGPT(string userText)
    {
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

        string responseText = await AskLLM(userText);

        if (!string.IsNullOrWhiteSpace(responseText))
        {
            HandleLLMResponse(responseText);
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

    private void HandleLLMResponse(string responseText)
    {
        Debug.Log(responseText);

        string processedResponse = ProcessResponseText(responseText);

        if (OutputTranscript != null)
        {
            OutputTranscript.GetComponent<OutputTranscript>().agentResponse.Add(processedResponse);
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
