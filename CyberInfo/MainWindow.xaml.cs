using CyberInfo.Engine;
using CyberInfo.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberInfo
{
    public partial class MainWindow : Window
    {
        private readonly BotEngine _bot = new BotEngine();
        private Action _refreshMemory;

        public MainWindow()
        {
            InitializeComponent();

            _refreshMemory = UpdateMemoryPanel;

            _bot.Keywords.SetOnMatch((kw, topic) =>
                System.Diagnostics.Debug.WriteLine($"[KeywordEngine] '{kw}' => {topic}"));

            _bot.Library.OnTipSelected += (topic, tip) =>
                System.Diagnostics.Debug.WriteLine($"[Library] Tip served for: {topic}");

            AudioManager.PlayGreetingAsync();

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

        private void UpdateMemoryPanel()
        {
            Dispatcher.Invoke(() =>
            {
                MemLastTopic.Text = string.IsNullOrEmpty(_bot.Memory.LastTopic) ? "—" : _bot.Memory.LastTopic;
                MemInterests.Text = _bot.Memory.GetInterestSummary();
                UserBadge.Text = string.IsNullOrEmpty(_bot.Memory.UserName) ? "Welcome!" : $"Hi, {_bot.Memory.UserName}!";
            });
        }

        private void AppendBotBubble(string message, string sender)
        {
            Dispatcher.Invoke(() =>
            {
                var bubble = new Border
                {
                    Background = sender == "User" ? (Brush)FindResource("BgUserBubble") : (Brush)FindResource("BgBotBubble"),
                    CornerRadius = new CornerRadius(12),
                    Margin = new Thickness(0, 4, 0, 4),
                    Padding = new Thickness(12, 8, 12, 8),
                    MaxWidth = 500,
                    HorizontalAlignment = sender == "User" ? HorizontalAlignment.Right : HorizontalAlignment.Left
                };
                var text = new TextBlock
                {
                    Text = $"{sender}: {message}",
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = (Brush)FindResource("TextPrimary"),
                    FontSize = 13
                };
                bubble.Child = text;
                ChatPanel.Children.Add(bubble);
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
            UpdateMemoryPanel();
        }

        private void ShowTyping(bool show)
        {
            Dispatcher.Invoke(() => TypingRow.Visibility = show ? Visibility.Visible : Visibility.Collapsed);
        }

        private async void SendUserMessage(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput)) return;

            AppendBotBubble(userInput, "User");
            InputBox.Clear();

            string response = _bot.ProcessInput(userInput, out string sentimentResponse, out var sentiment);

            // Show sentiment bar
            if (!string.IsNullOrEmpty(sentimentResponse))
            {
                SentimentBar.Visibility = Visibility.Visible;
                SentimentLabel.Text = _bot.Sentiment.GetLabel(sentiment);
                SentimentEmoji.Text = _bot.Sentiment.GetEmoji(sentiment);
                SentimentBar.Background = (Brush)new BrushConverter().ConvertFromString(_bot.Sentiment.GetStatusBarColour(sentiment));
                var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                timer.Tick += (s, e) => { SentimentBar.Visibility = Visibility.Collapsed; timer.Stop(); };
                timer.Start();
            }

            if (_bot.HasActiveQuiz())
                ShowQuizPanel(_bot.GetActiveQuiz());
            else
                HideQuizPanel();

            await AddBotBubbleAsync(response, "CyberInfo");
        }

        private void ShowQuizPanel(SecurityChallenge? challenge)
        {
            if (challenge == null) return;
            Dispatcher.Invoke(() =>
            {
                QuizDesc.Text = challenge.Description;
                QuizOptions.ItemsSource = challenge.Options.Select((opt, idx) => new { Label = $"{idx + 1}. {opt}", Idx = idx });
                QuizPanel.Visibility = Visibility.Visible;
            });
        }

        private void HideQuizPanel()
        {
            Dispatcher.Invoke(() => QuizPanel.Visibility = Visibility.Collapsed);
        }

        // Event handlers
        private async void SendButton_Click(object sender, RoutedEventArgs e) => SendUserMessage(InputBox.Text);
        private void InputBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) SendUserMessage(InputBox.Text); }
        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasText = !string.IsNullOrWhiteSpace(InputBox.Text);
            SendBtn.IsEnabled = hasText;
            Placeholder.Visibility = hasText ? Visibility.Collapsed : Visibility.Visible;
        }

        // Clear button handler
        private void ClearInput_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Clear();
            InputBox.Focus();
        }

        private async void ScenarioButton_Click(object sender, RoutedEventArgs e) => SendUserMessage("scenario");
        private async void FactButton_Click(object sender, RoutedEventArgs e) => SendUserMessage("fact");
        private async void TipButton_Click(object sender, RoutedEventArgs e) => SendUserMessage("tip");

        private async void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int idx)
            {
                HideQuizPanel();
                SendUserMessage((idx + 1).ToString());
            }
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            HideQuizPanel();
            _ = AddBotBubbleAsync("Quiz cancelled. Feel free to ask me anything else!", "CyberInfo");
        }
    }
}