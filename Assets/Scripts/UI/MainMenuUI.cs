using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.Events;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _playerVsPlayerButton;
    [SerializeField] private Button _playerVsComputerButton;
    [SerializeField] private Button _quitButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _gamePanel;
    
    private void Awake()
    {
        // Setup button listeners
        if (_playerVsPlayerButton != null)
            _playerVsPlayerButton.onClick.AddListener(OnPlayerVsPlayerClicked);
        
        if (_playerVsComputerButton != null)
            _playerVsComputerButton.onClick.AddListener(OnPlayerVsComputerClicked);
        
        if (_quitButton != null)
            _quitButton.onClick.AddListener(OnQuitClicked);
    }
    
    private void Start()
    {
        ShowMainMenu();
    }
    
    private void OnEnable()
    {
        EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
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
        ShowGamePanel();
    }
    
    private void ShowMainMenu()
    {
        if (_mainMenuPanel != null)
            _mainMenuPanel.SetActive(true);
        
        if (_gamePanel != null)
            _gamePanel.SetActive(false);
    }
    
    private void ShowGamePanel()
    {
        if (_mainMenuPanel != null)
            _mainMenuPanel.SetActive(false);
        
        if (_gamePanel != null)
            _gamePanel.SetActive(true);
    }
    
    public void BackToMainMenu()
    {
        ShowMainMenu();
    }
}
