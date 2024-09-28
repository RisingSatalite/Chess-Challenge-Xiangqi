using ChessChallenge.API;
using System;
using System.Linq;
using System.Linq.Expressions;
//For attempted troubleshooting
//using System.Threading.Tasks;
//using System.Diagnostics;

//Add bot is currently preforming worse against evilbot
public class MyBot701 : IChessBot
{
    //using Bot 404
    private Random random = new Random();
    //static Board board;

    public Move Think(Board board, Timer timer)
    {
        //NOte make it only promote to queen under all circumstances

        //Note, function will be moved into the main build for the final submission to save space and potentially add more
        Move[] allMoves = board.GetLegalMoves();
        //Randomising the move orders will still have an effect, it makes it more likely to pick a move that is in the center and better, need to be tested tho
        allMoves = RandomizeArray(allMoves);

        //Make negative incase all moves are come up as negative, so it can play best terrible move
        int score = -99999999;
        Move bestMove = allMoves[random.Next(0, allMoves.Length)];
        /*foreach(Move move in allMoves)
        {
            Console.WriteLine(move.ToString);
        }*/
        //Attempted to rearrange
        //allMoves = allMoves.OrderBy(move => MoveTakePower(board, move)).ToArray();

        //return allMoves.FirstOrDefault();

        //bool IsEndgame = Endgame(board);

        foreach (Move possibleMoves in allMoves)
        {
            if (MoveIsCheckmate(board, possibleMoves))
            {
                Console.WriteLine("Found checkmate");
                return possibleMoves;
            }
            //Always ingoring mate seem to make bot worse
            if (WillGetMated(board, possibleMoves) || (AntiStalement(board, possibleMoves) == 0))
            {
                //This move lead to defeat should be ingored, if not checkmate
                Console.WriteLine("Insta defeat or stalement opponaut move detected");
                continue;
            }
            int currentScore = (FutureAttackTotal(board, possibleMoves) + MateAble(board, possibleMoves) + MoveTakePower(board, possibleMoves) - MaxDangerDetection(board, possibleMoves) /*- FutureDefenceTotal(board, possibleMoves)*/ + FutureDefenceTotal2(board, possibleMoves));
            //Depending on result, might want to run more tests, 
            Console.WriteLine("Move score is :");
            Console.WriteLine(currentScore.ToString());
            if (currentScore > score)
            {
                Console.WriteLine("Best move so far");
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
    private int AntiStalement(Board board, Move move)
    {
        board.MakeMove(move);
        Move[] moveOptions = board.GetLegalMoves();
        board.UndoMove(move);
        return moveOptions.Length;
    }

    //Function does not work
    private bool WillGetMated(Board board, Move move)
    {
        //board.MakeMove(move);
        board.MakeMove(move);
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
                board.UndoMove(move);
                return true;
            }
        }
        //Safe move
        board.UndoMove(move);
        return false;
    }
    //Check how to future attackes
    /*private int FutureAttack(Board board, Move move)
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
    }*/
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
            return ((maxPower / 50) + (power / (possibleMoves.Length * 10)));
        }
        catch
        {
            return (maxPower / 10);
        }//add return max power ans see if it improves
        //return (maxPower / 10);
        //return (power/(possibleMoves.Length*10));
    }

    //Basic version, create an advance version
    private int FutureDefenceTotal(Board board, Move move)
    {
        int power = 0;
        int maxPower = 0;
        //int numOfMoves = 0;
        board.MakeMove(move);
        Move[] possibleMoves = board.GetLegalMoves();
        foreach (Move possibleMove in possibleMoves)
        {
            int potential = MoveTakePower(board, possibleMove);
            power += potential;
            if (potential > maxPower)
            {
                maxPower = potential;
            }
        }
        board.UndoMove(move);
        try
        {
            return (maxPower / 50 + power / (possibleMoves.Length * 10) / 2);
        }
        catch
        {
            //Move results in a draw and should not be played
            return (10000);
        }

    }
    //Just makes it hoorible for some reason, and somehow, making it return a positive number is beneficial
    private int FutureDefenceTotal2(Board board, Move move)
    {
        int power = 0;
        int maxPower = 0;
        int numOfMoves = 0;
        board.MakeMove(move);
        Move[] possibleMoves = board.GetLegalMoves();
        foreach (Move possibleMove in possibleMoves)
        {
            board.MakeMove(possibleMove);
            Move[] futureBotMOves = board.GetLegalMoves();
            foreach (Move futureMove in futureBotMOves)
            {
                board.MakeMove(futureMove);
                Move[] counterMoves = board.GetLegalMoves();
                foreach (Move counterMove in counterMoves)
                {
                    int potential = MoveTakePower(board, possibleMove);
                    power += potential;
                    numOfMoves += counterMoves.Length;
                    if (potential > maxPower)
                    {
                        maxPower = potential;
                    }
                }
                board.UndoMove(futureMove);
            }
            board.UndoMove(possibleMove);
        }
        board.UndoMove(move);
        try
        {
            return (maxPower / 50 + power / (numOfMoves * 100));
        }
        catch
        {
            //Move results in a draw and should not be played
            return (10000);
        }

    }
    //An attempt to check every square on the board to see how many squares are attacked
    /*public int ControlAttack(Board board, Move move) 
    {
        Square[] squareList;
        int AttackWatch = 0;
        board.MakeMove(move);
        foreach(Square square in squareList)
        {
            if (SquareIsAttackedByOpponent(square))
            {
                AttackWatch++;
            }
        }
        board.UndoMove(move);
        return AttackWatch;
    }*/
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
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

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
        Console.WriteLine("Max danger detected");
        Console.WriteLine(capturedPieceValue.ToString());
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
                board.UndoMove(ifMove);*/

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
    /*public bool Endgame(Board board)
    {
        //Check if only black or white king and a few piece remains, if so, it does deep search
        int recorderBlack;
        int recorderWhite;
        PieceList[] piece = board.GetAllPieceLists();
        foreach(PieceList pieces in piece)
        {
            Piece type = pieces.TypeOfPieceInList;
            bool WhiteOrNot = pieces.IsWhitePieceList;
            if (WhiteOrNot)
            {
                recorderWhite++;
            }
            else
            {
                recorderBlack++;
            }
        }
        if(recorderWhite<3 ||recorderBlack < 3)
        {
            //This is the endgame
            return true;
        }
        //This is not the endgame
        return false;
    }*/
    /*public int EndGameWinner(Board board, Move move)
    {
        int isMoveGood = 0;
        board.MakeMove(move);
        board.UndoMove(move);
    }*/

}
