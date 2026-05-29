using CyberInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberInfo.Engine
{
    public class BotEngine
    {
        public List<SecurityTopic> Topics { get; }
        public UserProfile Memory { get; }
        public ThreatRecognizer Keywords { get; }
        public EmotionAnalyzer Sentiment { get; }
        public SecurityTipsLibrary Library { get; }

        private readonly List<SecurityChallenge> _scenarios;
        private readonly List<string> _facts;
        private readonly Random _rand = new();
        private string _lastTopic = "";
        private SecurityChallenge? _activeQuiz;

        public BotEngine()
        {
            Topics = CreateTopics();
            Memory = new UserProfile();
            Keywords = new ThreatRecognizer();
            Sentiment = new EmotionAnalyzer();
            Library = new SecurityTipsLibrary();
            _scenarios = CreateScenarios();
            _facts = CreateFacts();
        }

        private List<SecurityTopic> CreateTopics()
        {
            return new List<SecurityTopic>
            {
                new SecurityTopic("Passwords",
                    "A password is a secret word or phrase used to authenticate your identity.",
                    "Weak or reused passwords are the #1 way attackers break into accounts.",
                    "When logging into any online service – email, banking, social media, work systems.",
                    "Use a password manager, enable 2FA, never reuse passwords, use 16+ random characters.",
                    "NIST SP 800-63B Digital Identity Guidelines"),
                new SecurityTopic("Phishing",
                    "Phishing is a fraudulent attempt to obtain sensitive information by pretending to be a trustworthy entity.",
                    "Phishing causes billions in losses annually and is the top initial infection vector.",
                    "Via email, SMS, WhatsApp, or fake websites mimicking banks, SARS, or delivery services.",
                    "Check sender addresses, hover over links, don't click unexpected attachments, report suspicious emails.",
                    "SABRIC Annual Crime Stats 2024"),
                new SecurityTopic("Safe Browsing",
                    "Safe browsing means protecting yourself from malicious websites and online trackers.",
                    "One wrong click can install malware or expose your private data.",
                    "When visiting unfamiliar sites, clicking ads, downloading software, or entering personal info.",
                    "Look for HTTPS, use ad-blockers, avoid torrent sites, keep browser updated.",
                    "Google Safe Browsing Report"),
                new SecurityTopic("Two-Factor Authentication (2FA)",
                    "2FA adds a second verification step beyond your password, like a code from an app.",
                    "Even if your password is stolen, 2FA blocks 99.9% of automated attacks.",
                    "On any account that supports it: email, banking, social media, work systems.",
                    "Use authenticator apps, not SMS; store backup codes; consider hardware keys.",
                    "Microsoft Digital Defense Report"),
                new SecurityTopic("Malware",
                    "Malware is malicious software including viruses, ransomware, and spyware.",
                    "Ransomware attacks in SA increased 57% in 2024, costing businesses millions.",
                    "When opening email attachments, downloading cracked software, clicking pop-ups.",
                    "Keep antivirus active, install updates immediately, don't use unknown USBs.",
                    "Kaspersky Security Bulletin"),
                new SecurityTopic("Social Engineering",
                    "Psychological manipulation to trick people into giving up secrets or access.",
                    "Over 70% of breaches involve a human element, not technical hacking.",
                    "Phone calls (vishing), in-person impersonation, fake IT support, pretexting.",
                    "Verify caller identity via official numbers, never share passwords, slow down under pressure.",
                    "Verizon DBIR 2024"),
                new SecurityTopic("Software Updates",
                    "Updates fix security holes and bugs in operating systems and applications.",
                    "Unpatched software is exploited within days of a patch's release.",
                    "On your PC, phone, router, smart TV, and even IoT devices.",
                    "Enable automatic updates, restart after updates, never ignore update notifications.",
                    "CISA Known Exploited Vulnerabilities Catalog"),
                new SecurityTopic("Public Wi-Fi",
                    "Open Wi-Fi networks in cafes, airports, hotels that anyone can join.",
                    "Attackers can intercept your traffic or set up evil twin hotspots.",
                    "When checking email, logging into accounts, or doing banking while traveling.",
                    "Use a VPN, avoid sensitive transactions, confirm network name with staff, use mobile data for critical tasks.",
                    "OWASP Wireless Security Guidelines")
            };
        }

        private List<SecurityChallenge> CreateScenarios()
        {
            return new List<SecurityChallenge>
            {
                new SecurityChallenge(
                    "You receive an email from 'SASSA' saying your grant payment is on hold. It contains a link to 'verify identity' and asks for your ID and password. What do you do?",
                    new List<string> {
                        "Click the link and fill in details – it's urgent",
                        "Reply to the email asking for more information",
                        "Forward it to SASSA's official fraud line and delete it",
                        "Call the number in the email immediately"
                    }, 2,
                    "Correct! You recognised a phishing attempt. Never click links or call numbers from unsolicited emails – always use official channels."),
                new SecurityChallenge(
                    "You're at OR Tambo airport and need to check your bank balance. A free Wi-Fi called 'Airport_Free_WiFi' appears. What's safest?",
                    new List<string> {
                        "Connect and check quickly – it's a busy airport",
                        "Use a VPN then check your banking app",
                        "Use your mobile data hotspot instead",
                        "Ask the nearest shop for the real Wi-Fi password"
                    }, 2,
                    "Best choice! Public Wi-Fi is risky for banking. Mobile data is encrypted and safer. If you must use Wi-Fi, a VPN helps but mobile data is better."),
                new SecurityChallenge(
                    "A colleague calls from 'IT support' saying your computer has a virus and they need remote access immediately. What's your move?",
                    new List<string> {
                        "Give access – they're from IT",
                        "Hang up and call the official IT number",
                        "Ask them to prove who they are",
                        "Let them connect while you watch"
                    }, 1,
                    "Perfect! Always verify via official channels. Social engineers impersonate IT support to install malware or steal data."),
                new SecurityChallenge(
                    "You see a USB stick on the floor at your office. What do you do?",
                    new List<string> {
                        "Plug it in to see whose it is",
                        "Hand it to security or IT without plugging in",
                        "Format it first then check the files",
                        "Take it home to check later"
                    }, 1,
                    "Correct! Never plug in unknown USBs – attackers drop infected drives hoping someone will connect them."),
                new SecurityChallenge(
                    "You get a WhatsApp from your 'bank' saying suspicious activity, click to verify. The link looks real. What now?",
                    new List<string> {
                        "Click the link and log in – better safe than sorry",
                        "Ignore it completely",
                        "Call the bank using the number on your card, not the message",
                        "Reply STOP to unsubscribe"
                    }, 2,
                    "Well done! Always use the number from your card or official website. Those WhatsApp messages are almost always scams.")
            };
        }

        private List<string> CreateFacts()
        {
            return new List<string>
            {
                "📊 In South Africa, over 80% of organisations experienced a phishing attack in 2024 – SABRIC.",
                "📊 Only 57% of South Africans use unique passwords for each account – security.org.",
                "📊 Ransomware attacks cost South African businesses an average of R2.5 million per incident – Sophos.",
                "📊 2FA would have prevented 96% of bulk phishing attacks – Microsoft.",
                "📊 The most common password in SA is still 'password123' – NordPass study.",
                "📊 34% of SA adults have been victims of financial fraud – SA Fraud Prevention Services.",
                "📊 Over 65% of public Wi-Fi hotspots can be intercepted with basic tools – Kaspersky.",
                "📊 Software updates fix an average of 60-100 security holes per month – CISA.",
                "📊 Social engineering accounts for 75% of all data breaches – Verizon DBIR.",
                "📊 The average time to detect a breach is 207 days – IBM Security."
            };
        }

        public string ProcessInput(string input, out string? sentimentResponse, out EmotionAnalyzer.Sentiment detectedSentiment)
        {
            input = input.Trim();
            Memory.PushInput(input);

            detectedSentiment = Sentiment.Detect(input);
            sentimentResponse = Sentiment.GetEmpathyResponse(detectedSentiment);

            // Handle active quiz answer
            if (_activeQuiz != null && TryParseOptionIndex(input, out int idx) && idx >= 0 && idx < _activeQuiz.Options.Count)
            {
                string feedback = _activeQuiz.Feedback;
                _activeQuiz = null;
                return $"✅ {feedback}\n\nWould you like another scenario, or shall we continue learning?";
            }

            // Scenario request
            if (input.Contains("scenario", StringComparison.OrdinalIgnoreCase) ||
                input.Contains("quiz", StringComparison.OrdinalIgnoreCase) ||
                input.Contains("challenge", StringComparison.OrdinalIgnoreCase))
            {
                _activeQuiz = _scenarios[_rand.Next(_scenarios.Count)];
                var opts = string.Join("\n", _activeQuiz.Options.Select((opt, i) => $"  {i + 1}. {opt}"));
                return $"📋 **SCENARIO**\n\n{_activeQuiz.Description}\n\n{opts}\n\nType the number of your answer (1-4).";
            }

            // Fact request
            if (input.Contains("fact", StringComparison.OrdinalIgnoreCase) || input.Contains("did you know", StringComparison.OrdinalIgnoreCase))
                return GetRandomFact();

            // Tip request
            if (input.Contains("tip", StringComparison.OrdinalIgnoreCase) || input.Contains("give me a tip", StringComparison.OrdinalIgnoreCase))
            {
                string topicKey = MapToTipKey(_lastTopic);
                string tip = Library.GetRandomTip(topicKey);
                return $"💡 **SECURITY TIP**\n\n{tip}";
            }

            // Follow-up "more"
            if (Keywords.IsFollowUp(input) && !string.IsNullOrEmpty(_lastTopic))
            {
                string tip = Library.GetRandomTip(MapToTipKey(_lastTopic));
                return $"Here's another tip about {_lastTopic}:\n\n{tip}";
            }

            // Declared interest
            string? declared = Keywords.ExtractDeclaredInterest(input);
            if (declared != null)
            {
                string topicName = MapKeyToTopicName(declared);
                if (!string.IsNullOrEmpty(topicName))
                {
                    Memory.RememberInterest(topicName);
                    return $"{sentimentResponse}\n\nGot it – you're interested in {topicName}. Let me share some key points:\n\n{GetTopicResponse(topicName)}";
                }
            }

            // Keyword match
            string? matchedKey = Keywords.FindTopicKey(input);
            if (matchedKey != null)
            {
                string topicName = MapKeyToTopicName(matchedKey);
                _lastTopic = topicName;
                Memory.LastTopic = topicName;
                Memory.RememberInterest(topicName);
                return $"{sentimentResponse}\n\n{GetTopicResponse(topicName)}";
            }

            // Number (1-8)
            if (int.TryParse(input, out int num) && num >= 1 && num <= Topics.Count)
            {
                var topic = Topics[num - 1];
                _lastTopic = topic.Name;
                Memory.LastTopic = topic.Name;
                Memory.RememberInterest(topic.Name);
                return $"{sentimentResponse}\n\n{topic.BuildGuiResponse()}";
            }

            // Direct topic name
            var directTopic = Topics.Find(t => t.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (directTopic != null)
            {
                _lastTopic = directTopic.Name;
                Memory.LastTopic = directTopic.Name;
                Memory.RememberInterest(directTopic.Name);
                return $"{sentimentResponse}\n\n{directTopic.BuildGuiResponse()}";
            }

            // Name capture
            if (!Memory.NameCaptured && !string.IsNullOrWhiteSpace(input) && input.Length > 1)
            {
                Memory.UserName = input;
                return $"Nice to meet you, {input}! 🎉\n\nI can teach you about:\n1. Passwords\n2. Phishing\n3. Safe Browsing\n4. 2FA\n5. Malware\n6. Social Engineering\n7. Software Updates\n8. Public Wi-Fi\n\nJust type a number, topic name, or ask for a 'scenario', 'fact', or 'tip'.";
            }

            // Fallback
            return $"{sentimentResponse}\n\nI can help you learn about cybersecurity. Try:\n• Type a topic name (e.g., 'phishing')\n• Type a number 1-8\n• Ask for a 'scenario', 'fact', or 'tip'\n• Say 'more' for another tip on the same topic";
        }

        private bool TryParseOptionIndex(string input, out int idx)
        {
            idx = -1;
            if (int.TryParse(input, out int raw) && raw >= 1 && raw <= 4)
            {
                idx = raw - 1;
                return true;
            }
            return false;
        }

        private string GetRandomFact() => _facts[_rand.Next(_facts.Count)];
        private string GetTopicResponse(string topicName)
        {
            var topic = Topics.Find(t => t.Name.Equals(topicName, StringComparison.OrdinalIgnoreCase));
            return topic?.BuildGuiResponse() ?? $"I have detailed information on {topicName}. Type the exact name again or ask for a tip!";
        }

        private string MapKeyToTopicName(string key) => key.ToLower() switch
        {
            "phishing" => "Phishing",
            "password" => "Passwords",
            "malware" => "Malware",
            "2fa" => "Two-Factor Authentication (2FA)",
            "wifi" => "Public Wi-Fi",
            "social engineering" => "Social Engineering",
            "updates" => "Software Updates",
            "safe browsing" => "Safe Browsing",
            "scam" => "Phishing",
            "privacy" => "Safe Browsing",
            _ => "Passwords"
        };

        private string MapToTipKey(string topicName) => topicName.ToLower() switch
        {
            "passwords" => "password",
            "phishing" => "phishing",
            "safe browsing" => "safe browsing",
            "two-factor authentication (2fa)" => "2fa",
            "malware" => "malware",
            "social engineering" => "social engineering",
            "software updates" => "updates",
            "public wi-fi" => "wifi",
            _ => "password"
        };

        public string GetCurrentTopic() => _lastTopic;
        public bool HasActiveQuiz() => _activeQuiz != null;
        public SecurityChallenge? GetActiveQuiz() => _activeQuiz;
    }
}