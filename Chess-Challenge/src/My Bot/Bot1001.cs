using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
//For attempted troubleshooting
//using System.Threading.Tasks;
//using System.Diagnostics;

//Is preforming badly aganist evil bot
public class MyBot1001 : IChessBot
{
    //using Bot 603
    private Random random = new Random();
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 100000 };
    //static Board board;

    public Move Think(Board board, Timer timer)
    {
        //NOte make it only promote to queen under all circumstances

        //Note, function will be moved into the main build for the final submission to save space and potentially add more
        Move[] allMoves = board.GetLegalMoves();
        //Randomising the move orders will still have an effect, it makes it more likely to pick a move that is in the center and better, need to be tested tho
        allMoves = RandomizeArray(allMoves);

        //Make negative incase all moves are come up as negative, so it can play best terrible move
        int score = -999999999;

        Move drawMove = allMoves[random.Next(0, allMoves.Length)];
        //intially assigned an illegal move here and check if that move appear, then decide what to do
        Move bestMove = drawMove;
        //Move bestMove = allMoves[random.Next(0, allMoves.Length)];
        //Move drawMove = randomMove
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
                //Console.WriteLine("Found checkmate");
                return possibleMoves;
            }
            //Always ingoring mate seem to make bot worse
            if (WillGetMated(board, possibleMoves))
            {
                //This move lead to defeat should be ingored, if not checkmate
                //Console.WriteLine("Insta defeat move detected");
                continue;
            }
            //Detecting repetitions, lowers draws by 3 move and increases wind rates 
            board.MakeMove(possibleMoves);
            //I hate draw, this stops draw at all costs, it will make bot lose games, but it is better to win than lose
            if (board.IsDraw() || (0 == board.GetLegalMoves().Count()))//board.IsRepeatedPosition()
            {
                //Console.WriteLine("Do not repeat, stalement or draw");
                board.UndoMove(possibleMoves);
                drawMove = possibleMoves;//If there is no other good move, priorties drawing move
                continue;
            }
            board.UndoMove(possibleMoves);
            int currentScore = (FutureAttackTotal(board, possibleMoves) /*+ MateAble(board, possibleMoves)*/ + MoveTakePower(board, possibleMoves) + WinEndGame(board, possibleMoves) - MaxDangerDetection(board, possibleMoves) - FutureDefenceTotal(board, possibleMoves));
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
        if (score == -999999999)
        {
            return drawMove;
        }
        return bestMove;
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
    /*public int MateAble(Board board, Move move)
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
                //This search enemies move that might mate*/
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
    /*
            }
            board.UndoMove(potentialMove);
        }
        board.UndoMove(move);
        return mateAttack;
    }*/
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
    public int WinEndGame(Board board, Move move)
    {
        if (board.GetAllPieceLists().Length > 8)
        {
            return 0;
        }
        int isMoveGood = 0;
        if (MoveIsCheckmate(board, move))
        {
            isMoveGood += 2700;
        }
        board.MakeMove(move);

        Move[] moves0 = board.GetLegalMoves();
        foreach (Move ifMove0 in moves0)
        {
            board.MakeMove(ifMove0);

            Move[] moves1 = board.GetLegalMoves();
            foreach (Move ifMove1 in moves1)
            {
                if (MoveIsCheckmate(board, ifMove1))
                {
                    isMoveGood += 900;
                    continue;
                }
                else
                {
                    board.MakeMove(ifMove1);
                    Move[] moves2 = board.GetLegalMoves();
                    foreach (Move ifMove2 in moves2)
                    {
                        board.MakeMove(ifMove2);
                        Move[] moves3 = board.GetLegalMoves();
                        foreach (Move ifMove3 in moves3)
                        {
                            if (MoveIsCheckmate(board, ifMove3))
                            {
                                isMoveGood += 30;
                            }
                            else
                            {
                                board.MakeMove(ifMove3);
                                Move[] moves4 = board.GetLegalMoves();

                                foreach (Move ifMove4 in moves4)
                                {
                                    board.MakeMove(ifMove4);
                                    Move[] moves5 = board.GetLegalMoves();

                                    foreach (Move ifMove5 in moves5)
                                    {

                                        if (MoveIsCheckmate(board, ifMove5))
                                        {
                                            isMoveGood += 1;
                                        }
                                        /*
                                        else
                                        {
                                            //If want to go more deep
                                            board.MakeMove(ifMove6);
                                            Move[] moves6 = board.GetLegalMoves();



                                            board.MakeMove(ifMove6);
                                        }*/

                                    }

                                    board.MakeMove(ifMove4);
                                }


                                board.MakeMove(ifMove3);
                            }
                        }
                        board.MakeMove(ifMove2);
                    }
                    board.UndoMove(ifMove1);
                }
            }
            board.UndoMove(ifMove0);
        }
        board.UndoMove(move);
        return isMoveGood;
    }

    //This function forces bot to always promo to queen become queen is always better than a rook bishop and knight
    /*public int PieceValue(Board board, Move move)
    {
        PieceList[] piece = board.GetAllPieceLists();
        bool goWhite = board.IsWhiteToMove;
        board.MakeMove(move);
        PieceList[] piece2 = board.GetAllPieceLists();
        board.UndoMove(move);
        int queenCount = 0;
        foreach (PieceList pieces1 in piece)
        {
            if (pieces1.IsQueen && (pieces1.IsWhite == goWhite))
            {
                queenCount--;
            }
        }
        foreach (PieceList pieces2 in piece2)
        {
            //piece2
            if (pieces2.IsQueen && (pieces2.IsWhite == goWhite))
            {
                queenCount++;
            }
        }
        //Return a higher number when promotioning to queen
        return queenCount;
    }*/
}
