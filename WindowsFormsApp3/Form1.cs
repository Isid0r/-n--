using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        int countCells;
        Solution solution;
        string generations;
        int[] answ;

        public Form1()
        {
            InitializeComponent();
            label1.Text = "";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            DrawField();
            PutQueens(answ);
        }

            private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int result1) 
                && int.TryParse(textBox3.Text, out int result2) && (result2 >= 0) 
                && (result2 < 75) && (result1 < Convert.ToInt32(generations)) && (result1 >= 0))
            {
                int gen = result1;
                int pop = result2;
                int[] s = solution.GiveGenPop(gen, pop);
                DrawField();
                PutQueens(s);
            }
            else
            {
                label1.Text = "ВВЕДЕНО НЕКОРРЕКТНОЕ ЗНАЧЕНИЕ!";
            }
        }

        private void DrawField()
        {
            int sizePanel = 601;

            while (((sizePanel - 1) % countCells) != 0)
            {
                sizePanel--;
            }
            Size panelSize = new Size(sizePanel, sizePanel);
            panel1.Size = panelSize;
            int indent = (sizePanel - 1) / countCells;

            Graphics gPanel = panel1.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.LightSlateGray);
            Pen p = new Pen(Color.Violet, 1);
            Pen p1 = new Pen(Color.LightPink, 1);

            gPanel.FillRectangle(brush, 0, 0, 601, 601);
            gPanel.DrawLine(p, new Point(0, 0), new Point(600, 0));
            gPanel.DrawLine(p, new Point(600, 0), new Point(600, 600));
            gPanel.DrawLine(p, new Point(0, 0), new Point(0, 600));
            gPanel.DrawLine(p, new Point(0, 600), new Point(600, 600));

            for (int i = indent; i <= sizePanel; i += indent)
            {
                gPanel.DrawLine(p, new Point(i, 0), new Point(i, 600));
                gPanel.DrawLine(p, new Point(0, i), new Point(600, i));
            }
        }

        private void PutQueens(int[] s)
        {
            Graphics gPanel = panel1.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.LightPink);

            int sz = panel1.Height - 1;

            int x, y;
            for (int i = 0; i < countCells; i++)
            {
                x = i * (sz / countCells);
                y = s[i] * (sz / countCells);
                gPanel.FillEllipse(brush, x, y, (sz / countCells), (sz / countCells));
            }
        }

        private void button1_MouseClick(object sender, MouseEventArgs e) // сброс игры
        {
            if ((int.TryParse(textBox1.Text, out int result)) && (result > 3) && (result <= 50))
            {
                countCells = result;
                if (backgroundWorker1.IsBusy != true)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                pictureBox1.Visible = true;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                label1.Text = "ВВЕДЕНО НЕКОРРЕКТНОЕ ЗНАЧЕНИЕ!";
            }

        }


        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            solution = new Solution(countCells);
            int k = 0;
            Task task = Task.Run(() =>
            {
                while (backgroundWorker1.IsBusy == true)
                {
                    label2.Text = Convert.ToString(solution.lol.countGenerations);
                }
            });
            answ = solution.Solve(); // ищет решение с помощью генетического алгоритма

            if (answ.Length == 0)
            {
                label2.Text = "РЕШЕНИЕ НЕ НАЙДЕНО! ПРЕВЫШЕН ЛИМИТ ПОКОЛЕНИЙ!";
            }
            else
            {
                if (answ.Length == 1)
                {
                    label2.Text = Convert.ToString(answ[1]);
                }
                DrawField();
                PutQueens(answ);
                generations = Convert.ToString(answ[countCells]);
                label2.Text = "РЕШЕНИЕ НАЙДЕНО! КОЛ-ВО ПОКОЛЕНИЙ: " + generations;
                label1.Text = "";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }
    }
}
