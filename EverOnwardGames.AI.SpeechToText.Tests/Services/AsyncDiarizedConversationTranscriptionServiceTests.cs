using EverOnwardGames.AI.SpeechToText.Services;
using EverOnwardGames.AI.SpeechToText.Services.Interfaces;
using FluentAssertions;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EverOnwardGames.AI.SpeechToText.Tests.Services
{
    [TestFixture]
    public class DiarizedConversationTranscriptServiceImplementationTests : TestConfigurationBase
    {
        public static IEnumerable<IAsyncDiarizedConversationTranscriptionService> GetDiarizedConversationTranscriptionServices()
        {   
            yield return new AzureSpeechToTextService(_configuration);
        }

        [Test, TestCaseSource(nameof(GetDiarizedConversationTranscriptionServices))]
        public async Task GetDiarizedConversationTranscriptAsync_WhenCalled_WithAudioFilePath_ReturnsDiarizedConversationTranscript(IAsyncDiarizedConversationTranscriptionService service)
        {
            // Arrange


            // Act
            var transcription = await service.GetDiarizedAudioTranscriptionAsync("dinner-conversation.wav");

            // Assert
            transcription.ErrorDetails.Should().Be("");
            transcription.SpeechTranscripts.Select(st => st.SpeakerId).Distinct().Count().Should().Be(4);

            var transcript = transcription.SpeechTranscripts.OrderBy(st => st.Start).ToList();
            var serializedTranscript = JsonConvert.SerializeObject(transcript);
            if(File.Exists("last-run-transcript.json"))
            {
                File.Delete("last-run-transcript.json");
            }

            File.WriteAllText("last-run-transcript.json", serializedTranscript);
        }
    }
}

