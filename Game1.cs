
using FinalGameProject.Enemies;
using FinalGameProject.Towers;
using FinalGameProject.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace FinalGameProject
{
    public enum TowerType
    {
        Null = 0,
        Regular = 1,
        Lightning = 2
    }

    public class WaveInformation
    {
        public int enemies { get; }
        public float acceleration { get; }
        public int toughness { get; }

        public WaveInformation(int e, float a, int t)
        {
            enemies = e;
            acceleration = a;
            toughness = t;
        }
    }


    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        private TileMap _tileMap;


        private List<WaveInformation> waveInfo = new();

        private int waveNumber => waveInfo.Count;

        private int waveIndex = 0;

        private BuildMenu buildMenu;

        private List<ITower> towers;
        private List<Enemy> enemies;
        private List<Vector2> waypoints;

        private Texture2D towerTexture;
        private Texture2D lightningTowerTexture;
        private Texture2D enemyTexture;
        private Texture2D projectileTexture;
        private Texture2D pathTexture;
        private Texture2D buttonTexture;
        public Texture2D ghostedTower;


        private MouseState currentMouseState;
        private MouseState previousMouseState;


        private bool[,] isPlacedArray = new bool[25, 15];




        private int toughness = 1;
        private int resources;
        private int towerCost = 150;
        private bool isPlacingTower;
        private float selectedRange;
        int timeBeforeNextSpawn;
        int enemiesLeft;
        int acceleration;
        int accelerationTimer;

        private TowerType towerTypeSelected = TowerType.Null;

        public bool gameOver = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            towers = new List<ITower>();
            enemies = new List<Enemy>();

            resources = 150;
            enemiesLeft = 10;

            acceleration = 1;
            accelerationTimer = 5;
            WaveInformation waveOne = new WaveInformation(15, 2f, 3);

            WaveInformation waveTwo = new WaveInformation(25, 3f, 5);

            WaveInformation waveThree = new WaveInformation(50, 4f, 10);

            WaveInformation waveFour = new WaveInformation(60, 8f, 12);





            waveInfo.Add(waveOne);
            waveInfo.Add(waveTwo);
            waveInfo.Add(waveThree);
            waveInfo.Add(waveFour);


            enemiesLeft = waveInfo[waveIndex].enemies;
            toughness = waveInfo[waveIndex].toughness;
            acceleration = (int)waveInfo[waveIndex].acceleration;



            waypoints = new List<Vector2>
            {
                new Vector2(0, 32),
                new Vector2(736, 32),
                new Vector2(736, 224),
                new Vector2(384, 224),
                new Vector2(384, 288),
                new Vector2(64, 288),
                new Vector2(64, 384),
                new Vector2(800, 384)

            };

            BlockOffTileBlocksFromTowers(waypoints);


            timeBeforeNextSpawn = 100;

            _tileMap = new TileMap("tilemap.txt", waypoints);
            base.Initialize();
        }

        private void BlockOffTileBlocksFromTowers(List<Vector2> positions)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                Vector2 vecOne = positions[i];
                Vector2 vecTwo = positions[i + 1];

                if (vecOne.X == vecTwo.X)
                {
                    for (int j = 0; j < Math.Abs(vecTwo.Y - vecOne.Y) / 32; j++)
                    {
                        isPlacedArray[(int)vecOne.X / 32, vecTwo.Y > vecOne.Y ? j + (int)vecOne.Y / 32 : (int)vecOne.Y / 32 - j] = true;

                    }
                }
                else
                {
                    for (int j = 0; j < Math.Abs(vecTwo.X - vecOne.X) / 32; j++)
                    {
                        isPlacedArray[vecTwo.X > vecOne.X ? j + (int)vecOne.X / 32 : (int)vecOne.X / 32 - j, (int)vecOne.Y / 32] = true;
                    }
                }
            }
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _tileMap.LoadContent(Content);
            towerTexture = Content.Load<Texture2D>("tower");
            lightningTowerTexture = Content.Load<Texture2D>("LightningTower");
            enemyTexture = Content.Load<Texture2D>("FullEnemySprites");
            projectileTexture = Content.Load<Texture2D>("projectile");
            font = Content.Load<SpriteFont>("FONT");

            buildMenu = new BuildMenu(font);
            buildMenu.AddButton(towerTexture, new Vector2(GraphicsDevice.Viewport.Width / 2 - 64, GraphicsDevice.Viewport.Height - 48), "Regular Tower: 150", () => StartPlacingTower(TowerType.Regular));
            buildMenu.AddButton(lightningTowerTexture, new Vector2(GraphicsDevice.Viewport.Width / 2 + 64, GraphicsDevice.Viewport.Height - 48), "Lightning Tower: 200", () => StartPlacingTower(TowerType.Lightning));




        }

        protected override void Update(GameTime gameTime)
        {



            if (timeBeforeNextSpawn <= 0 && enemiesLeft > 0)
            {
                enemies.Add(new Enemy(enemyTexture, new Vector2(1, 32), 30f, toughness, waypoints));
                timeBeforeNextSpawn = 500;
                enemiesLeft--;
            }

            if (enemiesLeft <= 0 && enemies.Count <= 0)
            {
                waveIndex++;
                resources += 50 * waveIndex;
                if (waveIndex == waveNumber)
                {
                    gameOver = true;
                }
                else
                {
                    enemiesLeft = waveInfo[waveIndex].enemies;
                    acceleration = (int)waveInfo[waveIndex].acceleration;
                    toughness = waveInfo[waveIndex].toughness;

                }
            }

            timeBeforeNextSpawn -= acceleration;


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            buildMenu.Update(gameTime);
            if (!buildMenu.Selecting && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                TryPlaceTower(mousePosition);
            }

            foreach (ITower tower in towers)
            {
                tower.Update(gameTime, enemies);

            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
                if (enemy.ReachedEnd)
                {
                    gameOver = true;
                }

            }
            int oldEnemiesCount = enemies.Count;
            enemies.RemoveAll(e => e.isDead);
            if (oldEnemiesCount > enemies.Count)
            {
                resources += 10;
            }

            if (gameTime.ElapsedGameTime.Seconds > accelerationTimer)
            {
                accelerationTimer *= 2;
                acceleration = (int)((float)acceleration * 1.1);
            }




            switch (towerTypeSelected)
            {

                case TowerType.Null: ghostedTower = null; break;
                case TowerType.Regular:
                    ghostedTower = towerTexture;
                    selectedRange = Tower.range;
                    break;
                case TowerType.Lightning:
                    ghostedTower = lightningTowerTexture;
                    selectedRange = LightningTower.range;
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Green);
            _spriteBatch.Begin();

            if (gameOver)
            {
                if (enemies.Count > 0)
                {
                    string resourceText = $"Game over, enemy has reached the end";
                    _tileMap.Draw(gameTime, _spriteBatch);
                    Vector2 position = new Vector2(10, 10);
                    Color textColor = Color.Yellow;
                    _spriteBatch.DrawString(font, resourceText, position, textColor);
                }
                else
                {
                    string resourceText = $"Congratulations, you have beaten the game!";
                    Vector2 position = new Vector2(10, 10);
                    Color textColor = Color.Yellow;
                    _spriteBatch.DrawString(font, resourceText, position, textColor);
                    _tileMap.Draw(gameTime, _spriteBatch);



                }

                _spriteBatch.End();
                return;
            }
            else
            {

                _tileMap.Draw(gameTime, _spriteBatch);
                foreach (Enemy enemy in enemies)
                {
                    enemy.Draw(_spriteBatch);
                }
                foreach (Vector2 waypoint in waypoints)
                {
                    _spriteBatch.Draw(enemyTexture, waypoint, new Rectangle(0, 0, 32, 32), Color.White, 0f, Vector2.Zero, .1f, SpriteEffects.None, 0.1f);


                }
                foreach (ITower tower in towers)
                {
                    tower.Draw(_spriteBatch);

                }

                string resourceText = $"Resources: {resources}      Enemies Left: {enemiesLeft}    Waves Left: {waveNumber - waveIndex}";

                Vector2 position = new Vector2(10, 10);
                Color textColor = Color.White;
                _spriteBatch.DrawString(font, resourceText, position, textColor);

                buildMenu.Draw(_spriteBatch);
                if (isPlacingTower)
                {
                    Texture2D pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1); pixel.SetData(new[] { Color.White });

                    _spriteBatch.Draw(ghostedTower, currentMouseState.Position.ToVector2(), Color.White * .25f);
                    DrawCircleOutline(_spriteBatch, currentMouseState.Position.ToVector2(), selectedRange, 1, Color.White);
                }
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }

        private void StartPlacingTower(TowerType towerType)
        {
            towerTypeSelected = towerType;
            isPlacingTower = true;
        }

        private void TryPlaceTower(Vector2 position)
        {
            if (isPlacingTower && IsValidTowerPlacement(position))
            {
                //Adjusts the position so that it is in mulitples of 32
                Vector2 adjustedPosition = new Vector2(((int)position.X / 32) * 32, ((int)position.Y / 32) * 32);

                switch (towerTypeSelected)
                {

                    case TowerType.Null: return;
                    case TowerType.Regular:
                        if (resources >= towerCost)
                        {
                            towers.Add(new Tower(towerTexture, adjustedPosition, 1f, projectileTexture));
                            resources -= towerCost;
                        }
                        break;
                    case TowerType.Lightning:
                        if (resources >= 200)
                        {
                            towers.Add(new LightningTower(lightningTowerTexture, adjustedPosition, 2f, projectileTexture));
                            resources -= 200;
                        }
                        break;
                }

                int xTile = (int)position.X / 32;
                int yTile = (int)position.Y / 32;
                isPlacedArray[xTile, yTile] = true;
            }
        }

        private bool IsValidTowerPlacement(Vector2 position)
        {
            int xTile = (int)position.X / 32;
            int yTile = (int)position.Y / 32;

            if (position.X < 0 || position.Y < 0 || position.X > _graphics.GraphicsDevice.Viewport.Width || position.Y > _graphics.GraphicsDevice.Viewport.Height || isPlacedArray[xTile, yTile])
                return false;

            return true;
        }


        public void DrawCircleOutline(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 center, float radius, int segments, Color color)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            double increment = Math.PI * 2.0 / segments;
            double theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                float x1 = (float)(radius * Math.Cos(theta)) + center.X;
                float y1 = (float)(radius * Math.Sin(theta)) + center.Y;
                float x2 = (float)(radius * Math.Cos(theta + increment)) + center.X;
                float y2 = (float)(radius * Math.Sin(theta + increment)) + center.Y;

                spriteBatch.Draw(pixel, new Vector2(x1, y1), color);

                theta += increment;
            }

        }



    }
}
