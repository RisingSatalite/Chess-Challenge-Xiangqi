using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot901 : IChessBot
{
    //using Bot 404
    private Random random = new Random();
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 100000 };

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

        //return allMoves.FirstOrDefault();

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
            /*if (AntiStalement(board, possibleMoves) == 0)
            {
                Console.WriteLine("Stalement detected");
                if (score < 0)
                    return possibleMoves;
                else
                    continue;
            }*/
            int currentScore = FutureAttackTotal(board, possibleMoves) + MateAble(board, possibleMoves) + MoveTakePower(board, possibleMoves) - MaxDangerDetection(board, possibleMoves) + FutureDefenceTotal(board, possibleMoves);
            Console.WriteLine("Next move");
            Console.WriteLine(MoveCalculate(board, possibleMoves).ToString());
            Console.WriteLine(FutureAttackTotal(board, possibleMoves).ToString());
            Console.WriteLine(MateAble(board, possibleMoves).ToString());
            //Depending on result, might want to run more tests, 
            //Console.WriteLine("Move score is :");
            //Console.WriteLine(currentScore.ToString());
            if (currentScore > score)
            {
                //Console.WriteLine("Best move so far");
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
            //Console.WriteLine(score.ToString());
            return bestMove;
        }
        catch
        {
            // If will get mated in one, choose a random move
            return allMoves[random.Next(0, allMoves.Length)];
        }
    }

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

    private int AntiStalement(Board board, Move move)
    {
        board.MakeMove(move);
        Move[] moveOptions = board.GetLegalMoves();
        board.UndoMove(move);
        return moveOptions.Length;
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
                if (potential > maxPower)
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

            }
            board.UndoMove(potentialMove);
        }
        board.UndoMove(move);
        return mateAttack;
    }
    public int MoveCalculate(Board board, Move move)
    {
        int mateAttack = 0;
        //int mateDanger = 0;
        int takePower = 0;
        int maxTakePower = 0;
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
                int potential = MoveTakePower(board, ifMove);
                takePower += potential;
                if (potential > maxTakePower)
                {
                    maxTakePower = potential;
                }


                if (MoveIsCheckmate(board, ifMove))
                {
                    mateAttack += 30;
                }
                /*board.MakeMove(ifMove);
                Move[] EnemyMoves = board.GetLegalMoves();
                foreach (Move enemyMove in EnemyMoves)
                {
                    if (MoveIsCheckmate(board, enemyMove))
                    {
                        mateDanger -= 1;
                    }
                }
                board.UndoMove(ifMove);*/
            }
            board.UndoMove(potentialMove);
        }
        board.UndoMove(move);
        return mateAttack + ((maxTakePower + 1) / 50) + ((takePower + 1) / ((counterMoves.Length + 1) * 10));
    }
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
            return (((maxPower + 1) / 50) + power / (possibleMoves.Length * 10) / 2);
        }
        catch
        {
            //Move results in a draw and should not be played
            return (10000);
        }

    }

}
