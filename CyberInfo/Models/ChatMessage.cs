using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CyberInfo.Models

{
    /// <summary>Single chat bubble in the conversation.  Implements INPC for live binding.</summary>
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text = string.Empty;

        public string Sender { get; init; } = "Bot";
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public string Icon { get; init; } = string.Empty;

        public bool IsUser => Sender == "User";
        public bool IsBot => Sender == "Bot";
        public bool IsSystem => Sender == "System";
        public string TimestampText => Timestamp.ToString("HH:mm");

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

