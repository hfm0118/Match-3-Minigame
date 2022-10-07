using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockGame
{

    public partial class Form1 : Form
    {
        Chessboard B = new Chessboard();
        const int N = 6;
        Button[,] button_array = new Button[N, N];
        public int[,] selected_labels = new int[N, N];

        // decide block color for illustration
        Dictionary<TrueColor, Color> icolor0 = new Dictionary<TrueColor, Color>();
        Dictionary<TrueColor, Color> icolor1 = new Dictionary<TrueColor, Color>();

        bool prev_label = false;
        Position prev_pos = new Position(-1, -1);

        int score;
        int step_max = 10;
        int step_remaining;

        // disable button clicks when sleeping
        bool in_animation = false;
        bool grids_generated = false;

        public Form1()
        {
            InitializeComponent();

            // icolor0 are paler while icolor1 are brighter
            icolor0.Add(TrueColor.Red, Color.LightCoral);
            icolor0.Add(TrueColor.Yellow, Color.LemonChiffon);
            icolor0.Add(TrueColor.Blue, Color.SkyBlue);
            icolor0.Add(TrueColor.Green, Color.PaleGreen);
            icolor0.Add(TrueColor.Gray, Color.WhiteSmoke);

            icolor1.Add(TrueColor.Red, Color.Crimson);
            icolor1.Add(TrueColor.Yellow, Color.Gold);
            icolor1.Add(TrueColor.Blue, Color.SteelBlue);
            icolor1.Add(TrueColor.Green, Color.LimeGreen);
            icolor1.Add(TrueColor.Gray, Color.DarkGray);

            B.RandomInitialize();
            ClearSelectedLabels();
            score = 0;
            step_remaining = step_max;
            UpdateText();
        }

        // normal blocks are paler and will turn brighter when selected
        public Color GetIllusColor(TrueColor c, int is_selected=0)
        {
            if (is_selected != 0)
            {
                return icolor1[c];
            }
            return icolor0[c];
        }

        void GenerateGrids()
        {
            int x_offset = 60, y_offset = 120;
            int btn_size = 55, btn_distance = 60;

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Button btn = new Button();
                    TrueColor clr = B.color_array[i, j];
                    btn.BackColor = GetIllusColor(clr);
                    btn.Top = y_offset + i*btn_distance;
                    btn.Left = x_offset + j*btn_distance;
                    btn.Width = btn_size;
                    btn.Height = btn_size;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Visible = true;

                    btn.Click += new EventHandler(board_Click);
                    this.Controls.Add(btn);

                    button_array[i, j] = btn;

                }
            }
            grids_generated = true;
        }

        void RestartGrids()
        {
            B.RandomInitialize();
            score = 0;
            step_remaining = step_max;
            UpdateText();
            RefreshGrids();
        }

        // refresh block colors
        void RefreshGrids()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    TrueColor clr = B.color_array[i, j];
                    int is_selected = selected_labels[i, j];
                    int is_collapsing = B.collapse_labels[i, j];
                    button_array[i, j].BackColor = GetIllusColor(clr, is_selected+is_collapsing);

                }
            }
            Refresh();
        }

        // swap p1 and p2, then collapse until stable.
        void Step(Position p1, Position p2)
        {
            in_animation = true;

            B.Swap(p1, p2);
            RefreshGrids();

            int sum = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(500);
                int clear_cnt = B.LabelBlocks();
                if (clear_cnt == 0) break;
                sum += clear_cnt;
                RefreshGrids();
                System.Threading.Thread.Sleep(1000);
                B.Collapse();
                RefreshGrids();
            }

            // score += ax + bxlogx
            int increment = 100 * sum + (int)(10 * sum * Math.Log(sum));
            score += increment;
            step_remaining--;
            UpdateText(increment);
            if (step_remaining <= 0)
            {
                Form endgame = new Form2(score);
                endgame.ShowDialog();
                RestartGrids();
            }

            in_animation = false; 
        }

        // chessboard callback
        void board_Click(object sender, EventArgs e)
        {
            if (in_animation) return;
            Button btn = sender as Button;
            Position ps = GetButtonIndex(btn);
            if (prev_label)
            {
                prev_label = false;               
                ClearSelectedLabels();
                if (IsAdjacent(ps, prev_pos))
                {
                    //selected_labels[ps.i, ps.j] = 1;                   
                    Step(prev_pos, ps);
                    prev_pos = new Position(-1, -1);
                    return;
                }
                else
                {
                    prev_pos = new Position(-1, -1);
                }
            }
            else
            {
                selected_labels[ps.i, ps.j] = 1;
                prev_label = true;
                prev_pos = ps;
            }
            RefreshGrids();
        }

        // start game callback
        private void button1_Click(object sender, EventArgs e)
        {
            if (grids_generated)
            {
                RestartGrids();
            }
            else
            {
                GenerateGrids();
            }
        }

        // used in board_Click
        Position GetButtonIndex(Button btn)
        {
            Position ps = new Position(-1, -1);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (button_array[i,j] == btn)
                    {
                        ps = new Position(i, j);
                        return ps;
                    }
                }
            }
            return ps;
        }

        bool IsAdjacent(Position p1, Position p2)
        {
            if (p1.i == p2.i && (p1.j - p2.j == 1 || p1.j - p2.j == -1))
            {
                return true;
            }
            if (p1.j == p2.j && (p1.i - p2.i == 1 || p1.i - p2.i == -1))
            {
                return true;
            }
            return false;
        }

        public void ClearSelectedLabels()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    selected_labels[i, j] = 0;
                }
            }
        }

        public void UpdateText(int increment=0)
        {
            label1.Text = GetScoreOutput(increment);
            label2.Text = "Time remaining: " + step_remaining.ToString();
        }

        string GetScoreOutput(int increment)
        {
            return "Score: " + score.ToString() + 
                " (+" + increment.ToString() + ")";
        }
    }
}
