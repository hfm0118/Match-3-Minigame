using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BlockGame
{
    // TrueColor is the color for game logics.
    public enum TrueColor
    {
        Red, Yellow, Blue, Green, Gray
    }

    public enum Direction
    {
        Up, Down, Left, Right
    }

    public struct Position
    {
        public int i, j;
        public Position(int a, int b)
        {
            i = a; j = b;
        }
    }

    // wrap TrueColor[,] to deal with overflow
    public class ColorGrids {
        const int N = 6;
        private TrueColor[,] array = new TrueColor[N, N];
        public TrueColor this[int i, int j]
        {
            set
            {
                if (i > -1 && j > -1 && i < N && j < N)
                {
                    array[i, j] = value;
                }
            }
            get
            {
                if (i > -1 && j > -1 && i < N && j < N)
                {
                    return array[i, j];
                }
                return TrueColor.Gray;
            }
        }
    }

    public class Chessboard
    {
        const int N = 6;
        Random rnd = new Random();

        // public TrueColor[,] color_array = new TrueColor[N, N];
        public ColorGrids color_array = new ColorGrids();

        public int[,] collapse_labels = new int[N, N];
        
        // the array is only used for RandomColor.
        TrueColor[] tcolors = 
            { TrueColor.Red, TrueColor.Yellow, TrueColor.Blue, TrueColor.Green };

        public Chessboard()
        {
            for(int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    color_array[i, j] = TrueColor.Gray;
                }
            }
        }

        // randomly select a TrueColor.
        TrueColor RandomColor()
        {
            return tcolors[rnd.Next(0, tcolors.Length)];
        }

        // randomly distribute colors when initializing.
        public void RandomInitialize()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    color_array[i, j] = RandomColor();
                }
            }

            while (true)
            {
                int lcnt = LabelBlocks();
                if (lcnt == 0) break;
                Collapse();
            }
            
        }

        // set collapse_labels to 0.
        public void ClearCollapseLabels()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    collapse_labels[i, j] = 0;
                }
            }
        }

        // label the blocks that will be cleared.
        public int LabelBlocks()
        {
            ClearCollapseLabels();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (color_array[i,j] == color_array[i-1,j] && color_array[i,j] == color_array[i+1,j])
                    {
                        collapse_labels[i, j] = 1;
                        collapse_labels[i-1, j] = 1;
                        collapse_labels[i+1, j] = 1;
                    }
                    if (color_array[i,j] == color_array[i,j-1] && color_array[i,j] == color_array[i,j+1])
                    {
                        collapse_labels[i, j] = 1;
                        collapse_labels[i, j-1] = 1;
                        collapse_labels[i, j+1] = 1;
                    }
                }
            }
            int score = 0;
            foreach (int i in collapse_labels)
            {
                score += i;
            }
            return score;
        }
        
        // clear adjacent identical blocks, drop remaining blocks to ground, and add new blocks.
        public void Collapse()
        {
            for (int j = 0; j < N; j++)
            {
                int remain_cnt = 0;
                for (int i = N-1; i > -1; i--)
                {
                    if (collapse_labels[i, j] == 0)
                    {
                        remain_cnt++;
                        color_array[N-remain_cnt, j] = color_array[i, j];
                    }
                }
                for (int i = 0; i < N-remain_cnt; i++)
                {
                    color_array[i, j] = RandomColor();
                }
            }
            ClearCollapseLabels();
        }

        // exchange two adjacent blocks
        public void Swap(Position p1, Position p2)
        {
            TrueColor temp = color_array[p1.i, p1.j];
            color_array[p1.i, p1.j] = color_array[p2.i, p2.j];
            color_array[p2.i, p2.j] = temp;
        }

    }
}
