# CyberInfo - Cybersecurity Awareness Bot

## Overview

**CyberInfo** is an interactive WPF-based cybersecurity awareness chatbot designed to educate users about online safety and cybersecurity best practices. This application provides real-world security challenges, expert tips, and personalized guidance to help users stay safe in the digital world.

## Features

### 🎓 Core Features
- **Interactive Chat Interface**: Engage in natural conversations about cybersecurity topics
- **8 Core Security Topics**: 
  - Passwords
  - Phishing Attacks
	- 
  - Safe Browsing
  - Two-Factor Authentication (2FA)
  - Malware
  - Social Engineering
  - Software Updates
  - Public Wi-Fi Security

- **Real-World Scenarios**: Test your knowledge with interactive security challenges and quizzes
- **Random Security Tips**: Get quick security tips on demand
- **Cybersecurity Facts**: Learn interesting SA-focused cybersecurity statistics
- **Sentiment Detection**: The bot responds with empathy based on your emotional tone

### 💭 Smart Features
- **Keyword Recognition**: Automatically detects when you mention cybersecurity topics
- **User Memory**: Remembers your interests and personalizes responses
- **Empathetic Responses**: Analyzes your emotional state and responds accordingly
- **Topic Navigation**: Type topic names, numbers, or keywords to learn more
- **Follow-up Tips**: Ask for 'more' or 'another tip' for additional information

### 🎵 Audio Features
- **Automatic Greeting**: Audio greeting plays when the application starts
- **Visual Feedback**: Real-time sentiment analysis with visual indicators

## System Requirements

- **.NET 10** or later
- **Windows 10/11** (WPF application)
- **4GB RAM** minimum
- **500MB** disk space

## Installation

### Prerequisites
- Visual Studio 2022 or later (Community Edition works fine)
- .NET 10 SDK installed

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/owethu948/CyberInfo.git
   cd CyberInfo
   ```

2. Open the solution in Visual Studio:
   ```bash
   start CyberInfo/CyberInfo.sln
   ```

3. Build the solution (Ctrl+Shift+B or Build → Build Solution)

4. Run the application (F5 or Debug → Start Debugging)

## Usage Guide

### Starting the Bot
1. Launch the application
2. You'll see a greeting from CyberInfo
3. Enter your name when prompted
4. Start asking questions or exploring topics!

### How to Interact

#### **Topic Selection**
- **By Name**: Type "passwords", "phishing", "malware", etc.
- **By Number**: Type 1-8 to select a topic
- **Keyword Matching**: Mention any cybersecurity term and the bot will respond

#### **Commands**
- `scenario` or `quiz` - Start an interactive security challenge
- `fact` or `did you know` - Get a cybersecurity statistic
- `tip` or `random tip` - Get a quick security tip
- `more` or `another tip` - Get additional tips on the current topic
- `help` or `?` - Show all available commands

#### **Emotional Engagement**
- Express your feelings: "I'm worried about passwords" or "This is awesome!"
- The bot analyzes your sentiment and responds with appropriate support
- Empathetic responses help you feel supported while learning

### Example Interactions
```
You: Hi, what should I call you?
CyberInfo: I'm CyberInfo — your friendly cyber guardian. 💙

You: I'm concerned about phishing attacks
CyberInfo: [Empathetic response] + tips on phishing protection

You: scenario
CyberInfo: [Presents a real-world security challenge]

You: What about passwords?
CyberInfo: [Detailed guide on password security]
```

## Architecture

### Project Structure
```
CyberInfo/
├── Engine/
│   ├── SecurityAdvisor.cs        # Core bot logic
│   ├── ThreatRecognizer.cs       # Keyword/threat detection
│   ├── UserProfile.cs            # User memory & preferences
│   ├── EmotionAnalyzer.cs        # Sentiment detection
│   ├── SecurityTipsLibrary.cs    # Response pools & tips
│   ├── AudioManager.cs           # Audio playback
│   └── MemoryStore.cs            # (Legacy) User data storage
├── Models/
│   ├── Message.cs                # Chat message structure
│   ├── SecurityChallenge.cs      # Quiz scenarios
│   └── SecurityTopic.cs          # Topic data model
├── Resources/
│   └── Greeting AI.wav           # Audio greeting file
├── MainWindow.xaml               # WPF UI layout
├── MainWindow.xaml.cs            # WPF code-behind
└── App.xaml                      # Application configuration
```

### Key Classes

#### **SecurityAdvisor** (formerly Bot)
- Core application logic
- Manages topics, scenarios, and facts
- Generates appropriate responses
- **Properties**: Topics, Memory, Keywords, Sentiment, Library

#### **ThreatRecognizer** (formerly KeywordEngine)
- Detects cybersecurity keywords in user input
- Maps keywords to topics
- Identifies follow-up requests and interest declarations
- **Methods**: FindTopicKey(), FindTopicName(), IsFollowUp()

#### **UserProfile** (formerly MemoryStore)
- Stores user name and interests
- Tracks conversation history
- Provides personalization
- **Properties**: UserName, Interests, LastTopic

#### **EmotionAnalyzer** (formerly SentimentDetector)
- Analyzes user sentiment (Worried, Curious, Frustrated, Happy, Neutral)
- Provides empathetic responses
- Returns sentiment-based visual indicators
- **Enum**: Sentiment

#### **SecurityTipsLibrary** (formerly ResponseLibrary)
- Repository of security tips organized by topic
- Custom delegate-based response selection
- Event-driven tip tracking
- **Delegate**: ResponseSelector

#### **AudioManager** (formerly SoundOutput)
- Static utility for audio playback
- Plays greeting audio on startup
- Non-blocking async audio playback
- **Method**: PlayGreetingAsync()

## Features in Detail

### 1. **Natural Language Processing**
The bot uses a keyword recognition engine to understand user input:
- Matches common security terms and concepts
- Identifies follow-up requests ("tell me more", "another tip")
- Recognizes interest declarations ("I'm interested in...", "I'm worried about...")

### 2. **Sentiment Analysis**
Five sentiment levels with different response strategies:
- **Worried/Concerned**: Provides reassurance + practical tips
- **Curious**: Detailed explanations + additional resources
- **Frustrated**: Comfort + encouragement + helpful tips
- **Happy**: Celebrates the positive sentiment + reinforcement
- **Neutral**: Standard informative response

### 3. **Scenario-Based Learning**
Interactive challenges covering real-world situations:
- Email phishing attempts
- Public Wi-Fi security dilemmas
- Tech support scams
- Social engineering attacks
- Advance fee fraud

### 4. **Memory & Personalization**
- Remembers user's name and interests
- Tracks conversation history
- Provides personalized greetings
- References previous topics in recommendations

### 5. **South African Context**
- South Africa-specific cybersecurity statistics
- SABRIC (South African Banking Risk Information Centre) data
- Relevant threat examples for SA users
- Local best practices and recommendations

## Technologies Used

- **Language**: C# 12 (.NET 10)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: Object-Oriented Programming with Encapsulation
- **Patterns**: Delegate patterns, Factory patterns
- **Collections**: Generic collections (List<T>, Dictionary<K,V>, HashSet<T>, Queue<T>)

## Code Highlights

### Delegates & Functional Programming
```csharp
// Func<T, TResult> delegates
public readonly Func<string, string?> FindTopicKey;
public readonly Func<string, string?> FindTopicName;

// Predicate<T> delegates
public readonly Predicate<string> HasCyberKeyword;
public readonly Predicate<string> IsWorried;

// Custom ResponseSelector delegate
public delegate string ResponseSelector(string topic);
```

### LINQ & Generic Collections
```csharp
public Topic? FindTopic(string name) =>
	Topics.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
```

### Async/Await for Responsive UI
```csharp
private async Task AddBotBubbleAsync(string text, string senderLabel)
{
	ShowTyping(true);
	int ms = Math.Clamp(text.Length * 2, 400, 1800);
	await Task.Delay(ms);
	ShowTyping(false);
	AppendBotBubble(text, senderLabel);
}
```

## Development Setup

### For Contributors
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Follow C# naming conventions:
   - Classes: PascalCase
   - Methods: PascalCase
   - Properties: PascalCase
   - Fields: _camelCase (private) or camelCase (local)
4. Add comments for complex logic
5. Test thoroughly before submitting PR

### Building
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests (if applicable)
dotnet test
```

### Debugging
- Set breakpoints in Visual Studio
- Use Debug → Windows → Output to view diagnostic messages
- Check Ctrl+Alt+D for Debug output

## Known Limitations

1. **Audio Playback**: Requires WAV file in Resources folder named "Greeting AI .wav"
2. **Single User**: Designed for single-user interaction (no multi-user support)
3. **Offline Only**: No internet connection required or used
4. **Windows Only**: WPF is Windows-specific
5. **Fixed Topics**: Topics are hardcoded; no dynamic content loading

## Future Enhancements

- [ ] Database integration for user profiles
- [ ] Multi-language support
- [ ] Machine learning sentiment analysis
- [ ] Web-based version (ASP.NET Core)
- [ ] Mobile app companion
- [ ] Advanced NLP using ML.NET
- [ ] Admin dashboard for content management
- [ ] User progress tracking & certificates

## Troubleshooting

### Audio Not Playing
- **Problem**: Greeting audio doesn't play on startup
- **Solution**: 
  1. Verify "Greeting AI .wav" exists in CyberInfo/Resources/
  2. Check file path is correct
  3. Ensure Windows Media Player is not disabled
  4. Check system volume settings

### Application Crashes on Startup
- **Problem**: Application throws exception
- **Solution**:
  1. Ensure .NET 10 is installed: `dotnet --version`
  2. Clean & rebuild: Build → Clean Solution, then Build
  3. Check Output window (Ctrl+Alt+O) for errors

### Chatbot Doesn't Respond
- **Problem**: No response to user input
- **Solution**:
  1. Check that text is entered in input box
  2. Verify SEND button is enabled
  3. Check debug output for exceptions

## Contributing

We welcome contributions! Please:
1. Report bugs via GitHub Issues
2. Suggest features in Discussions
3. Submit PRs with clear descriptions
4. Follow the code style guide above
5. Update documentation with changes

## License

[Specify your license here - e.g., MIT, Apache 2.0, etc.]

## Support & Contact

- **GitHub**: [https://github.com/owethu948/CyberInfo](https://github.com/owethu948/CyberInfo)
- **Issues**: Report bugs on [GitHub Issues](https://github.com/owethu948/CyberInfo/issues)
- **Email**: [your-email@example.com]

## Acknowledgments

- **Data Sources**: SABRIC (South African Banking Risk Information Centre), Verizon DBIR, NIST, Pieterse (2021)
- **Inspiration**: Modern cybersecurity awareness training programs
- **Technology**: Microsoft .NET Team

## Version History

### v1.0.0 (Current)
- ✅ Core chatbot functionality
- ✅ 8 security topics
- ✅ 5 interactive scenarios
- ✅ Sentiment analysis
- ✅ Audio greeting
- ✅ User memory system
- ✅ Keyword recognition

### v0.1.0 (Initial Release)
- Basic chatbot structure
- Topic database
- Sentiment detection basics

---

**CyberInfo** - Making Cybersecurity Education Accessible to Everyone 🛡️

Last Updated: 2024
