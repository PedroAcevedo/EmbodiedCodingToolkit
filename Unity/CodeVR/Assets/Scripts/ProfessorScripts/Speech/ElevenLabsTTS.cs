// Licensed under the MIT License. See LICENSE in the project root for license information.

using ElevenLabs.Models;
using ElevenLabs.Voices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Async;
using System.Collections;
using ElevenLabs.TextToSpeech;

namespace ElevenLabs.Demo
{
    [RequireComponent(typeof(AudioSource))]
    public class ElevenLabsTTS : MonoBehaviour
    {
        public GameObject character;
        public GameObject loadingAnimatorObject;
        public GameObject nextButton;
        //public GazeTimerTrigger GazeTimerTrigger;

        [SerializeField] private ElevenLabsConfiguration configuration;
        [SerializeField] private Voice voice;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject ConverseAnim;
        [SerializeField] private GameObject ChatGPTManager;

        //private readonly Queue<AudioClip> streamClipQueue = new ();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool isAnimation = false;


        private void Start()
        {
        }

        private void OnDestroy()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

 

        public async void ElevenLabGenerateSpeech(string speechText)
        {
            if (nextButton)
            {
                nextButton.SetActive(false);
            }

            try
            {
                var api = new ElevenLabsClient(configuration){};

                if (voice == null)
                {
                    voice = (await api.VoicesEndpoint.GetAllVoicesAsync(cancellationTokenSource.Token)).FirstOrDefault();
                }
              
                var chunks = SplitIntoChunks(speechText);

                foreach (var chunk in chunks)
                {
                    var request = new TextToSpeechRequest(
                        voice: voice,
                        text: chunk,
                        voiceSettings: await api.VoicesEndpoint.GetDefaultVoiceSettingsAsync(),
                        outputFormat: OutputFormat.PCM_24000
                    );

                    var voiceClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(
                        request,
                        cancellationToken: destroyCancellationToken
                    );

                    var clip = voiceClip.AudioClip;

                    if (clip.loadType != AudioClipLoadType.DecompressOnLoad)
                        Debug.LogWarning("Make sure clip load type is 'DecompressOnLoad' in import settings or create method.");

                    while (audioSource.isPlaying)
                        await Task.Delay(50);

                    audioSource.clip = clip;

                    if (!isAnimation)
                    {
                        loadingAnimatorObject.SetActive(false);
                        ConverseAnim.GetComponent<ConversationAnimation>().StartAnimation();
                        isAnimation = true;
                    }

                    audioSource.Play();

                }
                while (audioSource.isPlaying)
                    await Task.Delay(50);

                //end clip, so end animation
                ConverseAnim.GetComponent<ConversationAnimation>().EndAnimation();
                isAnimation = false;

                if (nextButton)
                {
                    nextButton.SetActive(true);
                    //GazeTimerTrigger.isLoading = false;
                    ChatGPTManager.GetComponent<Samples.Whisper.WhisperSTT>().responseTimer = 0;
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
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
    }
}
