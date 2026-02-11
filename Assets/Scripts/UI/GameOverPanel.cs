using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Events;

public class GameOverPanel : UIPanel
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backButton;
    
    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
                  

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        GameLogger.LogInfo("GameOverPanel OnEnable");
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        GameLogger.LogInfo("GameOverPanel OnDisable");
    }
    
    private void OnGameOver(GameOverEvent evt)
    {
        GameLogger.LogInfo("GameOverPanel OnGameOver");
       
        if (gameOverText != null)
        {
            string resultText = "";
            
            switch (evt.Result)
            {
                case GameResult.Player1Win:
                    resultText = $"Player 1 ({GameConfigManager.Instance.Player1Symbol}) Wins!";
                    break;
                case GameResult.Player2Win:
                    resultText = $"Player 2 ({GameConfigManager.Instance.Player2Symbol}) Wins!";
                    break;
                case GameResult.Draw:
                    resultText = "It's a Draw!";
                    break;
            }
            
            gameOverText.text = resultText;
        }
        
        // First, make sure other panels are hidden to avoid conflicts
        var gameHud = UIManager.Instance.GetPanel<GameHUD>();
        if (gameHud != null && gameHud.IsVisible())
        {
            gameHud.Hide();
        }
        
        var mainMenu = UIManager.Instance.GetPanel<MainMenuUI>();
        if (mainMenu != null && mainMenu.IsVisible())
        {
            mainMenu.Hide();
        }
        
        // Now show this panel
        Show();
    }
    
    private void OnPlayAgainClicked()
    {
        // Publish reset event to start new game first
        EventBus.Publish(new GameResetEvent());
        
        // Hide this panel
        Hide();
        
        // Show GameHUD again for new game
        var gameHud = UIManager.Instance.GetPanel<GameHUD>();
        if (gameHud != null)
        {
            gameHud.Show();
        }
    }
    
    private void OnBackClicked()
    {
        // Publish reset event first
        EventBus.Publish(new GameResetEvent());
        
        // Hide this panel
        Hide();
        
        // Show main menu
        UIManager.Instance.ShowPanel<MainMenuUI>();
    }
}