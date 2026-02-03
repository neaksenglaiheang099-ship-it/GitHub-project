using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class Form1 : Form
    {
        private Button firstButton;
        private Button secondButton;
        private int moves;
        private int pairsFound;
        private int elapsedSeconds;
        private readonly Random rng = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeGameBoard();
            StartNewGame();
        }

        private void InitializeGameBoard()
        {
            // create 4x4 grid of buttons inside tableLayoutPanel1
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            for (int i = 0; i < 4; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            }

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    var btn = new Button
                    {
                        Dock = DockStyle.Fill,
                        Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point),
                        BackColor = Color.LightGray,
                        ForeColor = Color.Black,
                        Margin = new Padding(6),
                        Tag = null, // assigned during StartNewGame
                        Text = string.Empty
                    };
                    btn.Click += CardButton_Click;
                    tableLayoutPanel1.Controls.Add(btn, c, r);
                }
            }
        }

        private void StartNewGame()
        {
            // symbols for pairs (8 pairs)
            var baseSymbols = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
            var symbols = new List<string>();
            foreach (var s in baseSymbols)
            {
                symbols.Add(s);
                symbols.Add(s);
            }

            // shuffle symbols
            symbols = symbols.OrderBy(x => rng.Next()).ToList();

            // assign symbols to buttons
            int i = 0;
            foreach (Control ctl in tableLayoutPanel1.Controls)
            {
                if (ctl is Button btn)
                {
                    btn.Enabled = true;
                    btn.Text = string.Empty;
                    btn.Tag = symbols[i++];
                    btn.BackColor = Color.LightSteelBlue;
                }
            }

            // reset state
            firstButton = null;
            secondButton = null;
            moves = 0;
            pairsFound = 0;
            elapsedSeconds = 0;
            lblMoves.Text = "Moves: 0";
            lblTime.Text = "Time: 0s";
            timerFlip.Stop();
            timerGame.Stop();
            timerGame.Start();
        }

        private void CardButton_Click(object sender, EventArgs e)
        {
            if (timerFlip.Enabled)
                return; // waiting to flip back

            if (!(sender is Button clicked))
                return;

            if (!string.IsNullOrEmpty(clicked.Text))
                return; // already revealed

            // reveal
            clicked.Text = clicked.Tag?.ToString();
            clicked.BackColor = Color.WhiteSmoke;

            if (firstButton == null)
            {
                firstButton = clicked;
                return;
            }

            secondButton = clicked;
            moves++;
            lblMoves.Text = $"Moves: {moves}";

            // check match
            if (firstButton.Tag?.ToString() == secondButton.Tag?.ToString())
            {
                // match found
                firstButton.Enabled = false;
                secondButton.Enabled = false;
                pairsFound++;

                firstButton = null;
                secondButton = null;

                if (pairsFound == 8)
                {
                    timerGame.Stop();
                    MessageBox.Show($"You won! Moves: {moves}, Time: {elapsedSeconds}s", "Congratulations");
                }
            }
            else
            {
                // start flip-back timer
                timerFlip.Start();
            }
        }

        private void timerFlip_Tick(object sender, EventArgs e)
        {
            timerFlip.Stop();

            if (firstButton != null)
            {
                firstButton.Text = string.Empty;
                firstButton.BackColor = Color.LightSteelBlue;
            }
            if (secondButton != null)
            {
                secondButton.Text = string.Empty;
                secondButton.BackColor = Color.LightSteelBlue;
            }

            firstButton = null;
            secondButton = null;
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++;
            lblTime.Text = $"Time: {elapsedSeconds}s";
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }
    }
}