using System.Windows;
using Microsoft.MultiPoint.MultiPointSDK;

namespace NOHunterGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public MultiPointSDK MultiPointObject = MultiPointSDK.GetInstance();
    }
}
