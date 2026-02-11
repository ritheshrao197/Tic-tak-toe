using TicTacToe.Events;
using UnityEngine;

// ============================================================
// Game Events
// ============================================================

public struct GameModeSelectedEvent : IGameEvent
{
    public GameMode Mode;
}

public struct GameStartedEvent : IGameEvent
{
    public GameMode Mode;
    public PlayerType StartingPlayer;
}

public struct CellClickedEvent : IGameEvent
{
    public int Row;
    public int Column;
}

public struct MoveExecutedEvent : IGameEvent
{
    public int Row;
    public int Column;
    public PlayerType Player;
    public string Symbol;
}

public struct TurnChangedEvent : IGameEvent
{
    public PlayerType CurrentPlayer;
    public PlayerType PreviousPlayer;
}

public struct GameOverEvent : IGameEvent
{
    public GameResult Result;
    public PlayerType Winner;
    public Vector2Int[] WinningLine; // For highlighting winning cells
}

public struct GameResetEvent : IGameEvent { }

public struct ScoreUpdatedEvent : IGameEvent
{
    public int Player1Score;
    public int Player2Score;
    public int DrawScore;
}

// ============================================================
// Enums
// ============================================================

public enum GameMode
{
    PlayerVsPlayer,
    PlayerVsComputer
}

public enum PlayerType
{
    None,
    Player1,
    Player2
}

public enum GameResult
{
    None,
    Player1Win,
    Player2Win,
    Draw
}

public enum GameState
{
    MainMenu,
    Playing,
    GameOver,
    Paused
}
