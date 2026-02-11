using UnityEngine;

/// <summary>
/// Singleton manager for accessing GameConfig throughout the application.
/// Provides centralized access to game configuration settings.
/// </summary>
public class GameConfigManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;

    private static GameConfigManager _instance;
    
    public static GameConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance in scene
                _instance = FindObjectOfType<GameConfigManager>();
                
                if (_instance == null)
                {
                    // Create new GameObject with manager
                    GameObject managerObject = new GameObject("GameConfigManager");
                    _instance = managerObject.AddComponent<GameConfigManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure singleton pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Validate configuration
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig not assigned! Please assign it in the inspector.");
                enabled = false;
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Get the current game configuration
    /// </summary>
    public GameConfig Config
    {
        get 
        {
            if (gameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned!");
            }
            return gameConfig;
        }
        set
        {
            gameConfig = value;
        }
    }

    /// <summary>
    /// Quick access to board size
    /// </summary>
    public int BoardSize => Config != null ? Config.BoardSize : 3;

    /// <summary>
    /// Quick access to win condition
    /// </summary>
    public int WinCondition => Config != null ? Config.WinCondition : 3;

    /// <summary>
    /// Quick access to player symbols
    /// </summary>
    public string Player1Symbol => Config != null ? Config.Player1Symbol : "X";
    public string Player2Symbol => Config != null ? Config.Player2Symbol : "O";

    /// <summary>
    /// Quick access to player colors
    /// </summary>
    public Color Player1Color => Config != null ? Config.Player1Color : Color.blue;
    public Color Player2Color => Config != null ? Config.Player2Color : Color.red;

    /// <summary>
    /// Quick access to AI settings
    /// </summary>
    public float AIThinkingTime => Config != null ? Config.AIThinkingTime : 0.5f;
    public AIDifficulty Difficulty => Config != null ? Config.Difficulty : AIDifficulty.Hard;

    /// <summary>
    /// Quick access to win line color
    /// </summary>
    public Color WinLineColor => Config != null ? Config.WinLineColor : Color.green;

    /// <summary>
    /// Quick access to draw color
    /// </summary>
    public Color DrawColor => Config != null ? Config.DrawColor : Color.yellow;
}