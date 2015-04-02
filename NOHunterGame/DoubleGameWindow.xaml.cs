using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using Microsoft.MultiPoint.MultiPointControls;
using Microsoft.MultiPoint.MultiPointMousePlugIn;
using Microsoft.MultiPoint.MultiPointSDK;
using NOHunterGame.Classes;

namespace NOHunterGame
{
    /// <summary>
    /// Interaction logic for DoubleGameWindow.xaml
    /// </summary>
    public partial class DoubleGameWindow : Window
    {
        NOHunterGameInfo myGameInfo = new NOHunterGameInfo();
        NOHunterPlayers myGamePlayers = new NOHunterPlayers();
        NOHunterGameEngine game1 = new NOHunterGameEngine();
        NOHunterGameEngine game2 = new NOHunterGameEngine();
        public MediaPlayer musicPlayer = new MediaPlayer();
        int[] cacheTable;

        #region Properties
        public int GameHeight
        {
            get
            {
                return myGameInfo.GameHeight;
            }
            set
            {
                myGameInfo.GameHeight = value;
            }
        }

        public int GameWidth
        {
            get
            {
                return myGameInfo.GameWidth;
            }
            set
            {
                myGameInfo.GameWidth = value;
            }
        }
        #endregion //Properties



        #region Methods

        public DoubleGameWindow()
        {
            InitializeComponent();

            // Set structure of the game
            game1.GameInfo = myGameInfo;
            game1.GamePlayers = myGamePlayers;
            game2.GameInfo = myGameInfo;
            game2.GamePlayers = myGamePlayers;

            SetEngineSound(game1);
            SetEngineSound(game2);

            // DataBind UI controls
            stackCurrentNumber.DataContext = myGameInfo;
            listPlayer.DataContext = myGamePlayers;

            // Set size of this game
            myGameInfo.GameHeight = 5;
            myGameInfo.GameWidth = 5;

            // Set default background music
            musicPlayer.Open(new Uri(string.Format(@"file:///{0}Music/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Properties.Settings.Default.BACKGROUND_MUSIC)));

            ////myGameInfo.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(myGameInfo_PropertyChanged);
            myGameInfo.GameOver += new GameOverEventHandler(myGameInfo_GameOver);
            game1.RightChoice += new RightChoiceEventHandler(game1_RightChoice);
            game2.RightChoice += new RightChoiceEventHandler(game2_RightChoice);

            this.Loaded += new RoutedEventHandler(DoubleGameWindow_Loaded);
            this.Closed += new EventHandler(DoubleGameWindow_Closed);
            App.MultiPointObject.DeviceArrivalEvent += new EventHandler<DeviceNotifyEventArgs>(MultiPointObject_DeviceArrivalEvent);
            App.MultiPointObject.DeviceRemoveCompleteEvent += new EventHandler<DeviceNotifyEventArgs>(MultiPointObject_DeviceRemoveCompleteEvent);
        }

        /// <summary>
        /// When a number in game2 is chosen
        ///  refresh the game1.
        /// </summary>
        void game2_RightChoice(object sender, int i)
        {
            var gameInfo = ((NOHunterGameEngine)sender).GameInfo as NOHunterGameInfo;
            foreach (MultiPointButton button in game1.NumberField.Children)
                if (button.IsEnabled)
                    if (int.Parse(button.Content.ToString()) <= gameInfo.CurrentNumber)
                    {
                        button.IsEnabled = false;
                        button.Background = Brushes.Transparent;
                        button.BeginStoryboard((Application.Current.Resources["GlassButton"] as Style).Resources["OnRightChoice"] as System.Windows.Media.Animation.Storyboard);
                    }
        }

        /// <summary>
        /// When a number in game1 is chosen
        ///  refresh the game2
        /// </summary>
        void game1_RightChoice(object sender, int i)
        {
            var gameInfo = ((NOHunterGameEngine)sender).GameInfo as NOHunterGameInfo; 
            foreach (MultiPointButton button in game2.NumberField.Children)
                if (button.IsEnabled)
                    if (int.Parse(button.Content.ToString()) <= gameInfo.CurrentNumber)
                    {
                        button.IsEnabled = false;
                        button.Background = Brushes.Transparent;
                        button.BeginStoryboard((Application.Current.Resources["GlassButton"] as Style).Resources["OnRightChoice"] as System.Windows.Media.Animation.Storyboard);
                    }
        }

        /// <summary>
        /// Effect when game is over
        /// </summary>
        void myGameInfo_GameOver(object sender, EventArgs e)
        {
            numField1.Background = Brushes.Transparent;
            numField2.Background = Brushes.Transparent;
        }

        /// <summary>
        /// When game window is closed
        ///  disable MultiPoint,
        ///  close background music,
        ///  discard all sounds.
        /// </summary>
        void DoubleGameWindow_Closed(object sender, EventArgs e)
        {
            App.MultiPointObject.UnRegisterMouseDevice();
            MultiPointSDK.ShowSystemCursor();
            musicPlayer.Close();
            game1.DiscardSounds();
            game2.DiscardSounds();
        }

        void MultiPointObject_DeviceRemoveCompleteEvent(object sender, DeviceNotifyEventArgs e)
        {
            if (e.DeviceInfo.DeviceType != DeviceType.Mouse) return;

            // Remove cursor from mouse to make it available for new mouse later
            MouseHelper.RemoveCursorFromMouse(e.DeviceInfo);

            // Remove player from this game
            myGamePlayers.Remove(e.DeviceInfo.ID);
        }

        void MultiPointObject_DeviceArrivalEvent(object sender, DeviceNotifyEventArgs e)
        {
            if (e.DeviceInfo.DeviceType != DeviceType.Mouse) return;
            if (MouseHelper.CheckIfTooManyMice()) return;

            // Assign the CursorImage for this mouse device
            MouseHelper.AssignCursorToMouse(e.DeviceInfo);

            // Create new player for the game
            var newPlayer = new NOHunterPlayerInfo(e.DeviceInfo.ID);
            newPlayer.Avatar = ((MultiPointMouseDevice)e.DeviceInfo.DeviceVisual).CursorImage;

            // Add new player to the game
            //  Set Master player for the first player
            if (myGamePlayers.Count == 0)
                myGamePlayers.Master = newPlayer;
            myGamePlayers.Players.Add(newPlayer);
        }

        void DoubleGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.MultiPointObject.CurrentWindow = this;
            App.MultiPointObject.RegisterMouseDevice();
            App.MultiPointObject.DrawMouseDevices();

            MultiPointSDK.HideSystemCursor();

            // Check all the mouses pluged into computer
            //  in order to add all existing player
            foreach (DeviceInfo di in App.MultiPointObject.MouseDeviceList)
            {
                // Assign cursor to mouse
                MouseHelper.AssignCursorToMouse(di);

                // Create new player for the game
                var newPlayer = new NOHunterPlayerInfo(di.ID);
                newPlayer.Avatar = ((MultiPointMouseDevice)di.DeviceVisual).CursorImage;

                // Add new player to the game
                //  Set Master player for the first player
                if (myGamePlayers.Count == 0)
                    myGamePlayers.Master = newPlayer;
                myGamePlayers.Players.Add(newPlayer);
            }

            // Prepare game1
            game1.NumberField = numField1;
            game1.BUTTON_STYLE = Application.Current.Resources["GlassButton"] as Style;
            // Prepare game2
            game2.NumberField = numField2;
            game2.BUTTON_STYLE = Application.Current.Resources["GlassButton"] as Style;
            // Prepare the time to appear
            if (myGameInfo.MaxNumber > 50)
                game1.TIME_TO_APPEAR = game2.TIME_TO_APPEAR = 5;
            else game1.TIME_TO_APPEAR = game2.TIME_TO_APPEAR = 2;

            // Prepare for the numbers of game 1 and prepare cacheTable
            myGameInfo.InitGame();
            myGameInfo.Shuffle();
            cacheTable = new int[myGameInfo.Numbers.Length];
            myGameInfo.Numbers.CopyTo(cacheTable, 0);

            // Timer to start the two games
            DispatcherTimer startTimer = new DispatcherTimer();
            startTimer.Interval = TimeSpan.FromSeconds(2);
            startTimer.Tick += new EventHandler(startTimer_Tick);
            startTimer.Start();

            // Play background music and loop it
            musicPlayer.Play();
            musicPlayer.MediaEnded += new EventHandler(musicPlayer_MediaEnded);
        }

        /// <summary>
        /// Start game2 after a short time waiting in order to shuffle the number list
        /// </summary>
        void startTimer_Tick(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;
            // Shuffle and start game 2
            myGameInfo.Shuffle();
            game2.StartGame();
            // Start game 1 with the cacheTable
            game1.StartGameFromTable(cacheTable);
            timer.Stop();
        }

        /// <summary>
        /// Exit game only if Master player click the Exit button
        /// </summary>
        private void ExitGame(object sender, RoutedEventArgs e)
        {
            var args = e as MultiPointMouseEventArgs;
            var deviceInfo = (DeviceInfo)args.DeviceInfo;
            if (deviceInfo.ID == myGamePlayers.Master.ID)
                this.Close();
        }

        /// <summary>
        /// Effect for Exit button when MouseEnter
        /// </summary>
        private void btnExit_MultiPointMouseEnterEvent(object sender, RoutedEventArgs e)
        {
            var myButton = sender as MultiPointButton;
            var args = e as MultiPointMouseEventArgs;
            if (args.DeviceInfo.ID == game1.GamePlayers.Master.ID)
                myButton.Background = Brushes.Transparent;
        }

        /// <summary>
        /// Effect for Exit button when MouseLeave
        /// </summary>
        private void btnExit_MultiPointMouseLeaveEvent(object sender, RoutedEventArgs e)
        {
            var myButton = sender as MultiPointButton;
            var args = e as MultiPointMouseEventArgs;
            if (args.DeviceInfo.ID == game1.GamePlayers.Master.ID)
                myButton.Background = Brushes.Red;
        }

        #endregion //Methods



        #region Media logic

        /// <summary>
        /// Set game sounds for a GameEngine
        /// </summary>
        void SetEngineSound(NOHunterGameEngine engine)
        {
            SetSoundUri(out engine.APPEAR_SOUND, Properties.Settings.Default.BUTTON_APPEAR_SOUND);
            SetSoundUri(out engine.MOUSE_ENTER_SOUND, Properties.Settings.Default.BUTTON_MOUSE_ENTER_SOUND);
            SetSoundUri(out engine.MOUSE_LEAVE_SOUND, Properties.Settings.Default.BUTTON_MOUSE_LEAVE_SOUND);
            SetSoundUri(out engine.RIGHT_SOUND, Properties.Settings.Default.BUTTON_RIGHT_SOUND);
            SetSoundUri(out engine.WRONG_SOUND, Properties.Settings.Default.BUTTON_WRONG_SOUND);
            SetSoundUri(out engine.START_SOUND, Properties.Settings.Default.START_SOUND);
            SetSoundUri(out engine.FINISHED_SOUND, Properties.Settings.Default.FINISHED_SOUND);
        }

        void SetSoundUri(out Uri sound, string fileName)
        {
            sound = new Uri(string.Format(@"file:///{0}Sounds/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                fileName));
        }

        /// <summary>
        /// Loop MediaPlayer
        /// </summary>
        void musicPlayer_MediaEnded(object sender, EventArgs e)
        {
            var myPlayer = sender as MediaPlayer;
            myPlayer.Position = TimeSpan.Zero;
        }

        #endregion //Media logic
    }
}
