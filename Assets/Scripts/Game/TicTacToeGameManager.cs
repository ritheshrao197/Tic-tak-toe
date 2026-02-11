using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TicTacToe.Events;

public class TicTacToeGameManager : MonoBehaviour
{
    private BoardModel _boardModel;
    private AIPlayer _aiPlayer;
    private GameMode _currentMode;
    private GameState _gameState;
    private PlayerType _currentPlayer;

    // Score tracking
    private int _player1Score = 0;
    private int _player2Score = 0;
    private int _drawScore = 0;

    private void Awake()
    {
        // Ensure game is properly initialized
        // if (!GameInitializer.IsInitialized)
        // {
        //     GameInitializer.ForceInitialize();
        // }

        // Validate configuration through manager
        if (GameConfigManager.Instance.Config == null)
        {
            GameLogger.LogError("GameConfig not assigned! Please assign it in the GameConfigManager.", GameLogger.LogCategory.Configuration);
            enabled = false;
            return;
        }

        _boardModel = new BoardModel(GameConfigManager.Instance.BoardSize, GameConfigManager.Instance.WinCondition);
        GameLogger.LogInfo($"Initialized with board size {GameConfigManager.Instance.BoardSize} and win condition {GameConfigManager.Instance.WinCondition}", GameLogger.LogCategory.GameLogic);
    }

    private void OnEnable()
    {
        GameLogger.LogDebug("Enabled - Subscribing to events", GameLogger.LogCategory.Events);
        // Subscribe to events
        EventBus.Subscribe<GameModeSelectedEvent>(OnGameModeSelected);
        EventBus.Subscribe<CellClickedEvent>(OnCellClicked);
        EventBus.Subscribe<GameResetEvent>(OnGameReset);
    }

    private void OnDisable()
    {
        GameLogger.LogDebug("Disabled - Unsubscribing from events", GameLogger.LogCategory.Events);
        // Unsubscribe from events
        EventBus.Unsubscribe<GameModeSelectedEvent>(OnGameModeSelected);
        EventBus.Unsubscribe<CellClickedEvent>(OnCellClicked);
        EventBus.Unsubscribe<GameResetEvent>(OnGameReset);
    }

    private void OnGameModeSelected(GameModeSelectedEvent evt)
    {
        GameLogger.LogInfo($"Game mode selected: {evt.Mode}", GameLogger.LogCategory.GameLogic);
        _currentMode = evt.Mode;
        ScoreReset();
        StartNewGame();
    }

    private void ScoreReset()
    {
        _player1Score = 0;
        _player2Score = 0;
        _drawScore = 0;

        // Publish score update
        EventBus.Publish(new ScoreUpdatedEvent
        {
            Player1Score = _player1Score,
            Player2Score = _player2Score,
            DrawScore = _drawScore
        });
    }

    private void StartNewGame()
    {
        GameLogger.LogInfo("Starting new game...", GameLogger.LogCategory.GameLogic);
        // Reset board
        _boardModel.Clear();

        // Reset game state
        _gameState = GameState.Playing;
        _currentPlayer = PlayerType.Player1;

        // Initialize AI if needed
        if (_currentMode == GameMode.PlayerVsComputer)
        {
            GameLogger.LogInfo("Initializing AI Player", GameLogger.LogCategory.AI);
            _aiPlayer = new AIPlayer(PlayerType.Player2, _boardModel.Size);
        }

        // Publish game started event
        EventBus.Publish(new GameStartedEvent
        {
            Mode = _currentMode,
            StartingPlayer = _currentPlayer
        });

        // Publish initial turn
        EventBus.Publish(new TurnChangedEvent
        {
            CurrentPlayer = _currentPlayer,
            PreviousPlayer = PlayerType.None
        });

        GameLogger.LogInfo($"Game started. First turn: {_currentPlayer}", GameLogger.LogCategory.GameLogic);
    }

    private void OnCellClicked(CellClickedEvent evt)
    {
        GameLogger.LogDebug($"Cell clicked at [{evt.Row}, {evt.Column}]", GameLogger.LogCategory.GameLogic);
        // Ignore clicks if game is over or not player's turn
        if (_gameState != GameState.Playing)
        {
            GameLogger.LogDebug("Click ignored: Game is not in Playing state.", GameLogger.LogCategory.GameLogic);
            return;
        }

        // In PvC mode, ignore clicks when it's AI's turn
        if (_currentMode == GameMode.PlayerVsComputer && _currentPlayer == PlayerType.Player2)
        {
            GameLogger.LogDebug("Click ignored: It is AI's turn.", GameLogger.LogCategory.GameLogic);
            return;
        }

        // Attempt to make move
        if (TryMakeMove(evt.Row, evt.Column, _currentPlayer))
        {
            ProcessMove();
        }
        else
        {
            GameLogger.LogWarning($"Invalid move attempted at [{evt.Row}, {evt.Column}]", GameLogger.LogCategory.GameLogic);
        }
    }

    private bool TryMakeMove(int row, int col, PlayerType player)
    {
        if (!_boardModel.IsValidMove(row, col))
        {
            GameLogger.LogWarning($"Move invalid: [{row}, {col}] is not empty or out of bounds.", GameLogger.LogCategory.GameLogic);
            return false;
        }

        _boardModel.MakeMove(row, col, player);
        GameLogger.LogDebug($"Move executed: Player {player} at [{row}, {col}]", GameLogger.LogCategory.GameLogic);

        // Get symbol for this player
        string symbol = player == PlayerType.Player1 ? GameConfigManager.Instance.Player1Symbol : GameConfigManager.Instance.Player2Symbol;

        // Publish move event
        EventBus.Publish(new MoveExecutedEvent
        {
            Row = row,
            Column = col,
            Player = player,
            Symbol = symbol
        });

        return true;
    }

    private void ProcessMove()
    {
        // Check for win or draw
        List<Vector2Int> winningLine;
        GameResult result = _boardModel.CheckWinner(out winningLine);

        if (result != GameResult.None)
        {
            GameLogger.LogInfo($"Game Over condition met: {result}", GameLogger.LogCategory.GameLogic);
            HandleGameOver(result, winningLine);
            return;
        }

        // Switch turns
        SwitchTurn();

        // If it's AI's turn, make AI move
        if (_currentMode == GameMode.PlayerVsComputer && _currentPlayer == PlayerType.Player2)
        {
            GameLogger.LogDebug("Triggering AI move coroutine.", GameLogger.LogCategory.AI);
            StartCoroutine(MakeAIMove());
        }
    }

    private void SwitchTurn()
    {
        PlayerType previousPlayer = _currentPlayer;
        _currentPlayer = _currentPlayer == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;

        GameLogger.LogDebug($"Switching turn from {previousPlayer} to {_currentPlayer}", GameLogger.LogCategory.GameLogic);

        EventBus.Publish(new TurnChangedEvent
        {
            CurrentPlayer = _currentPlayer,
            PreviousPlayer = previousPlayer
        });
    }

    private IEnumerator MakeAIMove()
    {
        GameLogger.LogDebug($"AI thinking for {GameConfigManager.Instance.AIThinkingTime} seconds...", GameLogger.LogCategory.AI);
        // Wait for thinking time (for visual feedback)
        yield return new WaitForSeconds(GameConfigManager.Instance.AIThinkingTime);

        // Get AI move
        var move = _aiPlayer.GetBestMove(_boardModel.GetBoardCopy(), GameConfigManager.Instance.Difficulty);
        GameLogger.LogDebug($"AI selected move: [{move.x}, {move.y}]", GameLogger.LogCategory.AI);

        if (move.x >= 0 && move.y >= 0)
        {
            if (TryMakeMove(move.x, move.y, _currentPlayer))
            {
                ProcessMove();
            }
        }
        else
        {
            GameLogger.LogError("AI returned invalid move!", GameLogger.LogCategory.AI);
        }
    }

    private void HandleGameOver(GameResult result, List<Vector2Int> winningLine)
    {
        GameLogger.LogInfo($"Handling Game Over. Result: {result}", GameLogger.LogCategory.GameLogic);
        _gameState = GameState.GameOver;

        // Update scores
        switch (result)
        {
            case GameResult.Player1Win:
                _player1Score++;
                break;
            case GameResult.Player2Win:
                _player2Score++;
                break;
            case GameResult.Draw:
                _drawScore++;
                break;
        }

        GameLogger.LogInfo($"Scores Updated - P1: {_player1Score}, P2: {_player2Score}, Draw: {_drawScore}", GameLogger.LogCategory.GameLogic);

        // Publish score update
        EventBus.Publish(new ScoreUpdatedEvent
        {
            Player1Score = _player1Score,
            Player2Score = _player2Score,
            DrawScore = _drawScore
        });

        // Determine winner
        PlayerType winner = PlayerType.None;
        if (result == GameResult.Player1Win)
            winner = PlayerType.Player1;
        else if (result == GameResult.Player2Win)
            winner = PlayerType.Player2;

        // Publish game over event
        EventBus.Publish(new GameOverEvent
        {
            Result = result,
            Winner = winner,
            WinningLine = winningLine?.ToArray()
        });
    }

    private void OnGameReset(GameResetEvent evt)
    {
        GameLogger.LogInfo("Game Reset requested.", GameLogger.LogCategory.GameLogic);
        StartNewGame();
    }

    // Public methods for testing/debugging
    public GameState GetCurrentState() => _gameState;
    public PlayerType GetCurrentPlayer() => _currentPlayer;
    public GameMode GetGameMode() => _currentMode;
}