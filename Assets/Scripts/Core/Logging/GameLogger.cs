using UnityEngine;

/// <summary>
/// Centralized logging system for the game.
/// Provides configurable logging levels and categories for better production management.
/// </summary>
public static class GameLogger
{
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4
    }

    public enum LogCategory
    {
        System,
        GameLogic,
        UI,
        AI,
        Events,
        Configuration,
        Performance
    }

    // Current logging level (can be adjusted for production)
    private static LogLevel _currentLogLevel = LogLevel.Debug;
    
    // Enable/disable specific categories
    private static bool _logSystem = true;
    private static bool _logGameLogic = true;
    private static bool _logUI = true;
    private static bool _logAI = true;
    private static bool _logEvents = true;
    private static bool _logConfiguration = true;
    private static bool _logPerformance = true;

    #region Configuration Methods

    /// <summary>
    /// Set the global logging level
    /// </summary>
    public static void SetLogLevel(LogLevel level)
    {
        _currentLogLevel = level;
    }

    /// <summary>
    /// Enable/disable specific log categories
    /// </summary>
    public static void SetCategoryEnabled(LogCategory category, bool enabled)
    {
        switch (category)
        {
            case LogCategory.System:
                _logSystem = enabled;
                break;
            case LogCategory.GameLogic:
                _logGameLogic = enabled;
                break;
            case LogCategory.UI:
                _logUI = enabled;
                break;
            case LogCategory.AI:
                _logAI = enabled;
                break;
            case LogCategory.Events:
                _logEvents = enabled;
                break;
            case LogCategory.Configuration:
                _logConfiguration = enabled;
                break;
            case LogCategory.Performance:
                _logPerformance = enabled;
                break;
        }
    }

    #endregion

    #region Logging Methods

    public static void LogInfo(string message, LogCategory category = LogCategory.System)
    {
        if (_currentLogLevel >= LogLevel.Info && IsCategoryEnabled(category))
        {
            Debug.Log($"[{category}] {message}");
        }
    }

    public static void LogWarning(string message, LogCategory category = LogCategory.System)
    {
        if (_currentLogLevel >= LogLevel.Warning && IsCategoryEnabled(category))
        {
            Debug.LogWarning($"[{category}] {message}");
        }
    }

    public static void LogError(string message, LogCategory category = LogCategory.System)
    {
        if (_currentLogLevel >= LogLevel.Error && IsCategoryEnabled(category))
        {
            Debug.LogError($"[{category}] {message}");
        }
    }

    public static void LogDebug(string message, LogCategory category = LogCategory.System)
    {
        if (_currentLogLevel >= LogLevel.Debug && IsCategoryEnabled(category))
        {
            Debug.Log($"[DEBUG][{category}] {message}");
        }
    }

    #endregion

    #region Helper Methods

    private static bool IsCategoryEnabled(LogCategory category)
    {
        switch (category)
        {
            case LogCategory.System:
                return _logSystem;
            case LogCategory.GameLogic:
                return _logGameLogic;
            case LogCategory.UI:
                return _logUI;
            case LogCategory.AI:
                return _logAI;
            case LogCategory.Events:
                return _logEvents;
            case LogCategory.Configuration:
                return _logConfiguration;
            case LogCategory.Performance:
                return _logPerformance;
            default:
                return true;
        }
    }

    #endregion

    #region Production Configuration

    /// <summary>
    /// Configure logging for production environment
    /// </summary>
    public static void ConfigureForProduction()
    {
        _currentLogLevel = LogLevel.Warning; // Only show warnings and errors
        _logPerformance = false; // Disable performance logging
    }

    /// <summary>
    /// Configure logging for development environment
    /// </summary>
    public static void ConfigureForDevelopment()
    {
        _currentLogLevel = LogLevel.Debug;
        // Enable all categories
        _logSystem = true;
        _logGameLogic = true;
        _logUI = true;
        _logAI = true;
        _logEvents = true;
        _logConfiguration = true;
        _logPerformance = true;
    }

    #endregion
}