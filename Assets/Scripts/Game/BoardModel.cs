using System;
using System.Collections.Generic;
using TicTacToe.Events;
using UnityEngine;

public class BoardModel
{
    private PlayerType[,] _board;
    private int _size;
    private int _winCondition;
    
    public int Size => _size;
    
    public BoardModel(int size, int winCondition)
    {
        _size = size;
        _winCondition = winCondition;
        _board = new PlayerType[size, size];
        Clear();
    }
    
    public void Clear()
    {
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                _board[row, col] = PlayerType.None;
            }
        }
    }
    
    public bool IsValidMove(int row, int col)
    {
        if (row < 0 || row >= _size || col < 0 || col >= _size)
            return false;
        return _board[row, col] == PlayerType.None;
    }
    
    public bool MakeMove(int row, int col, PlayerType player)
    {
        if (!IsValidMove(row, col))
            return false;
            
        _board[row, col] = player;
        return true;
    }
    
    public PlayerType GetCell(int row, int col)
    {
        if (row < 0 || row >= _size || col < 0 || col >= _size)
            return PlayerType.None;
            
        return _board[row, col];
    }
    
    public bool IsFull()
    {
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                if (_board[row, col] == PlayerType.None)
                    return false;
            }
        }
        return true;
    }
    
    public GameResult CheckWinner(out List<Vector2Int> winningLine)
    {
        winningLine = new List<Vector2Int>();
        
        // Check rows
        for (int row = 0; row < _size; row++)
        {
            if (CheckLine(row, 0, 0, 1, out winningLine))
            {
                return GetResult(_board[row, 0]);
            }
        }
        
        // Check columns
        for (int col = 0; col < _size; col++)
        {
            if (CheckLine(0, col, 1, 0, out winningLine))
            {
                return GetResult(_board[0, col]);
            }
        }
        
        // Check diagonal (top-left to bottom-right)
        if (CheckLine(0, 0, 1, 1, out winningLine))
        {
            return GetResult(_board[0, 0]);
        }
        
        // Check diagonal (top-right to bottom-left)
        if (CheckLine(0, _size - 1, 1, -1, out winningLine))
        {
            return GetResult(_board[0, _size - 1]);
        }
        
        // Check for draw
        if (IsFull())
        {
            return GameResult.Draw;
        }
        
        return GameResult.None;
    }
    
    private bool CheckLine(int startRow, int startCol, int rowDir, int colDir, out List<Vector2Int> line)
    {
        line = new List<Vector2Int>();
        PlayerType first = _board[startRow, startCol];
        
        if (first == PlayerType.None)
            return false;
        
        for (int i = 0; i < _winCondition; i++)
        {
            int row = startRow + i * rowDir;
            int col = startCol + i * colDir;
            
            if (row < 0 || row >= _size || col < 0 || col >= _size)
                return false;
            
            if (_board[row, col] != first)
                return false;
                
            line.Add(new Vector2Int(row, col));
        }
        
        return true;
    }
    
    private GameResult GetResult(PlayerType player)
    {
        switch (player)
        {
            case PlayerType.Player1:
                return GameResult.Player1Win;
            case PlayerType.Player2:
                return GameResult.Player2Win;
            default:
                return GameResult.None;
        }
    }
    
    public List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                if (_board[row, col] == PlayerType.None)
                {
                    moves.Add(new Vector2Int(row, col));
                }
            }
        }
        
        return moves;
    }
    
    public PlayerType[,] GetBoardCopy()
    {
        PlayerType[,] copy = new PlayerType[_size, _size];
        Array.Copy(_board, copy, _board.Length);
        return copy;
    }
}

// Simple Vector2Int for non-Unity environments
public struct Vector2Int
{
    public int x;
    public int y;
    
    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        return a.x == b.x && a.y == b.y;
    }
    
    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return !(a == b);
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Vector2Int)
        {
            return this == (Vector2Int)obj;
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }
}
