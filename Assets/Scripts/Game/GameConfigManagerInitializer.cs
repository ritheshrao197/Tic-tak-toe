using UnityEngine;

/// <summary>
/// Initializes the GameConfigManager in the scene.
/// Place this on a GameObject in your scene and assign the GameConfig in the inspector.
/// </summary>
public class GameConfigManagerInitializer : MonoBehaviour
{
    [Header("Game Configuration")]
    [SerializeField] private GameConfig gameConfig;

    private void Awake()
    {
        // Initialize the GameConfigManager with the assigned config
        if (gameConfig != null)
        {
            GameConfigManager.Instance.Config = gameConfig;
        }
        else
        {
            Debug.LogError("GameConfig not assigned to GameConfigManagerInitializer!");
        }
    }
}