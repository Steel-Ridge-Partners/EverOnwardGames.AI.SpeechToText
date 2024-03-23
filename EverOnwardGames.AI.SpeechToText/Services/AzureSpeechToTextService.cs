using EverOnwardGames.AI.SpeechToText.Models;
using EverOnwardGames.AI.SpeechToText.Services.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Transcription;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace EverOnwardGames.AI.SpeechToText.Services
{
    public class AzureSpeechToTextService : IAsyncDiarizedConversationTranscriptionService
    {
        private readonly SpeechConfig _speechConfig;
        private readonly Dictionary<Guid, DiarizedConversationTranscript> _transcriptionCache;

        public AzureSpeechToTextService(IConfiguration configuration)
        {
            var uri = configuration["eog-carenotes-endpoint"];
            var key = configuration["eog-carenotes-stt-key"];
            var region = configuration["eog-carenotes-stt-region"];
            _speechConfig = SpeechConfig.FromSubscription(key, region);
            _speechConfig.EndpointId = uri;
            _transcriptionCache = new Dictionary<Guid, DiarizedConversationTranscript>();
        }

        public async Task<DiarizedConversationTranscript> GetDiarizedAudioTranscriptionAsync(string audioFilePath, TaskCompletionSource<int>? stopRecognition = null)
        {
            using (var audioConfig = AudioConfig.FromWavFileInput(audioFilePath))
            {
                return await TranscribeAudioConverstaion(audioConfig, stopRecognition);
            }
        }

        public async Task<DiarizedConversationTranscript> GetDiarizedAudioTranscriptionAsync(AudioInputStream stream, TaskCompletionSource<int>? stopRecognition = null)
        {
            using (var audioConfig = AudioConfig.FromStreamInput(stream))
            {
                return await TranscribeAudioConverstaion(audioConfig, stopRecognition);
            }
        }

        private async Task<DiarizedConversationTranscript> TranscribeAudioConverstaion(AudioConfig audioConfig, TaskCompletionSource<int>? stopRecognition = null)
        {
            stopRecognition ??= new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            var transcriptionKey = Guid.NewGuid();
            _transcriptionCache.Add(transcriptionKey, new DiarizedConversationTranscript());
            using (var conversationTranscriber = new ConversationTranscriber(_speechConfig, audioConfig))
            {
                conversationTranscriber.Transcribed += ConversationTranscriber_Transcribed(transcriptionKey);

                conversationTranscriber.Canceled += ConversationTranscriber_Canceled(transcriptionKey, stopRecognition);

                conversationTranscriber.SessionStopped += ConversationTranscriber_SessionStopped(transcriptionKey, stopRecognition);

                await conversationTranscriber.StartTranscribingAsync();

                Task.WaitAny(new[] { stopRecognition.Task });

                await conversationTranscriber.StopTranscribingAsync();

                var transcript = _transcriptionCache[transcriptionKey];
                _transcriptionCache.Remove(transcriptionKey);
                return transcript;
            }
        }

        private EventHandler<SessionEventArgs> ConversationTranscriber_SessionStopped(Guid transcriptionKey, TaskCompletionSource<int> stopRecognition)
        {
            return (sender, e) =>
            {
                stopRecognition.TrySetResult(0);
            };
        }

        private EventHandler<ConversationTranscriptionCanceledEventArgs> ConversationTranscriber_Canceled(Guid transcriptionKey, TaskCompletionSource<int> stopRecognition)
        {
            return (sender, e) =>
            {
                _transcriptionCache[transcriptionKey].CancellationReason = e.Reason.ToString();
                _transcriptionCache[transcriptionKey].ErrorDetails = e.ErrorDetails;

                stopRecognition.TrySetResult(0);
            };
        }

        private EventHandler<ConversationTranscriptionEventArgs> ConversationTranscriber_Transcribed(Guid transcriptionKey)
        {
            return (sender, e) =>
            {
                var transcript = new DiarizedTranscript
                {
                    Text = e.Result.Text,
                    SpeakerId = e.Result.SpeakerId,
                    Start = e.Result.OffsetInTicks,
                    Duration = e.Result.Duration.Ticks,
                    End = e.Result.OffsetInTicks + e.Result.Duration.Ticks
                };

                _transcriptionCache[transcriptionKey].SpeechTranscripts.Add(transcript);
            };
        }
    }
}
