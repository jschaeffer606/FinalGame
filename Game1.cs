
using FinalGameProject.Enemies;
using FinalGameProject.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace FinalGameProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        
        private TileMap _tileMap;

        private List<Tower> towers;
        private List<Enemy> enemies;
        private List<Vector2> waypoints;

        private Texture2D towerTexture;
        private Texture2D enemyTexture;
        private Texture2D projectileTexture;
        private Texture2D pathTexture;

        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private int resources;
        private int towerCost = 100;
        int timeBeforeNextSpawn;
        int enemiesLeft;
        int acceleration;
        int accelerationTimer;

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
            towers = new List<Tower>();
            enemies = new List<Enemy>();

            resources = 100;
            enemiesLeft = 10;

            acceleration = 1;
            accelerationTimer = 5;


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


            timeBeforeNextSpawn = 100;
            enemiesLeft = 50;

            _tileMap = new TileMap("tilemap.txt", waypoints);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _tileMap.LoadContent(Content);
            towerTexture = Content.Load<Texture2D>("tower");
            enemyTexture = Content.Load<Texture2D>("enemy");
            projectileTexture = Content.Load<Texture2D>("projectile");

            //1,32 here because for some reason 0,32 deletes the enemy
            
            font = Content.Load<SpriteFont>("FONT");
        }

        protected override void Update(GameTime gameTime)
        {
           


            if(timeBeforeNextSpawn <= 0 && enemiesLeft > 0)
            {
                enemies.Add(new Enemy(enemyTexture, new Vector2(1, 32), 30f, 5, waypoints));
                timeBeforeNextSpawn = 500;
                acceleration++;
                enemiesLeft--;
            }

            if(enemiesLeft <= 0 && enemies.Count <=0)
            {
                gameOver = true;
            }

            timeBeforeNextSpawn -= acceleration;



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                TryPlaceTower(mousePosition);
            }

            foreach (Tower tower in towers)
            {
                tower.Update(gameTime, enemies);

            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
                if(enemy.ReachedEnd)
                {
                    gameOver = true;
                }

            }
            int oldEnemiesCount = enemies.Count;
            enemies.RemoveAll(e => e.isDead);
            if(oldEnemiesCount > enemies.Count)
            {
                resources += 50;
            }
            
            if(gameTime.ElapsedGameTime.Seconds > accelerationTimer)
            {
                accelerationTimer *= 2;
                acceleration= (int)((float)acceleration*1.1);
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
                foreach (Tower tower in towers)
                {
                    tower.Draw(_spriteBatch);

                }

                string resourceText = $"Resources: {resources}      Enemies Left: {enemiesLeft}    Required Resources for Towers:{towerCost}";
                Vector2 position = new Vector2(10, 10);
                Color textColor = Color.Red;
                _spriteBatch.DrawString(font, resourceText, position, textColor);
            }
            
            base.Draw(gameTime);
            _spriteBatch.End();
        }


        private void TryPlaceTower(Vector2 position)
        {
            if (IsValidTowerPlacement(position) && resources >= towerCost)
            {
                towers.Add(new Tower(towerTexture, position, 1f, 200f, projectileTexture));
                resources -= towerCost;
            }
        }

        private bool IsValidTowerPlacement(Vector2 position)
        {
            if (position.X < 0 || position.Y < 0 || position.X > _graphics.GraphicsDevice.Viewport.Width || position.Y > _graphics.GraphicsDevice.Viewport.Height)
                return false;

            foreach (var tower in towers)
            {
                if (Vector2.Distance(tower.Position, position) < towerTexture.Width || Vector2.Distance(tower.Position, position) < towerTexture.Height)
                    return false;
            }

            return true;
        }




    }
}
