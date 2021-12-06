using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using BattleShips.Customs;

namespace BattleShips.UI
{
    public partial class PlayWindow : Window
    {
        private AiBehav aiBehav = new();
        private ManualPlacer manualPlacer = new();
        private ShootChecker shotChecker = new();
  
        private GameSave gameSave = new();
        private Coordinate[] p1Ships = new Coordinate[12], p2Ships = new Coordinate[12],
            manualCords = new Coordinate[2], p1Shots = new Coordinate[36], p2Shots = new Coordinate[36],
            p1Hits = new Coordinate[12], p2Hits = new Coordinate[12];
        private Coordinate aiShot = new();

        private bool aiShipsCreated, p1ShipsCreated, gameEnded, fogHidden;
        private string currentPlayer, startingPlayer;
        private int manualChoosen, shipsPlaced, p1HitsNum, p2HitsNum, p1ShotNum, p2ShotNum, missesInaRow;

        public PlayWindow()
        {
            InitializeComponent();
            TopLabel.Content = "Coose game mode!";
            PvAIB.Visibility = Visibility.Visible;
            PvPB.Visibility = Visibility.Visible;
            aiShipsCreated = false;
            p1ShipsCreated = false;
            gameEnded = false;
            currentPlayer = "p1";
            startingPlayer = "";
            manualChoosen = 0;
            shipsPlaced = 0;
            p1HitsNum = 0;
            p2HitsNum = 0;
            p1ShotNum = 0;
            p2ShotNum = 0;
            gameSave = new();
            gameSave.rounds = 1;
            missesInaRow = 0;
            fogHidden = false;
        }

        private void PlayW_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Visibility = Visibility.Collapsed;
            mainWindow.Show();  
        }

        private void HIdeAiFog(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && gameSave.gameMode == "PvAi" && Keyboard.IsKeyDown(Key.F))
            {
                P2Fog.Visibility = Visibility.Hidden;
                fogHidden = true;
            }
        }

        private void ShowAiFog(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && gameSave.gameMode == "PvAi" && Keyboard.IsKeyDown(Key.F))
            {
                P2Fog.Visibility = Visibility.Visible;
                fogHidden = false;
            }
        }

        private void P1SetupClick(object sender, RoutedEventArgs e)
        {
            if (!p1ShipsCreated && manualChoosen<2)
            {
                Button? button = sender as Button;
                string[] cordsS = button.Content.ToString().Split('_');
                Coordinate manCord = new();
                manCord.R = Int32.Parse(cordsS[0]);
                manCord.C = Int32.Parse(cordsS[1]);
                manualCords[manualChoosen] = manCord;
                manualChoosen++;
                button.Background = new SolidColorBrush(Colors.Gray);
                if (manualChoosen == 2)
                {
                    PlaceShip();
                }
            }
        }

        private void P2SetupClick(object sender, RoutedEventArgs e)
        {
            if (!aiShipsCreated && manualChoosen < 2)
            {
                Button? button = sender as Button;
                string[] cordsS = button.Content.ToString().Split('_');
                Coordinate manCord = new();
                manCord.R = Int32.Parse(cordsS[0]);
                manCord.C = Int32.Parse(cordsS[1]);
                manualCords[manualChoosen] = manCord;
                manualChoosen++;
                button.Background = new SolidColorBrush(Colors.Gray);
                if (manualChoosen == 2)
                {
                    PlaceShip();
                }
            }
        }

        private void GameStart()
        {
            PvAIB.Visibility = Visibility.Collapsed;
            PvPB.Visibility = Visibility.Collapsed;
            P1NameTB.Visibility = Visibility.Visible;
            P1SaveB.Visibility = Visibility.Visible;
        }

        private void PvAIB_Click(object sender, RoutedEventArgs e)
        {
            gameSave.gameMode = "PvAi";
            gameSave.player2 = "Ai";
            GameStart();
        }

        private void PvPB_Click(object sender, RoutedEventArgs e)
        {
            gameSave.gameMode = "PvP";
            GameStart();
        }

        private void SaveP1_Click(object sender, RoutedEventArgs e)
        {
            if (GoodName(P1NameTB.Text))
            {
                gameSave.player1 = P1NameTB.Text;
                P1NameTB.Visibility = Visibility.Hidden;
                P1SaveB.Visibility = Visibility.Hidden;
                if (gameSave.gameMode == "PvP")
                {
                    P2NameTB.Visibility = Visibility.Visible;
                    P2SaveB.Visibility = Visibility.Visible;
                }
                else
                {
                    ChooseStarterAi();
                }
            }
            else
            {
                MessageBox.Show("Enter only letters and/or numbers!");
                P1NameTB.Text = "Name";
            }
        }

        private void SaveP2_Click(object sender, RoutedEventArgs e)
        {
            if (GoodName(P2NameTB.Text))
            {
                gameSave.player2 = P2NameTB.Text;
                P2NameTB.Visibility = Visibility.Hidden;
                P2SaveB.Visibility = Visibility.Hidden;
                ChooseStarterPvP();
            }
            else
            {
                MessageBox.Show("Enter only letters and/or numbers!");
                P2NameTB.Text = "Name2";
            }
        }

        private bool GoodName(string name)
        {
            bool correct = false;
            string cleanName = name;
            if (cleanName != "")
            {
                cleanName = cleanName.ToLower();
                cleanName = Regex.Replace(cleanName, "[0-9]", "");
                cleanName = Regex.Replace(cleanName, "[a-z]", "");
                if (cleanName == "")
                {
                    correct = true;
                }
            }
            return correct;
        }

        private void ChooseStarterAi()
        {
            Coordinate[] aiGenerated = aiBehav.GenerateShipsAi();
            p2Ships = aiGenerated;
            SetAiShips(12);
            var rand = new Random();
            int num = rand.Next(0, 2);
            if (num == 0)
            {
                startingPlayer = "p1";
                MessageBox.Show("You start the game!");
            }
            else
            {
                startingPlayer = "p2";
                MessageBox.Show("The Ai starts the game!");
            }
            currentPlayer = "p1";
            TopLabel.Content = gameSave.player1 + " coose ship\nplacement mode!";
            AutoSetupB.Visibility = Visibility.Visible;
            ManualSetupB.Visibility = Visibility.Visible;
        }

        private void ChooseStarterPvP()
        {
            var rand = new Random();
            int num = rand.Next(0, 2);
            if (num == 0)
            {
                MessageBox.Show(gameSave.player1 + " starts the game!");
                TopLabel.Content = gameSave.player1 + " coose ship\nplacement mode!";
                AutoSetupB.Visibility = Visibility.Visible;
                ManualSetupB.Visibility = Visibility.Visible;
                currentPlayer = "p1";
                startingPlayer = "p1";
            }
            else
            {
                MessageBox.Show(gameSave.player2 + " starts the game!");
                TopLabel.Content = gameSave.player2 + " coose ship\nplacement mode!";
                AutoSetupB.Visibility = Visibility.Visible;
                ManualSetupB.Visibility = Visibility.Visible;
                currentPlayer = "p2";
                startingPlayer = "p2";
            }
        }

        private void AutoSetup_Click(object sender, RoutedEventArgs e)
        {
            switch (currentPlayer)
            {
                case "p1":
                    Coordinate[] p1Generated = aiBehav.GenerateShipsP1();
                    p1Ships = p1Generated;
                    SetP1Ships(12);
                    if (gameSave.gameMode == "PvAi")
                    {
                        ShipSetupReset();
                        P1Fog.Visibility = Visibility.Hidden;
                    }
                    else if (startingPlayer == "p2")
                    {
                        currentPlayer = "p2";
                        ShipSetupReset();
                        P2Fog.Visibility = Visibility.Hidden;
                    }
                    else if (startingPlayer == "p1")
                    {
                        currentPlayer = "p2";
                        TopLabel.Content = gameSave.player2 + " coose ship\nplacement mode!";
                    }
                    break;
                case "p2":
                    Coordinate[] p2Generated = aiBehav.GenerateShipsAi();
                    p2Ships = p2Generated;
                    SetAiShips(12);
                    if (startingPlayer == "p2")
                    {
                        currentPlayer = "p1";
                        TopLabel.Content = gameSave.player1 + " coose ship\nplacement mode!";
                    }
                    else if (startingPlayer == "p1")
                    {
                        currentPlayer = "p1";
                        ShipSetupReset();
                        P1Fog.Visibility = Visibility.Hidden;
                    }
                    break;
            }   
        }

        private void ManualSetup_Click(object sender, RoutedEventArgs e)
        {
            switch (currentPlayer)
            {
                case "p1":
                    ShipSetupReset();
                    TopLabel.Content = "Place Carrier(4)\n(first 2 places)";
                    P1Fog.Visibility = Visibility.Hidden;
                    shipsPlaced = 0;
                    break;
                case "p2":
                    ShipSetupReset();
                    TopLabel.Content = "Place Carrier(4)\n(first 2 places)";
                    P2Fog.Visibility = Visibility.Hidden;
                    shipsPlaced = 0;
                    break;
            }
        }

        private void SetP1Ships(int ships)
        {
            for (int i = 0; i < ships; i++)
            {
                Button button = (Button)P1Own.FindName("P1Field_" + p1Ships[i].R + "_" + p1Ships[i].C);
                button.Background = new SolidColorBrush(Colors.Black);
            }
            if (ships == 12)
            {
                p1ShipsCreated = true;
            }
        }

        private void SetAiShips(int ships)
        {
            for (int i = 0; i < ships; i++)
            {
                Button button = (Button)P2Own.FindName("P2Field_" + p2Ships[i].R + "_" + p2Ships[i].C);
                button.Background = new SolidColorBrush(Colors.Black);
            }
            if (ships == 12)
            {
                aiShipsCreated = true;
            }
        }

        private void ShipSetupReset()
        {
            TopLabel.Content = "Round "+ gameSave.rounds;
            AutoSetupB.Visibility = Visibility.Hidden;
            ManualSetupB.Visibility = Visibility.Hidden;
        }

        private void ManualReset(int shipNum, string message)
        {
            MessageBox.Show(message);
            manualChoosen = 0;
            if (currentPlayer == "p1")
            {
                Button? enemyB = (Button)P1Own.FindName("P1Field_" + manualCords[0].R + "_" + manualCords[0].C);
                enemyB.Background = new SolidColorBrush(Color.FromRgb(100, 152, 255));
                Button? enemyB2 = (Button)P1Own.FindName("P1Field_" + manualCords[1].R + "_" + manualCords[1].C);
                enemyB2.Background = new SolidColorBrush(Color.FromRgb(100, 152, 255));
                SetP1Ships(shipNum);
            }
            else
            {
                Button? enemyB = (Button)P2Own.FindName("P2Field_" + manualCords[0].R + "_" + manualCords[0].C);
                enemyB.Background = new SolidColorBrush(Color.FromRgb(100, 152, 255));
                Button? enemyB2 = (Button)P2Own.FindName("P2Field_" + manualCords[1].R + "_" + manualCords[1].C);
                enemyB2.Background = new SolidColorBrush(Color.FromRgb(100, 152, 255));
                SetAiShips(shipNum);
            }
        }

        private void PlaceShip()
        {
            switch (shipsPlaced)
            {
                case 0:
                    PlaceCarrier();
                    break;
                case 1:
                    PlaceDestr(4);
                    break;
                case 2:
                    PlaceDestr(7);
                    break;
                case 3:
                    PlaceHunter();
                    break;
            }
        }

        private void PlaceCarrier()
        {
            if (!manualCords[0].Equals(manualCords[1]))
            {
                if (manualPlacer.CarrPlaceCheck(manualCords[0], manualCords[1]))
                {
                    if (currentPlayer == "p1")
                    {
                        Coordinate[] carrCords = manualPlacer.Carrier(manualCords[0], manualCords[1]);
                        if (!carrCords[0].Equals(new Coordinate()))
                        {
                            p1Ships[0] = carrCords[0];
                            p1Ships[1] = carrCords[1];
                            p1Ships[2] = carrCords[2];
                            p1Ships[3] = carrCords[3];
                            SetP1Ships(4);
                            manualChoosen = 0;
                            shipsPlaced++;
                            TopLabel.Content = "Place Destroyer(3)\n(first 2 places)";
                        }
                        else
                        {
                            ManualReset(0, "Collision with existing ship!");
                        }
                    }
                    else if (currentPlayer == "p2")
                    {
                        Coordinate[] carrCords = manualPlacer.Carrier(manualCords[0], manualCords[1]);
                        if (!carrCords[0].Equals(new Coordinate()))
                        {
                            p2Ships[0] = carrCords[0];
                            p2Ships[1] = carrCords[1];
                            p2Ships[2] = carrCords[2];
                            p2Ships[3] = carrCords[3];
                            SetAiShips(4);
                            manualChoosen = 0;
                            shipsPlaced++;
                            TopLabel.Content = "Place Destroyer(3)\n(first 2 places)";
                        }
                        else
                        {
                            ManualReset(0, "Collision with existing ship!");
                        }
                    }
                }
                else
                {
                    ManualReset(0, "Wrong coordinates");
                }
            }
            else
            {
                ManualReset(0, "Give separate coordinates!");
            }
            
        }

        private void PlaceDestr(int shipNum)
        {
            if (!manualCords[0].Equals(manualCords[1]))
            {
                if (manualPlacer.DestrPlaceCheck(manualCords[0], manualCords[1]))
                {
                    if (currentPlayer == "p1")
                    {
                        Coordinate[] coordinates = manualPlacer.Destroyer(manualCords[0], manualCords[1]);
                        if (!manualPlacer.CollisionCheck(coordinates, p1Ships) && !coordinates[0].Equals(new Coordinate()))
                        {
                            p1Ships[shipNum] = coordinates[0];
                            p1Ships[shipNum + 1] = coordinates[1];
                            p1Ships[shipNum + 2] = coordinates[2];
                            SetP1Ships(shipNum + 3);
                            manualChoosen = 0;
                            shipsPlaced++;
                            if(shipsPlaced == 2)
                            {
                                TopLabel.Content = "Place Destroyer(3)\n(first 2 places)";
                            }
                            else
                            {
                                TopLabel.Content = "Place Hunter(2)\n(both places)";
                            }
                        }
                        else
                        {
                            ManualReset(shipNum, "Collision with existing ship!");
                        }
                    }
                    else if (currentPlayer == "p2")
                    {
                        Coordinate[] coordinates = manualPlacer.Destroyer(manualCords[0], manualCords[1]);
                        if (!manualPlacer.CollisionCheck(coordinates, p2Ships) && !coordinates[0].Equals(new Coordinate()))
                        {
                            p2Ships[shipNum] = coordinates[0];
                            p2Ships[shipNum + 1] = coordinates[1];
                            p2Ships[shipNum + 2] = coordinates[2];
                            SetAiShips(shipNum + 3);
                            manualChoosen = 0;
                            shipsPlaced++;
                            if (shipsPlaced == 2)
                            {
                                TopLabel.Content = "Place Destroyer(3)\n(first 2 places)";
                            }
                            else
                            {
                                TopLabel.Content = "Place Hunter(2)\n(both places)";
                            }
                        }
                        else
                        {
                            ManualReset(shipNum, "Collision with existing ship!");
                        }
                    }
                }
                else
                {
                    ManualReset(shipNum, "Wrong coordinates");
                }
            }
            else
            {
                ManualReset(shipNum, "Give separate coordinates!");
            }
            
        }

        private void PlaceHunter()
        {
            if (!manualCords[0].Equals(manualCords[1]))
            {
                if (manualPlacer.HuntPlaceCheck(manualCords[0], manualCords[1]))
                {
                    if (currentPlayer == "p1")
                    {
                        Coordinate[] coordinates = manualPlacer.Hunter(manualCords[0], manualCords[1]);
                        if (!manualPlacer.CollisionCheck(coordinates, p1Ships) && !coordinates[0].Equals(new Coordinate()))
                        {
                            p1Ships[10] = coordinates[0];
                            p1Ships[11] = coordinates[1];
                            ManualSetupFinish();
                            shipsPlaced++;
                        }
                        else
                        {
                            ManualReset(10, "Collision with existing ship!");
                        }
                    }
                    else if (currentPlayer == "p2")
                    {
                        Coordinate[] coordinates = manualPlacer.Hunter(manualCords[0], manualCords[1]);
                        if (!manualPlacer.CollisionCheck(coordinates, p2Ships) && !coordinates[0].Equals(new Coordinate()))
                        {
                            p2Ships[10] = coordinates[0];
                            p2Ships[11] = coordinates[1];
                            ManualSetupFinish();
                            shipsPlaced++;
                        }
                        else
                        {
                            ManualReset(10, "Collision with existing ship!");
                        }
                    }
                }
                else
                {
                    ManualReset(10, "Wrong coordinates");
                }
            }
            else
            {
                ManualReset(10, "Give separate coordinates!");
            } 
        }
        
        private void ManualSetupFinish()
        {
            switch (currentPlayer)
            {
                case "p1":
                    SetP1Ships(12);
                    if (gameSave.gameMode == "PvAi")
                    {
                        ShipSetupReset();
                        P1Fog.Visibility = Visibility.Hidden;
                    }
                    else if (startingPlayer == "p2")
                    {
                        currentPlayer = "p2";
                        ShipSetupReset();
                        P1Fog.Visibility = Visibility.Visible;
                        P2Fog.Visibility = Visibility.Hidden;
                    }
                    else if (startingPlayer == "p1")
                    {
                        P1Fog.Visibility= Visibility.Visible;
                        currentPlayer = "p2";
                        TopLabel.Content = gameSave.player2 + " coose ship\nplacement mode!";
                        AutoSetupB.Visibility = Visibility.Visible;
                        ManualSetupB.Visibility = Visibility.Visible;
                        manualChoosen = 0;
                        shipsPlaced = 0;
                    }
                    break;
                case "p2":
                    SetAiShips(12);
                    if (startingPlayer == "p2")
                    {
                        P2Fog.Visibility = Visibility.Visible;
                        currentPlayer = "p1";
                        TopLabel.Content = gameSave.player1 + " coose ship\nplacement mode!";
                        AutoSetupB.Visibility = Visibility.Visible;
                        ManualSetupB.Visibility = Visibility.Visible;
                        manualChoosen = 0;
                        shipsPlaced = 0;
                    }
                    else if (startingPlayer == "p1")
                    {
                        currentPlayer = "p1";
                        ShipSetupReset();
                        P1Fog.Visibility = Visibility.Hidden;
                        P2Fog.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        //END OF SETUP PHASE, START OF GAMEPLAY PHASE
        //============================================

        private void P1AttackClick(object sender, RoutedEventArgs e)
        {
            if(currentPlayer == "p1" && !gameEnded && !fogHidden)
            {
                Button? button = sender as Button;
                string[] cordsS = button.Content.ToString().Split('_');
                Coordinate coordinate = new();
                coordinate.R = Int32.Parse(cordsS[0]);
                coordinate.C = Int32.Parse(cordsS[1]);
                if (!shotChecker.ShotMatch(coordinate, p1Shots))
                {
                    p1Shots[p1ShotNum++] = coordinate;
                    Button? enemyB = (Button)P2Own.FindName("P2Field_" + coordinate.R + "_" + coordinate.C);
                    bool hit = shotChecker.ShotMatch(coordinate, p2Ships);
                    if (hit)
                    {
                        p1Hits[p1HitsNum++] = coordinate;
                        button.Background = new SolidColorBrush(Colors.Red);
                        enemyB.Background = new SolidColorBrush(Colors.Red);
                        string ship = "Hit";
                        if (p1HitsNum == 12)
                        {
                            gameEnded = true;
                            ship = "Win";
                        }
                        else{
                            ship = shotChecker.HitCheck(coordinate ,p1Hits, p2Ships); 
                        }
                        ShotMessage(ship);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        enemyB.Background = new SolidColorBrush(Colors.White);
                        ShotMessage("Miss");
                    }

                    P1NextRound();
                }
                else
                {
                    MessageBox.Show("You already shot there!");
                }
            }
        }

        private void P1NextRound()
        {
            if (!gameEnded)
            {
                if (startingPlayer == "p2")
                {
                    gameSave.rounds++;
                    TopLabel.Content = "Round " + gameSave.rounds;
                }
                currentPlayer = "p2";
                if (gameSave.gameMode == "PvP")
                {
                    P1Fog.Visibility = Visibility.Visible;
                    P2Fog.Visibility = Visibility.Hidden;
                }
                else
                {
                    AiAttack();
                }
            }
            else
            {
                gameSave.winner = gameSave.player1;
                gameSave.p1Hits = p1HitsNum;
                gameSave.p2Hits = p2HitsNum;
                GameEnded();
            }
        }

        private void AiAttack()
        {
            if(!aiShot.Equals(new Coordinate()))
            {
                aiShot = aiBehav.Attack(aiShot, p2Hits, p2Shots, missesInaRow);
            }
            else
            {
                aiShot = aiBehav.Attack(new Coordinate(7,7), p2Hits, p2Shots, missesInaRow);
            }
            p2Shots[p2ShotNum++] = aiShot;
            Button? enemyB = (Button)P1Own.FindName("P1Field_" + aiShot.R + "_" + aiShot.C);
            if (shotChecker.ShotMatch(aiShot, p1Ships))
            {
                missesInaRow = 0;
                p2Hits[p2HitsNum++] = aiShot;
                enemyB.Background = new SolidColorBrush(Colors.Red);
                if(p2HitsNum == 12)
                {
                    gameEnded = true;
                    ShotMessage("Win");
                }
            }
            else
            {
                missesInaRow++;
                enemyB.Background = new SolidColorBrush(Colors.White);
            }
            AiNextRound();
        }

        private void AiNextRound()
        {
            if (!gameEnded)
            {
                if (startingPlayer == "p1")
                {
                    gameSave.rounds++;
                    TopLabel.Content = "Round " + gameSave.rounds;
                }
                currentPlayer = "p1";
            }
            else
            {
                gameSave.winner = gameSave.player2;
                gameSave.p1Hits = p1HitsNum;
                gameSave.p2Hits = p2HitsNum;
                GameEnded();
            }
        }

        private void P2AttackClick(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == "p2" && !gameEnded)
            {
                Button? button = sender as Button;
                string[] cordsS = button.Content.ToString().Split('_');
                Coordinate coordinate = new();
                coordinate.R = Int32.Parse(cordsS[0]);
                coordinate.C = Int32.Parse(cordsS[1]);
                if (!shotChecker.ShotMatch(coordinate, p2Shots))
                {
                    p2Shots[p2ShotNum++] = coordinate;
                    Button? enemyB = (Button)P1Own.FindName("P1Field_" + coordinate.R + "_" + coordinate.C);
                    bool hit = shotChecker.ShotMatch(coordinate, p1Ships);
                    if (hit)
                    {
                        p2Hits[p2HitsNum++] = coordinate;
                        button.Background = new SolidColorBrush(Colors.Red);
                        enemyB.Background = new SolidColorBrush(Colors.Red);
                        string ship = "Hit";
                        if (p2HitsNum == 12)
                        {
                            gameEnded = true;
                            ship = "Win";
                        }
                        else
                        {
                            ship = shotChecker.HitCheck(coordinate, p2Hits, p1Ships);
                        }
                        ShotMessage(ship);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.White);
                        enemyB.Background = new SolidColorBrush(Colors.White);
                        ShotMessage("Miss");
                    }
                    P2NextRound();
                }
                else
                {
                    MessageBox.Show("You already shot there!");
                }
            }
        }

        private void P2NextRound()
        {
            if (!gameEnded)
            {
                if (startingPlayer == "p1")
                {
                    gameSave.rounds++;
                    TopLabel.Content = "Round " + gameSave.rounds;
                }
                currentPlayer = "p1";
                P2Fog.Visibility = Visibility.Visible;
                P1Fog.Visibility = Visibility.Hidden;
            }
            else
            {
                gameSave.winner = gameSave.player2;
                gameSave.p1Hits = p1HitsNum;
                gameSave.p2Hits = p2HitsNum;
                GameEnded();
            }
        }

        private void ShotMessage(string message)
        {
            switch (message)
            {
                case "Carrier":
                    MessageBox.Show("Hit, Carrier sank!");
                    break;
                case "Destroyer":
                    MessageBox.Show("Hit, Destroyer sank!");
                    break;
                case "Hunter":
                    MessageBox.Show("Hit, Hunter sank!");
                    break;
                case "Hit":
                    MessageBox.Show("Hit!");
                    break;
                case "Miss":
                    MessageBox.Show("Miss!");
                    break;
                case "Win":
                    if(currentPlayer == "p1")
                    {
                        MessageBox.Show("All ships destroyed, " + gameSave.player1 + " won!");
                    }
                    else
                    {
                        MessageBox.Show("All ships destroyed, " + gameSave.player2 + " won!");
                    }    
                    break;
                default:
                    MessageBox.Show("Shot!");
                    break;
            }
        }

        private async void GameEnded()
        {
            await Save();
            P1Fog.Visibility = Visibility.Hidden;
            P2Fog.Visibility = Visibility.Hidden;
            TopLabel.Content = gameSave.winner + " won\nthe game!";
            HomeB.Visibility = Visibility.Visible;
        }

        private Task Save()
        {
            return Task.Factory.StartNew(() =>
            {
                string path = @"GameSaves.json";
                ListOfGameSaves listOfGameSaves = new();
                if (File.Exists(path))
                {
                    string gameSaves = File.ReadAllText(@"GameSaves.json");
                    listOfGameSaves = JsonConvert.DeserializeObject<ListOfGameSaves>(gameSaves);
                    List<GameSave> gameSavesList = listOfGameSaves.listOfGameSaves.ToList();
                    gameSavesList.Add(gameSave);
                    listOfGameSaves.listOfGameSaves = gameSavesList.ToArray();
                    string newSave = JsonConvert.SerializeObject(listOfGameSaves);
                    File.WriteAllText(path, newSave);
                }
                else
                {
                    List<GameSave> gameSavesList = new List<GameSave>();
                    gameSavesList.Add(gameSave);
                    listOfGameSaves.listOfGameSaves = gameSavesList.ToArray();
                    string newSave = JsonConvert.SerializeObject(listOfGameSaves);
                    File.WriteAllText(path, newSave);
                }
            });
        }


        private void HomeB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Visibility = Visibility.Collapsed;
            mainWindow.Show();
        }
    }
}
