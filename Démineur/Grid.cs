using System;
using System.Collections.Generic;

namespace Démineur
{
    public struct Grid
    {
        private Box[,] boxes_grid;
        private short lines_nbr;
        private short rows_nbr;
        private short bombs_nbr;
        private short to_find;
        private short[] actual_box;
        public enum Game_state { Continue, Lost, Won };
        private Game_state game_state;
        public Box[,] Box_grid
        {
            get { return boxes_grid; }
            set { boxes_grid = value; }
        }
        public Game_state Game_State
        {
            get { return game_state; }
            set { game_state = value; }
        }
        public Grid(short lines_nbr, short rows_nbr, short bombs_nbr)
        {
            this.lines_nbr = lines_nbr;
            this.rows_nbr = rows_nbr;
            this.bombs_nbr = bombs_nbr;
            boxes_grid = new Box[lines_nbr,rows_nbr];
            actual_box = new short[2];
            actual_box[0] = 0;
            actual_box[1] = 0;
            game_state = Game_state.Continue;
            to_find = (short) ((lines_nbr * rows_nbr) - bombs_nbr);
            Create_grid();
        }
        private void Create_grid()
        {
            short increment_i = 2;
            short increment_j = 2;
            for (short i = 0; i < lines_nbr; i++)
            {
                increment_j = 2;
                for (short j = 0; j < rows_nbr; j++)
                {
                    boxes_grid[i, j] = new Box();
                    boxes_grid[i, j].Cursor_line = increment_i;
                    boxes_grid[i, j].Cursor_row = increment_j;
                    increment_j += 4;
                }
                increment_i += 2;
            }
            Random rand = new Random();
            short cpt = 0;
            while (cpt < bombs_nbr)
            {
                short line = (short) rand.Next(0, lines_nbr);
                short row = (short) rand.Next(0, rows_nbr);
                if (!boxes_grid[line, row].Is_bomb)
                {
                    boxes_grid[line, row].Is_bomb = true;
                    cpt++;
                }
            }
            for (short i = 0; i < lines_nbr; i++)
            {
                for (short j = 0; j < rows_nbr; j++) Search_around(i, j);
            }
        }
        private void Line_print(short mode)
        {
            if (mode == 0) Console.Write("┌");
            else if (mode == 1) Console.Write("└");
            else Console.Write("├");
            for (short i = 0; i < lines_nbr; i++)
            {
                if (i == lines_nbr - 1) Console.Write("───────");
                else Console.Write("────────");
            }
            if (mode == 0) Console.Write("┐");
            else if (mode == 1) Console.Write("┘");
            else Console.Write("┤");
        }
        private void Search_around(short line, short row)
        {
            short bomb_count = 0;
            for (short i = (short)(line - 1); i <= (short)(line + 1); i++)
            {
                for (short j = (short)(row - 1); j <= (short)(row + 1); j++)
                {
                    if ((i >= 0 && i < lines_nbr) && (j >= 0 && j < rows_nbr))
                    {
                        if(boxes_grid[i, j].Is_bomb) bomb_count++;
                    }
                }
            }
            boxes_grid[line, row].Mines_around = bomb_count;
        }
        private void Refresh_box(short line, short row)
        {
            Console.SetCursorPosition(boxes_grid[line, row].Cursor_row-1, boxes_grid[line, row].Cursor_line);
            switch (boxes_grid[line, row].Status)
            {
                case Box.State.Flag:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($" ▒ ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("│");
                    break;
                case Box.State.Discovered:
                    if (boxes_grid[line, row].Is_bomb) Console.ForegroundColor = ConsoleColor.Red;
                    else
                    {
                        switch (boxes_grid[line, row].Mines_around)
                        {
                            case 0:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 1:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 3:
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;
                            case 4:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                        }
                    }
                    Console.Write($" {((boxes_grid[line, row].Is_bomb) ? "X" : boxes_grid[line, row].Mines_around)} ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("│");
                    break;
                default:
                    Console.Write($" ▒ ");
                    Console.Write("│");
                    break;
            }
            Console.SetCursorPosition(boxes_grid[line, row].Cursor_row, boxes_grid[line, row].Cursor_line);
        }
        private void Reveal_around(short line, short row)
        {
            List<(short, short)> unmined_boxes = new List<(short, short)>();
            for (short i = (short)(line - 1); i <= (short)(line + 1); i++)
            {
                for (short j = (short)(row - 1); j <= (short)(row + 1); j++)
                {
                    if ((i >= 0 && i < lines_nbr) && (j >= 0 && j < rows_nbr) && (boxes_grid[i, j].Status == Box.State.None))
                    {
                        if (boxes_grid[i, j].Mines_around == 0) unmined_boxes.Add((i, j));
                        boxes_grid[i, j].Status = Box.State.Discovered;
                        Refresh_box(i, j);
                        to_find--;
                    }
                }
            }
            if (unmined_boxes.Count > 0)
            {
                foreach ((short, short) unmined_box in unmined_boxes)
                {
                    if (!boxes_grid[unmined_box.Item1, unmined_box.Item2].Is_bomb) Reveal_around(unmined_box.Item1, unmined_box.Item2);
                }
            }
        }
        private void Reveal_bombs()
        {
            for (short i = 0; i < lines_nbr; i++)
            {
                for (short j = 0; j < rows_nbr; j++)
                {
                    if (boxes_grid[i, j].Is_bomb)
                    {
                        boxes_grid[i, j].Status = Box.State.Discovered;
                        Refresh_box(i,j);
                    }
                }
            }
        }
        public void Print_grid()
        {
            Console.Clear();
            Console.WriteLine("*** DEMINOR 3000 ***");
            Line_print(0);
            for (short i = 0; i < lines_nbr; i++)
            {
                if (i > 0) Line_print(2);
                Console.WriteLine();
                Console.Write("│");
                for (short j = 0; j < rows_nbr; j++)
                {
                    Console.Write($" ▒ ");
                    Console.Write("│");
                }
                Console.WriteLine();
            }
            Line_print(1);
        }
        public void Move(short line, short row)
        {
            if (line != 0)
            {
                if (actual_box[0] + line>=0 && actual_box[0] + line < lines_nbr)
                {
                    actual_box[0] += line; 
                }
            }
            else if (row != 0)
            {
                if (actual_box[1] + row >= 0 && actual_box[1] + row < rows_nbr)
                {
                    actual_box[1] += row;
                }
            }
            Console.SetCursorPosition(boxes_grid[actual_box[0], actual_box[1]].Cursor_row, boxes_grid[actual_box[0],actual_box[1]].Cursor_line);
        }
        public void SetFlag()
        {
            if (boxes_grid[actual_box[0], actual_box[1]].Status == Box.State.None) boxes_grid[actual_box[0], actual_box[1]].Status = Box.State.Flag;
            else if (boxes_grid[actual_box[0], actual_box[1]].Status == Box.State.Flag) boxes_grid[actual_box[0], actual_box[1]].Status = Box.State.None;
            Refresh_box(actual_box[0], actual_box[1]);
        }
        public void Reveal()
        {
            boxes_grid[actual_box[0], actual_box[1]].Status = Box.State.Discovered;
            Refresh_box(actual_box[0], actual_box[1]);
            if (boxes_grid[actual_box[0], actual_box[1]].Is_bomb)
            {
                game_state = Game_state.Lost;
                Reveal_bombs();
            }
            else
            {
                to_find--;
                if (boxes_grid[actual_box[0], actual_box[1]].Mines_around == 0) Reveal_around(actual_box[0], actual_box[1]);
                if (to_find == 0) game_state = Game_state.Won;
                else game_state = Game_state.Continue;
            }
            if (game_state != Game_state.Continue) Console.SetCursorPosition(0, 22);
        }
    }
}
