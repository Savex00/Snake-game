using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class SnakeGame : Form
    {
        //Snake
        PictureBox[] snakeParts;
        int snakeSize = 5;
        Point location = new Point(120, 120);
        string direction = "Right";
        bool chanchingDirection = false;

        //Hrana
        PictureBox food = new PictureBox();
        Point foodLocation = new Point(0, 0);

        //Baza podataka
        
        string connectionString = "Data Source=DESKTOP-LTCVGEH;Initial Catalog=data;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public SnakeGame()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            //ako korisnik zeli da ponovo pokrene igru
            gamePanel.Controls.Clear();
            snakeParts = null;
            scoreLabel.Text = "0";
            snakeSize = 5;
            direction = "Right";
            location = new Point(120, 120);

            //Pokretanje igre 
            drawSnake();
            drawFood();


            timer1.Start();

            //onemogucavanje nekih kontrola kada je igra zavrsena
            trackBar1.Enabled= false;
            startButton.Enabled= false;
            nameBox.Enabled= false;

            //omogucene kontole na nakon sto je igra zavrsena
            stopButton.Enabled = true;

        }

        private void drawSnake()
        {
            snakeParts= new PictureBox[snakeSize];

            //petlja koja "izcrtava zmiju"
            for(int i = 0 ; i < snakeSize; i++)
            {
                snakeParts[i] = new PictureBox();
                snakeParts[i].Size = new Size(15,15);
                snakeParts[i].BackColor= Color.Green;
                snakeParts[i].BorderStyle= BorderStyle.FixedSingle;
                snakeParts[i].Location = new Point(location.X - (15 * i), location.Y);
                gamePanel.Controls.Add(snakeParts[i]);
            }
        }

        private void drawFood()
        {
            Random random = new Random();
            int xRand = random.Next(38) * 15;
            int yRand = random.Next(30) * 15;

            bool isOnSnake = true;

            //provera da li je zmijica jede hranu
            while (isOnSnake)
            {
                for(int i = 0; i < snakeSize; i++)
                {
                    if (snakeParts[i].Location == new Point(xRand, yRand))
                    {
                        xRand = random.Next(38) * 15;
                        yRand = random.Next(30) * 15;
                    }
                    else
                    {
                        isOnSnake = false;
                    }
                }
            }

            //iscrtavanje hrane
            if(isOnSnake == false)
            {
                foodLocation = new Point(xRand,yRand);
                food.Size = new Size(15, 15);
                food.BackColor = Color.DimGray;
                food.BorderStyle = BorderStyle.FixedSingle;
                food.Location = foodLocation;
                gamePanel.Controls.Add(food);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //promena intervala tajmera (pomocu kojeg se pravi da se zmija pokrece)
            //zavisno od promene brzine
            timer1.Interval = 501 - (5 * trackBar1.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            move();
        }

        private void move()
        {
            Point point = new Point(0, 0);

            //pomeranje zmijice zavisno od pravca
            for (int i = 0; i < snakeSize; i++)
            {
                if (i == 0)
                {
                    point = snakeParts[i].Location;
                    if (direction == "Left")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X - 15, snakeParts[i].Location.Y);
                    }
                    if (direction == "Right")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X + 15, snakeParts[i].Location.Y);
                    }
                    if (direction == "Top")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X, snakeParts[i].Location.Y - 15);
                    }
                    if (direction == "Down")
                    {
                        snakeParts[i].Location = new Point(snakeParts[i].Location.X, snakeParts[i].Location.Y + 15);
                    }
                }
                else
                {
                    Point newPoint = snakeParts[i].Location;
                    snakeParts[i].Location = point;
                    point = newPoint;
                }
            }

            //zmijica jede hranu
            if (snakeParts[0].Location == foodLocation)
            {
                eatFood();
                drawFood();
            }

            //ako udari od ivicu
            if (snakeParts[0].Location.X < 0 || snakeParts[0].Location.Y < 0 || snakeParts[0].Location.X >= 570 || snakeParts[0].Location.Y >= 570)
            {
                stopGame();
            }

            //ako udari samu sebe 
            for(int i = 3; i < snakeSize; i++)
            {
                if (snakeParts[0].Location == snakeParts[i].Location)
                {
                    stopGame();
                }
            }

            chanchingDirection = false;
        }

            //User input
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == (Keys.Up) && direction != "Down" && chanchingDirection != true)
            {
                direction = "Top";
                chanchingDirection = true;
            }
            if (keyData == (Keys.Down) && direction != "Up" && chanchingDirection != true)
            {
                direction = "Down";
                chanchingDirection = true;
            }
            if (keyData == (Keys.Left) && direction != "Right" && chanchingDirection != true)
            {
                direction = "Left";
                chanchingDirection = true;
            }
            if (keyData == (Keys.Right) && direction != "Left" && chanchingDirection != true)
            {
                direction = "Right";
                chanchingDirection = true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void eatFood()
        {
            snakeSize++;

            //cuvanje stare vrednosti i dobijanje nove(povecavanje zmijice)
            PictureBox[] oldSnake = snakeParts;
            gamePanel.Controls.Clear();
            snakeParts = new PictureBox[snakeSize];

            for(int i = 0; i < snakeSize; i++)
            {
                snakeParts[i] = new PictureBox();
                snakeParts[i].Size = new Size(15, 15);
                snakeParts[i].BackColor = Color.Green;
                snakeParts[i].BorderStyle = BorderStyle.FixedSingle;

                if(i == 0)
                {
                    snakeParts[i].Location = foodLocation;
                }
                else
                {
                    snakeParts[i].Location = oldSnake[i - 1].Location;
                }

                gamePanel.Controls.Add(snakeParts[i]);
            }

            //Updejtovanje skora
            int currentScore = Int32.Parse(scoreLabel.Text);
            int newScore = currentScore + 10;
            scoreLabel.Text = newScore +""; 
        }

        private void stopGame()
        {
            timer1.Stop();
            trackBar1.Enabled = true;
            stopButton.Enabled = false;
            startButton.Enabled = true;
            nameBox.Enabled = true;

            //Game over labela
            Label over = new Label();
            over.Text = "Game\nOver";
            over.ForeColor= Color.White;
            over.Font = new Font("Arial", 100, FontStyle.Bold);
            over.Size = over.PreferredSize;
            over.TextAlign = ContentAlignment.MiddleCenter;

            int X = gamePanel.Width / 2 - over.Width / 2;
            int Y = gamePanel.Height / 2 - over.Height / 2;
            over.Location = new Point(X, Y);

            gamePanel.Controls.Add(over);
            over.BringToFront();

            //Dodavanje poena u bazu
            addCurrentScoreToDatabase();
            updateScoreBoard();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopGame();
        }

        private void updateScoreBoard()
        {
            //Uzimanje poena iz baze i njihovo prikazivanje na data gridu
            String query = "SELECT Date,Name,Scores FROM scores";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);

                var ds = new DataSet();
                adapter.Fill(ds);

                dataGridView1.DataSource = ds.Tables[0];

                dataGridView1.Columns[0].AutoSizeMode= DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGridView1.Sort(this.dataGridView1.Columns[0] ,ListSortDirection.Descending);

            }


        }

        private void addCurrentScoreToDatabase()
        {
            string query = "INSERT INTO scores(Date,Name,Scores) VALUES(@Date,@Name,@Scores);";

            using (SqlConnection con = new SqlConnection(connectionString)) 
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = nameBox.Text;
                cmd.Parameters.Add("@Scores", SqlDbType.Int).Value = scoreLabel.Text;

                try 
                { 
                    con.Open();
                    cmd.ExecuteNonQuery(); 
                    con.Close();
                }
                catch (Exception ex) { 
                    ex.ToString();
                    Console.WriteLine(ex.ToString());
                }
                


            }
        }

        private void SnakeGame_Load(object sender, EventArgs e)
        {
            updateScoreBoard();
        }

    }
}

