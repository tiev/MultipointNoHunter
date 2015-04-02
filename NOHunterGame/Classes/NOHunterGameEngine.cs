using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using Microsoft.MultiPoint.MultiPointControls;
using Microsoft.MultiPoint.MultiPointMousePlugIn;

namespace NOHunterGame.Classes
{
    public delegate void RightChoiceEventHandler(object sender, int i);
    class NOHunterGameEngine : INotifyPropertyChanged
    {
        #region Engine Configurations
        /// <summary>
        /// The information about the Style and timing in game
        /// </summary>
        public Style BUTTON_STYLE;
        public double TIME_TO_APPEAR = 5;
        public Uri WRONG_SOUND, RIGHT_SOUND, MOUSE_ENTER_SOUND, MOUSE_LEAVE_SOUND, APPEAR_SOUND;
        public Uri START_SOUND, FINISHED_SOUND;
        private int minColor = 70, maxColor = 256;

        #endregion //Engine Configurations

        public event PropertyChangedEventHandler PropertyChanged;
        public event RightChoiceEventHandler RightChoice;

        private NOHunterGameInfo gameInfo = new NOHunterGameInfo();
        private NOHunterPlayers gamePlayers = new NOHunterPlayers();
        public Grid NumberField;
        private Random random = new Random();
        private MediaPlayer mpMouseE = new MediaPlayer(), mpChoiceR = new MediaPlayer();
        private MediaPlayer mpMouseL = new MediaPlayer(), mpChoiceW = new MediaPlayer();
        private MediaPlayer mpGame = new MediaPlayer(), mpAppear = new MediaPlayer();
        private int[] cacheTable;

        #region Properties

        public NOHunterGameInfo GameInfo
        {
            get
            {
                return gameInfo;
            }
            set
            {
                gameInfo = value;
                OnPropertyChanged("GameInfo");
            }
        }

        public NOHunterPlayers GamePlayers
        {
            get
            {
                return gamePlayers;
            }
            set
            {
                gamePlayers = value;
                OnPropertyChanged("GamePlayers");
            }
        }

        #endregion //Properties



        #region Constructor

        public NOHunterGameEngine()
        {
            // Set volume for all MediaPlayer
            mpMouseE.Volume = 1;
            mpMouseL.Volume = 1;
            mpChoiceW.Volume = 1;
            mpGame.Volume = 1;
        }

        #endregion //Constructor



        #region Methods

        /// <summary>
        /// Raise event when PropertyChanged
        /// </summary>
        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Raise event when a right number is chosen
        /// </summary>
        /// <param name="number">the number has just been chosen</param>
        protected void OnRightChoice(int number)
        {
            RightChoiceEventHandler handler = RightChoice;
            if (handler != null)
                handler(this, number);
        }

        /// <summary>
        /// Create a new GameInfo to have a new game
        /// </summary>
        public void NewGame()
        {
            GameInfo = new NOHunterGameInfo();
        }

        /// <summary>
        /// Appearing the numbers after GameInfo has been set already
        /// </summary>
        public void StartGame()
        {
            // Create the grid with (GameHeight * GameWidth) cells
            NumberField.RowDefinitions.Clear();
            NumberField.ColumnDefinitions.Clear();
            for (int i = 0; i < gameInfo.GameHeight; i++)
                NumberField.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < gameInfo.GameWidth; i++)
                NumberField.ColumnDefinitions.Add(new ColumnDefinition());

            // Prepare for sounds
            mpAppear.Open(APPEAR_SOUND); mpAppear.Play();
            mpMouseE.Open(MOUSE_ENTER_SOUND); mpMouseE.Play();
            mpMouseL.Open(MOUSE_LEAVE_SOUND); mpMouseL.Play();
            mpChoiceR.Open(RIGHT_SOUND); mpChoiceR.Play();
            mpChoiceW.Open(WRONG_SOUND); mpChoiceW.Play();

            // Prepare for the cacheTable to generate buttons
            cacheTable = new int[gameInfo.Numbers.Length];
            gameInfo.Numbers.CopyTo(cacheTable, 0);
            // Timer to fill button to NumberField partially
            var createTimer = new DispatcherTimer();
            createTimer.Interval = TimeSpan.FromSeconds(TIME_TO_APPEAR / gameInfo.MaxNumber);
            createTimer.Tick += new EventHandler(createTimer_Tick);
            createTimer.Start();
            mpGame.Open(START_SOUND);
            mpGame.Play();
        }

        /// <summary>
        /// The same as StartGame but generate buttons with data from an array of int
        /// </summary>
        /// <param name="table">the array contains data for generating buttons</param>
        public void StartGameFromTable(int[] table)
        {
            // Create the grid with (GameHeight * GameWidth) cells
            NumberField.RowDefinitions.Clear();
            NumberField.ColumnDefinitions.Clear();
            for (int i = 0; i < gameInfo.GameHeight; i++)
                NumberField.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < gameInfo.GameWidth; i++)
                NumberField.ColumnDefinitions.Add(new ColumnDefinition());

            // Prepare for sounds
            mpAppear.Open(APPEAR_SOUND); mpAppear.Play();
            mpMouseE.Open(MOUSE_ENTER_SOUND); mpMouseE.Play();
            mpMouseL.Open(MOUSE_LEAVE_SOUND); mpMouseL.Play();
            mpChoiceR.Open(RIGHT_SOUND); mpChoiceR.Play();
            mpChoiceW.Open(WRONG_SOUND); mpChoiceW.Play();

            // Prepare for the cacheTable to generate buttons
            cacheTable = new int[table.Length];
            table.CopyTo(cacheTable, 0);
            // Timer to fill button to NumberField partially
            var createTimer = new DispatcherTimer();
            createTimer.Interval = TimeSpan.FromSeconds(TIME_TO_APPEAR / gameInfo.MaxNumber);
            createTimer.Tick += new EventHandler(createTimer_Tick);
            createTimer.Start();
            mpGame.Open(START_SOUND);
            mpGame.Play();
        }

        /// <summary>
        /// Create a button
        /// </summary>
        void createTimer_Tick(object sender, EventArgs e)
        {
            // Enough buttons, stop creation
            int count = NumberField.Children.Count;
            if (count >= gameInfo.MaxNumber)
            {
                ((DispatcherTimer)sender).Stop();
                return;
            }

            // Create a new MultiPointButton
            var myButton = new MultiPointButton
            {
                Style = BUTTON_STYLE,
                Content = cacheTable[count],
                Background = new SolidColorBrush(Color.FromRgb((byte)random.Next(minColor, maxColor), (byte)random.Next(minColor, maxColor), (byte)random.Next(minColor, maxColor))),
            };

            // Arrange new button to the grid
            myButton.SetValue(Grid.RowProperty, count / gameInfo.GameWidth);
            myButton.SetValue(Grid.ColumnProperty, count % gameInfo.GameWidth);
            NumberField.Children.Add(myButton);

            // Attach events to button
            myButton.MultiPointClick += new RoutedEventHandler(myButton_MultiPointClick);
            myButton.MultiPointMouseEnterEvent += new RoutedEventHandler(myButton_MultiPointMouseEnterEvent);
            myButton.MultiPointMouseLeaveEvent += new RoutedEventHandler(myButton_MultiPointMouseLeaveEvent);

            // Start animation for first appearance
            myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnAppear"] as Storyboard);
            mpAppear.Position = TimeSpan.Zero;
        }

        /// <summary>
        /// When MouseLeave, check if Button.IsEnabled
        ///   True  : begin Storyboard "OnNumberLeave"
        ///   False : begin Storyboard "OnFinishedNumberLeave"
        /// </summary>
        void myButton_MultiPointMouseLeaveEvent(object sender, RoutedEventArgs e)
        {
            var myButton = (MultiPointButton)sender;

            if (myButton.IsEnabled)
                myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnNumberLeave"] as Storyboard);
            else
                myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnFinishedNumberLeave"] as Storyboard);
            mpMouseL.Position = TimeSpan.Zero;
        }

        /// <summary>
        /// When MouseEnter, check if Button.IsEnabled:
        ///   True  : begin Storyboard "OnNumberEnter"
        ///   False : begin Storyboard "OnFinishedNumberEnter"
        /// </summary>
        void myButton_MultiPointMouseEnterEvent(object sender, RoutedEventArgs e)
        {
            var myButton = (MultiPointButton)sender;
            if (myButton.IsEnabled)
                myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnNumberEnter"] as Storyboard);
            else
                myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnFinishedNumberEnter"] as Storyboard);
            mpMouseE.Position = TimeSpan.Zero;
        }

        /// <summary>
        /// When Click, check if:
        ///  Right choice -> update the CurrentNumber and begin the Storyboard "OnRightChoice"
        ///  Wrong choice -> begin the Storyboard "OnWrongChoice" to make them know
        /// </summary>
        void myButton_MultiPointClick(object sender, RoutedEventArgs e)
        {
            var myButton = sender as MultiPointButton;
            if (!myButton.IsEnabled) return;

            var value = int.Parse(myButton.Content.ToString());
            if (value == gameInfo.CurrentNumber + 1)
            //Right choice
            {
                // Correct, user has won this button
                myButton.IsEnabled = false;
                gameInfo.CurrentNumber++;
                // Let player own the button by display its CursorImage behind the button
                var args = e as MultiPointMouseEventArgs;
                var device = (MultiPointMouseDevice)args.DeviceInfo.DeviceVisual;
                myButton.Background = new ImageBrush(device.CursorImage);
                myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnRightChoice"] as Storyboard);
                mpChoiceR.Position = TimeSpan.Zero;
                // Add score for the player
                GamePlayers.AddScore(args.DeviceInfo.ID);
                // Raise the RightChoice event
                OnRightChoice(value);
            }
            //Wrong choice
            else
            {
                System.Media.SoundPlayer a = new System.Media.SoundPlayer();

                myButton.BeginStoryboard(BUTTON_STYLE.Resources["OnWrongChoice"] as Storyboard);
                mpChoiceW.Position = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Close all media streams in the GameEngine
        /// </summary>
        internal void DiscardSounds()
        {
            mpAppear.Close();
            mpChoiceR.Close();
            mpChoiceW.Close();
            mpGame.Close();
            mpMouseE.Close();
            mpMouseL.Close();
        }

        #endregion //Methods

    }
}
