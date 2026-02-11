using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized manager for all UI panels in the game.
/// Handles showing, hiding, and managing the lifecycle of UI panels.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private List<UIPanel> registeredPanels = new List<UIPanel>();
    
    [Header("Panel Settings")]
    [SerializeField] private bool hideOtherPanelsOnShow = true;
    [SerializeField] private bool animatePanels = true;

    private Dictionary<Type, UIPanel> panelMap = new Dictionary<Type, UIPanel>();
    private Stack<UIPanel> panelStack = new Stack<UIPanel>(); // For modal-like behavior

    #region Singleton Pattern

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject managerObj = new GameObject("UIManager");
                _instance = managerObj.AddComponent<UIManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
            GameLogger.LogInfo("UIManager initialized", GameLogger.LogCategory.UI);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Initialization

    private void Initialize()
    {
        panelMap.Clear();
        panelStack.Clear();
        
        // Register all panels that were assigned in the inspector
        foreach (var panel in registeredPanels)
        {
            if (panel != null)
            {
                RegisterPanel(panel);
            }
        }
        
        GameLogger.LogDebug($"Registered {panelMap.Count} panels", GameLogger.LogCategory.UI);
    }

    #endregion

    #region Panel Registration

    /// <summary>
    /// Register a panel with the UI manager
    /// </summary>
    public void RegisterPanel(UIPanel panel)
    {
        if (panel == null)
        {
            GameLogger.LogWarning("Cannot register a null panel", GameLogger.LogCategory.UI);
            return;
        }

        Type panelType = panel.GetType();
        if (panelMap.ContainsKey(panelType))
        {
            GameLogger.LogWarning($"Panel of type {panelType.Name} is already registered. Replacing...", GameLogger.LogCategory.UI);
            panelMap[panelType] = panel;
        }
        else
        {
            panelMap.Add(panelType, panel);
            GameLogger.LogDebug($"Registered panel: {panelType.Name}", GameLogger.LogCategory.UI);
        }

        if (!registeredPanels.Contains(panel))
        {
            registeredPanels.Add(panel);
        }
    }

    /// <summary>
    /// Unregister a panel from the UI manager
    /// </summary>
    public void UnregisterPanel<T>() where T : UIPanel
    {
        Type panelType = typeof(T);
        if (panelMap.ContainsKey(panelType))
        {
            panelMap.Remove(panelType);
        }

        // Remove from registered panels list
        registeredPanels.RemoveAll(p => p != null && p.GetType() == panelType);
    }

    /// <summary>
    /// Get a registered panel by type
    /// </summary>
    public T GetPanel<T>() where T : UIPanel
    {
        Type panelType = typeof(T);
        if (panelMap.TryGetValue(panelType, out UIPanel panel))
        {
            return panel as T;
        }
        
        GameLogger.LogWarning($"Panel of type {typeof(T).Name} not found in registry", GameLogger.LogCategory.UI);
        return null;
    }

    #endregion

    #region Panel Management

    /// <summary>
    /// Show a panel by type
    /// </summary>
    public void ShowPanel<T>() where T : UIPanel
    {
        T panel = GetPanel<T>();
        if (panel != null)
        {
            GameLogger.LogDebug($"Showing panel: {typeof(T).Name}", GameLogger.LogCategory.UI);
            ShowPanel(panel);
        }
    }

    /// <summary>
    /// Show a specific panel instance
    /// </summary>
    public void ShowPanel(UIPanel panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("Cannot show a null panel");
            return;
        }

        // Hide other panels if needed
        if (hideOtherPanelsOnShow)
        {
            HideOtherPanels(panel);
        }

        // Push to stack if it's a modal-like panel
        if (!panelStack.Contains(panel))
        {
            panelStack.Push(panel);
        }

        panel.Show();
    }

    /// <summary>
    /// Hide a panel by type
    /// </summary>
    public void HidePanel<T>() where T : UIPanel
    {
        T panel = GetPanel<T>();
        if (panel != null)
        {
            HidePanel(panel);
        }
    }

    /// <summary>
    /// Hide a specific panel instance
    /// </summary>
    public void HidePanel(UIPanel panel )
    {
        if (panel == null)
        {
            GameLogger.LogWarning("Cannot hide a null panel", GameLogger.LogCategory.UI);
            return;
        }

        GameLogger.LogDebug($"Hiding panel: {panel.GetType().Name}", GameLogger.LogCategory.UI);
        panel.Hide();

        // Remove from stack if it was pushed
        if (panelStack.Contains(panel))
        {
            var tempStack = new Stack<UIPanel>();
            while (panelStack.Count > 0)
            {
                var currentPanel = panelStack.Pop();
                if (currentPanel != panel)
                {
                    tempStack.Push(currentPanel);
                }
            }
            
            // Restore the remaining panels back to the stack
            while (tempStack.Count > 0)
            {
                panelStack.Push(tempStack.Pop());
            }
        }
    }

    /// <summary>
    /// Toggle a panel's visibility by type
    /// </summary>
    public void TogglePanel<T>() where T : UIPanel
    {
        T panel = GetPanel<T>();
        if (panel != null)
        {
            if (panel.IsVisible())
            {
                HidePanel(panel);
            }
            else
            {
                ShowPanel(panel);
            }
        }
    }

    /// <summary>
    /// Hide all currently active panels
    /// </summary>
    public void HideAllPanels()
    {
        GameLogger.LogDebug("Hiding all panels", GameLogger.LogCategory.UI);
        foreach (var panel in panelMap.Values)
        {
            if (panel != null && panel.IsVisible())
            {
                panel.Hide();
            }
        }
        
        panelStack.Clear();
    }

    /// <summary>
    /// Hide other panels except the specified one
    /// </summary>
    private void HideOtherPanels(UIPanel currentPanel)
    {
        foreach (var panel in panelMap.Values)
        {
            if (panel != null && panel != currentPanel && panel.IsVisible())
            {
                panel.Hide();
            }
        }
    }

    #endregion

    #region Modal Stack Management

    /// <summary>
    /// Show a modal panel that can be closed in stack order
    /// </summary>
    public void ShowModalPanel<T>() where T : UIPanel
    {
        T panel = GetPanel<T>();
        if (panel != null)
        {
            ShowModalPanel(panel);
        }
    }

    /// <summary>
    /// Show a modal panel that can be closed in stack order
    /// </summary>
    public void ShowModalPanel(UIPanel panel )
    {
        if (panel == null)
        {
            Debug.LogWarning("Cannot show a null modal panel");
            return;
        }

        // Don't hide other panels for modal behavior
        panelStack.Push(panel);
        panel.Show();
    }

    /// <summary>
    /// Close the topmost modal panel
    /// </summary>
    public void CloseTopModalPanel()
    {
        if (panelStack.Count > 0)
        {
            UIPanel topPanel = panelStack.Pop();
            if (topPanel != null)
            {
                topPanel.Hide();
            }
        }
    }

    /// <summary>
    /// Close all modal panels
    /// </summary>
    public void CloseAllModals()
    {
        while (panelStack.Count > 0)
        {
            UIPanel topPanel = panelStack.Pop();
            if (topPanel != null)
            {
                topPanel.Hide();
            }
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Check if a specific panel is currently visible
    /// </summary>
    public bool IsPanelVisible<T>() where T : UIPanel
    {
        T panel = GetPanel<T>();
        return panel != null && panel.IsVisible();
    }

    /// <summary>
    /// Get the currently active modal panel
    /// </summary>
    public UIPanel GetActiveModalPanel()
    {
        return panelStack.Count > 0 ? panelStack.Peek() : null;
    }

    /// <summary>
    /// Get the count of active modal panels
    /// </summary>
    public int GetModalPanelCount()
    {
        return panelStack.Count;
    }

    #endregion
}