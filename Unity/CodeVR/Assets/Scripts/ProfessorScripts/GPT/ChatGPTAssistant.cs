using ElevenLabs.Demo;
using OpenAI;
using OpenAI.Responses;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class ChatGPTAssistant : MonoBehaviour
{
    public GameObject TextToSpeech;
    public GameObject OutputTranscript;

    public TTSAPI TTSapi;

    [Header("OpenAI")]
    public string Model = "gpt-4o";

    [TextArea(5, 20)] public string instructions;
    [TextArea(5, 20)] public string persona;
    [TextArea(5, 20)] public string thoughts;
    [TextArea(5, 20)] public string info;
    [TextArea(5, 20)] public string syllabus105;
    [TextArea(5, 20)] public string syllabus345;

    public OnResponseEvent OnResponse;
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIClient openAI;
    private string previousResponseId;

    private void Start()
    {
        OpenaiAPI apiKeyComponent = GetComponent<OpenaiAPI>();
        if (apiKeyComponent == null || string.IsNullOrWhiteSpace(apiKeyComponent.API_key))
        {
            Debug.LogError("Missing OpenaiAPI component or API_key.");
            return;
        }

        OpenAIAuthentication auth = new OpenAIAuthentication(apiKeyComponent.API_key);
        openAI = new OpenAIClient(auth);
    }

    // Send inquiries to ChatGPT.
    public async void AskChatGPT(string newText)
    {
        if (openAI == null)
        {
            Debug.LogError("OpenAI client was not initialized.");
            return;
        }

        string systemInstructions = instructions + persona + thoughts + info + syllabus105 + syllabus345;
        string responseText = await AskLLM(newText, systemInstructions);

        if (string.IsNullOrWhiteSpace(responseText))
        {
            return;
        }

        Debug.Log(responseText);

        string replaced = responseText.Replace(',', '.');
        if (OutputTranscript != null)
        {
            OutputTranscript.GetComponent<OutputTranscript>().agentResponse.Add(replaced);
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

    private async Task<string> AskLLM(string userText, string systemInstructions)
    {
        CreateResponseRequest request = new CreateResponseRequest(
            userText,
            model: Model,
            instructions: systemInstructions,
            previousResponseId: previousResponseId
        );

        Response response = await openAI.ResponsesEndpoint.CreateModelResponseAsync(request);
        response.PrintUsage();

        previousResponseId = response.Id;

        if (!string.IsNullOrWhiteSpace(response.OutputText))
        {
            return response.OutputText;
        }

        return response.ToString();
    }

    public async void AskAssistant()
    {
        await Task.CompletedTask;
    }
}
