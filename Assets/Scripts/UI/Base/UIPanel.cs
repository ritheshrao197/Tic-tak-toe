using UnityEngine;

/// <summary>
/// Base class for all UI panels in the game.
/// Provides common functionality for showing/hiding panels and lifecycle management.
/// </summary>
public abstract class UIPanel : MonoBehaviour
{
    [Header("Panel Configuration")]
    [SerializeField] protected CanvasGroup canvasGroup;
    
    protected bool isVisible = false;
    protected bool isInitialized = false;

    #region Lifecycle Methods

    protected virtual void Awake()
    {
        InitializePanel();
    }

    protected virtual void Start()
    {
        InitializeComponents();
    }

    protected virtual void OnDestroy()
    {
        Cleanup();
    }

    protected virtual void OnEnable()
    {
        // Override in derived classes for event subscription
    }

    protected virtual void OnDisable()
    {
        // Override in derived classes for event unsubscription
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the panel components and references
    /// </summary>
    protected virtual void InitializePanel()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            
        isInitialized = true;
    }

    /// <summary>
    /// Override this method to initialize specific panel components
    /// </summary>
    protected virtual void InitializeComponents()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Override this method to cleanup resources when panel is destroyed
    /// </summary>
    protected virtual void Cleanup()
    {
        // Override in derived classes
    }

    #endregion

    #region Panel Visibility

    /// <summary>
    /// Show the panel with optional animation
    /// </summary>
    public virtual void Show()
    {
        GameLogger.LogDebug("Showing panel", GameLogger.LogCategory.UI);

        if (!isInitialized) 
            InitializePanel();

        isVisible = true;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        OnShow();
    }

    /// <summary>
    /// Hide the panel with optional animation
    /// </summary>
    public virtual void Hide()
    {
        isVisible = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

   

    /// <summary>
    /// Called when panel is shown
    /// </summary>
    protected virtual void OnShow()
    {
        // Override in derived classes
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Check if panel is currently visible
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }

    /// <summary>
    /// Toggle panel visibility
    /// </summary>
    public void Toggle()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    /// <summary>
    /// Set panel interactivity
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (canvasGroup != null)
            canvasGroup.interactable = interactable;
    }

    /// <summary>
    /// Set whether panel blocks raycasts
    /// </summary>
    public void SetBlocksRaycasts(bool blocksRaycasts)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = blocksRaycasts;
    }

    #endregion
}