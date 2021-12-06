using BattleShips.Customs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BattleShips.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PlayB_Click(object sender, RoutedEventArgs e)
        {
            UI.PlayWindow playWindow = new UI.PlayWindow();
            this.Visibility = Visibility.Hidden;
            playWindow.Show();
        }

        private void HSB_Click(object sender, RoutedEventArgs e)
        {
            BackB.Visibility = Visibility.Visible;
            HSPanel.Visibility = Visibility.Visible;
            PlayB.Visibility = Visibility.Hidden;
            HSB.Visibility = Visibility.Hidden;
            BSLabel.Visibility = Visibility.Hidden;
            PopulateListBox();
        }

        private void BackB_Click(object sender, RoutedEventArgs e)
        {
            BackB.Visibility = Visibility.Hidden;
            HSPanel.Visibility = Visibility.Hidden;
            PlayB.Visibility = Visibility.Visible;
            HSB.Visibility = Visibility.Visible;
            BSLabel.Visibility = Visibility.Visible;
        }

        public void PopulateListBox()
        {
            List<Customs.GameSave> gameSaves = LoadGameSaves();
            List<GroupBox> gameSavesString = GameSavesConvert(gameSaves);
            HighScoresLB.ItemsSource = gameSavesString;
        }

        private List<Customs.GameSave> LoadGameSaves()
        {
            string path = @"GameSaves.json";
            List<Customs.GameSave> gameSavesList = new();
            if (File.Exists(path))
            {
                string gameSaves = File.ReadAllText(@"GameSaves.json");
                Customs.ListOfGameSaves listOfGameSaves = new();
                listOfGameSaves = JsonConvert.DeserializeObject<Customs.ListOfGameSaves>(gameSaves);
                gameSavesList = listOfGameSaves.listOfGameSaves.ToList();
            }
            return gameSavesList;
        }

        private List<GroupBox> GameSavesConvert(List<GameSave> gs)
        {
            List<GroupBox> gsString = new List<GroupBox>();
            for(int i = 0; i < gs.Count; i++)
            {
                scoresLB = new GroupBox();
                scoresLB.Header = gs[i].winner;
                string line = (gs[i].player1 + " vs. " + gs[i].player2 + " (" + 
                    gs[i].p1Hits+ "-" + gs[i].p2Hits+") in "+ gs[i].rounds+" rounds");
                scoresLB.Content = line;
                scoresLB.Background = new SolidColorBrush(Color.FromRgb(125,200,255));     
                gsString.Add(scoresLB);
            }
            return gsString;
        }
    }
}
