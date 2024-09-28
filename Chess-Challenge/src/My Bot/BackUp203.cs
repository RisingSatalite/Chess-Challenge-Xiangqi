using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot203 : IChessBot
{
    private Random random = new Random();
    //static Board board;

    public Move Think(Board board, Timer timer)
    {
        //if want to make board global
        //board= theBoard;

        //Note, function will be moved into the main build for the final submission to save space and potentially add more
        Move[] allMoves = board.GetLegalMoves();



        //Default move is first one
        //Move moveToPlay = allMoves[0];
        // Always play checkmate in one, if possible
        /*foreach (Move move in allMoves)
        {
            if (!MoveIsCheckmate(board, move))
            {
                return move;
            }
        }*/

        //Array.sort instead

        Array.Sort(allMoves, (a, b) => CustomComparison(a, b, board));

        /*foreach(Move move in allMoves)
        {
            Console.WriteLine(move.ToString);
        }*/
        //Attempted to rearrange
        //allMoves = allMoves.OrderBy(move => MoveTakePower(board, move)).ToArray();

        //return allMoves.FirstOrDefault();

        foreach (Move possibleMoves in allMoves)
        {

            /*if (WillGetMated(board, possibleMoves))
            {*/
            return possibleMoves;
            //}
        }

        // If will get mated in one, choose a random move
        return allMoves[random.Next(0, allMoves.Length)];

    }
    public int CustomComparison(Move a, Move b, Board board)
    {
        int placeA = MoveTakePower(board, a);
        int placeB = MoveTakePower(board, b);
        return placeA.CompareTo(placeB);
    }

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return !isMate;
    }

    //Function does not work
    /*private bool WillGetMated(Board board, Move move)
    {
        //board.MakeMove(move);
        Move[] allMoves = board.GetLegalMoves();
        foreach (Move possibleMove in allMoves)
        {
            Console.WriteLine("Checking possible moves");
            board.MakeMove(possibleMove);
            bool isMate = board.IsInCheckmate();
            //Console.WriteLine(isMate.ToString());
            board.UndoMove(possibleMove);
            if (isMate)
            {
                //Do not preform, mateable
                return false;
            }
        }
        //Safe move
        return true;
    }*/

    //Randomise list, quite useful
    //make sure the 2 random connect
    /*public Move[] RandomizeArray(Move[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Move value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
        return array;
    }*/

    //Judge an attacked piece value
    /*public static int MoveTakePower2(Move move)
    {
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        Piece capturedPiece = board.GetPiece(move.TargetSquare);
        int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];
        Console.WriteLine(capturedPieceValue.ToString());
        return capturedPieceValue;
    }*/
    public int MoveTakePower(Board board, Move move)
    {
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        Piece capturedPiece = board.GetPiece(move.TargetSquare);
        int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];
        Console.WriteLine(capturedPieceValue.ToString());
        return capturedPieceValue;
    }
}
