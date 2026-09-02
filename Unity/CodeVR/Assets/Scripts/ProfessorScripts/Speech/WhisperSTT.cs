using OpenAI;
using OpenAI.Audio;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Samples.Whisper
{
    public class WhisperSTT : MonoBehaviour
    {
        public InputActionReference recordAction;
        public float responseTimer = 0;

        [SerializeField] private GameObject OutputTranscript;
        [SerializeField] private GameObject Thoughtbubble;
        [SerializeField] private TMPro.TextMeshProUGUI message;
        [SerializeField] private GameObject RecordingLight;
        [SerializeField] private GameObject RecordingText;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private PromptGenerator promptGenerator;

        private readonly string fileName = "output.wav";
        private readonly int maxDuration = 30;

        private ChatGPTManager ChatGPTManager;
        private AudioClip clip;
        private bool isRecording = false;
        private int index;

        private void Awake()
        {
            recordAction.action.Enable();
            recordAction.action.performed += ToggleRecording;
        }

        private void OnDestroy()
        {
            recordAction.action.Disable();
            recordAction.action.performed -= ToggleRecording;
        }

        private void Start()
        {
            ChatGPTManager = GetComponent<ChatGPTManager>();

#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
            foreach (string device in Microphone.devices)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(device));
            }

            dropdown.onValueChanged.AddListener(ChangeMicrophone);

            index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);

            RecordingLight.GetComponent<Renderer>().material.color = Color.gray;
#endif
        }

        private void ChangeMicrophone(int newIndex)
        {
            index = newIndex;
            PlayerPrefs.SetInt("user-mic-device-index", newIndex);
        }

        private void ToggleRecording(InputAction.CallbackContext context)
        {
            if (!isRecording)
            {
                isRecording = true;
                StartRecording();
            }
            else
            {
                isRecording = false;
                EndRecording();
            }
        }

        private void StartRecording()
        {
            RecordingLight.GetComponent<Renderer>().material.color = Color.green;
            RecordingText.GetComponent<TextMeshProUGUI>().text = "Recording";

            OutputTranscript.GetComponent<OutputTranscript>().userResponseTime.Add(responseTimer);
            Debug.Log("timer ends");

#if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, maxDuration, 44100);
#endif
        }

        private async void EndRecording()
        {
            message.text = "Transcribing...";
            RecordingLight.GetComponent<Renderer>().material.color = Color.gray;
            RecordingText.GetComponent<TextMeshProUGUI>().text = "...";

#if !UNITY_WEBGL
            Microphone.End(dropdown.options[index].text);
#endif

            if (ChatGPTManager.GetOpenAIInstance() == null)
            {
                Debug.LogError("OpenAI client was not initialized for transcription.");
                return;
            }

            byte[] data = SaveWav.Save(fileName, clip);
            string transcript;

            using (MemoryStream audioStream = new MemoryStream(data))
            using (AudioTranscriptionRequest request = new AudioTranscriptionRequest(
                audioStream,
                "audio.wav",
                model: "whisper-1",
                language: "en",
                responseFormat: AudioResponseFormat.Text))
            {
                transcript = await ChatGPTManager.GetOpenAIInstance().AudioEndpoint.CreateTranscriptionTextAsync(request);
            }

            if (promptGenerator != null)
            {
                StartCoroutine(
                    promptGenerator.BuildAndSendPrompt(transcript)
                );
            }

            string replaced = transcript.Replace(',', '.');
            OutputTranscript.GetComponent<OutputTranscript>().userResponse.Add(replaced);

            Thoughtbubble.SetActive(true);
            message.text = transcript;
        }

        private void Update()
        {
            if (!isRecording)
            {
                responseTimer += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isRecording)
            {
                isRecording = true;
                StartRecording();
            }

            if (Input.GetKeyUp(KeyCode.Space) && isRecording)
            {
                isRecording = false;
                EndRecording();
            }
        }
    }
}