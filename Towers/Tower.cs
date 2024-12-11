using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalGameProject.Enemies;
using FinalGameProject.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FinalGameProject.Towers
{
    public class Tower : ITower
    {
        private Texture2D texture;
        private Vector2 position;
        private float fireRate;
        public static float range { get; } = 150f;
        private float timeSinceLastShot;
        private float damage;
        private Texture2D projectileTexture; 
        private List<Projectile> projectiles;


        public Vector2 Position => position;

        public Tower(Texture2D texture, Vector2 position, float fireRate, Texture2D projectileTexture)
        {
            this.texture = texture;
            this.position = position;
            this.fireRate = fireRate;
            this.timeSinceLastShot = 0f;
            this.projectileTexture = projectileTexture;
            this.projectiles = new List<Projectile>();

        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastShot >= fireRate)
            {
                Shoot(enemies);
                timeSinceLastShot = 0f;
            }
            foreach (var projectile in projectiles)
            {
                projectile.Update(gameTime); 
            }
            projectiles.RemoveAll(p => !p.IsActive);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }

        }




        private void Shoot(List<Enemy> enemies)
        {
            if (enemies.Count == 0) return;
            Enemy target = null;

            foreach(Enemy e in enemies)
            {
                if (Vector2.Distance(e.Position, position) < range)
                {
                    target = e;
                    break;
                 }
            }
            
            
            if(target != null)
            {
                Projectile projectile = new Projectile(projectileTexture, new Vector2(position.X + 16, position.Y + 16), target.Position, 500f, 1, target);
                projectiles.Add(projectile);


            }
        }
    }
}
