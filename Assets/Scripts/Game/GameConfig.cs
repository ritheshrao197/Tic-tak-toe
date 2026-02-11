using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "TicTacToe/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Game Settings")]
    [Tooltip("Board size (3 for standard Tic Tac Toe)")]
    public int BoardSize = 3;
    
    [Tooltip("Number of marks in a row needed to win")]
    public int WinCondition = 3;
    
    [Header("Player Settings")]
    [Tooltip("Symbol for Player 1")]
    public string Player1Symbol = "X";
    
    [Tooltip("Symbol for Player 2 (or Computer)")]
    public string Player2Symbol = "O";
    
    [Header("AI Settings")]
    [Tooltip("Delay before AI makes a move (seconds)")]
    [Range(0.1f, 2f)]
    public float AIThinkingTime = 0.5f;
    
    [Tooltip("AI difficulty level")]
    public AIDifficulty Difficulty = AIDifficulty.Hard;
    
    [Header("UI Settings")]
    [Tooltip("Color for Player 1 marks")]
    public Color Player1Color = Color.blue;
    
    [Tooltip("Color for Player 2 marks")]
    public Color Player2Color = Color.red;
    
    [Tooltip("Color for winning line")]
    public Color WinLineColor = Color.green;
    
    [Tooltip("Color for draw state")]
    public Color DrawColor = Color.yellow;
}

public enum AIDifficulty
{
    Easy,       // Random moves
    Medium,     // Mix of random and smart moves
    Hard        // Minimax algorithm
}
