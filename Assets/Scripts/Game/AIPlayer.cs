using UnityEngine;
using System.Collections.Generic;

public class AIPlayer
{
    private PlayerType _aiPlayer;
    private PlayerType _humanPlayer;
    private int _boardSize;

    public AIPlayer(PlayerType aiPlayer, int boardSize)
    {
        _aiPlayer = aiPlayer;
        _humanPlayer = aiPlayer == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        _boardSize = boardSize;
    }

    public Vector2Int GetBestMove(PlayerType[,] currentBoard, AIDifficulty difficulty)
    {
        var emptyCells = GetEmptyCells(currentBoard);

        if (difficulty == AIDifficulty.Easy)
        {
            return emptyCells[Random.Range(0, emptyCells.Count)];
        }

        if (difficulty == AIDifficulty.Medium)
        {
            // 1️⃣ Immediate win
            foreach (var move in emptyCells)
            {
                var copy = CloneBoard(currentBoard);
                copy[move.x, move.y] = _aiPlayer;

                if (CheckWinner(copy) == _aiPlayer)
                    return move;
            }

            // 2️⃣ Block opponent
            foreach (var move in emptyCells)
            {
                var copy = CloneBoard(currentBoard);
                copy[move.x, move.y] = _humanPlayer;

                if (CheckWinner(copy) == _humanPlayer)
                    return move;
            }

            // 3️⃣ Otherwise random
            return emptyCells[Random.Range(0, emptyCells.Count)];
        }

        // Hard difficulty - Minimax algorithm
        int bestScore = int.MinValue;
        Vector2Int bestMove = emptyCells[0];

        foreach (var move in emptyCells)
        {
            var copy = CloneBoard(currentBoard);
            copy[move.x, move.y] = _aiPlayer;

            int score = Minimax(copy, false); // false means it's now the opponent's turn (minimizing)

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int Minimax(PlayerType[,] board, bool isMaximizing)
    {
        PlayerType winner = CheckWinner(board);

        // Terminal states scoring
        if (winner == _aiPlayer)
            return 10; // AI wins

        if (winner == _humanPlayer)
            return -10; // Human wins

        if (IsBoardFull(board))
            return 0; // Draw

        var emptyCells = GetEmptyCells(board);

        if (isMaximizing)
        {
            // AI's turn - wants to maximize score
            int bestScore = int.MinValue;

            foreach (var move in emptyCells)
            {
                var copy = CloneBoard(board);
                copy[move.x, move.y] = _aiPlayer;

                int score = Minimax(copy, false); // Switch to opponent's turn (minimizing)
                bestScore = Mathf.Max(bestScore, score);
            }

            return bestScore;
        }
        else
        {
            // Opponent's turn - wants to minimize score for AI
            int bestScore = int.MaxValue;

            foreach (var move in emptyCells)
            {
                var copy = CloneBoard(board);
                copy[move.x, move.y] = _humanPlayer;

                int score = Minimax(copy, true); // Switch back to AI's turn (maximizing)
                bestScore = Mathf.Min(bestScore, score);
            }

            return bestScore;
        }
    }

    private List<Vector2Int> GetEmptyCells(PlayerType[,] board)
    {
        var list = new List<Vector2Int>();

        for (int r = 0; r < _boardSize; r++)
        {
            for (int c = 0; c < _boardSize; c++)
            {
                if (board[r, c] == PlayerType.None)
                    list.Add(new Vector2Int(r, c));
            }
        }

        return list;
    }

    private bool IsBoardFull(PlayerType[,] board)
    {
        for (int r = 0; r < _boardSize; r++)
            for (int c = 0; c < _boardSize; c++)
                if (board[r, c] == PlayerType.None)
                    return false;

        return true;
    }

    private PlayerType[,] CloneBoard(PlayerType[,] original)
    {
        PlayerType[,] copy = new PlayerType[_boardSize, _boardSize];

        for (int r = 0; r < _boardSize; r++)
            for (int c = 0; c < _boardSize; c++)
                copy[r, c] = original[r, c];

        return copy;
    }

    private PlayerType CheckWinner(PlayerType[,] board)
    {
        // Rows
        for (int r = 0; r < _boardSize; r++)
        {
            PlayerType first = board[r, 0];
            if (first == PlayerType.None)
                continue;

            bool win = true;
            for (int c = 1; c < _boardSize; c++)
            {
                if (board[r, c] != first)
                {
                    win = false;
                    break;
                }
            }

            if (win)
                return first;
        }

        // Columns
        for (int c = 0; c < _boardSize; c++)
        {
            PlayerType first = board[0, c];
            if (first == PlayerType.None)
                continue;

            bool win = true;
            for (int r = 1; r < _boardSize; r++)
            {
                if (board[r, c] != first)
                {
                    win = false;
                    break;
                }
            }

            if (win)
                return first;
        }

        // Main diagonal
        PlayerType diag = board[0, 0];
        if (diag != PlayerType.None)
        {
            bool win = true;
            for (int i = 1; i < _boardSize; i++)
            {
                if (board[i, i] != diag)
                {
                    win = false;
                    break;
                }
            }

            if (win)
                return diag;
        }

        // Anti diagonal
        PlayerType anti = board[0, _boardSize - 1];
        if (anti != PlayerType.None)
        {
            bool win = true;
            for (int i = 1; i < _boardSize; i++)
            {
                if (board[i, _boardSize - 1 - i] != anti)
                {
                    win = false;
                    break;
                }
            }

            if (win)
                return anti;
        }

        return PlayerType.None;
    }
}