using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverOnwardGames.AI.SpeechToText.Models
{
    public class ConversationTranscript<T> where T : SpeechTranscript
    {
        public long TotalDuration 
        { 
            get 
            {
                return SpeechTranscripts.Select(st => st.End).Max();
            } 
        }
        public virtual List<T> SpeechTranscripts { get; set; } = new List<T>();
        public string Status { get; set; }
        public string CancellationReason { get; set; }
        public string ErrorDetails { get; set; }

    }
}
