# Tic-Tac-Toe Game

A modern, feature-rich Tic-Tac-Toe implementation built with Unity. This project demonstrates professional game development practices with a clean architecture, event-driven system, and production-ready code structure.

## 🎮 Features

### Core Gameplay
- **Classic 3x3 Tic-Tac-Toe** with standard rules
- **Player vs Player** mode for local multiplayer
- **Player vs Computer** mode with AI opponent
- **Scalable Board Size** (configurable beyond 3x3)
- **Win Condition Customization** (adjustable number of marks needed to win)

### AI System
- **Three Difficulty Levels**:
  - Easy: Random moves
  - Medium: Strategic blocking and winning moves
  - Hard: Unbeatable Minimax algorithm
- **Configurable Thinking Time** for realistic AI behavior

### UI System
- **Modular UI Architecture** with base panel system
- **Event-Driven UI Updates** for responsive gameplay
- **Animated Transitions** between game states
- **Score Tracking** with persistent statistics
- **Game Over Panel** with play again and menu options

### Technical Features
- **Production-Ready Architecture** with proper error handling
- **Centralized Configuration Management**
- **Configurable Logging System** with categories and levels
- **Build Configuration** for different environments (Development/Production)
- **Singleton Pattern** implementation for managers
- **Event Bus System** for decoupled communication

## 🏗️ Architecture Overview

### Core Components

```
TicTacToe/
├── Scripts/
│   ├── Core/
│   │   ├── Events/          # Event system and interfaces
│   │   ├── Logging/         # Centralized logging system
│   │   └── GameConstants.cs # Game-wide constants
│   ├── Game/
│   │   ├── TicTacToeGameManager.cs  # Main game controller
│   │   ├── AIPlayer.cs              # AI implementation
│   │   ├── BoardModel.cs            # Game logic (pure C#)
│   │   ├── GameConfigManager.cs     # Configuration management
│   │   └── GameInitializer.cs       # System initialization
│   └── UI/
│       ├── Base/
│       │   └── UIPanel.cs           # Base UI panel class
│       ├── UIManager.cs             # UI panel management
│       ├── GameHUD.cs               # In-game UI
│       ├── MainMenuUI.cs            # Main menu
│       ├── BoardUIManager.cs        # Board visualization
│       └── GameOverPanel.cs         # Game over screen
```

### Key Design Patterns

- **Event-Driven Architecture**: Decoupled components communicate through events
- **Singleton Pattern**: Managers for global access (EventBus, UIManager, etc.)
- **Inheritance Hierarchy**: Base UIPanel class with specialized implementations
- **State Management**: Clear game states (MainMenu, Playing, GameOver)
- **Dependency Injection**: Configuration through GameConfigManager

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 LTS or later
- TextMeshPro package (included in project)

### Setup
1. Clone or download the project
2. Open in Unity Hub
3. Ensure all required packages are imported
4. Open the main scene
5. Assign GameConfig asset in the inspector to GameConfigManagerInitializer
6. Play the game!

### Configuration
The game can be configured through the `GameConfig` ScriptableObject:
- Board size and win conditions
- Player symbols and colors
- AI difficulty and thinking time
- UI settings and animations

## 🎯 Usage

### Playing the Game
1. **Start Screen**: Choose between Player vs Player or Player vs Computer
2. **Gameplay**: Click on board cells to place your marks
3. **Win/Lose**: Game automatically detects wins, losses, and draws
4. **Continue**: Use "Play Again" to continue with current scores or "Back to Menu" to reset

### AI Difficulty Levels
- **Easy**: Makes random valid moves
- **Medium**: Attempts to win and block opponent wins
- **Hard**: Uses Minimax algorithm for optimal play

## 🔧 Development

### Project Structure
The codebase follows Unity best practices with clear separation of concerns:
- **Pure Game Logic**: BoardModel contains no Unity dependencies
- **UI Separation**: Visual components separated from game logic
- **Event System**: Loose coupling between components
- **Configuration Management**: Centralized settings management

### Logging System
The game includes a sophisticated logging system:
```csharp
GameLogger.LogInfo("Game started", GameLogger.LogCategory.GameLogic);
GameLogger.LogWarning("Invalid move", GameLogger.LogCategory.GameLogic);
GameLogger.LogError("Configuration missing", GameLogger.LogCategory.Configuration);
```

## 📊 Technical Details

### Performance Considerations
- **Minimax Optimization**: Alpha-beta pruning for AI efficiency
- **Event System**: Efficient pub/sub pattern for communication
- **Object Pooling**: Reuse of UI elements where applicable
- **Memory Management**: Proper cleanup and resource handling

### Code Quality
- **SOLID Principles**: Single responsibility, open/closed principles
- **Clean Code**: Meaningful naming, proper documentation
- **Error Handling**: Graceful degradation and user feedback
- **Testability**: Pure functions and mockable components

## 🛠️ Customization

### Adding New Features
1. **New Game Modes**: Extend GameMode enum and add logic in GameManager
2. **Additional UI Panels**: Inherit from UIPanel base class
3. **New AI Strategies**: Extend AIPlayer with different algorithms
4. **Custom Rules**: Modify BoardModel win condition logic

### Configuration Options
Most game aspects can be customized through:
- GameConfig ScriptableObject
- UI Panel inspector settings
- Animation controllers
- Event subscriptions

## 🐛 Troubleshooting

### Common Issues
- **UI Not Showing**: Check UIManager registration and panel references
- **AI Not Moving**: Verify GameConfig assignment and AI initialization
- **Scores Not Saving**: Ensure GameResetEvent is properly handled
- **Configuration Errors**: Check GameConfigManager setup in scene

### Debugging
- Use GameLogger with different log levels
- Enable specific log categories for targeted debugging
- Check Unity console for initialization messages

## 📝 License

This project is for educational and demonstration purposes.

## 🙏 Acknowledgments

- Unity Technologies for the game engine
- TextMeshPro for enhanced text rendering
- Minimax algorithm community for AI implementation inspiration

---

*Built with ❤️ using Unity and C#*