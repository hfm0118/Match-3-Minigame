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
    public partial class Form2 : Form
    {
        public Form2(int score)
        {
            InitializeComponent();
            label1.Text = GetEvaluation(score);
        }

        string GetEvaluation(int score)
        {
            string eval = "C";
            if (score > 15000) eval = "S+";
            else if (score > 10000) eval = "S";
            else if (score > 7500) eval = "A";
            else if (score > 5000) eval = "B";
            return "Your score is " + score.ToString() + 
                "(" + eval + ")!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
