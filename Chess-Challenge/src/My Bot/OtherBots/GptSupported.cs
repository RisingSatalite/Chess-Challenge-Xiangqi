using ChessChallenge.API;
using System;

public class GptSupported : IChessBot
{
    private Random random = new Random();

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();

        // Always play checkmate in one, if possible
        foreach (Move move in allMoves)
        {
            if (!MoveIsCheckmate(board, move) && MoveIsGetMateInOne(board, move))
            {
                return move;
            }
        }

        // If no mate in one, choose a random move
        return allMoves[random.Next(0, allMoves.Length)];
    }

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

    private bool MoveIsGetMateInOne(Board board, Move move)
    {
        board.MakeMove(move);
        /*bool isMateInOne = board.GetLegalMoves().Any(possibleMove =>
        {
            board.MakeMove(possibleMove);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(possibleMove);
            return isMate;
        });*/
        //remove this line and fix the top
        bool isMateInOne = true;
        board.UndoMove(move);
        return isMateInOne;
    }
}
