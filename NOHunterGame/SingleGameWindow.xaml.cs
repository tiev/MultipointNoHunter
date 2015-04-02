using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.MultiPoint.MultiPointCommonTypes;
using Microsoft.MultiPoint.MultiPointMousePlugIn;
using Microsoft.MultiPoint.MultiPointSDK;
using NOHunterGame.Classes;
using System.Linq;
using System.Windows.Controls;
using Microsoft.MultiPoint.MultiPointControls;

namespace NOHunterGame
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class SingleGameWindow : Window
    {
        NOHunterGameEngine game = new NOHunterGameEngine();
        public MediaPlayer musicPlayer = new MediaPlayer();

        #region Properties

        public int GameHeight
        {
            get
            {
                return game.GameInfo.GameHeight;
            }
            set
            {
                game.GameInfo.GameHeight = value;
            }
        }

        public int GameWidth
        {
            get
            {
                return game.GameInfo.GameWidth;
            }
            set
            {
                game.GameInfo.GameWidth = value;
            }
        }

        #endregion //Properties



        #region Methods

        public SingleGameWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(SingleGameWindow_Loaded);
            this.Closed += new EventHandler(SingleGameWindow_Closed);
            App.MultiPointObject.DeviceArrivalEvent += new EventHandler<DeviceNotifyEventArgs>(MultiPointObject_DeviceArrivalEvent);
            App.MultiPointObject.DeviceRemoveCompleteEvent += new EventHandler<DeviceNotifyEventArgs>(MultiPointObject_DeviceRemoveCompleteEvent);

            // Set information for this instance of NOHunterGame
            game.NewGame();
            game.GameInfo.GameHeight = 4;
            game.GameInfo.GameWidth = 4;

            SetEngineSound(game);

            game.NumberField = numField;
            game.TIME_TO_APPEAR = 2;
            game.BUTTON_STYLE = Application.Current.Resources["GlassButton"] as Style;

            game.GameInfo.GameOver += new GameOverEventHandler(game_GameOver);

            // Set default background music
            musicPlayer.Open(new Uri(string.Format(@"file:///{0}Music/{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Properties.Settings.Default.BACKGROUND_MUSIC)));
        }

        /// <summary>
        /// Sort players descending according to their scores
        /// </summary>
        void game_GameOver(object sender, EventArgs e)
        {
            //musicPlayer.Stop();
            var list = game.GamePlayers.Players;
            for (int i = 0; i < list.Count; i++)
                for (int j = i + 1; j < list.Count; j++)
                    if (list[i].Score < list[j].Score)
                    {
                        var x = list[i];
                        list[i] = list[j];
                        list[j] = x;
                    }
        }

        /// <summary>
        /// When game window is close,
        ///  disable MultiPoint,
        ///  close background music,
        ///  discard the sounds in GameEngine.
        /// </summary>
        void SingleGameWindow_Closed(object sender, EventArgs e)
        {
            App.MultiPointObject.UnRegisterMouseDevice();
            MultiPointSDK.ShowSystemCursor();
            musicPlayer.Close();
            game.DiscardSounds();
        }

        void SingleGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.MultiPointObject.CurrentWindow = this;
            App.MultiPointObject.RegisterMouseDevice();
            App.MultiPointObject.DrawMouseDevices();

            MultiPointSDK.HideSystemCursor();

            // Set DataContext in order to display information about game
            myGrid.DataContext = game;

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
                if (game.GamePlayers.Count == 0)
                    game.GamePlayers.Master = newPlayer;
                game.GamePlayers.Players.Add(newPlayer);
            }

            // Start playing this instance of NOHunterGame
            game.GameInfo.InitGame();
            game.GameInfo.Shuffle();
            if (game.GameInfo.MaxNumber > 50)
                game.TIME_TO_APPEAR = 4;
            game.StartGame();

            // Play background music and loop it
            musicPlayer.Play();
            musicPlayer.MediaEnded += new EventHandler(musicPlayer_MediaEnded);
        }

        /// <summary>
        /// If a mouse is plugged out of computer
        ///  remove cursor from mouse,
        ///  remove player from player list.
        /// </summary>
        void MultiPointObject_DeviceRemoveCompleteEvent(object sender, DeviceNotifyEventArgs e)
        {
            if (e.DeviceInfo.DeviceType != DeviceType.Mouse) return;

            // Remove cursor from mouse to make it available for new mouse later
            MouseHelper.RemoveCursorFromMouse(e.DeviceInfo);

            // Remove player from this game
            game.GamePlayers.Remove(e.DeviceInfo.ID);
        }

        /// <summary>
        /// If new mouse is plugged into computer
        ///  assign cursor to mouse,
        ///  create and add new player to player list.
        /// </summary>
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
            if (game.GamePlayers.Count == 0)
                game.GamePlayers.Master = newPlayer;
            game.GamePlayers.Players.Add(newPlayer);
        }

        /// <summary>
        /// Exit game only if Master player click the Exit button
        /// </summary>
        private void ExitGame(object sender, RoutedEventArgs e)
        {
            var args = e as MultiPointMouseEventArgs;
            var deviceInfo = (DeviceInfo)args.DeviceInfo;
            if (deviceInfo.ID == game.GamePlayers.Master.ID)
                this.Close();
        }

        /// <summary>
        /// Effect for Exit button when MouseEnter
        /// </summary>
        private void btnExit_MultiPointMouseEnterEvent(object sender, RoutedEventArgs e)
        {
            var myButton = sender as MultiPointButton;
            var args = e as MultiPointMouseEventArgs;
            if (args.DeviceInfo.ID == game.GamePlayers.Master.ID)
                myButton.Background = Brushes.Transparent;
        }

        /// <summary>
        /// Effect for Exit button when MouseLeave
        /// </summary>
        private void btnExit_MultiPointMouseLeaveEvent(object sender, RoutedEventArgs e)
        {
            var myButton = sender as MultiPointButton;
            var args = e as MultiPointMouseEventArgs;
            if (args.DeviceInfo.ID == game.GamePlayers.Master.ID)
                myButton.Background = Brushes.Red;
        }

        #endregion //Methods



        #region Media logic

        /// <summary>
        /// Set game sounds for a GameEngine
        /// </summary>
        /// <param name="engine">the GameEngine</param>
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
        /// Loop the MediaPlayer
        /// </summary>
        void musicPlayer_MediaEnded(object sender, EventArgs e)
        {
            var myPlayer = sender as MediaPlayer;
            myPlayer.Position = TimeSpan.Zero;
        }

        #endregion //Media logic
    }
}
