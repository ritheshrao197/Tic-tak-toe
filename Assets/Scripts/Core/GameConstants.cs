/// <summary>
/// Centralized constants for the game.
/// Provides type-safe access to game constants and default values.
/// </summary>
public static class GameConstants
{
    #region Board Configuration
    
    public const int DEFAULT_BOARD_SIZE = 3;
    public const int DEFAULT_WIN_CONDITION = 3;
    public const int MIN_BOARD_SIZE = 3;
    public const int MAX_BOARD_SIZE = 10;
    
    #endregion

    #region Player Configuration
    
    public const string DEFAULT_PLAYER1_SYMBOL = "X";
    public const string DEFAULT_PLAYER2_SYMBOL = "O";
    
    #endregion

    #region AI Configuration
    
    public const float DEFAULT_AI_THINKING_TIME = 0.5f;
    public const float MIN_AI_THINKING_TIME = 0.1f;
    public const float MAX_AI_THINKING_TIME = 2.0f;
    
    #endregion

    #region UI Configuration
    
    public const float DEFAULT_ANIMATION_DELAY = 0.1f;
    public const float MIN_ANIMATION_DELAY = 0.05f;
    public const float MAX_ANIMATION_DELAY = 1.0f;
    
    #endregion

    #region Game States
    
    public const int MAX_SCORE = 999;
    public const int MIN_SCORE = -999;
    
    #endregion

    #region Validation Methods
    
    /// <summary>
    /// Validates board size is within acceptable range
    /// </summary>
    public static bool IsValidBoardSize(int size)
    {
        return size >= MIN_BOARD_SIZE && size <= MAX_BOARD_SIZE;
    }
    
    /// <summary>
    /// Validates win condition is valid for given board size
    /// </summary>
    public static bool IsValidWinCondition(int winCondition, int boardSize)
    {
        return winCondition >= 3 && winCondition <= boardSize;
    }
    
    /// <summary>
    /// Validates AI thinking time is within acceptable range
    /// </summary>
    public static bool IsValidAIThinkingTime(float time)
    {
        return time >= MIN_AI_THINKING_TIME && time <= MAX_AI_THINKING_TIME;
    }
    
    #endregion
}