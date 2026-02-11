using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TicTacToe.Events;

public class BoardUIManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfig _config;
    
    [Header("References")]
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private BoardCell _cellPrefab;
    
    [Header("Animation")]
    [SerializeField] private float _winAnimationDelay = 0.1f;
    
    private BoardCell[,] _cells;
    private List<BoardCell> _cellList = new List<BoardCell>();
    
    private void Awake()
    {
        if (_config == null)
        {
            Debug.LogError("GameConfig not assigned!");
            enabled = false;
            return;
        }
    }
    
    private void OnEnable()
    {
        EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        EventBus.Subscribe<MoveExecutedEvent>(OnMoveExecuted);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
        EventBus.Unsubscribe<MoveExecutedEvent>(OnMoveExecuted);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }
    
    private void OnGameStarted(GameStartedEvent evt)
    {
        CreateBoard();
    }
    
    private void CreateBoard()
    {
        // Clear existing cells
        ClearBoard();
        
        // Setup grid layout
        _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayout.constraintCount = _config.BoardSize;
        
        // Create cells
        _cells = new BoardCell[_config.BoardSize, _config.BoardSize];
        
        for (int row = 0; row < _config.BoardSize; row++)
        {
            for (int col = 0; col < _config.BoardSize; col++)
            {
                BoardCell cell = Instantiate(_cellPrefab, _gridLayout.transform);
                cell.Initialize(row, col);
                _cells[row, col] = cell;
                _cellList.Add(cell);
            }
        }
    }
    
    private void ClearBoard()
    {
        foreach (var cell in _cellList)
        {
            if (cell != null)
                Destroy(cell.gameObject);
        }
        
        _cellList.Clear();
        _cells = null;
    }
    
    private void OnMoveExecuted(MoveExecutedEvent evt)
    {
        if (_cells == null)
            return;
        
        BoardCell cell = _cells[evt.Row, evt.Column];
        if (cell == null)
            return;
        
        // Get color for player
        Color color = evt.Player == PlayerType.Player1 ? _config.Player1Color : _config.Player2Color;
        
        // Set symbol on cell
        cell.SetSymbol(evt.Symbol, color);
    }
    
    private void OnGameOver(GameOverEvent evt)
    {
        if (evt.WinningLine != null && evt.WinningLine.Length > 0)
        {
            StartCoroutine(HighlightWinningCells(evt.WinningLine));
        }
    }
    
    private System.Collections.IEnumerator HighlightWinningCells(Vector2Int[] winningLine)
    {
        foreach (var pos in winningLine)
        {
            if (_cells != null && pos.x >= 0 && pos.x < _config.BoardSize && 
                pos.y >= 0 && pos.y < _config.BoardSize)
            {
                _cells[pos.x, pos.y].HighlightAsWinning(_config.WinLineColor);
                yield return new WaitForSeconds(_winAnimationDelay);
            }
        }
    }
}
