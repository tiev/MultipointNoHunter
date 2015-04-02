using System;
using System.ComponentModel;
using System.Linq;

namespace NOHunterGame.Classes
{
    public delegate void GameOverEventHandler(object sender, EventArgs e);
    class NOHunterGameInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event GameOverEventHandler GameOver;

        private int currentNumber = 0;
        private int[] numbers = new int[1];
        private int game_Width = 1;
        private int game_Height = 1;
        private Random random = new Random();

        #region Properties

        /// <summary>
        /// The number players reach so far
        ///  raise the GameOver event if CurrentNumber reach the MaxNumber
        /// </summary>
        public int CurrentNumber
        {
            get
            {
                return currentNumber;
            }
            set
            {
                currentNumber = value;
                OnPropertyChanged("CurrentNumber");
                if (CurrentNumber == MaxNumber)
                    OnGameOver();
            }
        }

        public int GameHeight
        {
            get
            {
                return game_Height;
            }
            set
            {
                game_Height = value;
                OnPropertyChanged("GameHeight");
                OnPropertyChanged("MaxNumber");
            }
        }

        public int GameWidth
        {
            get
            {
                return game_Width;
            }
            set
            {
                game_Width = value;
                OnPropertyChanged("GameWidth");
                OnPropertyChanged("MaxNumber");
            }
        }

        /// <summary>
        /// Max number of the game
        /// </summary>
        public int MaxNumber
        {
            get
            {
                return game_Height * game_Width;
            }
        }

        /// <summary>
        /// The array indicate the arrangement of all numbers in game
        /// </summary>
        public int[] Numbers
        {
            get
            {
                return numbers;
            }
            set
            {
                numbers = value;
                OnPropertyChanged("Numbers");
            }
        }
       
        #endregion //Properties

        

        #region Constructor

        public NOHunterGameInfo()
        {
        }

        #endregion //Constructor



        #region Methods

        /// <summary>
        /// Notify UI when property is changed
        /// </summary>
        /// <param name="property">Name of the property to notify</param>
        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Raise GameOver event when Game is over (CurrentNumber reach MaxNumber)
        /// </summary>
        protected virtual void OnGameOver()
        {
            GameOverEventHandler handler = GameOver;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determine the maxNumber and fill the array numbers
        /// </summary>
        public void InitGame()
        {
            numbers = new int[MaxNumber];

            // Fill numbers to array
            for (int i = 0; i < MaxNumber; i++)
                numbers[i] = i + 1;
        }

        /// <summary>
        /// Init game with the size height*width
        /// </summary>
        public void InitGame(int height, int width)
        {
            GameHeight = height;
            GameWidth = width;
            InitGame();
        }

        /// <summary>
        /// Shuffle numbers
        /// </summary>
        public void Shuffle()
        {
            Random random = new Random();
            for (int i = 0; i < MaxNumber; i++)
            {
                int j = random.Next(MaxNumber);
                int tmp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = tmp;
            }
        }

        /// <summary>
        /// Determine the number at a position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>The number</returns>
        public int NumberAt(int row, int column)
        {
            return numbers[row * game_Width + column];
        }

        /// <summary>
        /// Find the position (x, y) of a number
        /// </summary>
        /// <param name="num"> number value </param>
        /// <out> x = y = -1 if there is not that value in numbers</out>
        public void FindPositionOf(int num, out int x, out int y)
        {
            int max = MaxNumber;
            if (numbers.Count() < max)
                max = numbers.Count();
            x = y = -1;
            if (num > max)
                return;
            int i = 0;
            while (i < max && Numbers[i] != num)
                i++;

            // If found then calculate the position
            if (i < max)
            {
                x = i / game_Width;
                y = i % game_Width;
                return;
            }
        }

        #endregion //Methods
    }
}
