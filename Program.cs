using System;
using System.IO;

namespace Chess
{
    class Program
    {
        static int[] notation_reader(string movement, bool color, Pieces[,] board) //To read notations (e4)
        {
            string type = ""; // type of moving piece in notation (P, K, B, ...)
            int[] move = { -1, -1, -1, -1 }; // Move in notation; Old x, Old y, New x, New y
            string pieces = "RNBQK"; //All piece types except Pawn
            string location = "abcdefgh"; //To get index from letter
            string[] arr = new string[2]; //To split 2 parts capture conditions
            try
            {
                if (movement.Contains('x')) //Capture Conditions
                {
                    arr = movement.Split('x');
                    if (movement.Contains("e.p."))
                    {
                        move[0] = location.IndexOf(arr[0][0]);
                        move[2] = location.IndexOf(arr[1][0]);
                        move[3] = Int32.Parse($"{arr[1][1]}");
                    }
                    else if (arr[0].Length == 2 && pieces.Contains(arr[0][0]))
                    { // Captrue conditions except Pawn (2 conditions)
                        type = $"{arr[0][0]}";
                        if (location.Contains(arr[0][1]))
                        { //To find Old x
                            move[0] = location.IndexOf(arr[0][1]);
                        }
                        else
                        { //To find Old y
                            move[1] = Int32.Parse($"{arr[0][1]}");
                        }
                        move[2] = location.IndexOf(arr[1][0]);
                        move[3] = Int32.Parse($"{arr[1][1]}");
                    }
                    else if (pieces.Contains(arr[0]))
                    { //(1 condition)
                        type = $"{arr[0][0]}";
                        move[2] = location.IndexOf(arr[1][0]);
                        move[3] = Int32.Parse($"{arr[1][1]}");
                    }
                    else //Capture Case for Pawn
                    {
                        type = "P";
                        move[0] = location.IndexOf(arr[0]);
                        move[2] = location.IndexOf(arr[1][0]);
                        move[3] = Int32.Parse($"{arr[1][1]}");
                    }
                }
                else if (movement.Contains("0-0"))
                { //Castling
                    type = "K";
                    if (movement == "0-0") //Castling (Short)
                    {
                        if (color)
                        {
                            move[0] = 4;
                            move[1] = 8;
                            move[2] = 6;
                            move[3] = 8;
                        }
                        else
                        {
                            move[0] = 4;
                            move[1] = 1;
                            move[2] = 6;
                            move[3] = 1;
                        }
                    }
                    else if (movement == "0-0-0") //Castling Long
                    {
                        if (color)
                        {
                            move[0] = 4;
                            move[1] = 8;
                            move[2] = 2;
                            move[3] = 8;
                        }
                        else
                        {
                            move[0] = 4;
                            move[1] = 1;
                            move[2] = 2;
                            move[3] = 1;
                        }
                    }
                }
                else //Movement Case (except capture)
                {
                    if (pieces.Contains(movement[0]))
                    { //All pieces except Pawn
                        if (movement.Length == 3 || (movement.Length == 4 && movement.Contains("+")))
                        { //2 Conditions
                            type = $"{movement[0]}";
                            move[2] = location.IndexOf(movement[1]);
                            move[3] = Int32.Parse($"{movement[2]}");
                        }
                        else
                        { //Single Condition
                            type = $"{movement[0]}";
                            move[0] = location.IndexOf(movement[1]);
                            move[2] = location.IndexOf(movement[2]);
                            move[3] = Int32.Parse($"{movement[3]}");
                        }
                    }
                    else if (movement.Length == 2 || (movement.Length == 3 && movement.Contains("+")))//For Pawn
                    {
                        type = "P";
                        move[2] = location.IndexOf(movement[0]);
                        move[3] = Int32.Parse($"{movement[1]}");
                    }
                    else if (pieces.Contains(movement[2])) //Promote
                    {
                        type = "P";
                        move[2] = location.IndexOf(movement[0]);
                        move[3] = Int32.Parse($"{movement[1]}");
                    }
                }
                if (move[1] != -1) move[1] = 8 - move[1];
                if (move[3] != -1) move[3] = 8 - move[3];
            }
            catch // Invalid Notation
            {
                move[0] = -1;
                move[1] = -1;
                move[2] = -1;
                move[3] = -1;
                return move;
            }
            if (move[0] == -1 && move[1] == -1) //If two positions are unknown
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i, j].type == type && board[i, j].color == color) //If there are parts of the same type and color
                        {
                            int[] loc = { i, j };
                            int[,] array = Place_Generator(board[i, j], loc, board, false);
                            for (int a = 0; a < 64; a++)
                            {
                                if (array[a, 0] != -1 || array[a, 1] != -1)
                                {
                                    if (move[2] == array[a, 0] && move[3] == array[a, 1])
                                    {
                                        move[0] = i;
                                        move[1] = j;
                                        return move;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (move[0] == -1) //If x position is unknown
            {
                for (int i = 0; i < 8; i++) //searching in columns
                {
                    if (board[i, move[1]].type == type && board[move[0], i].color == color) //If there are parts of the same type and color
                    {
                        int[] loc = { i, move[1] };
                        int[,] array = Place_Generator(board[i, move[1]], loc, board, false);
                        for (int a = 0; a < 64; a++)
                        {
                            if (array[a, 0] != -1 || array[a, 1] != -1)
                            {
                                if (move[2] == array[a, 0] && move[3] == array[a, 1])
                                {
                                    move[0] = i;
                                    return move;
                                }
                            }
                        }
                    }
                }
            }
            else if (move[1] == -1) //If y position is unknown
            {
                for (int j = 0; j < 8; j++) //searching in rows
                {
                    if (board[move[0], j].type == type && board[move[0], j].color == color)
                    {
                        int[] loc = { move[0], j };
                        int[,] array = Place_Generator(board[move[0], j], loc, board, false);
                        for (int a = 0; a < 64; a++)
                        {
                            if (array[a, 0] != -1 && array[a, 1] != -1)
                            {
                                if (move[2] == array[a, 0] && move[3] == array[a, 1])
                                {
                                    move[1] = j;
                                    return move;
                                }
                            }
                        }
                    }
                }
            }
            return move;
        }
        static int[,] hint(bool color, Pieces[,] board) //Hint function that returns all possible capture moves (hint for which color, current board)
        {
            int movement = 0;
            bool point;
            int[,] logic_mov = new int[50, 4];
            for (int x = 0; x < 50; x++) //Resetting array with -1
            {
                for (int y = 0; y < 4; y++)
                {
                    logic_mov[x, y] = -1;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].color == color && board[i, j].control) //Searches in all pieces that same color in the argument 
                    {
                        int[] loc = { i, j };
                        int[,] array = Place_Generator(board[i, j], loc, board, false);
                        for (int a = 0; a < 64; a++) //Then searches on all capturable moves
                        {
                            if (array[a, 0] != -1 || array[a, 1] != -1)
                            {
                                if (board[array[a, 0], array[a, 1]].type == "P" && board[array[a, 0], array[a, 1]].control && board[array[a, 0], array[a, 1]].color != color) point = true;
                                else if (board[array[a, 0], array[a, 1]].type == "R" && board[array[a, 0], array[a, 1]].control && board[array[a, 0], array[a, 1]].color != color) point = true;
                                else if (board[array[a, 0], array[a, 1]].type == "N" && board[array[a, 0], array[a, 1]].control && board[array[a, 0], array[a, 1]].color != color) point = true;
                                else if (board[array[a, 0], array[a, 1]].type == "B" && board[array[a, 0], array[a, 1]].control && board[array[a, 0], array[a, 1]].color != color) point = true;
                                else if (board[array[a, 0], array[a, 1]].type == "Q" && board[array[a, 0], array[a, 1]].control && board[array[a, 0], array[a, 1]].color != color) point = true;
                                else point = false;
                                if (point) //Looks for piecees that can be capture
                                {
                                    logic_mov[movement, 0] = i;
                                    logic_mov[movement, 1] = j;
                                    logic_mov[movement, 2] = array[a, 0];
                                    logic_mov[movement, 3] = array[a, 1];
                                    movement++;
                                }
                            }
                        }
                    }
                }
            }
            return logic_mov; //returns all possible moves
        }
        static int evaluation(int[] logic_mov, bool color, string type) //Evaluation board of positions part for AI
        {
            int[,] pawnEvalWhite ={{ 0, 0, 0, 0, 0, 0, 0, 0 },
                { 5, 5, 5, 5, 5, 5, 5, 5},
                { 1, 1, 2, 3, 3, 2, 1, 1},
                { 1, 1, 1, 3, 3, 1, 1, 1},
                { 0, 0, 0, 2, 2, 0, 0, 0},
                {1, -1, -1, 0, 0, -1, -1, 1},
                {1, 1, 1, -2, -2, 1, 1, 1},
                {0, 0, 0, 0, 0, 0, 0, 0}};
            int[,] pawnEvalBlack = {{0,0,0,0,0,0,0,0},
                                     {1 ,1  ,1 ,- 2, - 2,  1 , 1 , 1},
                                     {1 ,- 1 ,- 1 , 0 , 0 ,- 1 ,- 1 , 1},
                                     {0 , 0,  0,  2 , 2,  0 , 0,  0},
                                     {1 , 1 , 1 , 3,  3 , 1 , 1,  1},
                                     {1 , 1 , 2,  3 , 3,  2 , 1,  1},
                                     {5 , 5 , 5,  5 , 5 , 5 , 5 , 5},
                                     {0 , 0 , 0 , 0 , 0 , 0,  0 , 0 }};
            int[,] knightEval ={{-5, -4, -3, -3, -3, -3, -4, -5},
                    {-4, -2, 0, 0, 0, 0, -2, -4},
                    {-3, 0, 1, 2, 2, 1, 0, -3},
                    {-3, 1, 2, 2, 2, 2, 1, -3},
                    {-3, 0, 2, 2, 2, 2, 0, -3},
                    {-3, 1, 1, 2, 2, 1, 1, -3},
                    {-4, -2, 0, 1, 1, 0, -2, -4},
                    {-5, -4, -3, -3, -3, -3, -4, -5}};
            int[,] bishopEvalWhite = {{-2, -1, -1, -1, -1, -1, -1, -2},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {-1, 0, 1, 1, 1, 1, 0, -1},
                {-1, 1, 1, 1, 1, 1, 1, -1},
                {-1, 0, 1, 1, 1, 1, 0, -1},
                {-1, 1, 1, 1, 1, 1, 1, -1},
                {-1, 1, 0, 0, 0, 0, 1, -1},
                {-2, -1, -1, -1, -1, -1, -1, -2}};
            int[,] bishopEvalBlack = { { -2, -1, -1, -1, -1, -1, -1, -2 },
                { -1, 0, 0, 0, 0, 0, 0, -1 },
                { -1, 0, 1, 1, 1, 1, 0, -1 },
                { -1, 1, 1, 1, 1, 1, 1, -1 },
                { -1, 0, 1, 1, 1, 1, 0, -1 },
                { -1, 1, 1, 1, 1, 1, 1, -1 },
                { -1, 1, 0, 0, 0, 0, 1, -1 },
                { -2, -1, -1, -1, -1, -1, -1, -2 } };
            int[,] rookEvalWhite = {{0, 0, 0, 0, 0, 0, 0, 0},
                {1, 1, 1, 1, 1, 1, 1, 1},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {0, 0, 0, 1, 1, 0, 0, 0}};
            int[,] rookEvalBlack = { { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1 },
                { -1, 0, 0, 0, 0, 0, 0, -1 },
                { -1, 0, 0, 0, 0, 0, 0, -1 },
                { -1, 0, 0, 0, 0, 0, 0, -1 },
                { -1, 0, 0, 0, 0, 0, 0, -1 },
                { -1, 0, 0, 0, 0, 0, 0, -1 },
                { 0, 0, 0, 1, 1, 0, 0, 0 } };
            int[,] evalQueen ={{-2, -1, -1, -1, -1, -1, -1, -2},
                {-1, 0, 0, 0, 0, 0, 0, -1},
                {-1, 0, 1, 1, 1, 1, 0, -1},
                {-1, 0, 1, 1, 1, 1, 0, -1},
                {0, 0, 1, 1, 1, 1, 0, -1},
                {-1, 1, 1, 1, 1, 1, 0, -1},
                {-1, 0, 1, 0, 0, 0, 0, -1},
                {-2, -1, -1, -1, -1, -1, -1, -2}};
            int[,] kingEvalWhite = { {-3, -4, -4, -5, -5, -4, -4, -3},
                {-3, -4, -4, -5, -5, -4, -4, -3},
                {-3, -4, -4, -5, -5, -4, -4, -3},
                {-3, -4, -4, -5, -5, -4, -4, -3},
                {-2, -3, -3, -4, -4, -3, -3, -2},
                {-1, -2, -2, -2, -2, -2, -2, -1},
                {2, 2, 0, 0, 0, 0, 2, 2},
                {2, 3, 1, 0, 0, 1, 3, 2}};
            int[,] kingEvalBlack = {{ -3, -4, -4, -5, -5, -4, -4, -3 },
                { -3, -4, -4, -5, -5, -4, -4, -3 },
                { -3, -4, -4, -5, -5, -4, -4, -3 },
                { -3, -4, -4, -5, -5, -4, -4, -3 },
                { -2, -3, -3, -4, -4, -3, -3, -2 },
                { -1, -2, -2, -2, -2, -2, -2, -1 },
                { 2, 2, 0, 0, 0, 0, 2, 2 },
                { 2, 3, 1, 0, 0, 1, 3, 2 } };
            int eval_point = 0;
            if (type == "P")
            {
                if (color)
                {
                    eval_point = pawnEvalBlack[logic_mov[3], logic_mov[2]] - pawnEvalBlack[logic_mov[1], logic_mov[0]];
                }
                else
                {
                    eval_point = pawnEvalWhite[logic_mov[3], logic_mov[2]] - pawnEvalWhite[logic_mov[1], logic_mov[0]];
                }
            }
            else if (type == "R")
            {
                if (color)
                {
                    eval_point = rookEvalBlack[logic_mov[3], logic_mov[2]] - rookEvalBlack[logic_mov[1], logic_mov[0]];
                }
                else
                {
                    eval_point = rookEvalWhite[logic_mov[3], logic_mov[2]] - rookEvalWhite[logic_mov[1], logic_mov[0]];
                }
            }
            else if (type == "N")
            {
                eval_point = knightEval[logic_mov[3], logic_mov[2]] - knightEval[logic_mov[1], logic_mov[0]];
            }
            else if (type == "B")
            {
                if (color)
                {
                    eval_point = bishopEvalBlack[logic_mov[3], logic_mov[2]] - bishopEvalBlack[logic_mov[1], logic_mov[0]];
                }
                else
                {
                    eval_point = bishopEvalWhite[logic_mov[3], logic_mov[2]] - bishopEvalWhite[logic_mov[1], logic_mov[0]];
                }
            }
            else if (type == "Q")
            {
                eval_point = evalQueen[logic_mov[3], logic_mov[2]] - evalQueen[logic_mov[1], logic_mov[0]];
            }
            else if (type == "K")
            {
                if (color)
                {
                    eval_point = kingEvalBlack[logic_mov[3], logic_mov[2]] - kingEvalBlack[logic_mov[1], logic_mov[0]];
                }
                else
                {
                    eval_point = kingEvalWhite[logic_mov[3], logic_mov[2]] - kingEvalWhite[logic_mov[1], logic_mov[0]];
                }
            }
            return eval_point;
        }
        static int minimise(int depth, int[] logic_mov, Pieces[,] board, bool min) //Minimax part of AI
        {
            if (depth == 3) //Changes the search depth
            {
                return 0;
            }
            int[] logic_mov2 = new int[5];
            logic_mov2[4] = -1;
            Pieces[,] virtual_board = board.Clone() as Pieces[,];
            Pieces pattern_AI;
            pattern_AI = virtual_board[logic_mov[0], logic_mov[1]];
            virtual_board[logic_mov[2], logic_mov[3]] = pattern_AI;
            virtual_board[logic_mov[0], logic_mov[1]].control = false;
            virtual_board[logic_mov[0], logic_mov[1]].type = "";
            if (virtual_board[logic_mov[2], logic_mov[3]].type == "K" || virtual_board[logic_mov[2], logic_mov[3]].type == "R") virtual_board[logic_mov[2], logic_mov[3]].played = true; //For AI's castling
            int virtual_point;
            int[] mov = new int[5];
            bool first = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (min) //minimise part (searches for opponents moves)
                    {
                        if (virtual_board[i, j].control && !virtual_board[i, j].color)
                        {
                            int[] loc = { i, j };
                            int[,] array = Place_Generator(virtual_board[i, j], loc, virtual_board, false);
                            for (int a = 0; a < 64; a++)
                            {
                                if (array[a, 0] != -1 || array[a, 1] != -1)
                                {
                                    if (virtual_board[array[a, 0], array[a, 1]].type == "P" && virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = -15;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "R" && virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = -75;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "N" && virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = -45;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "B" && virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = -45;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "Q" && virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = -135;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "K" && virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = -999;
                                    else virtual_point = 0;
                                    mov[0] = i;
                                    mov[1] = j;
                                    mov[2] = array[a, 0];
                                    mov[3] = array[a, 1];
                                    mov[4] = virtual_point - evaluation(mov, virtual_board[i, j].color, virtual_board[i, j].type) + minimise(depth + 1, mov, virtual_board, !min); //Recursive minimax tree
                                    if (first) //First Assigment
                                    {
                                        logic_mov2[0] = i;
                                        logic_mov2[1] = j;
                                        logic_mov2[2] = array[a, 0];
                                        logic_mov2[3] = array[a, 1];
                                        logic_mov2[4] = mov[4];
                                        first = false;
                                    }
                                    if (mov[4] < logic_mov2[4]) //Chooses best move
                                    {
                                        logic_mov2[0] = i;
                                        logic_mov2[1] = j;
                                        logic_mov2[2] = array[a, 0];
                                        logic_mov2[3] = array[a, 1];
                                        logic_mov2[4] = mov[4];
                                    }
                                }
                            }
                        }
                    }
                    else if (!min) //maximise part (searches for own movements)
                    {
                        if (virtual_board[i, j].control && virtual_board[i, j].color)
                        {
                            int[] loc = { i, j };
                            int[,] array = Place_Generator(virtual_board[i, j], loc, virtual_board, false);
                            for (int a = 0; a < 64; a++)
                            {
                                if (array[a, 0] != -1 || array[a, 1] != -1)
                                {
                                    if (virtual_board[array[a, 0], array[a, 1]].type == "P" && !virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = 15;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "R" && !virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = 75;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "N" && !virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = 45;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "B" && !virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = 45;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "Q" && !virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = 135;
                                    else if (virtual_board[array[a, 0], array[a, 1]].type == "K" && !virtual_board[array[a, 0], array[a, 1]].color && virtual_board[array[a, 0], array[a, 1]].control) virtual_point = 999;
                                    else virtual_point = 0;
                                    mov[0] = i;
                                    mov[1] = j;
                                    mov[2] = array[a, 0];
                                    mov[3] = array[a, 1];
                                    mov[4] = virtual_point + evaluation(mov, virtual_board[i, j].color, virtual_board[i, j].type) + minimise(depth + 1, mov, virtual_board, !min); //Recursive minimax tree

                                    if (first) //First assigment
                                    {
                                        logic_mov2[0] = i;
                                        logic_mov2[1] = j;
                                        logic_mov2[2] = array[a, 0];
                                        logic_mov2[3] = array[a, 1];
                                        logic_mov2[4] = mov[4];
                                        first = false;
                                    }
                                    else if (mov[4] > logic_mov2[4]) //Chooses best move
                                    {
                                        logic_mov2[0] = i;
                                        logic_mov2[1] = j;
                                        logic_mov2[2] = array[a, 0];
                                        logic_mov2[3] = array[a, 1];
                                        logic_mov2[4] = mov[4];
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return logic_mov2[4];
        }
        static bool is_gameover(bool color, Pieces[,] board) //The function that checks if the game is over
        {
            int[] mov = new int[5];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].color == color && board[i, j].control)
                    {
                        int[] loc = { i, j };
                        int[,] arr = Place_Generator(board[i, j], loc, board, false);
                        for (int a = 0; a < 64; a++)
                        {
                            if (arr[a, 0] != -1 || arr[a, 1] != -1)
                            {
                                mov[0] = i; //Old x
                                mov[1] = j;//Old y
                                mov[2] = arr[a, 0];//New x
                                mov[3] = arr[a, 1];//New y
                                if (is_threat_blocked(color, board, mov)) return false; //Tries movement and checks whether threat blocked
                            }
                        }
                    }
                }
            }
            return true;
        }
        static bool is_threat_blocked(bool color, Pieces[,] board, int[] move) //Checks whether threat blocked (Color for check if that color is out of threat)
        {
            Pieces[,] virtual_board = board.Clone() as Pieces[,]; //Create a virtual board then try on it moves. 
            Pieces pattern;
            pattern = virtual_board[move[0], move[1]];
            virtual_board[move[2], move[3]] = pattern;
            virtual_board[move[2], move[3]].control = true;
            virtual_board[move[0], move[1]].control = false;
            virtual_board[move[0], move[1]].type = "";
            if (threat(color, virtual_board)) //Checks whether does the threat still exist.
            {
                return false;
            }
            else return true;
        }
        static bool threat(bool color, Pieces[,] virtual_board) //Function that checks there is any threat
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (virtual_board[i, j].type != "K" && virtual_board[i, j].color != color) //Searches on entire board and look if are colors different     
                    {
                        int[] loc = { i, j };
                        int[,] array = Place_Generator(virtual_board[i, j], loc, virtual_board, false); //then sends to Place Generator
                        for (int a = 0; a < 64; a++)
                        {
                            if (array[a, 0] != -1 || array[a, 1] != -1)
                            {
                                if (virtual_board[array[a, 0], array[a, 1]].type == "K" && virtual_board[array[a, 0], array[a, 1]].color == color)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        static int[,] Place_Generator(Pieces p, int[] loc, Pieces[,] board, bool king) //This function generates all POSSIBLE moves. Returns array that holds movements.
        {
            int[,] playable = new int[64, 2];
            for (int i = 0; i < 64; i++)
            {
                playable[i, 0] = loc[0];
                playable[i, 1] = loc[1];
            }
            if (p.type == "P") //If the type of argument is Pawn 
            {
                if (p.color) //If pawn's color is black
                {
                    if (king) //If checking for the king's movement
                    {
                        playable[48, 0] = loc[0] - 1;
                        playable[48, 1] = loc[1] + 1;
                        playable[32, 0] = loc[0] + 1;
                        playable[32, 1] = loc[1] + 1;
                    }
                    else //If checking for its own movement
                    {
                        if (loc[1] < 7)   //Forward
                        {
                            playable[0, 1] = loc[1] + 1;
                        }
                        if (loc[0] - 1 >= 0 && loc[0] - 1 <= 7 && loc[1] + 1 >= 0 && loc[1] + 1 <= 7 && board[loc[0] - 1, loc[1] + 1].control && !board[loc[0] - 1, loc[1] + 1].color) //capture (left)
                        {
                            playable[48, 0] = loc[0] - 1;
                            playable[48, 1] = loc[1] + 1;
                        }
                        if (loc[0] + 1 >= 0 && loc[0] + 1 <= 7 && loc[1] + 1 >= 0 && loc[1] + 1 <= 7 && board[loc[0] + 1, loc[1] + 1].control && !board[loc[0] + 1, loc[1] + 1].color)//capture (rigt)
                        {
                            playable[32, 0] = loc[0] + 1;
                            playable[32, 1] = loc[1] + 1;
                        }
                        if (loc[1] == 1) //First move (double)
                        {
                            playable[1, 1] = loc[1] + 2;
                        }
                    }

                }
                else //If pawn's color is white
                {
                    if (king) //If checking for the king's movement
                    {
                        playable[40, 0] = loc[0] - 1;
                        playable[40, 1] = loc[1] - 1;
                        playable[56, 0] = loc[0] + 1;
                        playable[56, 1] = loc[1] - 1;
                    }
                    else
                    {
                        if (loc[1] > 0) //Forward 
                        {
                            playable[0, 1] = loc[1] - 1;
                        }
                        if (loc[0] - 1 >= 0 && loc[0] - 1 <= 7 && loc[1] - 1 >= 0 && loc[1] - 1 <= 7 && board[loc[0] - 1, loc[1] - 1].control && board[loc[0] - 1, loc[1] - 1].color) //capture (left)
                        {
                            playable[40, 0] = loc[0] - 1;
                            playable[40, 1] = loc[1] - 1;
                        }
                        if (loc[1] - 1 >= 0 && loc[1] - 1 <= 7 && loc[0] + 1 >= 0 && loc[0] + 1 <= 7 && board[loc[0] + 1, loc[1] - 1].control && board[loc[0] + 1, loc[1] - 1].color) //capture (right)
                        {
                            playable[56, 0] = loc[0] + 1;
                            playable[56, 1] = loc[1] - 1;
                        }
                        if (loc[1] == 6) //First move (double)
                        {
                            playable[1, 1] = loc[1] - 2;
                        }
                    }
                }
                if (en_passant_bool && (en_Passant_loc[0] + 1 == loc[0] || en_Passant_loc[0] - 1 == loc[0]) && ((loc[1] == en_Passant_loc[1] + 1 && !p.color) || (loc[1] == en_Passant_loc[1] - 1 && p.color)) && en_passant_color != p.color) //En passant
                {
                    playable[3, 0] = en_Passant_loc[0];
                    playable[3, 1] = en_Passant_loc[1];
                }
            }
            else if (p.type == "R" || p.type == "Q") //Straight movements for Queen and Rook
            {
                for (int i = 0; i < 8; i++) //+x
                {
                    playable[i, 0] = loc[0] + 1 + i;
                }
                for (int i = 8; i < 16; i++) //-x
                {
                    playable[i, 0] = loc[0] + 7 - i;
                }
                for (int i = 16; i < 24; i++)//-y forward
                {
                    playable[i, 1] = loc[1] + 15 - i;
                }
                for (int i = 24; i < 32; i++)//+y backward
                {
                    playable[i, 1] = loc[1] - 23 + i;
                }
            }
            else if (p.type == "K") //King's movements
            {
                playable[56, 0] = loc[0] + 1;
                playable[56, 1] = loc[1] - 1;
                playable[48, 0] = loc[0] - 1;
                playable[48, 1] = loc[1] + 1;
                playable[40, 0] = loc[0] - 1;
                playable[40, 1] = loc[1] - 1;
                playable[32, 0] = loc[0] + 1;
                playable[32, 1] = loc[1] + 1;
                playable[24, 1] = loc[1] + 1;
                playable[16, 1] = loc[1] - 1;
                playable[8, 0] = loc[0] - 1;
                playable[0, 0] = loc[0] + 1;
                if (p.color && !p.played) //Castling for black king
                {
                    if (board[7, 0].type == "R" && board[7, 0].color && !board[7, 0].played && !board[6, 0].control && !board[5, 0].control) playable[1, 0] = loc[0] + 2; //Kingside 
                    if (board[0, 0].type == "R" && board[0, 0].color && !board[0, 0].played && !board[1, 0].control && !board[2, 0].control && !board[3, 0].control) playable[2, 0] = loc[0] - 2;//Queenside
                }
                else if (!p.color && !p.played) //Castling for white king
                {
                    if (board[0, 7].type == "R" && !board[0, 7].color && !board[0, 7].played && !board[1, 7].control && !board[2, 7].control && !board[3, 7].control) playable[2, 0] = loc[0] - 2;//Queenside
                    if (board[7, 7].type == "R" && !board[7, 7].color && !board[7, 7].played && !board[6, 7].control && !board[5, 7].control) playable[1, 0] = loc[0] + 2; //Kingside
                }
            }
            else if (p.type == "N") //Knight's movements
            {
                playable[0, 0] = loc[0] + 1;
                playable[0, 1] = loc[1] - 2;
                playable[1, 0] = loc[0] - 1;
                playable[1, 1] = loc[1] - 2;
                playable[2, 0] = loc[0] + 1;
                playable[2, 1] = loc[1] + 2;
                playable[3, 0] = loc[0] - 1;
                playable[3, 1] = loc[1] + 2;
                playable[4, 0] = loc[0] + 2;
                playable[4, 1] = loc[1] - 1;
                playable[5, 0] = loc[0] - 2;
                playable[5, 1] = loc[1] - 1;
                playable[6, 0] = loc[0] + 2;
                playable[6, 1] = loc[1] + 1;
                playable[7, 0] = loc[0] - 2;
                playable[7, 1] = loc[1] + 1;
            }
            if (p.type == "B" || p.type == "Q") //Cross Movements
            {
                for (int i = 32; i < 40; i++) //upper right cross
                {
                    playable[i, 0] = loc[0] - 31 + i;
                    playable[i, 1] = loc[1] - 31 + i;
                }
                for (int i = 40; i < 48; i++) //lower left cross
                {
                    playable[i, 0] = loc[0] + 39 - i;
                    playable[i, 1] = loc[1] + 39 - i;
                }
                for (int i = 48; i < 56; i++)//lower right cross
                {
                    playable[i, 0] = loc[0] + 47 - i;
                    playable[i, 1] = loc[1] - 47 + i;
                }
                for (int i = 56; i < 64; i++)//upper left cross
                {
                    playable[i, 0] = loc[0] - 55 + i;
                    playable[i, 1] = loc[1] + 55 - i;
                }
            }
            //Movement control part
            bool alert = false;
            if (p.type != "N" && p.type != "P") // determining the limit of movements except pawn and knight
            {
                for (int s = 0; s < 64; s++)
                {
                    if (s % 8 == 0) //resetting alert when the movement changes direction
                    {
                        alert = false;
                    }
                    if (playable[s, 0] != -1 || playable[s, 1] != -1)
                    {
                        if ((s == 1 || s == 2) && p.type == "K") continue; //Except King's castling movement, we have already checked it 
                        if (playable[s, 0] > 7 || playable[s, 1] > 7 || playable[s, 0] < 0 || playable[s, 1] < 0)
                        {
                            alert = true;
                        }
                        else if (board[playable[s, 0], playable[s, 1]].control && !alert) //Gideceği yerde taş varsa alerti true yapar
                        {
                            alert = true;
                            if (king) //Taşın korunup korunmadığını anlamak için (kral kontrolü için)
                            {
                                if (board[playable[s, 0], playable[s, 1]].color == p.color)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (board[playable[s, 0], playable[s, 1]].color != p.color) //Eğer gideceği yerde farklı renk taş varsa gidebilmesini sağlamak için continue yapar
                                {
                                    continue;
                                }
                            }
                        }
                        if (alert) //eğer alert true ise resetlenene kadar tüm hamleleri sıfırlar (-1)
                        {
                            playable[s, 0] = -1;
                            playable[s, 1] = -1;
                        }
                    }
                }
            }
            else if (p.type == "N" && !king) //Movement control for knight
            {
                for (int i = 0; i < 8; i++)
                {
                    if (playable[i, 0] < 0 || playable[i, 1] < 0 || playable[i, 0] > 7 || playable[i, 1] > 7 || (board[playable[i, 0], playable[i, 1]].color == p.color && board[playable[i, 0], playable[i, 1]].control)) //If there is a stone in its destination, it prevents movement
                    {
                        playable[i, 0] = -1;
                        playable[i, 1] = -1;
                    }
                }
            }
            else if (p.type == "P" && !king) //Movement control for Pawn
            {
                if (playable[0, 1] < 0 || playable[0, 1] > 7 || playable[0, 0] < 0 || playable[0, 0] > 7
                    || board[playable[0, 0], playable[0, 1]].control) //Checks for double movement
                {
                    playable[0, 0] = -1;
                    playable[0, 1] = -1;
                    playable[1, 0] = -1;
                    playable[1, 1] = -1;
                }
                else if (board[playable[1, 0], playable[1, 1]].control || (playable[1, 0] < 0
                    || playable[1, 0] > 7 || playable[1, 1] > 7 || playable[1, 1] < 0)) //Checks for single movement
                {
                    playable[1, 0] = -1;
                    playable[1, 1] = -1;
                }
                for (int i = 32; i <= 56; i += 8)
                {
                    if (playable[i, 1] < 0 || playable[i, 1] > 7 || playable[i, 0] < 0
                        || playable[i, 0] > 7 || !(board[playable[i, 0], playable[i, 1]].control
                        && (board[playable[i, 0], playable[i, 1]].color != p.color))) //Checks for capture movements
                    {
                        playable[i, 0] = -1;
                        playable[i, 1] = -1;
                    }
                }
            }
            else if (king) //Check for King Control (crossing the board)
            {
                for (int i = 0; i <= 56; i++)
                {
                    if (playable[i, 0] != -1 || playable[i, 1] != -1)
                    {
                        if (playable[i, 1] < 0 || playable[i, 1] > 7 || playable[i, 0] < 0 || playable[i, 0] > 7)
                        {
                            playable[i, 0] = -1;
                            playable[i, 1] = -1;
                        }
                    }
                }
            }
            if (p.type == "K") //Movement control for King
            {
                for (int i = 0; i < 64; i++)
                {
                    if (playable[i, 0] != -1 || playable[i, 1] != -1)
                    {
                        for (int c = 0; c < 8; c++)
                        {
                            for (int r = 0; r < 8; r++)
                            {
                                if (board[c, r].control && (board[c, r].color != p.color) && board[c, r].type != "K") //Tahtadaki tüm karşı renkteki taşların gidebileceği hamlelere göre şahın gidebileceği yerleri engeller
                                {
                                    int[] location = { c, r };
                                    int[,] array = Place_Generator(board[c, r], location, board, true);
                                    for (int a = 0; a < 64; a++)
                                    {
                                        if (array[a, 0] != -1 || array[a, 1] != -1)
                                        {
                                            if (i == 1) //For castling(SHORT)
                                            {
                                                if ((loc[0] + 2 == array[a, 0] || loc[0] + 1 == array[a, 0] || loc[0] == array[a, 0]) && loc[1] == array[a, 1])
                                                {
                                                    playable[i, 0] = -1;
                                                    playable[i, 1] = -1;
                                                }
                                            }
                                            if (i == 2)//For castling(LONG)
                                            {
                                                if ((loc[0] - 2 == array[a, 0] || loc[0] - 1 == array[a, 0] || loc[0] == array[a, 0]) && loc[1] == array[a, 1])
                                                {
                                                    playable[i, 0] = -1;
                                                    playable[i, 1] = -1;
                                                }
                                            }
                                            if (playable[i, 0] == array[a, 0] && playable[i, 1] == array[a, 1]) //Other movements
                                            {
                                                playable[i, 0] = -1;
                                                playable[i, 1] = -1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 64; i++) //Resetting untouched movements
            {
                if (playable[i, 0] == loc[0] && playable[i, 1] == loc[1])
                {
                    playable[i, 0] = -1;
                    playable[i, 1] = -1;
                }
            }
            if ((threatened_b && p.color && !AI_mode) || (threatened_w && !p.color)) //Şah çekilme sırasında hamlenin şah çekmesini önlediğini kontrol etme 
            {
                for (int i = 0; i < 64; i++)
                {
                    if (playable[i, 0] != -1 || playable[i, 1] != -1)
                    {
                        int[] mov = { loc[0], loc[1], playable[i, 0], playable[i, 1] };
                        if (!is_threat_blocked(p.color, board, mov))
                        {
                            playable[i, 0] = -1;
                            playable[i, 1] = -1;
                        }
                    }
                }
            }
            return playable;
        }
        static Pieces define(bool C, string T) //Function that defines pieces
        {
            Pieces p;
            p.type = T;
            p.color = C;
            p.control = true;
            p.played = false;
            return p;
        }
        struct Pieces //Struct for pieces 
        {
            public bool played;
            public string type;
            public bool color;
            public bool control;
        }
        //All the public variables
        public static bool threatened_w; //threat boolean for white 
        public static bool threatened_b; //threat boolean for black
        public static int[] move = new int[4]; //move array for notation (old x, oldy, new x, new y)
        public static string[] notat = new string[3]; //String that keeps notation (n. not1 not2) 
        public static int en_passant_turn = 0; //To limit en passant's tour 
        public static bool en_passant_color = false; //Mevcut en passant yapabilen renk
        public static int[] en_Passant_loc = { 0, 0 }; //En passant yapılabilecek konum
        public static bool en_passant_bool = false; //En passant yapılıp yapılamadığını tutan boolean
        public static bool notation_mode = false; //N'e basıldığında notasyon almamızı sağlayan anlık mod
        public static bool AI_mode = false; //The mode that allows us to play against artificial intelligence
        public static bool demo_mode = false; //The mode that allows us to play demo mode
        public static bool continue_mode = false; //The mode that allows us to continue from where we left
        static void Main(string[] args)
        {
            bool winner_color; //To determine winner color
            int checked_turn = 0; //Sadece hamle yapıldığında tehdit durumunu kontrol etmek için
            string[,] notation = new string[99, 2];
            do
            {
                Console.Clear(); //To clear bord
                int cx = 4, cy = 5;
                ConsoleKeyInfo cki;
                Console.ForegroundColor = ConsoleColor.Yellow; //To paint the board yellow
                Console.WriteLine("--------------------------");
                Console.WriteLine("|                        |");
                Console.WriteLine("|       Chess  Game      |");
                Console.WriteLine("|        MAIN MENU       |");
                Console.WriteLine("|                        |");
                Console.WriteLine("|            AI          |");
                Console.WriteLine("|                        |");
                Console.WriteLine("|         New Game       |");
                Console.WriteLine("|                        |");
                Console.WriteLine("|         Continue       |");
                Console.WriteLine("|                        |");
                Console.WriteLine("|        Demo Mode       |");
                Console.WriteLine("|                        |");
                Console.WriteLine("|           Exit         |");
                Console.WriteLine("|                        |");
                Console.WriteLine("-------------------------");
                while (true) //Loop of Main Menu
                {
                    if (cy == 5)
                        Console.ForegroundColor = ConsoleColor.Green; //To paint the selected part green
                    else
                        Console.ForegroundColor = ConsoleColor.Red; //To paint the other parts red
                    Console.SetCursorPosition(4, 5);
                    Console.WriteLine("        AI          ");
                    if (cy == 7)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(4, 7);
                    Console.WriteLine("     New Game       ");
                    if (cy == 9)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(4, 9);
                    Console.WriteLine("     Continue       ");
                    if (cy == 11)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(4, 11);
                    Console.WriteLine("     Demo Mode       ");
                    if (cy == 13)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(4, 13);
                    Console.WriteLine("       Exit         ");
                    Console.SetCursorPosition(cx, cy);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(">");
                    Console.SetCursorPosition(cx + 19, cy);
                    Console.Write("<");

                    Console.SetWindowSize(26, 16);
                    Console.SetCursorPosition(cx, cy);
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.UpArrow && cy > 5) cy -= 2; //Cursor movements in the menu
                    if (cki.Key == ConsoleKey.DownArrow && cy < 13) cy += 2;
                    if (cki.Key == ConsoleKey.Enter) break;
                }
                if (cy == 13)
                {
                    break;
                }
                else if (cy == 11 && File.Exists("notation.txt"))
                {
                    demo_mode = true; //Demo mode activation
                    cy = 7;
                }
                else if (cy == 9 && File.Exists("gamesave.txt"))
                {
                    continue_mode = true; //Continue mode activation
                    cy = 7;
                }
                else if (cy == 5)
                {
                    AI_mode = true; //AI mode activation
                    cy = 7;

                }
                if (cy == 7) //Main Game
                {
                    if (!AI_mode && !continue_mode && !demo_mode && !notation_mode) //To create new file on new game option
                    {
                        StreamWriter F = File.CreateText("gamesave.txt");
                        F.Close();
                    }
                    string letters = "abcdefgh"; //Notasyon tutarken indexten harfe çevirmek için
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetWindowSize(80, 25); //To fix the size of the board
                    bool flag; //to check the applicability of the move
                    int[] prev_loc = new int[2]; //Prev location of piece on the cursor
                    int turn = 2; //Turn of the game
                    Pieces pattern; //Pattern of piece in the cursor
                    pattern.type = "a"; //Assigning random value to pattern
                    pattern.color = false; //Assigning null value to pattern
                    pattern.control = false; //Assigning null value to pattern
                    pattern.played = false; //Assigning null value to pattern
                    string cursor = ""; //Type of piece on the cursor
                    int cursorx = 10, cursory = 5; //Cursor's locations
                    ConsoleKeyInfo key;
                    Pieces[,] board = new Pieces[8, 8];//Main board
                    //identification of whites
                    for (int i = 0; i < 8; i++)
                    {
                        board[i, 6] = define(false, "P");
                    }
                    string queue = "RNBQKBNR";
                    int x = 0;
                    foreach (char c in queue)
                    {
                        board[x, 7] = define(false, $"{c}");
                        x++;
                    }
                    //identification of blacks
                    for (int i = 0; i < 8; i++)
                    {
                        board[i, 1] = define(true, "P");
                    }
                    x = 0;
                    foreach (char c in queue)
                    {
                        board[x, 0] = define(true, $"{c}");
                        x++;
                    }
                    string[] save = new string[99];
                    string lne;
                    int k = 1;
                    if (continue_mode)
                    {
                        StreamReader file = new StreamReader("gamesave.txt");
                        while ((lne = file.ReadLine()) != null && lne != "")
                        {
                            save[k] = lne;
                            k++;
                        }
                        file.Close();
                        File.Delete("gamesave.txt");
                    }
                    else if (demo_mode)
                    {
                        StreamReader file = new StreamReader("notation.txt");
                        while ((lne = file.ReadLine()) != null && lne != "")
                        {
                            save[k] = lne;
                            k++;
                        }
                        file.Close();
                    }
                    while (true) //Main Game Loop
                    {
                        StreamWriter f = File.AppendText("gamesave.txt");
                        if (AI_mode) f.Close(); //Yapay modda yazdırma işlemi yapılmaz bu nedenle dosya kapatılır
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.SetCursorPosition(50, 1); //Print the number of laps
                        Console.Write("turn:");
                        Console.SetCursorPosition(55, 1);
                        Console.Write(turn / 2);
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine(" +-----------------------------+");
                        Console.WriteLine("8|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("7|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("6|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("5|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("4|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("3|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("2|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" |                             |");
                        Console.WriteLine("1|.   .   .   .   .   .   .   .|");
                        Console.WriteLine(" +-----------------------------+");
                        Console.WriteLine("  a   b   c   d   e   f   g   h ");
                        Console.SetCursorPosition(33, 12); //Printing Instructions
                        Console.Write("BackSpace: Drop");
                        Console.SetCursorPosition(33, 13);
                        Console.Write("Enter: Select");
                        Console.SetCursorPosition(33, 14);
                        Console.Write("H: Hint");
                        Console.SetCursorPosition(33, 15);
                        Console.Write("N: Notation");
                        Console.SetCursorPosition(33, 16);
                        Console.Write("Arrow Keys: Move");
                        for (int i = 0; i < 8; i++) //Printing the current board
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (board[i, j].control)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    if (board[i, j].color)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                    }
                                    Console.SetCursorPosition(i * 4 + 2, j * 2 + 1);
                                    Console.Write(board[i, j].type);
                                }
                            }
                        }
                        if (AI_mode && turn % 2 == 1) //Main AI Part
                        {
                            Console.SetCursorPosition(40, 5);
                            Console.Write("AI Thinking...");
                            int[] mov = new int[5]; //old_x, old_y, new_x,new,y, p
                            int[] logic_mov = new int[5]; //old_x, old_y, new_m, p
                            logic_mov[4] = -1; //The most logical move's point that AI can play
                            int point;
                            bool first = true;
                            for (int i = 0; i < 8; i++) //Searches on the entire board
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if (board[i, j].control && board[i, j].color)//Looking for own pieces
                                    {
                                        int[] location = { i, j };
                                        int[,] array = Place_Generator(board[i, j], location, board, false); //Controls all the moves that chosen piece can make
                                        for (int a = 0; a < 64; a++)
                                        {
                                            if (array[a, 0] != -1 || array[a, 1] != -1)
                                            {
                                                if (board[array[a, 0], array[a, 1]].type == "P" && board[array[a, 0], array[a, 1]].control && !board[array[a, 0], array[a, 1]].color) point = 15;
                                                else if (board[array[a, 0], array[a, 1]].type == "R" && board[array[a, 0], array[a, 1]].control && !board[array[a, 0], array[a, 1]].color) point = 75;
                                                else if (board[array[a, 0], array[a, 1]].type == "N" && board[array[a, 0], array[a, 1]].control && !board[array[a, 0], array[a, 1]].color) point = 45;
                                                else if (board[array[a, 0], array[a, 1]].type == "B" && board[array[a, 0], array[a, 1]].control && !board[array[a, 0], array[a, 1]].color) point = 45;
                                                else if (board[array[a, 0], array[a, 1]].type == "Q" && board[array[a, 0], array[a, 1]].control && !board[array[a, 0], array[a, 1]].color) point = 135;
                                                else if (board[array[a, 0], array[a, 1]].type == "K" && board[array[a, 0], array[a, 1]].control && !board[array[a, 0], array[a, 1]].color) point = 999;
                                                else point = 0;
                                                mov[0] = i;
                                                mov[1] = j;
                                                mov[2] = array[a, 0];
                                                mov[3] = array[a, 1];
                                                mov[4] = point + evaluation(mov, board[i, j].color, board[i, j].type) + minimise(0, mov, board, true); //Evalute all possible moves and return a point
                                                if (first) //First assiging
                                                {
                                                    logic_mov[0] = i;
                                                    logic_mov[1] = j;
                                                    logic_mov[2] = array[a, 0];
                                                    logic_mov[3] = array[a, 1];
                                                    logic_mov[4] = mov[4];
                                                    first = false;
                                                }
                                                if (mov[4] > logic_mov[4]) //Seeks the most logical move
                                                {
                                                    logic_mov[0] = i;
                                                    logic_mov[1] = j;
                                                    logic_mov[2] = array[a, 0];
                                                    logic_mov[3] = array[a, 1];
                                                    logic_mov[4] = mov[4];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //Plays the most logical move
                            Pieces pattern_AI;
                            pattern_AI = board[logic_mov[0], logic_mov[1]];
                            board[logic_mov[0], logic_mov[1]].control = false;
                            board[logic_mov[0], logic_mov[1]].type = "";
                            board[logic_mov[2], logic_mov[3]] = pattern_AI;
                            if (threatened_b) //Delete if there is a threat
                            {
                                Console.SetCursorPosition(50, 4);
                                Console.Write("                  ");
                                threatened_b = false;
                            }
                            if (logic_mov[3] == 7 && pattern_AI.type == "P") //Promotion of AI
                            {
                                board[logic_mov[2], logic_mov[3]].type = "Q";
                            }
                            turn++;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.SetCursorPosition(logic_mov[2] * 4 + 2, logic_mov[3] * 2 + 1);
                            Console.Write(board[logic_mov[2], logic_mov[3]].type);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(logic_mov[0] * 4 + 2, logic_mov[1] * 2 + 1);
                            Console.Write(".");
                            Console.SetCursorPosition(40, 5);
                            Console.Write("                ");
                            if (board[logic_mov[2], logic_mov[3]].type == "K") board[logic_mov[2], logic_mov[3]].played = true;
                            if (board[logic_mov[2], logic_mov[3]].type == "K" && (logic_mov[2] - logic_mov[0]) == 2) //Castling of AI
                            {
                                if (board[logic_mov[2], logic_mov[3]].color)
                                {
                                    pattern_AI = board[7, 0];
                                    board[7, 0].control = false;
                                    board[7, 0].type = "";
                                    board[5, 0] = pattern_AI;
                                }
                                else
                                {
                                    pattern_AI = board[7, 7];
                                    board[7, 7].control = false;
                                    board[7, 7].type = "";
                                    board[5, 7] = pattern_AI;
                                }
                            }
                            else if (board[logic_mov[2], logic_mov[3]].type == "K" && (logic_mov[2] - logic_mov[0]) == -2)
                            {
                                if (board[logic_mov[2], logic_mov[3]].color)
                                {
                                    pattern_AI = board[0, 0];
                                    board[0, 0].control = false;
                                    board[0, 0].type = "";
                                    board[3, 0] = pattern_AI;
                                }
                                else
                                {
                                    pattern_AI = board[0, 7];
                                    board[0, 7].control = false;
                                    board[0, 7].type = "";
                                    board[3, 7] = pattern_AI;
                                }
                            }
                        }
                        if (continue_mode) //If continue mode is true then makes all the moves in the array
                        {
                            string line;
                            if ((line = save[turn / 2]) != null && line != "") //If the array isn't empty
                            {
                                notat = line.Split(" "); //Notasyonları elde etmek için boşluklardan diziyi ayırır
                                if ((notat.Length == 2) && turn % 2 == 1) //Eğer satırın sonuan geldiyse durdurur
                                {
                                    continue_mode = false;
                                    f.Close();
                                    continue;
                                }
                                if (turn % 2 == 0)
                                {
                                    move = notation_reader(notat[1], false, board);
                                }
                                else move = notation_reader(notat[2], true, board);
                                cursor = board[move[0], move[1]].type;
                                pattern = board[move[0], move[1]];
                                prev_loc[0] = move[0];
                                prev_loc[1] = move[1];
                            }
                            else //Eğer satırın sonuan geldiyse durdurur
                            {
                                continue_mode = false;
                            }
                        }
                        if (demo_mode == true) //Continue modun aynısı fakat her hamle için space basmamız gerekir ve esc tuşu ile istediğimiz zaman çıkabiliriz ayrıca farklı bir dosyadan okur
                        {
                            string line;
                            if ((save[turn / 2]) != null && save[turn / 2] != "")
                            {
                                Console.SetCursorPosition(35, 6);
                                Console.Write("ESC to quit");
                                key = Console.ReadKey(true);
                                if (key.Key == ConsoleKey.Spacebar)
                                {
                                    line = save[turn / 2];
                                    notat = line.Split(" ");
                                    if ((notat.Length == 2) && turn % 2 == 1)
                                    {
                                        continue_mode = false;
                                        f.Close();
                                        continue;
                                    }
                                    if (turn % 2 == 0)
                                    {
                                        move = notation_reader(notat[1], false, board);
                                    }
                                    else move = notation_reader(notat[2], true, board);
                                    if (move[0] == -1 || move[1] == -1 || move[2] == -1 || move[3] == -1)
                                    {
                                        Console.SetCursorPosition(50, 4);
                                        Console.Write("Invalid Move");
                                        f.Close();
                                        continue;
                                    }
                                    Console.SetCursorPosition(50, 4);
                                    Console.Write("             ");
                                    cursor = board[move[0], move[1]].type;
                                    pattern = board[move[0], move[1]];
                                    prev_loc[0] = move[0];
                                    prev_loc[1] = move[1];
                                }
                                else if (key.Key == ConsoleKey.Escape)
                                {
                                    Console.SetCursorPosition(35, 6);
                                    Console.Write("            ");
                                    demo_mode = false;
                                }
                            }
                            else
                            {
                                demo_mode = false;
                            }

                        }
                        //Cursor movement
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.SetCursorPosition(cursorx, cursory);
                        Console.Write(cursor);
                        if ((!AI_mode || turn % 2 == 0) && !(continue_mode || demo_mode || notation_mode))
                        {
                            key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.UpArrow && cursory > 2) cursory -= 2;
                            else if (key.Key == ConsoleKey.DownArrow && cursory < 14) cursory += 2;
                            else if (key.Key == ConsoleKey.LeftArrow && cursorx > 2) cursorx -= 4;
                            else if (key.Key == ConsoleKey.RightArrow && cursorx < 30) cursorx += 4;
                        }
                        else if (AI_mode)
                        {
                            continue;
                        }
                        else
                        {
                            key = new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false); //Automatic enter pressing part
                        }
                        if (key.Key == ConsoleKey.Enter)
                        {
                            int column;
                            int row;
                            if (continue_mode || demo_mode || notation_mode) //In other modes row and column are avabile in move array
                            {
                                row = move[3];
                                column = move[2];
                            }
                            else //In normal mode row and column are equal to current location of cursor
                            {
                                row = (cursory - 1) / 2;
                                column = (cursorx - 2) / 4;
                            }
                            if (continue_mode || demo_mode || notation_mode || (cursor != "" && !(column == prev_loc[0] && row == prev_loc[1])) && (!AI_mode || turn % 2 == 0)) //Önceki konumu değilse, yapay zeka modunda sıra yapay zekada değisle veya cursor boş değilse 
                            {
                                int[,] array = Place_Generator(pattern, prev_loc, board, false);
                                flag = false;
                                for (int i = 0; i < 64; i++)
                                {
                                    if ((array[i, 0] == column && array[i, 1] == row))
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        Console.SetCursorPosition(50, 4);
                                        Console.Write("Invalid Move");
                                        continue;
                                    }
                                    if ((board[column, row].control ? (pattern.color != board[column, row].color) : true) && flag)
                                    {
                                        if (!AI_mode)
                                        {
                                            if (turn % 2 == 0)
                                            {
                                                f.Write($"{turn / 2}.");
                                                Console.SetCursorPosition(50, 5 + (turn / 2));
                                                Console.Write($"{turn / 2}.");
                                            }
                                            string symbol = pattern.type;
                                            if (symbol == "P")
                                            {
                                                symbol = "";
                                            }
                                            if ((row == 0 || row == 7) && pattern.type == "P") //promote
                                            {

                                            }
                                            else if (board[column, row].control)
                                            {
                                                if (symbol == "")
                                                {
                                                    f.Write($" {letters[prev_loc[0]]}x{letters[column]}{8 - row}");
                                                    if (turn % 2 == 0)
                                                    {
                                                        Console.SetCursorPosition(55, 5 + (turn / 2));
                                                        Console.Write($" {letters[prev_loc[0]]}x{letters[column]}{8 - row}");
                                                    }
                                                    else
                                                    {
                                                        Console.SetCursorPosition(65, 5 + (turn / 2));
                                                        Console.Write($" {letters[prev_loc[0]]}x{letters[column]}{8 - row}");
                                                    }
                                                }
                                                else
                                                {
                                                    f.Write($" {symbol}x{letters[column]}{8 - row}");
                                                    if (turn % 2 == 0)
                                                    {
                                                        Console.SetCursorPosition(55, 5 + (turn / 2));
                                                        Console.Write($" {symbol}x{letters[column]}{8 - row}");
                                                    }
                                                    else
                                                    {
                                                        Console.SetCursorPosition(65, 5 + (turn / 2));
                                                        Console.Write($" {symbol}x{letters[column]}{8 - row}");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (i == 3 && symbol == "")
                                                {
                                                    f.Write($" {letters[prev_loc[0]]}x{letters[column]}{8 - row}e.p.");
                                                    if (turn % 2 == 0)
                                                    {
                                                        Console.SetCursorPosition(55, 5 + (turn / 2));
                                                        Console.Write($" {letters[prev_loc[0]]}x{letters[column]}{8 - row}e.p.");
                                                    }
                                                    else
                                                    {
                                                        Console.SetCursorPosition(65, 5 + (turn / 2));
                                                        Console.Write($" {letters[prev_loc[0]]}x{letters[column]}{8 - row}e.p.");
                                                    }
                                                }
                                                else if (!(symbol == "K" && (i == 1 || i == 2)))
                                                {
                                                    f.Write($" {symbol}{letters[column]}{8 - row}");
                                                    if (turn % 2 == 0)
                                                    {
                                                        Console.SetCursorPosition(55, 5 + (turn / 2));
                                                        Console.Write($" {symbol}{letters[column]}{8 - row}");
                                                    }
                                                    else
                                                    {
                                                        Console.SetCursorPosition(65, 5 + (turn / 2));
                                                        Console.Write($" {symbol}{letters[column]}{8 - row}");
                                                    }
                                                }
                                            }
                                        }
                                        board[column, row] = pattern;
                                        pattern.control = false;
                                        cursor = "";
                                        board[prev_loc[0], prev_loc[1]].type = "";
                                        board[prev_loc[0], prev_loc[1]].control = false;
                                        Pieces pattern2;
                                        if (threatened_w && turn % 2 == 0)
                                        {
                                            Console.SetCursorPosition(50, 4);
                                            Console.Write("                  ");
                                            threatened_w = false;
                                        }
                                        else if (threatened_b && turn % 2 == 1)
                                        {
                                            Console.SetCursorPosition(50, 4);
                                            Console.Write("                  ");
                                            threatened_b = false;
                                        }
                                        if (pattern.type == "P" && i == 1)
                                        {

                                            if ((column + 1 < 8 && board[column + 1, row].type == "P" && board[column + 1, row].color != board[column, row].color) || (column - 1 >= 0 && board[column - 1, row].type == "P" && board[column - 1, row].color != board[column, row].color))
                                            {
                                                en_passant_turn = turn + 1;
                                                en_passant_bool = true;
                                                en_Passant_loc[0] = column;
                                                en_passant_color = pattern.color;
                                                if (pattern.color) en_Passant_loc[1] = row - 1;
                                                else if (!pattern.color) en_Passant_loc[1] = row + 1;
                                            }
                                        }
                                        if (pattern.type == "P" && i == 3)
                                        {

                                            if (en_passant_color)
                                            {
                                                board[en_Passant_loc[0], en_Passant_loc[1] + 1].type = "";
                                                board[en_Passant_loc[0], en_Passant_loc[1] + 1].control = false;
                                            }
                                            else
                                            {
                                                board[en_Passant_loc[0], en_Passant_loc[1] - 1].type = "";
                                                board[en_Passant_loc[0], en_Passant_loc[1] - 1].control = false;
                                            }
                                        }
                                        if (pattern.type == "K" && i == 1)
                                        {
                                            if (pattern.color)
                                            {
                                                if (!AI_mode)
                                                {
                                                    f.Write(" 0-0");
                                                    Console.SetCursorPosition(65, 5 + (turn / 2));
                                                    Console.Write(" 0-0");
                                                }
                                                pattern2 = board[7, 0];
                                                board[7, 0].control = false;
                                                board[7, 0].type = "";
                                                board[5, 0] = pattern2;
                                            }
                                            else
                                            {
                                                if (!AI_mode)
                                                {
                                                    f.Write(" 0-0");
                                                    if (turn % 2 == 0)
                                                        Console.SetCursorPosition(55, 5 + (turn / 2));
                                                    Console.Write(" 0-0");
                                                }
                                                pattern2 = board[7, 7];
                                                board[7, 7].control = false;
                                                board[7, 7].type = "";
                                                board[5, 7] = pattern2;
                                            }
                                        }
                                        else if (pattern.type == "K" && i == 2)
                                        {
                                            if (pattern.color)
                                            {
                                                if (!AI_mode)
                                                {
                                                    f.Write(" 0-0-0");
                                                    Console.SetCursorPosition(65, 5 + (turn / 2));
                                                    Console.Write(" 0-0-0");
                                                }
                                                pattern2 = board[0, 0];
                                                board[0, 0].control = false;
                                                board[0, 0].type = "";
                                                board[3, 0] = pattern2;
                                            }
                                            else
                                            {
                                                if (!AI_mode)
                                                {
                                                    f.Write(" 0-0-0");
                                                    Console.SetCursorPosition(55, 5 + (turn / 2));
                                                    Console.Write(" 0-0-0");
                                                }
                                                pattern2 = board[0, 7];
                                                board[0, 7].control = false;
                                                board[0, 7].type = "";
                                                board[3, 7] = pattern2;
                                            }
                                        }
                                        if (pattern.type == "K" || pattern.type == "R") board[column, row].played = true;
                                        if ((row == 0 || row == 7) && board[column, row].type == "P")
                                        {
                                            while (true)
                                            {
                                                if (!continue_mode && !demo_mode && !notation_mode)
                                                {
                                                    string pieces = "R N B Q K";
                                                    Console.SetCursorPosition(35, 1);
                                                    Console.Write("Promotion");
                                                    Console.SetCursorPosition(35, 2);
                                                    Console.Write(pieces);
                                                    key = Console.ReadKey();
                                                    if (pieces.Contains(Convert.ToChar(key.Key)))
                                                    {
                                                        board[column, row].type = Convert.ToString(key.Key);
                                                        Console.SetCursorPosition(35, 1);
                                                        Console.Write("                   ");
                                                        Console.SetCursorPosition(35, 2);
                                                        Console.Write("               ");
                                                        if (!AI_mode)
                                                        {
                                                            f.Write($" {letters[column]}{8 - row}{board[column, row].type}");
                                                            if (turn % 2 == 0)
                                                            {
                                                                Console.SetCursorPosition(55, 5 + (turn / 2));
                                                                Console.Write($" {letters[column]}{8 - row}{board[column, row].type}");
                                                            }
                                                            else
                                                            {
                                                                Console.SetCursorPosition(65, 5 + (turn / 2));
                                                                Console.Write($" {letters[column]}{8 - row}{board[column, row].type}");
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    string type;
                                                    if (turn % 2 == 0)
                                                    {
                                                        type = $"{notat[1][2]}";
                                                    }
                                                    else
                                                    {
                                                        type = $"{notat[2][2]}";
                                                    }
                                                    board[column, row].type = type;
                                                    f.Write($" {letters[column]}{8 - row}{board[column, row].type}");
                                                    if (turn % 2 == 0)
                                                    {
                                                        Console.SetCursorPosition(55, 5 + (turn / 2));
                                                        Console.Write($" {letters[column]}{8 - row}{board[column, row].type}");
                                                    }
                                                    else
                                                    {
                                                        Console.SetCursorPosition(65, 5 + (turn / 2));
                                                        Console.Write($" {letters[column]}{8 - row}{board[column, row].type}");
                                                    }
                                                    break;
                                                }

                                            }
                                        }
                                        if (threat(false, board) || threat(true, board) && !AI_mode)
                                        {
                                            f.Write("+");
                                            Console.Write("+");
                                        }
                                        if (turn % 2 == 1 && !AI_mode)
                                        {
                                            f.WriteLine("");
                                        }
                                        notation_mode = false;
                                        turn++;
                                        Console.SetCursorPosition(50, 4);
                                        Console.Write("            ");
                                        break;
                                    }
                                    else
                                    {
                                        Console.SetCursorPosition(50, 4);
                                        Console.Write("Invalid Move");
                                    }
                                }
                            }
                            else if (cursor == "" && board[column, row].control && ((board[column, row].color == true && turn % 2 == 1) || (board[column, row].color == false && turn % 2 == 0)))
                            {
                                bool first_check = false;
                                if (!threat(pattern.color, board))
                                {
                                    first_check = true;
                                }
                                prev_loc[0] = column;
                                prev_loc[1] = row;
                                pattern = board[column, row];
                                board[column, row].control = false;
                                if (threat(pattern.color, board) && first_check)
                                {
                                    board[column, row].control = true;
                                    pattern.control = false;
                                    Console.SetCursorPosition(50, 4);
                                    Console.Write("Your King Is in Check");
                                }
                                else
                                {
                                    cursor = $"{board[column, row].type}";
                                }
                            }
                        }
                        if (key.Key == ConsoleKey.Backspace && !(cursor == ""))
                        {
                            board[prev_loc[0], prev_loc[1]] = pattern;
                            cursor = "";
                            pattern.control = false;
                        }
                        if (key.Key == ConsoleKey.N && !AI_mode)
                        {
                            Console.SetCursorPosition(50, 3);
                            Console.Write("Enter Notation: ");
                            string not = Console.ReadLine();
                            if (turn % 2 == 0)
                            {
                                move = notation_reader(not, false, board);
                            }
                            else
                            {
                                move = notation_reader(not, true, board);
                            }
                            if (move[0] == -1 || move[0] == -1 || move[0] == -1 || move[0] == -1)
                            {
                                Console.SetCursorPosition(50, 4);
                                Console.WriteLine("Invalid Move");
                                Console.SetCursorPosition(50, 3);
                                Console.WriteLine("                                            ");
                                f.Close();
                                continue;
                            }
                            cursor = board[move[0], move[1]].type;
                            pattern = board[move[0], move[1]];
                            prev_loc[0] = move[0];
                            prev_loc[1] = move[1];
                            Console.SetCursorPosition(50, 3);
                            Console.WriteLine("                                            ");
                            notation_mode = true;
                            f.Close();
                            continue;
                        }
                        if (key.Key == ConsoleKey.H) //hint
                        {
                            int[,] arr = new int[50, 4];
                            if (turn % 2 == 0)
                            {
                                arr = hint(false, board);
                            }
                            else
                            {
                                arr = hint(true, board);
                            }
                            for (int i = 0; i < 50; i++)
                            {
                                if (arr[i, 0] != -1 && arr[i, 1] != -1 && arr[i, 2] != -1 && arr[i, 3] != -1)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.SetCursorPosition(arr[i, 2] * 4 + 2, arr[i, 3] * 2 + 1);
                                    Console.Write("*");
                                    Console.SetCursorPosition(arr[i, 0] * 4 + 2, arr[i, 1] * 2 + 1);
                                    Console.Write(board[arr[i, 0], arr[i, 1]].type);
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                }
                            }
                            Console.SetCursorPosition(35, 4);
                            Console.Write("Pressed H!");
                            Console.ReadKey(true);
                            Console.SetCursorPosition(35, 4);
                            Console.Write("           ");
                        }
                        if (checked_turn < turn && cursor == "")
                        {
                            if (turn % 2 == 0)
                            {
                                if (threat(false, board))
                                {
                                    Console.SetCursorPosition(50, 2);
                                    threatened_w = true;
                                    Console.WriteLine("Black threat White ");
                                    if (is_gameover(false, board))
                                    {
                                        winner_color = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (threat(true, board))
                                {
                                    Console.SetCursorPosition(50, 2);
                                    threatened_b = true;
                                    Console.WriteLine("White threat Black");
                                    if (is_gameover(true, board))
                                    {
                                        winner_color = false;
                                        break;
                                    }
                                }
                            }
                            checked_turn = turn;
                        }
                        if (en_passant_bool && en_passant_turn < turn)
                        {
                            en_passant_bool = false;
                        }
                        if (turn / 2 > 12 && !AI_mode)
                        {
                            Console.SetWindowSize(80, 25 + turn / 2);
                        }
                        f.Close();
                    }
                    for (int i = 0; i < 25 + turn / 2; i++)
                    {
                        Console.SetCursorPosition(35, i);
                        Console.Write("                                            ");
                    }
                    Console.SetCursorPosition(0, 20);
                    Console.Write("                    ");
                    Console.SetCursorPosition(40, 2);
                    Console.Write(@" |\_");
                    Console.SetCursorPosition(40, 3);
                    Console.Write(@" /  .\_");
                    Console.SetCursorPosition(40, 4);
                    Console.Write(@"|    ___)");
                    Console.SetCursorPosition(40, 5);
                    Console.Write(@"|    \");
                    Console.SetCursorPosition(40, 6);
                    Console.Write(@"|  =  |");
                    Console.SetCursorPosition(40, 7);
                    Console.Write(@"/ _____\");
                    Console.SetCursorPosition(40, 8);
                    Console.Write(@"[_______]");
                    Console.SetCursorPosition(38, 10);
                    if (winner_color) Console.Write("Black Won");
                    else Console.Write("White Won");
                    Console.ReadLine();
                }
                if (cy == 7)
                {
                    StreamWriter fi = File.CreateText("gamesave.txt");
                    fi.Close();
                }
            } while (true);
        }
    }
}
