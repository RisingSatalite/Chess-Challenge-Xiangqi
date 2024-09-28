using ChessChallenge.API;
using System;
using System.Linq;

public class BackUpBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        //There always need to be a defalut move which is the first one, or less C# get mad
        Move moveToPlay = allMoves[0];
        Random random = new Random();
        while (true)
        {
            Console.WriteLine("Programmin is thinking");
            Move move = allMoves[random.Next(0, allMoves.Count() - 1)];
            // Always play checkmate in one
            if (!(MoveIsCheckmate(board, move)))
            {
                //A void mates in one
                if (MoveIsGetMateInOne(board, move))
                {
                    moveToPlay = move;
                    break;
                }
            }
            //Delay for troubleshooting
            //Thread.Sleep(100);

        }
        return moveToPlay;
    }
    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        Console.WriteLine(isMate.ToString());
        board.UndoMove(move);
        return isMate;
    }
    bool MoveIsGetMateInOne(Board board, Move move)
    {
        board.MakeMove(move);
        Move[] allMoves = board.GetLegalMoves();
        foreach (Move possibleMove in allMoves)
        {
            board.MakeMove(possibleMove);
            bool isMate = board.IsInCheckmate();
            Console.WriteLine(isMate.ToString());
            board.UndoMove(possibleMove);
            if (isMate)
            {
                return true;
            }
        }
        return false;
    }
}
