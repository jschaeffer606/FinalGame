using FinalGameProject.Enemies;
using FinalGameProject.ScreenManagement;
using FinalGameProject.Towers;
using FinalGameProject.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FinalGameProject.Screens
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

    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private ContentManager Content;
        private SpriteFont _gameFont;



        private float _pauseAlpha;
        private readonly InputAction _pauseAction;
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
        private Texture2D customCursorTexture;
        Texture2D pixel;

        private MouseState currentMouseState;
        private MouseState previousMouseState;


        private bool[,] isPlacedArray = new bool[25, 15];

        private SoundEffect turretFire;
        private SoundEffect lightningFire;
        private SoundEffect hurtSound;



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

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            towers = new List<ITower>();
            enemies = new List<Enemy>();

            resources = NewGameScreen.startingResources;

            customCursorTexture = Content.Load<Texture2D>("CustomCursor");

            acceleration = 1;
            accelerationTimer = 5;

            for (int i = 0; i < NewGameScreen.numRounds; i++)
            {
                waveInfo.Add(new WaveInformation(NewGameScreen.startingEnemyCount + i * NewGameScreen.enemyGainPerRound, 2 + 2 * i, 3 + NewGameScreen.hpFactor * i));
            }



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
            GraphicsDevice GraphicsDevice = ScreenManager.GraphicsDevice;

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            turretFire = Content.Load<SoundEffect>("Laser_Shoot2");
            lightningFire = Content.Load<SoundEffect>("sound");
            hurtSound = Content.Load<SoundEffect>("Hit_Hurt");
            pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);

            // TODO: use this.Content to load your game content here
            _tileMap.LoadContent(Content);
            towerTexture = Content.Load<Texture2D>("tower");
            lightningTowerTexture = Content.Load<Texture2D>("LightningTower");
            enemyTexture = Content.Load<Texture2D>("FullEnemySprites");
            projectileTexture = Content.Load<Texture2D>("projectile");
            font = Content.Load<SpriteFont>("FONT");

            buildMenu = new BuildMenu(font);
            buildMenu.AddButton(towerTexture, new Vector2(GraphicsDevice.Viewport.Width / 2 - 96, GraphicsDevice.Viewport.Height - 48), "Regular Tower: 150", () => StartPlacingTower(TowerType.Regular));
            buildMenu.AddButton(lightningTowerTexture, new Vector2(GraphicsDevice.Viewport.Width / 2 + 96, GraphicsDevice.Viewport.Height - 48), "Lightning Tower: 200", () => StartPlacingTower(TowerType.Lightning));


            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            Content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (timeBeforeNextSpawn <= 0 && enemiesLeft > 0)
                {
                    enemies.Add(new Enemy(enemyTexture, hurtSound, new Vector2(1, 32), NewGameScreen.enemySpeed + waveIndex, NewGameScreen.hpFactor, waypoints));
                    timeBeforeNextSpawn = 500;
                    enemiesLeft--;
                }

                if (enemiesLeft <= 0 && enemies.Count <= 0)
                {
                    waveIndex++;
                    resources += NewGameScreen.resourceGain * waveIndex;
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



                // TODO: Add your update logic here
                previousMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();
                buildMenu.Update(gameTime);
                if (!buildMenu.Selecting && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                    TryPlaceTower(mousePosition);
                }

                if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
                {
                    buildMenu.Selecting = false;
                    isPlacingTower = false;

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

            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                var movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                var thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                // _playerPosition += movement * 8f;
            }
        }


        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(Color.Green);
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

                string resourceText = $"Resources: {resources}      Enemies: {enemiesLeft + enemies.Count}    Waves Left: {waveNumber - waveIndex}";

                Vector2 position = new Vector2(10, 10);
                Color textColor = Color.White;
                _spriteBatch.DrawString(font, resourceText, position, textColor);

                buildMenu.Draw(_spriteBatch);
                if (isPlacingTower)
                {
                    Texture2D pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1); pixel.SetData(new[] { Color.White });

                    _spriteBatch.Draw(ghostedTower, currentMouseState.Position.ToVector2(), Color.White * .25f);
                    
                    DrawCircle(_spriteBatch, currentMouseState.Position.ToVector2() + new Vector2(16, 16), selectedRange, 100, Color.White);
                }
            }

            MouseState mouseState = Mouse.GetState();
            Vector2 cursorPosition = new Vector2(mouseState.X, mouseState.Y);
            _spriteBatch.Draw(customCursorTexture, cursorPosition, Color.White);
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
                            towers.Add(new Tower(towerTexture, turretFire, adjustedPosition, 1f, projectileTexture));
                            resources -= towerCost;
                        }
                        break;
                    case TowerType.Lightning:
                        if (resources >= 200)
                        {
                            towers.Add(new LightningTower(lightningTowerTexture, lightningFire, adjustedPosition, 2f, projectileTexture));
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

            if (position.X < 0 || position.Y < 0 || position.X > ScreenManager.GraphicsDevice.Viewport.Width || position.Y > ScreenManager.GraphicsDevice.Viewport.Height || isPlacedArray[xTile, yTile])
                return false;

            return true;
        }


        public void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, int segments, Color color)
        {
           
            pixel.SetData(new[] { Color.White });

            Vector2[] vertices = new Vector2[segments];
            double increment = Math.PI * 2.0 / segments;
            double theta = 0.0;

            for (int i = 0; i < segments; i++)
            {
                float x = (float)(radius * Math.Cos(theta)) + center.X;
                float y = (float)(radius * Math.Sin(theta)) + center.Y;
                vertices[i] = new Vector2(x, y);
                theta += increment;
            }

            for (int i = 0; i < segments - 1; i++)
            {
                Vector2 start = vertices[i];
                Vector2 end = vertices[i + 1];
                spriteBatch.Draw(pixel, start, null, color, (float)Math.Atan2(end.Y - start.Y, end.X - start.X), Vector2.Zero, new Vector2(Vector2.Distance(start, end), 1), SpriteEffects.None, 0.99f);
            }

            // Draw the last segment to complete the circle
            spriteBatch.Draw(pixel, vertices[segments - 1], null, color, (float)Math.Atan2(vertices[0].Y - vertices[segments - 1].Y, vertices[0].X - vertices[segments - 1].X), Vector2.Zero, new Vector2(Vector2.Distance(vertices[segments - 1], vertices[0]), 1), SpriteEffects.None, 0.99f);

        }

    }
}

