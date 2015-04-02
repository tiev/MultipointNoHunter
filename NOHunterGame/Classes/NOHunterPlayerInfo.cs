using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace NOHunterGame.Classes
{
    /// <summary>
    /// Info of a player contains
    ///     ID associated with PointerID to distinguish players
    ///     and the player's Score
    /// </summary>
    class NOHunterPlayerInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int player_ID;
        private int player_Score = 0;
        private BitmapImage player_Image = new BitmapImage();

        #region Properties

        /// <summary>
        /// ID to distinguish players
        /// </summary>
        public int ID
        {
            get
            {
                return player_ID;
            }
        }

        /// <summary>
        /// Score of this player
        /// </summary>
        public int Score
        {
            get
            {
                return player_Score;
            }
            set
            {
                player_Score = value;
                OnPropertyChanged("Score");
            }
        }

        /// <summary>
        /// The avatar and also is the cursor image
        /// </summary>
        public BitmapImage Avatar
        {
            get
            {
                return player_Image;
            }
            set
            {
                player_Image = value;
                player_Image.SetValue(BitmapImage.DecodePixelHeightProperty, 10);
                OnPropertyChanged("Avatar");
            }
        }

        #endregion //Properties



        #region Constructors

        public NOHunterPlayerInfo(int id)
        {
            player_ID = id;
        }

        public NOHunterPlayerInfo(int id, int score)
        {
            player_ID = id;
            player_Score = score;
        }

        #endregion //Constructors



        #region Methods

        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }

        #endregion //Methods
    }

    class NOHunterPlayers : INotifyPropertyChanged
    {
        public static NOHunterPlayerInfo DummyPlayer = new NOHunterPlayerInfo(-1);
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<NOHunterPlayerInfo> players = new ObservableCollection<NOHunterPlayerInfo>();
        private NOHunterPlayerInfo masterPlayer = DummyPlayer;

        #region Properties

        public NOHunterPlayerInfo Master
        {
            get
            {
                return masterPlayer;
            }
            set
            {
                masterPlayer = value;
                OnPropertyChanged("Master");
            }
        }

        public ObservableCollection<NOHunterPlayerInfo> Players
        {
            get
            {
                return players;
            }
            set
            {
                players = value;
                OnPropertyChanged("Players");
            }
        }

        public int Count
        {
            get
            {
                return players.Count;
            }
        }

        #endregion //Properties



        #region Constructor

        public NOHunterPlayers()
        {
        }

        public NOHunterPlayers(NOHunterPlayerInfo master)
        {
            Master = master;
            Players.Add(Master);
        }

        #endregion //Constructor



        #region Methods

        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Remove a player known its ID
        /// </summary>
        /// <param name="id">player ID</param>
        /// <returns> 1/0 : player exists/not-exists </returns>
        public int Remove(int id)
        {
            // Find in the list
            var coincide = Players.FirstOrDefault(p => p.ID == id);
            if (coincide != null)
            {
                Players.Remove(coincide);
                return 1;
            }
            return 0;
        }


        /// <summary>
        /// Get score of a player
        /// </summary>
        /// <param name="id">The player</param>
        /// <returns>
        /// the score : if player exists
        /// 0 : if player not exists
        /// </returns>
        public int PlayerScore(int id)
        {
            var player = (from a in Players
                          where a.ID == id
                          select a).SingleOrDefault();
            if (player != null)
                return player.Score;
            return 0;
        }

        /// <summary>
        /// Add 1 point to the player's Score
        /// </summary>
        /// <param name="id">player ID</param>
        /// <returns> 1/0 : player exists/not-exists </returns>
        public int AddScore(int id)
        {
            // Find in the list
            var player = (from a in Players
                          where a.ID == id
                          select a).SingleOrDefault();
            if (player != null)
            {
                player.Score += 1;
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Set a normal player to be Master
        /// </summary>
        /// <param name="id">player ID</param>
        /// <returns> 1/0 : player exists/not-exists </returns>
        public int SetMaster(int id)
        {
            // Find in the list
            var newMaster = Players.FirstOrDefault(p => p.ID == id);
            if (newMaster != null)
            {
                Master = newMaster;
                return 1;
            }
            return 0;
        }

        #endregion //Methods
    }
}
