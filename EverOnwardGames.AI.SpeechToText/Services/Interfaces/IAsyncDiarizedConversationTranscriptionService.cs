using EverOnwardGames.AI.SpeechToText.Models;

namespace EverOnwardGames.AI.SpeechToText.Services.Interfaces
{
    public interface IAsyncDiarizedConversationTranscriptionService
    {
        Task<DiarizedConversationTranscript> GetDiarizedAudioTranscriptionAsync(string audioFilePath, TaskCompletionSource<int>? stopRecognition = null);
    }
}