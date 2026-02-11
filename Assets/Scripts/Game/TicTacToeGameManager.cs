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
        // Initialize UI Manager first
        UIManager.Instance.Initialize();
        
        // Validate configuration through manager
        if (GameConfigManager.Instance.Config == null)
        {
            Debug.LogError("GameConfig not assigned! Please assign it in the GameConfigManager.");
            enabled = false;
            return;
        }
        
        _boardModel = new BoardModel(GameConfigManager.Instance.BoardSize, GameConfigManager.Instance.WinCondition);
        Debug.Log($"[GameManager] Initialized with board size {GameConfigManager.Instance.BoardSize} and win condition {GameConfigManager.Instance.WinCondition}");
    }
    
    private void OnEnable()
    {
        Debug.Log("[GameManager] Enabled - Subscribing to events");
        // Subscribe to events
        EventBus.Subscribe<GameModeSelectedEvent>(OnGameModeSelected);
        EventBus.Subscribe<CellClickedEvent>(OnCellClicked);
        EventBus.Subscribe<GameResetEvent>(OnGameReset);
    }
    
    private void OnDisable()
    {
        Debug.Log("[GameManager] Disabled - Unsubscribing from events");
        // Unsubscribe from events
        EventBus.Unsubscribe<GameModeSelectedEvent>(OnGameModeSelected);
        EventBus.Unsubscribe<CellClickedEvent>(OnCellClicked);
        EventBus.Unsubscribe<GameResetEvent>(OnGameReset);
    }
    
    private void OnGameModeSelected(GameModeSelectedEvent evt)
    {
        Debug.Log($"[GameManager] Game mode selected: {evt.Mode}");
        _currentMode = evt.Mode;
        StartNewGame();
    }
    
    private void StartNewGame()
    {
        Debug.Log("[GameManager] Starting new game...");
        // Reset board
        _boardModel.Clear();
        
        // Reset game state
        _gameState = GameState.Playing;
        _currentPlayer = PlayerType.Player1;
        
        // Initialize AI if needed
        if (_currentMode == GameMode.PlayerVsComputer)
        {
            Debug.Log("[GameManager] Initializing AI Player");
            _aiPlayer = new AIPlayer( PlayerType.Player2,_boardModel.Size);
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
        Debug.Log($"[GameManager] Game started. First turn: {_currentPlayer}");
    }
    
    private void OnCellClicked(CellClickedEvent evt)
    {
        Debug.Log($"[GameManager] Cell clicked at [{evt.Row}, {evt.Column}]");
        // Ignore clicks if game is over or not player's turn
        if (_gameState != GameState.Playing)
        {
            Debug.Log("[GameManager] Click ignored: Game is not in Playing state.");
            return;
        }
        
        // In PvC mode, ignore clicks when it's AI's turn
        if (_currentMode == GameMode.PlayerVsComputer && _currentPlayer == PlayerType.Player2)
        {
            Debug.Log("[GameManager] Click ignored: It is AI's turn.");
            return;
        }
        
        // Attempt to make move
        if (TryMakeMove(evt.Row, evt.Column, _currentPlayer))
        {
            ProcessMove();
        }
        else
        {
            Debug.LogWarning($"[GameManager] Invalid move attempted at [{evt.Row}, {evt.Column}]");
        }
    }
    
    private bool TryMakeMove(int row, int col, PlayerType player)
    {
        if (!_boardModel.IsValidMove(row, col))
        {
            Debug.LogWarning($"[GameManager] Move invalid: [{row}, {col}] is not empty or out of bounds.");
            return false;
        }
        
        _boardModel.MakeMove(row, col, player);
        Debug.Log($"[GameManager] Move executed: Player {player} at [{row}, {col}]");
        
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
            Debug.Log($"[GameManager] Game Over condition met: {result}");
            HandleGameOver(result, winningLine);
            return;
        }
        
        // Switch turns
        SwitchTurn();
        
        // If it's AI's turn, make AI move
        if (_currentMode == GameMode.PlayerVsComputer && _currentPlayer == PlayerType.Player2)
        {
            Debug.Log("[GameManager] Triggering AI move coroutine.");
            StartCoroutine(MakeAIMove());
        }
    }
    
    private void SwitchTurn()
    {
        PlayerType previousPlayer = _currentPlayer;
        _currentPlayer = _currentPlayer == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        
        Debug.Log($"[GameManager] Switching turn from {previousPlayer} to {_currentPlayer}");
        
        EventBus.Publish(new TurnChangedEvent
        {
            CurrentPlayer = _currentPlayer,
            PreviousPlayer = previousPlayer
        });
    }
    
    private IEnumerator MakeAIMove()
    {
        Debug.Log($"[GameManager] AI thinking for {GameConfigManager.Instance.AIThinkingTime} seconds...");
        // Wait for thinking time (for visual feedback)
        yield return new WaitForSeconds(GameConfigManager.Instance.AIThinkingTime);
        
        // Get AI move
        var move = _aiPlayer.GetBestMove(_boardModel.GetBoardCopy(), GameConfigManager.Instance.Difficulty);
        Debug.Log($"[GameManager] AI selected move: [{move.x}, {move.y}]");
        
        if (move.x >= 0 && move.y >= 0)
        {
            if (TryMakeMove(move.x, move.y, _currentPlayer))
            {
                ProcessMove();
            }
        }
        else
        {
            Debug.LogError("[GameManager] AI returned invalid move!");
        }
    }
    
    private void HandleGameOver(GameResult result, List<Vector2Int> winningLine)
    {
        Debug.Log($"[GameManager] Handling Game Over. Result: {result}");
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
        
        Debug.Log($"[GameManager] Scores Updated - P1: {_player1Score}, P2: {_player2Score}, Draw: {_drawScore}");
        
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
        Debug.Log("[GameManager] Game Reset requested.");
        StartNewGame();
    }
    
    // Public methods for testing/debugging
    public GameState GetCurrentState() => _gameState;
    public PlayerType GetCurrentPlayer() => _currentPlayer;
    public GameMode GetGameMode() => _currentMode;
}