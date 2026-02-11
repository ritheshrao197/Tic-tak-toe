using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Events;

public class MainMenuUI : UIPanel
{
    [Header("References")]
    [SerializeField] private Button _playerVsPlayerButton;
    [SerializeField] private Button _playerVsComputerButton;
    [SerializeField] private Button _quitButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject _gamePanel;
    
    private GameHUD _gameHud;
    
    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        
        // Setup button listeners
        if (_playerVsPlayerButton != null)
            _playerVsPlayerButton.onClick.AddListener(OnPlayerVsPlayerClicked);
        
        if (_playerVsComputerButton != null)
            _playerVsComputerButton.onClick.AddListener(OnPlayerVsComputerClicked);
        
        if (_quitButton != null)
            _quitButton.onClick.AddListener(OnQuitClicked);
        
        // Find and cache GameHUD reference
        _gameHud = FindObjectOfType<GameHUD>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        // Remove automatic subscription to GameOverEvent - let GameHUD or GameOverPanel handle it
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
        // No need to unsubscribe from GameOverEvent as we didn't subscribe
    }
    
    private void OnPlayerVsPlayerClicked()
    {
        EventBus.Publish(new GameModeSelectedEvent
        {
            Mode = GameMode.PlayerVsPlayer
        });
    }
    
    private void OnPlayerVsComputerClicked()
    {
        EventBus.Publish(new GameModeSelectedEvent
        {
            Mode = GameMode.PlayerVsComputer
        });
    }
    
    private void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void OnGameStarted(GameStartedEvent evt)
    {
        // Hide main menu and show game UI
        UIManager.Instance.HidePanel<MainMenuUI>();
        if (_gameHud != null)
        {
            UIManager.Instance.ShowPanel(_gameHud);
        }
    }
    
    public void BackToMainMenu()
    {
        UIManager.Instance.ShowPanel<MainMenuUI>();
    }
}