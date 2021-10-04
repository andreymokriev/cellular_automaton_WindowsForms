using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace GOL4
{
    public partial class Form1 : Form
    {
        private Graphics graph;
        private int PixelSize, Density, TimerSpeed;
        private int xrow;//columns
        private int yrow;//rows
        private int generation;
        private bool initialized = false;

        private bool[,] field;

        public Form1()
        {
            //Иницилизация значений по умолчанию
            InitializeComponent();
            nudPixelSize.Value = 3;
            nudDensity.Value = 5;
            tbInterval.Value = 50;
            tbFilename.Text = "my_save";
            cbGenRule.Text = "GameOfLife";

            picBox.Image = new Bitmap(picBox.Width, picBox.Height);
            graph = Graphics.FromImage(picBox.Image);

            bStop.Enabled = false;
            bStart.Enabled = false;

            bGenerate.Click += BGenerate_Click;
            bStart.Click += BStart_Click;
            bStop.Click += BStop_Click;
            timer1.Tick += Timer1_Tick;
            picBox.MouseMove += PicBox_MouseMove;
            bSave.Click += BSave_Click;
            bLoad.Click += BLoad_Click;


        }


        //Кнопка Загрузить
        private void BLoad_Click(object sender, EventArgs e)
        {
            string path = tbFilename.Text;
            int tempint;
            bool tempbool = false;

            if (File.Exists(path))
            {

                using (StreamReader sr = new StreamReader(path))
                {
                    for (int x = 0; x < xrow; x++)
                    {
                        for (int y = 0; y < yrow; y++)
                        {
                            tempint = sr.Read();

                            if (tempint == 49)
                            {
                                tempbool = false;
                            }
                            if (tempint == 50)
                            {
                                tempbool = true;
                            }

                            field[x, y] = tempbool;
                        }
                    }
                    DrawScreen();
                }

                generation = 0;
            }
            else MessageBox.Show("Не найден файл по указнному пути");
        }


        //Кнопка сохранить
        private void BSave_Click(object sender, EventArgs e)
        {
            string path = tbFilename.Text;

            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int x = 0; x < xrow; x++)
                {
                    for (int y = 0; y < yrow; y++)
                    {
                        if (field[x, y] == false)
                        {
                            sw.Write(1);
                        }
                        if (field[x, y] == true)
                        {
                            sw.Write(2);
                        }
                    }
                }
            }
        }




        //Обработка нажатия на форму
        private void PicBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (initialized == true)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var x = e.Location.X / PixelSize;
                    var y = e.Location.Y / PixelSize;

                    if (x > 0 && y > 0 && y < picBox.Height / 5.18 && x < picBox.Width  / 5.18)
                        field[x, y] = true;

                    //string test = Convert.ToString(x) + " " +Convert.ToString(y) + " " + picBox.Height; 
                    //MessageBox.Show(test);

                    if (!timer1.Enabled)
                    {
                        DrawScreen();
                    }
                }

                if (e.Button == MouseButtons.Right)
                {
                    var x = e.Location.X / PixelSize;
                    var y = e.Location.Y / PixelSize;

                    if (x > 0 && y > 0 && y < picBox.Height / 5.18 && x < picBox.Width / 5.18)
                        field[x, y] = false;

                    if (!timer1.Enabled)
                    {
                        DrawScreen();
                    }
                }
            }
        }

        //Итерация таймера
        private void Timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            GenLabel.Text = Convert.ToString(generation++);
        }

        //Кнопка стоп
        private void BStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            bStart.Enabled = true;
            bStop.Enabled = false;
        }

        //Кнопка Старт
        private void BStart_Click(object sender, EventArgs e)
        {
            TimerSpeed = Convert.ToInt32(100 - tbInterval.Value);
            if (TimerSpeed == 0) { TimerSpeed++; }
            timer1.Interval = TimerSpeed;
            bStart.Enabled = false;
            bStop.Enabled = true;

            timer1.Enabled = true;
        }

        //Кнопка генерация
        private void BGenerate_Click(object sender, EventArgs e)
        {
            initialized = true;

            PixelSize = (int)nudPixelSize.Value+1;
            Density = (int)nudDensity.Value;
            if (PixelSize == 1)
            {
                nudPixelSize.Value = 1;
                PixelSize = 2;
            }

            Random rand = new Random();

            xrow = picBox.Width / PixelSize;
            yrow = picBox.Height / PixelSize;

            field = new bool[xrow, yrow];

            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    field[x, y] = rand.Next(Density) == 0;
                }
            }

            generation = 0;
            GenLabel.Text = Convert.ToString(generation);

            picBox.Image = new Bitmap(picBox.Width, picBox.Height);
            graph = Graphics.FromImage(picBox.Image);

            DrawScreen();
            BStart_Click(null,null);
            bStart.Enabled = true;
        }

        //Пользовательские методы



        //Генерация следующего поколения
        private void NextGeneration()
        {
            DrawScreen();

            bool[,] newField = new bool[xrow, yrow];

            string rule = cbGenRule.Text;

            switch (rule)
            {
                case "GameOfLife":
                    GOL(newField);
                    break;

                case "Maze":
                    MAZE(newField);
                    break;

                case "34":
                    TF(newField);
                    break;
                case "Assimilation":
                    ASIM(newField);
                    break;

                case "Coagulations":
                    COAG(newField);
                    break;
                case "Stains":
                    STAN(newField);
                    break;
                case "Diamoeba":
                    DMOE(newField);
                    break;
                case "Walled Cities":
                    WALL(newField);
                    break;
                default:
                    MessageBox.Show("Ошибка правила");
                    break;
            }


            field = newField;


        }

        //Diamoeba
        private void DMOE(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && (CountNeighbors == 3 || CountNeighbors == 5 || CountNeighbors == 6 || CountNeighbors == 7 || CountNeighbors == 8))
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 5 || CountNeighbors > 8))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }


        //Walled cities
        private void WALL(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && (CountNeighbors == 4 || CountNeighbors == 5 || CountNeighbors == 6 || CountNeighbors == 7 || CountNeighbors == 8))
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 2 || CountNeighbors > 5))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }

        //Stains
        private void STAN(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && (CountNeighbors == 3 || CountNeighbors == 6 || CountNeighbors == 7 || CountNeighbors == 8))
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 2 || CountNeighbors > 8) || CountNeighbors == 4)
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }

        //Coagulations
        private void COAG(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && (CountNeighbors == 3 || CountNeighbors == 7 || CountNeighbors == 8))
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 2 || CountNeighbors > 8) || CountNeighbors ==4)
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }

        //Assimilation
        private void ASIM(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && (CountNeighbors == 3 || CountNeighbors == 4 || CountNeighbors == 5))
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 4 || CountNeighbors > 7))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }

        //Правило 34
        private void TF(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && (CountNeighbors == 3 || CountNeighbors == 4))
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 3 || CountNeighbors > 4))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }


        //Правило Maze
        private void MAZE(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && CountNeighbors == 3)
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 1 || CountNeighbors > 5))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }

        //Правило GameOfLife
        private void GOL(bool[,] newField)
        {
            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    var CountNeighbors = NeighborsCount(x, y);

                    if (field[x, y] == false && CountNeighbors == 3)
                    {
                        newField[x, y] = true;
                    }
                    else if (field[x, y] == true && (CountNeighbors < 2 || CountNeighbors > 3))
                    {
                        newField[x, y] = false;
                    }
                    else
                    {
                        newField[x, y] = field[x, y];
                    }
                    if (field[x, y])
                        graph.FillRectangle(Brushes.Red, x * PixelSize, y * PixelSize, PixelSize, PixelSize);


                }
            }
        }

        //Подсчет соседей у клетки
        private int NeighborsCount(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + xrow) % xrow;
                    var row = (y + j + yrow) % yrow;


                    bool SelfChecking = col == x && row == y;
                    bool HasLife = field[col, row];

                    if (HasLife && !SelfChecking)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        //Прорисовка изоюражения на экран
        private void DrawScreen()
        {
            graph.Clear(Color.Black);

            for (int x = 0; x < xrow; x++)
            {
                for (int y = 0; y < yrow; y++)
                {
                    if (field[x, y])
                    {
                        graph.FillRectangle(Brushes.Green, x * PixelSize, y * PixelSize, PixelSize - 1, PixelSize - 1);
                    }
                }
            }

            picBox.Refresh();
        }
    }
}
