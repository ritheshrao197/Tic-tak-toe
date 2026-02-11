using UnityEngine;

/// <summary>
/// Initializes the UI manager and registers all UI panels in the scene.
/// This should be placed on a GameObject in your scene to initialize the UI system.
/// </summary>
public class UIManagerInitializer : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private GameHUD gameHUD;
    [SerializeField] private BoardUIManager boardUIManager;
    [SerializeField] private GameOverPanel gameOverPanel;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Register all panels with the UI manager
        if (mainMenuUI != null)
        {
            UIManager.Instance.RegisterPanel(mainMenuUI);
        }

        if (gameHUD != null)
        {
            UIManager.Instance.RegisterPanel(gameHUD);
        }

        if (boardUIManager != null)
        {
            UIManager.Instance.RegisterPanel(boardUIManager);
        }
        
        if (gameOverPanel != null)
        {
            UIManager.Instance.RegisterPanel(gameOverPanel);
        }

        // Show the main menu initially
        UIManager.Instance.ShowPanel<MainMenuUI>();
    }
}