using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot501 : IChessBot
{
    //Using machine learning to optimise certain numbers would be a good idea
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
            int currentScore = FutureAttackTotal(board, possibleMoves) + MateAble(board, possibleMoves) + MoveTakePower(board, possibleMoves) - MaxDangerDetection(board, possibleMoves);
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
    //Check how to future attackes
    private int FutureAttack(Board board, Move move)
    {
        int power = 0;
        board.MakeMove(move);
        Move[] possibleMoves = board.GetLegalMoves();
        foreach (Move possibleMove in possibleMoves)
        {
            //The s at the end differ for the varibles
            board.MakeMove(possibleMove);
            Move[] possibleMoves2 = board.GetLegalMoves();
            foreach (Move possibleMove2 in possibleMoves2)
            {
                int potential = MoveTakePower(board, possibleMove2);
                //It is currently run on a per move, best move basis, might be interesting to make it take scores from all runs
                if (potential > power)
                {
                    power = potential;
                }
            }
            board.UndoMove(possibleMove);
        }
        board.UndoMove(move);
        return (power / 10);
    }
    private int FutureAttackTotal(Board board, Move move)
    {
        int power = 0;
        int maxPower = 0;
        board.MakeMove(move);
        Move[] possibleMoves = board.GetLegalMoves();
        foreach (Move possibleMove in possibleMoves)
        {
            //The s at the end differ for the varibles
            board.MakeMove(possibleMove);
            Move[] possibleMoves2 = board.GetLegalMoves();
            foreach (Move possibleMove2 in possibleMoves2)
            {
                int potential = MoveTakePower(board, possibleMove2);
                //It is currently run on a per move, best move basis, might be interesting to make it take scores from all runs

                power += potential;
                if (potential > power)
                {
                    maxPower = potential;
                }

            }
            board.UndoMove(possibleMove);
        }
        board.UndoMove(move);
        //Needs more testing, can be mated more but will win more than other options
        //Note a work around if possibleMoves.lenght is 0, probably because of scenerous where already mate, but it move efficent later
        try
        {
            return ((maxPower + (power / possibleMoves.Length)) / 10);
        }
        catch
        {
            return 0;
        }
        //return (maxPower / 10);
        //return (power/(possibleMoves.Length*10));
    }

    private void FutureDefence(Board board, Move move)
    {
        board.MakeMove(move);
        board.UndoMove(move);
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
        //Console.WriteLine(capturedPieceValue.ToString);
        return capturedPieceValue;
    }
    public int MateAble(Board board, Move move)
    {
        int mateAttack = 0;
        board.MakeMove(move);
        Move[] counterMoves = board.GetLegalMoves();
        foreach (Move potentialMove in counterMoves)
        {
            //Each move avilable to enemy
            //Check every move avilable to bot and increase the changes of some moves 
            board.MakeMove(potentialMove);
            Move[] attackMoves = board.GetLegalMoves();
            foreach (Move ifMove in attackMoves)
            {

                if (MoveIsCheckmate(board, ifMove))
                {
                    mateAttack += 30;
                }

                //Extra layer of search, does not mean it better, take time and make profromance worse
                //This search enemies move that might mate
                /*board.MakeMove(ifMove);
                Move[] attackMoves2 = board.GetLegalMoves();
                foreach (Move ifMove2 in attackMoves2)
                {
                    if (MoveIsCheckmate(board, ifMove2))
                    {
                        mateAttack -= 1;
                    }
                }
                board.UndoMove(ifMove);
                */
            }
            board.UndoMove(potentialMove);
        }
        board.UndoMove(move);
        return mateAttack;
    }
    /*FUnctionality added to Mateable instead, tho slow and work improperly
    public int getMated(Board board, Move move) 
    {
        int Danger = 0;
        board.MakeMove(move);
        Move[] attackMoves = board.GetLegalMoves();
        foreach (Move ifMove in attackMoves)
        {

        }
        board.UndoMove(move);
        return Danger;
    }*/
    //It is currently very bad at endgame, add this search if it only enemy king left
    public void ENndgame(Board board, Move move)
    {

    }
}
