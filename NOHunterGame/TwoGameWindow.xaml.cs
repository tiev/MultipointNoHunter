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
    /// Interaction logic for TwoGame.xaml
    /// </summary>
    public partial class TwoGameWindow : Window
    {
        NOHunterGameEngine game1 = new NOHunterGameEngine();
        NOHunterGameEngine game2 = new NOHunterGameEngine();
        public MediaPlayer musicPlayer = new MediaPlayer();
        DispatcherTimer startTimer = new DispatcherTimer();

        #region Properties
        public int GameHeight_1
        {
            get
            {
                return game1.GameInfo.GameHeight;
            }
            set
            {
                game1.GameInfo.GameHeight = value;
            }
        }

        public int GameWidth_1
        {
            get
            {
                return game1.GameInfo.GameWidth;
            }
            set
            {
                game1.GameInfo.GameWidth = value;
            }
        }

        public int GameHeight_2
        {
            get
            {
                return game2.GameInfo.GameHeight;
            }
            set
            {
                game2.GameInfo.GameHeight = value;
            }
        }

        public int GameWidth_2
        {
            get
            {
                return game2.GameInfo.GameWidth;
            }
            set
            {
                game2.GameInfo.GameWidth = value;
            }
        }
        #endregion //Properties



        #region Methods

        public TwoGameWindow()
        {
            InitializeComponent();

            // Set structure of the game
            game1.NewGame();
            game2.NewGame();
            game2.GamePlayers = game1.GamePlayers;

            SetEngineSound(game1);
            SetEngineSound(game2);

            // DataBind UI controls
            listPlayer.DataContext = game1.GamePlayers;
            stackGame1_CurrentNumber.DataContext = game1.GameInfo;
            stackGame2_CurrentNumber.DataContext = game2.GameInfo;

            // Set the default size for the two games
            game1.GameInfo.GameHeight = game1.GameInfo.GameWidth = 4;
            game2.GameInfo.GameHeight = game2.GameInfo.GameWidth = 4;

            // Set default background music
            musicPlayer.Open(new Uri(string.Format(@"file:///{0}Music/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Properties.Settings.Default.BACKGROUND_MUSIC)));

            game1.GameInfo.GameOver += new GameOverEventHandler(GameInfo1_GameOver);
            game2.GameInfo.GameOver += new GameOverEventHandler(GameInfo2_GameOver);

            this.Loaded += new RoutedEventHandler(TwoGameWindow_Loaded);
            this.Closed += new EventHandler(TwoGameWindow_Closed);
            App.MultiPointObject.DeviceArrivalEvent += new EventHandler<DeviceNotifyEventArgs>(MultiPointObject_DeviceArrivalEvent);
            App.MultiPointObject.DeviceRemoveCompleteEvent += new EventHandler<DeviceNotifyEventArgs>(MultiPointObject_DeviceRemoveCompleteEvent);
        }

        /// <summary>
        /// Effect when game2 is over
        /// </summary>
        void GameInfo2_GameOver(object sender, EventArgs e)
        {
            game2.NumberField.Background = Brushes.Transparent;
        }

        /// <summary>
        /// Effect when game1 is over
        /// </summary>
        void GameInfo1_GameOver(object sender, EventArgs e)
        {
            game1.NumberField.Background = Brushes.Transparent;
        }

        /// <summary>
        /// When game window is closed
        ///  disable MultiPoint,
        ///  close background music,
        ///  discard all sounds.
        /// </summary>
        void TwoGameWindow_Closed(object sender, EventArgs e)
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
            game1.GamePlayers.Remove(e.DeviceInfo.ID);
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
            if (game1.GamePlayers.Count == 0)
                game1.GamePlayers.Master = newPlayer;
            game1.GamePlayers.Players.Add(newPlayer);
        }

        void TwoGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.MultiPointObject.CurrentWindow = this;
            App.MultiPointObject.RegisterMouseDevice();
            App.MultiPointObject.DrawMouseDevices();

            MultiPointSDK.HideSystemCursor();

            // Init game1
            game1.GameInfo.InitGame();
            game1.GameInfo.Shuffle();
            game1.BUTTON_STYLE = Application.Current.Resources["GlassButton"] as Style;
            game1.NumberField = numField1;
            game1.TIME_TO_APPEAR = 4;
            // Init game2;
            game2.GameInfo.InitGame();
            
            game2.BUTTON_STYLE = Application.Current.Resources["GlassButton"] as Style;
            game2.NumberField = numField2;
            game2.TIME_TO_APPEAR = 4;
            
            // Timer to make two game different by shuffle them at different times
            startTimer.Interval = TimeSpan.FromSeconds(2);
            startTimer.Tick += new EventHandler(startTimer_Tick);
            startTimer.Start();

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
                if (game1.GamePlayers.Count == 0)
                    game1.GamePlayers.Master = newPlayer;
                game1.GamePlayers.Players.Add(newPlayer);
            }

            // Play background music and loop it
            musicPlayer.Play();
            musicPlayer.MediaEnded += new EventHandler(musicPlayer_MediaEnded);
        }

        /// <summary>
        /// After have shuffled game1 for a while
        ///  shuffle game2,
        ///  start two games.
        /// </summary>
        void startTimer_Tick(object sender, EventArgs e)
        {
            game2.GameInfo.Shuffle();
            game1.StartGame();
            game2.StartGame();
            startTimer.Stop();
        }

        /// <summary>
        /// Exit game only if Master player click the Exit button
        /// </summary>
        private void ExitGame(object sender, RoutedEventArgs e)
        {
            var args = e as MultiPointMouseEventArgs;
            var deviceInfo = (DeviceInfo)args.DeviceInfo;
            if (deviceInfo.ID == game1.GamePlayers.Master.ID)
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
            musicPlayer.Position = TimeSpan.Zero;
        }

        #endregion //Media logic
    }
}
