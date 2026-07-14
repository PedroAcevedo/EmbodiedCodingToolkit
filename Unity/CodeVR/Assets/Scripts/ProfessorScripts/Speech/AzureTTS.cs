using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;

public enum AzureVoices
{
    Andrew,
    Emma
}

[RequireComponent(typeof(AudioSource))]
public class AzureTTS : MonoBehaviour
{
    public GameObject character;
    //public GameObject ChatGPTManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AzureVoices AzureVoices;
    [SerializeField] private GameObject ChatGPTManager;
    [SerializeField] private GameObject ConverseAnim;
    [SerializeField] private GameObject Thoughtbubble;

    private string SubscriptionKey;
    private string Region;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    private const int SampleRate = 24000;
    private object threadLocker = new object();
    //private bool waitingForSpeak;
    private bool audioSourceNeedStop;
    private string message;

    private Dictionary<AzureVoices, string> voicesDictionary = new Dictionary<AzureVoices, string>()
    {
        { AzureVoices.Andrew, "en-US-AndrewNeural"},
        { AzureVoices.Emma, "en-US-EmmaNeural"}
    };

    IEnumerator StartSpeakingAnim()
    {
        yield return new WaitForSeconds(2f);
        ConverseAnim.GetComponent<ConversationAnimation>().StartAnimation();
        character.GetComponent<SalsaFeedExternalAnalysis>().isFeeding = true;

    }
    public void AzureGenerateSpeech(string speechText)
    {

        string newMessage = null;
        var startTime = DateTime.Now;

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(speechText).Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Can speak 10mins audio as maximum
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;

                    }
                });

            //disable thoughtbubble
            Thoughtbubble.SetActive(false);

            //play audio
            audioSource.clip = audioClip;
            audioSource.Play();
            
            //start talking animation
            ConverseAnim.GetComponent<ConversationAnimation>().StartAnimation();
            character.GetComponent<SalsaFeedExternalAnalysis>().isFeeding = true;

            
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }
            //waitingForSpeak = false;
        }
    }

    private List<string> SplitIntoChunks(string text, int chunkWordLimit = 50)
    {
        List<string> chunks = new List<string>();
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        string currentChunk = "";
        int wordCount = 0;

        foreach (string s in sentences)
        {
            string sentence = s.Trim();
            if (string.IsNullOrEmpty(sentence)) continue;

            int sentenceWordCount = sentence.Split(' ').Length;

            if (wordCount + sentenceWordCount > chunkWordLimit)
            {
                if (!string.IsNullOrEmpty(currentChunk))
                {
                    chunks.Add(currentChunk.Trim() + ".");
                    currentChunk = "";
                    wordCount = 0;
                }
            }

            currentChunk += sentence + ". ";
            wordCount += sentenceWordCount;
        }

        if (!string.IsNullOrEmpty(currentChunk))
            chunks.Add(currentChunk.Trim());

        return chunks;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        SubscriptionKey = this.GetComponent<AzureAPI>().API_key;
        Region = this.GetComponent<AzureAPI>().API_region;
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

        //set the audio format to raw (24KHz for better quality)
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        //select voice
        speechConfig.SpeechSynthesisVoiceName = voicesDictionary[AzureVoices];

        // Creates a speech synthesizer.
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        // Make sure to dispose the synthesizer after use!
        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
        };


    }

    // Update is called once per frame
    void Update()
    {
        lock (threadLocker)
        {
            if (audioSourceNeedStop)
            {
                ConverseAnim.GetComponent<ConversationAnimation>().EndAnimation();
                character.GetComponent<SalsaFeedExternalAnalysis>().isFeeding = false;
                //start tracking user response time
                ChatGPTManager.GetComponent<Samples.Whisper.WhisperSTT>().responseTimer = 0;

                audioSource.Stop();
                audioSourceNeedStop = false;
            }
        }

    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}
