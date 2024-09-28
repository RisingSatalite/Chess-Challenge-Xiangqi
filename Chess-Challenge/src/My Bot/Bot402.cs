using ChessChallenge.API;
using System;
using System.Linq;
//Adding 3 mate search, is not better as bot begin priorotising moves that make win if enemy blunders, and is exponature slower, 401 also and a better win rate, 402 is significatly worse
public class MyBot402 : IChessBot
{
    private Random random = new Random();
    //static Board board;

    public Move Think(Board board, Timer timer)
    {
        //NOte make it only promote to queen under all circumstances

        //Note, function will be moved into the main build for the final submission to save space and potentially add more
        Move[] allMoves = board.GetLegalMoves();
        //Randomising the move orders will still have an effect, it makes it more likely to pick a move that is in the center and better, need to be tested tho
        allMoves = RandomizeArray(allMoves);

        int score = 0;
        Move bestMove = allMoves[random.Next(0, allMoves.Length)];
        /*foreach(Move move in allMoves)
        {
            Console.WriteLine(move.ToString);
        }*/
        //Attempted to rearrange
        //allMoves = allMoves.OrderBy(move => MoveTakePower(board, move)).ToArray();

        //return allMoves.FirstOrDefault();

        foreach (Move possibleMoves in allMoves)
        {
            if (MoveIsCheckmate(board, possibleMoves))
            {
                Console.WriteLine("Found checkmate");
                return possibleMoves;
            }
            //Always ingoring mate seem to make bot worse
            /*if (WillGetMated(board, possibleMoves))
            {
                //This move lead to defeat should be ingored, if not checkmate
                continue;
            }*/
            int currentScore = MateAble(board, possibleMoves) + MoveTakePower(board, possibleMoves) - MaxDangerDetection(board, possibleMoves);
            Console.WriteLine(currentScore.ToString());
            if (currentScore > score)
            {
                //My bot is better but does not know how to mate endgame
                score = currentScore;
                bestMove = possibleMoves;
            }
            /*if (WillGetMated(board, possibleMoves))
            {
                return possibleMoves;
            }*/
        }
        try
        {
            Console.WriteLine(score.ToString());
            return bestMove;
        }
        catch
        {
            // If will get mated in one, choose a random move
            return allMoves[random.Next(0, allMoves.Length)];
        }
    }
    /*public int CustomComparison(Move a, Move b, Board board)
    {
        int placeA = MoveTakePower(board, a);
        int placeB = MoveTakePower(board, b);
        return placeA.CompareTo(placeB);
    }*/

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

    //Function does not work
    private bool WillGetMated(Board board, Move move)
    {
        //board.MakeMove(move);
        Move[] allMoves = board.GetLegalMoves();
        foreach (Move possibleMove in allMoves)
        {
            //Console.WriteLine("Checking possible moves");
            board.MakeMove(possibleMove);
            bool isMate = board.IsInCheckmate();
            //Console.WriteLine(isMate.ToString());
            board.UndoMove(possibleMove);
            if (isMate)
            {
                //Do not preform, mateable
                return true;
            }
        }
        //Safe move
        return false;
    }

    //Randomise list, quite useful
    //make sure the 2 random connect
    public Move[] RandomizeArray(Move[] array)
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
    }

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
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 100000 };

        Piece capturedPiece = board.GetPiece(move.TargetSquare);
        int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];
        //Console.WriteLine(capturedPieceValue.ToString());
        return capturedPieceValue;
    }
    public int MaxDangerDetection(Board board, Move move)
    {
        int capturedPieceValue = 0;
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 100000 };
        board.MakeMove(move);
        Move[] counterMoves = board.GetLegalMoves();
        foreach (Move ifMove in counterMoves)
        {
            Piece capturedPiece = board.GetPiece(ifMove.TargetSquare);
            int capturedPiecePotential = pieceValues[(int)capturedPiece.PieceType];
            //Console.WriteLine(capturedPieceValue.ToString());
            if (capturedPiecePotential > capturedPieceValue)
            {
                capturedPieceValue = capturedPiecePotential;
            }
        }
        board.UndoMove(move);
        return capturedPieceValue;
    }
    public int MateAble(Board board, Move move)
    {
        int mateAttack = 0;
        board.MakeMove(move);
        Move[] counterMoves = board.GetLegalMoves();
        foreach (Move potentialMove in counterMoves)
        {
            board.MakeMove(potentialMove);
            Move[] attackMoves = board.GetLegalMoves();
            foreach (Move ifMove in attackMoves)
            {
                if (MoveIsCheckmate(board, ifMove))
                {
                    mateAttack += 30;
                }

                board.MakeMove(ifMove);
                Move[] attackMoves2 = board.GetLegalMoves();
                foreach (Move ifMove2 in attackMoves2)
                {
                    if (MoveIsCheckmate(board, ifMove2))
                    {
                        mateAttack += 1;
                    }
                }
                board.UndoMove(ifMove);

            }
            board.UndoMove(potentialMove);
        }
        board.UndoMove(move);
        return mateAttack;
    }
}
