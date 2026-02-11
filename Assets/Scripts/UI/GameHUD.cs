using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Events;

public class GameHUD : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfig _config;
    
    [Header("Turn Display")]
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private GameObject _player1TurnIndicator;
    [SerializeField] private GameObject _player2TurnIndicator;
    
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI _player1ScoreText;
    [SerializeField] private TextMeshProUGUI _player2ScoreText;
    [SerializeField] private TextMeshProUGUI _drawScoreText;
    
    [Header("Game Mode Display")]
    [SerializeField] private TextMeshProUGUI _gameModeText;
    
    [Header("Buttons")]
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _backToMenuButton;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private Button _playAgainButton;
    
    private void Awake()
    {
        // Setup button listeners
        if (_resetButton != null)
            _resetButton.onClick.AddListener(OnResetClicked);
        
        if (_backToMenuButton != null)
            _backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        
        if (_playAgainButton != null)
            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        
        // Hide game over panel
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
    }
    
    private void OnEnable()
    {
        EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        EventBus.Subscribe<TurnChangedEvent>(OnTurnChanged);
        EventBus.Subscribe<ScoreUpdatedEvent>(OnScoreUpdated);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
        EventBus.Unsubscribe<TurnChangedEvent>(OnTurnChanged);
        EventBus.Unsubscribe<ScoreUpdatedEvent>(OnScoreUpdated);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }
    
    private void OnGameStarted(GameStartedEvent evt)
    {
        // Update game mode text
        if (_gameModeText != null)
        {
            string modeText = evt.Mode == GameMode.PlayerVsPlayer ? 
                "Player vs Player" : "Player vs Computer";
            _gameModeText.text = modeText;
        }
        
        // Hide game over panel
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
        
        // Reset score display if needed
        UpdateScoreDisplay(0, 0, 0);
    }
    
    private void OnTurnChanged(TurnChangedEvent evt)
    {
        UpdateTurnDisplay(evt.CurrentPlayer);
    }
    
    private void UpdateTurnDisplay(PlayerType currentPlayer)
    {
        if (_turnText != null)
        {
            string playerName = currentPlayer == PlayerType.Player1 ? "Player 1" : "Player 2";
            string symbol = currentPlayer == PlayerType.Player1 ? 
                _config.Player1Symbol : _config.Player2Symbol;
            _turnText.text = $"{playerName}'s Turn ({symbol})";
        }
        
        // Update turn indicators
        if (_player1TurnIndicator != null)
            _player1TurnIndicator.SetActive(currentPlayer == PlayerType.Player1);
        
        if (_player2TurnIndicator != null)
            _player2TurnIndicator.SetActive(currentPlayer == PlayerType.Player2);
    }
    
    private void OnScoreUpdated(ScoreUpdatedEvent evt)
    {
        UpdateScoreDisplay(evt.Player1Score, evt.Player2Score, evt.DrawScore);
    }
    
    private void UpdateScoreDisplay(int p1Score, int p2Score, int draws)
    {
        if (_player1ScoreText != null)
            _player1ScoreText.text = $"Player 1: {p1Score}";
        
        if (_player2ScoreText != null)
            _player2ScoreText.text = $"Player 2: {p2Score}";
        
        if (_drawScoreText != null)
            _drawScoreText.text = $"Draws: {draws}";
    }
    
    private void OnGameOver(GameOverEvent evt)
    {
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);
        
        if (_gameOverText != null)
        {
            string resultText = "";
            
            switch (evt.Result)
            {
                case GameResult.Player1Win:
                    resultText = $"Player 1 ({_config.Player1Symbol}) Wins!";
                    break;
                case GameResult.Player2Win:
                    resultText = $"Player 2 ({_config.Player2Symbol}) Wins!";
                    break;
                case GameResult.Draw:
                    resultText = "It's a Draw!";
                    break;
            }
            
            _gameOverText.text = resultText;
        }
    }
    
    private void OnResetClicked()
    {
        EventBus.Publish(new GameResetEvent());
        
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
    }
    
    private void OnPlayAgainClicked()
    {
        OnResetClicked();
    }
    
    private void OnBackToMenuClicked()
    {
        // Find and call main menu
        MainMenuUI mainMenu = FindObjectOfType<MainMenuUI>();
        if (mainMenu != null)
        {
            mainMenu.BackToMainMenu();
        }
        
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
    }
}
