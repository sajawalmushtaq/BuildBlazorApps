using System.IO.Pipelines;
using System.Numerics;

namespace Connect4;

public class State
{
    public Player[] Players { get; private set; }
    public int GameRoundsPlayed { get; private set; }
    public bool GameOver { get; private set; }
    public Piece?[] Pieces { get; private set; }
    public int PlayerTurn { get; private set; }
    public enum WinState { None, Player1_Wins, Player2_Wins, Tie }

    public string CurrentTurn => GameOver ? "Game Over" : $"Player {PlayerTurn}'s Turn";

    public State()
    {
        Players = new Player[]
        {
            new Player() { Name = "Player 1", Points = 0 },
            new Player() { Name = "Player 2", Points = 0 }
        };
        GameRoundsPlayed = 0;
        GameOver = false;
        Pieces = new Piece?[42];
        PlayerTurn = 1;
    }

    public int PlayPiece(int column)
    {
        if (GameOver) throw new ArgumentException("Game is over");

        for (int row = 5; row >= 0; row--) // Start from bottom row
        {
            int index = row * 7 + column;
            if (Pieces[index] == null)
            {
                Pieces[index] = (PlayerTurn == 1) ? Piece.Player1 : Piece.Player2;
                PlayerTurn = (PlayerTurn == 1) ? 2 : 1;
                GameRoundsPlayed++;
                return row; // Return the row the piece lands in
            }
        }
        throw new ArgumentException("Column is full");
    }


    public WinState CheckForWin()
    {
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (Pieces[row * 7 + col] == null) continue;
                if (CheckDirection(row, col, 1, 0) || CheckDirection(row, col, 0, 1) ||
                    CheckDirection(row, col, 1, 1) || CheckDirection(row, col, 1, -1))
                {
                    GameOver = true;
                    return (Pieces[row * 7 + col] == Piece.Player1) ? WinState.Player1_Wins : WinState.Player2_Wins;
                }
            }
        }
        return Pieces.All(p => p != null) ? WinState.Tie : WinState.None;
    }

    private bool CheckDirection(int row, int col, int rowStep, int colStep)
    {
        Piece? start = Pieces[row * 7 + col];
        for (int i = 1; i < 4; i++)
        {
            int r = row + rowStep * i;
            int c = col + colStep * i;
            if (r < 0 || r >= 6 || c < 0 || c >= 7 || Pieces[r * 7 + c] != start)
                return false;
        }
        return true;
    }

    public void ResetBoard()
    {
        GameOver = false;
        Pieces = new Piece?[42];
        PlayerTurn = 1;
        GameRoundsPlayed = 0;
    }

    public void EndGame()
    {
        GameOver = true;
        GameRoundsPlayed++;
    }
}

public class Player
{
    public string Name { get; set; }
    public int Points { get; set; }
}

public enum Piece { Player1, Player2 }
