using System;
using ChessChallenge.API;

public class RandomBot:IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Random random = new Random();
        int nextMove = random.Next(0, moves.Length);
        return moves[nextMove];
    }
}
