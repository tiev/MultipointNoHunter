using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.MultiPoint.MultiPointSDK;
using Microsoft.Win32;

namespace NOHunterGame
{
    /// <summary>
    /// Interaction logic for StartGame.xaml
    /// </summary>
    public partial class StartGame : Window
    {
        public StartGame()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(StartGame_Loaded);
        }

        void StartGame_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.Click += new RoutedEventHandler(btnStart_Click);
            comboGameType.SelectionChanged += new SelectionChangedEventHandler(comboGameType_SelectionChanged);
            btnBrowse.Click += new RoutedEventHandler(btnBrowse_Click);
        }

        /// <summary>
        /// Show a dialog box to choose the media file when the button Browse is clicked
        /// </summary>
        void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == true)
            {
                tbxMusic.Text = dlg.FileName;
            }
        }

        /// <summary>
        /// Change the UI for each of game types
        /// </summary>
        void comboGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ComboBox;
            inputOneTable.Visibility = Visibility.Hidden;
            inputTwoTable.Visibility = Visibility.Hidden;
            switch (box.SelectedIndex)
            {
                case 0:
                case 1: inputOneTable.Visibility = Visibility.Visible; break;
                case 2: inputTwoTable.Visibility = Visibility.Visible; break;
            }
        }

        /// <summary>
        /// Start the chosen game
        /// </summary>
        void btnStart_Click(object sender, RoutedEventArgs e)
        {
            int x;
            Uri myUri = null;
            bool hasMusic;

            // In case user change the background music, make the Uri for the media file
            hasMusic = (chkMusic.IsChecked == true) && File.Exists(tbxMusic.Text);
            if (hasMusic)
                myUri = new Uri(tbxMusic.Text);

            MultiPointSDK.SystemCursorPosition = new Point(this.Left + 5, this.Top + 5);
            MultiPointSDK.HideSystemCursor();

            // Start corresponding game with the user settings
            switch (comboGameType.SelectedIndex)
            {
                case 0: SingleGameWindow singleGame = new SingleGameWindow();
                    singleGame.Owner = this;
                    if (int.TryParse(tbxHeight.Text, out x))
                        singleGame.GameHeight = x;
                    if (int.TryParse(tbxWidth.Text, out x))
                        singleGame.GameWidth = x;
                    if (hasMusic)
                        singleGame.musicPlayer.Open(myUri);
                    singleGame.Show();
                    singleGame.Closed += new EventHandler(Game_Closed);
                    break;

                case 1: DoubleGameWindow doubleGame = new DoubleGameWindow();
                    doubleGame.Owner = this;
                    if (int.TryParse(tbxHeight.Text, out x))
                        doubleGame.GameHeight = x;
                    if (int.TryParse(tbxWidth.Text, out x))
                        doubleGame.GameWidth = x;
                    if (hasMusic)
                        doubleGame.musicPlayer.Open(myUri);
                    doubleGame.Show();
                    doubleGame.Closed += new EventHandler(Game_Closed);
                    break;

                case 2: TwoGameWindow twoGame = new TwoGameWindow();
                    twoGame.Owner = this;

                    if (int.TryParse(tbxHeight_1.Text, out x))
                        twoGame.GameHeight_1 = x;
                    if (int.TryParse(tbxWidth_1.Text, out x))
                        twoGame.GameWidth_1 = x;
                    if (int.TryParse(tbxHeight_2.Text, out x))
                        twoGame.GameHeight_2 = x;
                    if (int.TryParse(tbxWidth_2.Text, out x))
                        twoGame.GameWidth_2 = x;
                    if (hasMusic)
                        twoGame.musicPlayer.Open(myUri);
                    twoGame.Show();
                    twoGame.Closed += new EventHandler(Game_Closed);
                    break;
            }
        }

        /// <summary>
        /// Return to the main window after a game window is closed
        /// </summary>
        void Game_Closed(object sender, EventArgs e)
        {
            this.Activate();
            MultiPointSDK.ShowSystemCursor();
        }
    }
}
