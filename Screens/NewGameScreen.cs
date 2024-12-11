namespace FinalGameProject.Screens
{

    public enum Map
    {
        Snake,
        Waterfall
    }

    public class NewGameScreen : MenuScreen
    {


        

        private readonly MenuEntry _ungulateMenuEntry;
        private readonly MenuEntry _languageMenuEntry;
        private readonly MenuEntry _frobnicateMenuEntry;
        private readonly MenuEntry _elfMenuEntry;

        private readonly MenuEntry _MEnumRounds;
        private readonly MenuEntry _MEenemySpeed;
        private readonly MenuEntry _MEstartingEnemyCount;
        private readonly MenuEntry _MEenemyGainPerRound;
        private readonly MenuEntry _MEmapSelection;
        private readonly MenuEntry _MEenemyHPFactor;
        private readonly MenuEntry _MEstartingResources;
        private readonly MenuEntry _MEresourceGain;

        public static int numRounds  = 4;
        public static float enemySpeed  = 1;
        public static int startingEnemyCount = 15;
        public static int enemyGainPerRound = 10;
        public static Map mapSelected = Map.Snake;
        public static int hpFactor = 1;
        public static int startingResources = 150;
        public static int resourceGain = 25;



        public NewGameScreen() : base("New Game")
        {
            _MEnumRounds = new MenuEntry(string.Empty);
            _MEenemySpeed = new MenuEntry(string.Empty);
            _MEstartingEnemyCount = new MenuEntry(string.Empty);
            _MEenemyGainPerRound = new MenuEntry(string.Empty);
           // _MEmapSelection = new MenuEntry(string.Empty);
            _MEenemyHPFactor = new MenuEntry(string.Empty);
            _MEstartingResources = new MenuEntry(string.Empty);
            _MEresourceGain = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");
            var start = new MenuEntry("Start");

            _MEnumRounds.Selected += NumRoundsSelected;
            _MEenemySpeed.Selected += EnemySpeedSelected;
            _MEstartingEnemyCount.Selected += StartingEnemyCountSelected;
            _MEenemyGainPerRound.Selected += EnemyGainPerRoundSelected;
           // _MEmapSelection.Selected += MapSelectionSelected;
            _MEenemyHPFactor.Selected += EnemyHPFactorSelected;
            _MEstartingResources.Selected += StartingResourcesSelected;
            _MEresourceGain.Selected += ResourceGainSelected;
            back.Selected += OnCancel;
            start.Selected += PlayGameMenuEntrySelected;
            MenuEntries.Add(start);
            MenuEntries.Add(_MEnumRounds);
            MenuEntries.Add(_MEenemySpeed);
            MenuEntries.Add(_MEstartingEnemyCount);
            MenuEntries.Add(_MEenemyGainPerRound);
           // MenuEntries.Add(_MEmapSelection);
            MenuEntries.Add(_MEenemyHPFactor);
            MenuEntries.Add(_MEstartingResources);
            MenuEntries.Add(_MEresourceGain);
            MenuEntries.Add(back);
            
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            _MEnumRounds.Text = $"Total rounds: {numRounds}";
            _MEenemySpeed.Text = $"Enemy Speed Factor: {enemySpeed}x";
            _MEstartingEnemyCount.Text = $"Starting Enemy Count: {startingEnemyCount}";
            _MEenemyGainPerRound.Text = $"Enemy Gain Per Round: {enemyGainPerRound}";
          //  _MEmapSelection.Text = $"Map Selected: {mapSelected.ToString()}";
            _MEenemyHPFactor.Text = $"Enemy HP Factor: {hpFactor}";
            _MEstartingResources.Text = $"Starting Resources: {startingResources}";
            _MEresourceGain.Text = $"Resource Gain: {resourceGain}";



        }

        private void NumRoundsSelected(object sender, PlayerIndexEventArgs e)
        {
            numRounds++;

            if (numRounds > 15)
                numRounds = 0;

            SetMenuEntryText();
        }

        private void EnemySpeedSelected(object sender, PlayerIndexEventArgs e) { enemySpeed += 1; if (enemySpeed > 5) enemySpeed = 0; SetMenuEntryText(); }

        private void StartingEnemyCountSelected(object sender, PlayerIndexEventArgs e) { startingEnemyCount += 5; if (startingEnemyCount > 50) startingEnemyCount = 10; SetMenuEntryText(); }

        private void EnemyGainPerRoundSelected(object sender, PlayerIndexEventArgs e) { enemyGainPerRound++; if (enemyGainPerRound > 20) enemyGainPerRound = 5; SetMenuEntryText(); }
        private void MapSelectionSelected(object sender, PlayerIndexEventArgs e)
        {
            mapSelected++;
            if ((int)mapSelected > 1)
            {
                mapSelected = 0;
            }
            SetMenuEntryText();
        }

        private void EnemyHPFactorSelected(object sender, PlayerIndexEventArgs e) { hpFactor++; if (hpFactor > 3) hpFactor = 1; SetMenuEntryText(); }
        private void StartingResourcesSelected(object sender, PlayerIndexEventArgs e) { startingResources += 50; if (startingResources > 500) startingResources = 100; SetMenuEntryText(); }

        private void ResourceGainSelected(object sender, PlayerIndexEventArgs e) { resourceGain += 10; if (resourceGain > 100) resourceGain = 20; SetMenuEntryText(); }


    }
}
