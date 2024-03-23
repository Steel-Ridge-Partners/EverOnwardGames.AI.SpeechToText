using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverOnwardGames.AI.SpeechToText.Models
{
    public class SpeechTranscript
    {
        public long Duration { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public string? Language { get; set; }
        public string? Text { get; set; }
    }
}
