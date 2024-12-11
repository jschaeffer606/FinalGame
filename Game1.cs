
using FinalGameExample;
using FinalGameProject.Enemies;
using FinalGameProject.ScreenManagement;
using FinalGameProject.Screens;
using FinalGameProject.Towers;
using FinalGameProject.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Media;


namespace FinalGameProject
{
    public class Game1 : Game
    {
        private readonly ScreenManager _screenManager;
        private GraphicsDeviceManager graphics;
        //private SpriteBatch _spriteBatch;
        //private SpriteFont font;

        //private TileMap _tileMap;


        //private List<WaveInformation> waveInfo = new();

        //private int waveNumber => waveInfo.Count;

        //private int waveIndex = 0;

        //private BuildMenu buildMenu;

        //private List<ITower> towers;
        //private List<Enemy> enemies;
        //private List<Vector2> waypoints;

        //private Texture2D towerTexture;
        //private Texture2D lightningTowerTexture;
        //private Texture2D enemyTexture;
        //private Texture2D projectileTexture;
        //private Texture2D pathTexture;
        //private Texture2D buttonTexture;
        //public Texture2D ghostedTower;


        //private MouseState currentMouseState;
        //private MouseState previousMouseState;


        //private bool[,] isPlacedArray = new bool[25, 15];




        //private int toughness = 1;
        //private int resources;
        //private int towerCost = 150;
        //private bool isPlacingTower;
        //private float selectedRange;
        //int timeBeforeNextSpawn;
        //int enemiesLeft;
        //int acceleration;
        //int accelerationTimer;

        //private TowerType towerTypeSelected = TowerType.Null;

        //public bool gameOver = false;


        private Song backgroundMusic;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            var screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            AddInitialScreens();
        }
        private void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
        }
         protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent() {
            backgroundMusic = Content.Load<Song>("Danish Mega Pony v. 4 OST - Track 02 (Fire Level)");
            // Play the background music and loop it
             MediaPlayer.IsRepeating = true;
             MediaPlayer.Play(backgroundMusic);
        }

        protected override void Update(GameTime gameTime)
        {


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Green);
            base.Draw(gameTime);
            
        }




    }
}
