namespace pong
{
    class Program
    {
        static void Main(string[] args)
        {
            Field field = new Field(51, 15);
            Racket left = new Racket(5, 1);
            Racket right = new Racket(5, field.GetCols - 2);
            Ball ball = new Ball(field.GetRows / 2, field.GetCols / 2);

            left.Draw(field);
            right.Draw(field);
            ball.Draw(field);

            Console.SetWindowSize(field.GetCols + 1, field.GetRows + 5);
            Console.CursorVisible = false;

            int leftScore = 0;
            int rightScore = 0;

            DrawScore(field, leftScore, rightScore);

            int skipBall = 2;
            while (true)
            {
                for (int i = 0; i < field.GetRows; i++)
                {
                    for (int j = 0; j < field.GetCols; j++)
                    {
                        DrawAt(i, j, field.Get(i, j).ToString());
                    }
                }
                ReadInput(field, left, right);
                skipBall--;
                if (skipBall == 0)
                {
                    int scored = CheckForGoal(field, ball);
                    if (scored != -1)
                    {
                        if (scored == 0)
                        {
                            leftScore++;
                        }
                        else
                        {
                            rightScore++;
                        }

                        DrawScore(field, leftScore, rightScore);
                        ball.Reset(field);
                        left.Reset(field);
                        right.Reset(field);
                    }
                    ball.CalculateTrajectory(field, left.Tile);
                    skipBall = 2;
                }

                Thread.Sleep(10);
            }
        }

        static void DrawScore(Field field, int leftScore, int rightScore)
        {
            DrawAt(field.GetRows + 1, 1, $"Player Left Score: {leftScore}");
            DrawAt(field.GetRows + 2, 1, $"Player Right Score: {rightScore}");
        }

        static void DrawAt(int x, int y, string s)
        {
            Console.SetCursorPosition(y, x);
            Console.WriteLine(s);
        }

        static void ReadInput(Field field, Racket left, Racket right)
        {
            if (!Console.KeyAvailable)
            {
                return;
            }
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.W:
                    left.MoveUp(field);
                    break;
                case ConsoleKey.S:
                    left.MoveDown(field);
                    break;
                case ConsoleKey.UpArrow:
                    right.MoveUp(field);
                    break;
                case ConsoleKey.DownArrow:
                    right.MoveDown(field);
                    break;
            }
        }

        static int CheckForGoal(Field field, Ball ball)
        {
            if (ball.Y == 0)
            {
                return 1;
            }
            if (ball.Y == field.GetCols - 1)
            {
                return 0;
            }
            return -1;
        }
    }

    public class Field
    {
        private readonly char[,] _field;

        public int GetRows => this._field.GetLength(0);
        public int GetCols => this._field.GetLength(1);

        public char Tile { get; }

        public char Get(int row, int col)
        {
            return this._field[row, col];
        }

        public void Set(int row, int col, char el)
        {
            this._field[row, col] = el;
        }

        public Field(int width, int height, char tile = '#')
        {
            this._field = new char[height, width];
            this.Tile = tile;
            for (int i = 0; i < this.GetCols; i++)
            {
                this.Set(0, i, tile);
                this.Set(this.GetRows - 1, i, tile);
            }
        }
    }

    public class Racket
    {
        private readonly int _y;
        private readonly int _lenght;
        private readonly int _initialX;

        private int _x;

        public Racket(int x, int y, int lenght = 5, char tile = '|')
        {
            this._x = this._initialX = x;
            this._y = y;
            this._lenght = lenght;
            this.Tile = tile;
        }

        public char Tile { get; set; }

        public void Draw(Field field)
        {
            for (int i = 0; i < this._lenght; i++)
            {
                field.Set(i + this._x, this._y, this.Tile);
            }
        }

        public void MoveUp(Field field)
        {
            if (this._x <= 1)
            {
                return;
            }

            field.Set(this._x + (this._lenght - 1), this._y, ' ');
            this._x--;
            field.Set(this._x, this._y, this.Tile);
        }

        public void MoveDown(Field field)
        {
            if (this._x + this._lenght >= field.GetRows - 1)
            {
                return;
            }

            field.Set(this._x, this._y, ' ');
            this._x++;
            field.Set(this._x + (this._lenght - 1), this._y, this.Tile);
        }

        public void Reset(Field field)
        {
            for (int i = 0; i < this._lenght; i++)
            {
                field.Set(i + this._x, this._y, ' ');
            }

            this._x = this._initialX;
            this.Draw(field);
        }
    }

    public class Ball
    {
        private readonly char _tile;
        private readonly int _initialX;
        private readonly int initialY;

        private int _x;
        private int _y;
        private bool _isGoingDown;
        private bool _isGoingRight;

        public Ball(int x, int y, char tile = '0')
        {
            this._x = this._initialX = x;
            this._y = this.initialY = y;
            this._tile = tile;
            this._isGoingDown = true;
            this._isGoingRight = true;
        }

        public int Y => this._y;

        public void Draw(Field field)
        {
            field.Set(this._x, this._y, this._tile);
        }

        public void CalculateTrajectory(Field field, char rackeTile)
        {
            field.Set(this._x, this._y, ' ');
            if (field.Get(this._x + 1, this._y) == field.Tile)
            {
                this._isGoingDown = false;
            }
            else if (field.Get(this._x - 1, this._y) == field.Tile)
            {
                this._isGoingDown = true;
            }

            if (field.Get(this._x, this._y + 1) == rackeTile)
            {
                this._isGoingRight = false;
            }
            else if (field.Get(this._x, this._y - 1) == rackeTile)
            {
                this._isGoingRight = true;
            }

            this._x = this._isGoingDown ? this._x + 1 : this._x - 1;
            this._y = this._isGoingRight ? this._y + 1 : this._y - 1;

            field.Set(this._x, this._y, this._tile);  
        }

        public void Reset(Field field)
        {
            field.Set(this._x, this._y, ' ');
            this._x = this._initialX;
            this._y = this.initialY;
            this.Draw(field);
        }
    }
}
