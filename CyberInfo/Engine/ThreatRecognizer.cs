using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberInfo.Engine
{
    public class ThreatRecognizer
    {
        private readonly Dictionary<string, string[]> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["phishing"] = new[] { "phish", "phishing", "fake email", "email scam", "smishing", "spam mail" },
            ["password"] = new[] { "password", "passwords", "passphrase", "credential", "login", "pass" },
            ["scam"] = new[] { "scam", "fraud", "con", "advance fee", "419", "fake job" },
            ["privacy"] = new[] { "privacy", "private", "personal data", "tracking", "data leak", "surveillance" },
            ["malware"] = new[] { "malware", "virus", "trojan", "ransomware", "spyware", "worm", "infected" },
            ["2fa"] = new[] { "2fa", "two factor", "two-factor", "mfa", "authenticator", "otp", "one time" },
            ["wifi"] = new[] { "wifi", "wi-fi", "hotspot", "public network", "free wifi", "wireless" },
            ["social engineering"] = new[] { "social engineering", "vishing", "pretexting", "impersonation", "manipulate" },
            ["updates"] = new[] { "update", "patch", "software update", "windows update", "outdated" },
            ["safe browsing"] = new[] { "safe browsing", "https", "browser security", "suspicious link" }
        };

        private readonly Dictionary<string, string> _toName = new(StringComparer.OrdinalIgnoreCase)
        {
            ["phishing"] = "Phishing",
            ["password"] = "Passwords",
            ["scam"] = "Phishing",
            ["privacy"] = "Safe Browsing",
            ["malware"] = "Malware",
            ["2fa"] = "Two-Factor Authentication (2FA)",
            ["wifi"] = "Public Wi-Fi",
            ["social engineering"] = "Social Engineering",
            ["updates"] = "Software Updates",
            ["safe browsing"] = "Safe Browsing"
        };

        public readonly Func<string, string?> FindTopicKey;
        public readonly Func<string, string?> FindTopicName;
        public readonly Predicate<string> HasCyberKeyword;

        private Action<string, string>? _onMatch;

        public ThreatRecognizer()
        {
            FindTopicKey = input =>
            {
                string lower = input.ToLowerInvariant();
                foreach (var (key, kws) in _map)
                    if (kws.Any(kw => lower.Contains(kw)))
                    {
                        string matched = kws.First(kw => lower.Contains(kw));
                        _onMatch?.Invoke(matched, key);
                        return key;
                    }
                return null;
            };

            FindTopicName = input =>
            {
                string? key = FindTopicKey(input);
                return key != null && _toName.TryGetValue(key, out var n) ? n : null;
            };

            HasCyberKeyword = input => FindTopicKey(input) != null;
        }

        public void SetOnMatch(Action<string, string> handler) => _onMatch = handler;

        private static readonly HashSet<string> FollowUps = new(StringComparer.OrdinalIgnoreCase)
        {
            "more","tell me more","give me more","explain more","another tip",
            "give me another tip","continue","go on","more info","more details","what else"
        };

        public bool IsFollowUp(string input) =>
            FollowUps.Any(p => input.ToLowerInvariant().Contains(p));

        private static readonly string[] InterestPhrases =
        {
            "interested in","i like","i care about","i'm worried about",
            "i want to know about","tell me about","my concern is"
        };

        public string? ExtractDeclaredInterest(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (string phrase in InterestPhrases)
            {
                int idx = lower.IndexOf(phrase, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    string tail = lower[(idx + phrase.Length)..];
                    foreach (var (key, kws) in _map)
                        if (kws.Any(kw => tail.Contains(kw))) return key;
                }
            }
            return null;
        }
    }
}