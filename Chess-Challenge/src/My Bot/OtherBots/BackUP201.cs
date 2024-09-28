﻿using ChessChallenge.API;
using System;

public class MyBot201_2 : IChessBot
{
    private static Random random = new Random();

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();

        //Default move is first one
        Move moveToPlay = allMoves[0];
        // Always play checkmate in one, if possible
        foreach (Move move in allMoves)
        {
            if (!MoveIsCheckmate(board, move))
            {
                return move;
            }
        }
        allMoves = RandomizeArray(allMoves);

        foreach (Move possibleMoves in allMoves)
        {
            if (WillGetMated(board, possibleMoves))
            {
                return possibleMoves;
            }
        }

        // If will get mated in one, choose a random move
        return allMoves[random.Next(0, allMoves.Length)];

    }

    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return !isMate;
    }

    //Functiondoes not work
    private bool WillGetMated(Board board, Move move)
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
    }

    //Randomise list, quite useful
    //make sure the 2 random connect
    public static Move[] RandomizeArray(Move[] array)
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
    public static void MoveJudgement(Board board, Move move)
    {

    }
}
