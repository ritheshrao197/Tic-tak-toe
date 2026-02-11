using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Events;

public class GameHUD : UIPanel
{
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
  
    
    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        
        // Setup button listeners
        if (_resetButton != null)
            _resetButton.onClick.AddListener(OnResetClicked);
        
        if (_backToMenuButton != null)
            _backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
      
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        EventBus.Subscribe<TurnChangedEvent>(OnTurnChanged);
        EventBus.Subscribe<ScoreUpdatedEvent>(OnScoreUpdated);
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
        EventBus.Unsubscribe<TurnChangedEvent>(OnTurnChanged);
        EventBus.Unsubscribe<ScoreUpdatedEvent>(OnScoreUpdated);
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        // Ensure game over panel is hidden when showing HUD
        // if (_gameOverPanel != null)
        //     _gameOverPanel.SetActive(false);
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
        // if (_gameOverPanel != null)
        //     _gameOverPanel.SetActive(false);
        
        // Hide the GameOverPanel if it's visible (in case we're starting a new game after game over)
        var gameOverPanel = UIManager.Instance.GetPanel<GameOverPanel>();
        if (gameOverPanel != null && gameOverPanel.IsVisible())
        {
            gameOverPanel.Hide();
        }
        
        // Show this HUD panel if it was hidden
        if (!IsVisible())
        {
            Show();
        }
        
        // Reset score display if needed
        // UpdateScoreDisplay(0, 0, 0);
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
                GameConfigManager.Instance.Player1Symbol : GameConfigManager.Instance.Player2Symbol;
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
    

    private void OnResetClicked()
    {
        EventBus.Publish(new GameResetEvent());
        
  
    }
    
    
    private void OnBackToMenuClicked()
    {
        // Publish event to go back to main menu
        EventBus.Publish(new GameResetEvent());
        
        // Show main menu
        UIManager.Instance.ShowPanel<MainMenuUI>();
        UIManager.Instance.HidePanel<GameHUD>();
        
     
    }
}