using CyberInfo.Engine;
using CyberInfo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CyberInfo
{
    /// <summary>
    /// Main WPF window code-behind.
    /// Demonstrates: delegates (Action, Func, Predicate, custom), ObservableCollection,
    ///               generic collections, async/await, WPF data binding.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ── Engine ────────────────────────────────────────────────────────────
        private readonly Bot _bot = new();

        // ── State ─────────────────────────────────────────────────────────────
        private bool _awaitingName = true;
        private bool _inQuiz = false;
        private Scenario? _currentScenario;

        // ── Action delegate (Part 2 requirement) ──────────────────────────────
        // Invoked whenever the memory panel needs refreshing
        private readonly Action _refreshMemory;

        // ── Quiz option binding helper ─────────────────────────────────────────
        private record QuizOption(int Idx, string Label);

        // ═════════════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═════════════════════════════════════════════════════════════════════

        public MainWindow()
        {
            InitializeComponent();

            // Assign Action delegate
            _refreshMemory = UpdateMemoryPanel;

            // Log keyword matches (Action<string,string> delegate)
            _bot.Keywords.SetOnMatch((kw, topic) =>
                System.Diagnostics.Debug.WriteLine($"[KeywordEngine] '{kw}' => {topic}"));

            // Subscribe to ResponseLibrary event (custom delegate)
            _bot.Library.OnTipSelected += (topic, _) =>
                System.Diagnostics.Debug.WriteLine($"[Library] Tip served for: {topic}");

            SoundOutput.PlayGreetingAsync();

            Loaded += async (_, _) =>
            {
                await Task.Delay(300);
                AppendBotBubble(
                    "Hi there! I'm CyberInfo — your Cybersecurity Awareness Bot.\n\n" +
                    "Before we dive in, what should I call you?",
                    "CyberInfo");
                InputBox.Focus();
            };
        }

        // ═════════════════════════════════════════════════════════════════════
        // SEND / INPUT
        // ═════════════════════════════════════════════════════════════════════

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SendBtn.IsEnabled) ProcessSend();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool has = InputBox.Text.Length > 0;
            SendBtn.IsEnabled = has && !_inQuiz;
            Placeholder.Visibility = has ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessSend();

        private void ProcessSend()
        {
            string raw = InputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return;
            InputBox.Clear();
            SendBtn.IsEnabled = false;
            AppendUserBubble(raw);

            if (_awaitingName) HandleNameInput(raw);
            else _ = HandleChatAsync(raw);
        }

        // ═════════════════════════════════════════════════════════════════════
        // NAME CAPTURE
        // ═════════════════════════════════════════════════════════════════════

        private void HandleNameInput(string raw)
        {
            string s = raw.Trim();
            string lo = s.ToLowerInvariant();
            if (lo.StartsWith("my name is ")) s = s[11..];
            else if (lo.StartsWith("i am ")) s = s[5..];
            else if (lo.StartsWith("i'm ")) s = s[4..];
            else if (lo.StartsWith("im ")) s = s[3..];

            var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
                if (parts[i].Length > 0)
                    parts[i] = char.ToUpper(parts[i][0]) + parts[i][1..].ToLower();
            string name = string.Join(" ", parts);

            if (string.IsNullOrWhiteSpace(name))
            {
                AppendBotBubble("I didn't catch that — could you tell me your name? 🙂", "CyberInfo");
                return;
            }

            _bot.Memory.UserName = name;
            _awaitingName = false;
            UserBadge.Text = name;

            _ = AddBotBubbleAsync(_bot.GetWelcomeMessage(name), "CyberInfo");
        }

        // ═════════════════════════════════════════════════════════════════════
        // MAIN CHAT HANDLER
        // ═════════════════════════════════════════════════════════════════════

        private async Task HandleChatAsync(string input)
        {
            string name = _bot.Memory.UserName;
            string lower = input.ToLowerInvariant();

            _bot.Memory.PushInput(input);

            await ShowTypingAsync(500);

            // ── 1. Sentiment detection ────────────────────────────────────────
            var sentiment = _bot.Sentiment.Detect(input);
            string empathy = _bot.Sentiment.GetEmpathyResponse(sentiment);
            if (sentiment != SentimentDetector.Sentiment.Neutral)
                UpdateSentimentBar(sentiment);
            else
                SentimentBar.Visibility = Visibility.Collapsed;

            // ── 2. Empathy short-circuits ──────────────────────────────────────
            if (lower.Contains("thank"))
            {
                await AddBotBubbleAsync($"You're so welcome, {name}! 😊 That makes my day.\n\n{_bot.GetEncouragement()}", "CyberInfo"); return;
            }
            if (lower.Contains("sorry") || lower.Contains("dumb"))
            {
                await AddBotBubbleAsync(_bot.GetComfortMessage(), "CyberInfo"); return;
            }
            if (lower.Contains("how are you"))
            {
                await AddBotBubbleAsync($"Wonderful now that you're here, {name}! 😄 Ready to keep you safe. 🛡️", "CyberInfo"); return;
            }

            // ── 3. Follow-up (tell me more / another tip) ─────────────────────
            if (_bot.Keywords.IsFollowUp(lower))
            {
                if (_bot.Memory.LastTopic is string last)
                {
                    // Func<string,string> delegate call
                    string tip = _bot.Library.GetRandomTip(MapToLibKey(last) ?? last);
                    await AddBotBubbleAsync(
                        $"Here's another tip on {last}:\n\n{tip}\n\n{_bot.GetEncouragement()}", "CyberInfo");
                }
                else
                    await AddBotBubbleAsync("No previous topic to revisit — pick one from the sidebar! 🙂", "CyberInfo");
                return;
            }

            // ── 4. Commands ────────────────────────────────────────────────────
            if (lower is "fact" or "did you know")
            {
                var (f, c) = _bot.GetRandomFact();
                await AddBotBubbleAsync($"📊  DID YOU KNOW?\n\n{f}\n\n📖  {c}", "CyberInfo"); return;
            }
            if (lower is "tip" or "random tip" or "give me a tip")
            {
                await AddBotBubbleAsync(_bot.GetRandomTip(), "CyberInfo"); return;
            }
            if (lower is "scenario" or "quiz" or "test")
            {
                StartQuiz(); return;
            }
            if (lower is "/help" or "help" or "?")
            {
                await AddBotBubbleAsync(HelpText(), "CyberInfo"); return;
            }

            // ── 5. Interest declaration ────────────────────────────────────────
            string? interest = _bot.Keywords.ExtractDeclaredInterest(lower);
            if (interest != null)
            {
                _bot.Memory.RememberInterest(interest);
                _bot.Memory.LastTopic = interest;
                _refreshMemory(); // Action delegate

                string ack = $"Great! I'll remember that you're interested in {interest}. " +
                             "It's a crucial part of staying safe online. 🛡️";
                if (!string.IsNullOrEmpty(empathy)) ack = empathy + "\n\n" + ack;

                // ResponseSelector custom delegate
                var sel = _bot.Library.CreateSelectorFor(interest);
                string tip = sel(interest);
                await AddBotBubbleAsync($"{ack}\n\n💡  Here's a tip to get you started:\n{tip}", "CyberInfo");
                _refreshMemory();
                return;
            }

            // ── 6. Numeric topic selection ─────────────────────────────────────
            if (int.TryParse(input, out int n))
            {
                var t = _bot.FindTopicByIndex(n);
                if (t != null) { await ShowTopicAsync(t, empathy); return; }
                await AddBotBubbleAsync($"Please enter a number between 1 and {_bot.Topics.Count}.", "CyberInfo"); return;
            }

            // ── 7. Keyword recognition (Func<string,string?> delegate call) ────
            string? topicName = _bot.Keywords.FindTopicName(input);
            if (topicName != null)
            {
                var t = _bot.FindTopic(topicName);
                if (t != null) { await ShowTopicAsync(t, empathy); return; }
            }

            // Library-only keyword match (e.g. "scam", "privacy")
            string? libKey = _bot.Keywords.FindTopicKey(input);
            if (libKey != null)
            {
                string tip = _bot.Library.GetRandomTip(libKey);
                string resp = string.IsNullOrEmpty(empathy) ? tip : $"{empathy}\n\n{tip}";
                await AddBotBubbleAsync(resp, "CyberInfo");
                _bot.Memory.LastTopic = libKey; _refreshMemory(); return;
            }

            // ── 8. Pure sentiment with no topic ───────────────────────────────
            if (sentiment != SentimentDetector.Sentiment.Neutral && !string.IsNullOrEmpty(empathy))
            {
                await AddBotBubbleAsync(
                    $"{empathy}\n\n💡  Here's a tip to help:\n{_bot.GetRandomTip()}\n\nType a topic name or number to learn more!",
                    "CyberInfo");
                return;
            }

            // ── 9. Personalised recall ─────────────────────────────────────────
            if (_bot.Memory.Interests.Count > 0 && (lower.Contains("safe") || lower.Contains("secure")))
            {
                string pfx = _bot.Memory.GetPersonalisedPrefix();
                await AddBotBubbleAsync(
                    $"{pfx}you might want to review your account security settings.\n{_bot.GetRandomTip()}", "CyberInfo");
                return;
            }

            // ── 10. Default ────────────────────────────────────────────────────
            await AddBotBubbleAsync(_bot.GetDefaultResponse(name), "CyberInfo");
        }

        // ═════════════════════════════════════════════════════════════════════
        // TOPIC DISPLAY
        // ═════════════════════════════════════════════════════════════════════

        private async Task ShowTopicAsync(Topic t, string empathy = "")
        {
            _bot.Memory.LastTopic = t.Name;
            _bot.Memory.RememberInterest(t.Name);
            _refreshMemory(); // Action delegate

            string body = t.BuildGuiResponse();
            if (!string.IsNullOrEmpty(empathy)) body = empathy + "\n\n" + body;
            await AddBotBubbleAsync(body, "CyberInfo");
            await Task.Delay(150);
            await AddBotBubbleAsync(
                $"💡  {_bot.GetEncouragement()}\n\nType 'more' or 'another tip' for an extra tip on this topic!",
                "CyberInfo");
        }

        // ═════════════════════════════════════════════════════════════════════
        // QUIZ
        // ═════════════════════════════════════════════════════════════════════

        private void StartQuiz()
        {
            _currentScenario = _bot.GetRandomScenario();
            _inQuiz = true;
            SendBtn.IsEnabled = false;
            QuizDesc.Text = _currentScenario.Description;

            var opts = new List<QuizOption>();
            for (int i = 0; i < _currentScenario.Options.Count; i++)
                opts.Add(new QuizOption(i, $"{i + 1}.  {_currentScenario.Options[i]}"));

            QuizOptions.ItemsSource = opts;
            QuizPanel.Visibility = Visibility.Visible;

            AppendBotBubble("⚠️  SCENARIO CHALLENGE\n\nRead the scenario above and pick the best response.", "Quiz");
        }

        private void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScenario is null || sender is not Button btn) return;
            int idx = (int)btn.Tag;
            bool correct = idx == _currentScenario.CorrectOptionIndex;
            AppendUserBubble($"My answer: {_currentScenario.Options[idx]}");
            EndQuiz();
            string res = correct
                ? $"✅  Correct!\n\n{_currentScenario.Feedback}\n\n{_bot.GetEncouragement()}"
                : $"❌  Not quite.\n\n{_currentScenario.Feedback}\n\n{_bot.GetComfortMessage()}";
            AppendBotBubble(res, "Quiz Result");
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e) => EndQuiz();

        private void EndQuiz()
        {
            _inQuiz = false; _currentScenario = null;
            QuizPanel.Visibility = Visibility.Collapsed;
            QuizOptions.ItemsSource = null;
            SendBtn.IsEnabled = InputBox.Text.Length > 0;
        }

        // ═════════════════════════════════════════════════════════════════════
        // SIDEBAR BUTTONS
        // ═════════════════════════════════════════════════════════════════════

        private void ScenarioButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_inQuiz && !_awaitingName) StartQuiz();
        }

        private async void FactButton_Click(object sender, RoutedEventArgs e)
        {
            if (_awaitingName) return;
            var (f, c) = _bot.GetRandomFact();
            await AddBotBubbleAsync($"📊  DID YOU KNOW?\n\n{f}\n\n📖  {c}", "CyberInfo");
        }

        private async void TipButton_Click(object sender, RoutedEventArgs e)
        {
            if (_awaitingName) return;
            await AddBotBubbleAsync($"✨  {_bot.GetRandomTip()}", "CyberInfo");
        }

        // ═════════════════════════════════════════════════════════════════════
        // SENTIMENT BAR
        // ═════════════════════════════════════════════════════════════════════

        private void UpdateSentimentBar(SentimentDetector.Sentiment s)
        {
            SentimentLabel.Text = _bot.Sentiment.GetLabel(s);
            SentimentEmoji.Text = " " + _bot.Sentiment.GetEmoji(s);
            string hex = _bot.Sentiment.GetStatusBarColour(s);
            SentimentLabel.Foreground =
                (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
            SentimentBar.Visibility = Visibility.Visible;
        }

        // ═════════════════════════════════════════════════════════════════════
        // MEMORY PANEL  (Action delegate target)
        // ═════════════════════════════════════════════════════════════════════

        private void UpdateMemoryPanel()
        {
            MemLastTopic.Text = $"Last topic: {_bot.Memory.LastTopic ?? "—"}";
            MemInterests.Text = $"Interests: {_bot.Memory.GetInterestSummary()}";
        }

        // ═════════════════════════════════════════════════════════════════════
        // BUBBLE HELPERS
        // ═════════════════════════════════════════════════════════════════════

        private void AppendBotBubble(string text, string senderLabel)
        {
            Dispatcher.Invoke(() =>
            {
                var panel = BuildBubblePanel(text, senderLabel, isUser: false);
                ChatPanel.Children.Add(panel);
                ChatScroll.ScrollToEnd();
            });
        }

        private void AppendUserBubble(string text)
        {
            Dispatcher.Invoke(() =>
            {
                string lbl = string.IsNullOrEmpty(_bot.Memory.UserName) ? "You" : _bot.Memory.UserName;
                var panel = BuildBubblePanel(text, lbl, isUser: true);
                ChatPanel.Children.Add(panel);
                ChatScroll.ScrollToEnd();
            });
        }

        private async Task AddBotBubbleAsync(string text, string senderLabel)
        {
            ShowTyping(true);
            int ms = Math.Clamp(text.Length * 2, 400, 1800);
            await Task.Delay(ms);
            ShowTyping(false);
            AppendBotBubble(text, senderLabel);
        }

        private async Task ShowTypingAsync(int ms)
        {
            ShowTyping(true);
            await Task.Delay(ms);
            ShowTyping(false);
        }

        private void ShowTyping(bool show)
        {
            Dispatcher.Invoke(() => TypingRow.Visibility = show ? Visibility.Visible : Visibility.Collapsed);
        }

        private Border BuildBubblePanel(string text, string senderLabel, bool isUser)
        {
            // Sender label
            var lbl = new TextBlock
            {
                Text = senderLabel,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = isUser
                    ? (Brush)FindResource("AccentPurple")
                    : (Brush)FindResource("AccentCyan"),
                Margin = new Thickness(0, 0, 0, 4),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };

            // Message text
            var msg = new TextBlock
            {
                Text = text,
                FontSize = 13.5,
                Foreground = (Brush)FindResource("TextPrimary"),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };

            // Timestamp
            var ts = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 10,
                Foreground = (Brush)FindResource("TextMuted"),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 4, 0, 0)
            };

            var inner = new StackPanel();
            inner.Children.Add(lbl);
            inner.Children.Add(msg);
            inner.Children.Add(ts);

            var bubble = new Border
            {
                Child = inner,
                Padding = new Thickness(14, 10, 14, 10),
                CornerRadius = isUser
                    ? new CornerRadius(14, 0, 14, 14)
                    : new CornerRadius(0, 14, 14, 14),
                MaxWidth = 680,
                Margin = isUser
                    ? new Thickness(80, 4, 12, 4)
                    : new Thickness(12, 4, 80, 4),
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = isUser
                    ? (Brush)FindResource("BgUserBubble")
                    : (Brush)FindResource("BgBotBubble"),
                BorderBrush = isUser
                    ? (Brush)FindResource("AccentPurple")
                    : (Brush)FindResource("BorderAccent"),
                BorderThickness = new Thickness(1)
            };

            // Fade-in animation
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            bubble.BeginAnimation(OpacityProperty, anim);

            return bubble;
        }

        // ═════════════════════════════════════════════════════════════════════
        // HELPERS
        // ═════════════════════════════════════════════════════════════════════

        private static string? MapToLibKey(string topicName) => topicName.ToLowerInvariant() switch
        {
            "passwords" => "password",
            "phishing" => "phishing",
            "safe browsing" => "safe browsing",
            "two-factor authentication (2fa)" => "2fa",
            "malware" => "malware",
            "social engineering" => "social engineering",
            "software updates" => "updates",
            "public wi-fi" => "wifi",
            _ => null
        };

        private static string HelpText() =>
            "📚  HELP — What CyberInfo can do:\n\n" +
            "  • Type a topic name: 'phishing', 'passwords', '2fa', 'malware'...\n" +
            "  • Type a number (1–8) to select a topic\n" +
            "  • 'more' or 'another tip' — extra tip on the last topic\n" +
            "  • 'scenario' — real-world quiz challenge\n" +
            "  • 'fact' — SA cybersecurity statistic\n" +
            "  • 'tip' — quick random security tip\n" +
            "  • Express your mood and I'll respond with empathy! 😊\n\n" +
            "Ask me anything about staying safe online. 🛡️";
    }
}
