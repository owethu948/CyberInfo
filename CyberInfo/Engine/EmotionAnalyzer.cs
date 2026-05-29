using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberInfo.Engine
{
    public class EmotionAnalyzer
    {
        public enum Sentiment { Neutral, Worried, Curious, Frustrated, Happy }

        private static readonly Dictionary<Sentiment, List<string>> Keywords = new()
        {
            [Sentiment.Worried] = new() { "worried", "scared", "afraid", "concerned", "nervous", "anxious", "fear", "unsafe", "vulnerable", "threatened", "stressed", "panic" },
            [Sentiment.Curious] = new() { "curious", "wondering", "interested", "how does", "what is", "tell me", "explain", "learn", "understand", "want to know" },
            [Sentiment.Frustrated] = new() { "frustrated", "annoyed", "confused", "angry", "ugh", "useless", "terrible", "hate", "stupid", "not working", "awful" },
            [Sentiment.Happy] = new() { "great", "thanks", "awesome", "good", "love", "helpful", "amazing", "perfect", "excellent", "brilliant", "wonderful", "thank you" }
        };

        public readonly Predicate<string> IsWorried;
        public readonly Predicate<string> IsFrustrated;
        public readonly Predicate<string> IsCurious;

        public EmotionAnalyzer()
        {
            IsWorried = input => Detect(input) == Sentiment.Worried;
            IsFrustrated = input => Detect(input) == Sentiment.Frustrated;
            IsCurious = input => Detect(input) == Sentiment.Curious;
        }

        public Sentiment Detect(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (var (s, kws) in Keywords)
                if (kws.Any(kw => lower.Contains(kw))) return s;
            return Sentiment.Neutral;
        }

        public string GetEmpathyResponse(Sentiment s) => s switch
        {
            Sentiment.Worried => "🤗 It's completely understandable to feel that way — cyber threats are real. Let me share some tips to help you stay safe.",
            Sentiment.Curious => "🧐 Love the curiosity! That's exactly the right mindset for cybersecurity. Here's what you need to know:",
            Sentiment.Frustrated => "😌 I hear you — this can feel overwhelming. Let's slow down and work through it together.",
            Sentiment.Happy => "😊 Glad you're feeling positive! Keep that energy going.",
            _ => string.Empty
        };

        public string GetEmoji(Sentiment s) => s switch
        {
            Sentiment.Worried => "😟",
            Sentiment.Curious => "🧐",
            Sentiment.Frustrated => "😤",
            Sentiment.Happy => "😊",
            _ => "💬"
        };

        public string GetLabel(Sentiment s) => s switch
        {
            Sentiment.Worried => "Worried",
            Sentiment.Curious => "Curious",
            Sentiment.Frustrated => "Frustrated",
            Sentiment.Happy => "Happy",
            _ => "Neutral"
        };

        public string GetStatusBarColour(Sentiment s) => s switch
        {
            Sentiment.Worried => "#FFB703",
            Sentiment.Curious => "#00CFFF",
            Sentiment.Frustrated => "#FF4757",
            Sentiment.Happy => "#00FF9F",
            _ => "#1E3A5F"
        };
    }
}