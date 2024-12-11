using FinalGameProject.Enemies;
using FinalGameProject.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FinalGameProject.Towers
{
    public class LightningTower : ITower
    {
        private Texture2D texture;
        private Vector2 position;
        private float fireRate;
        public static float range { get; } = 100f;
        private float timeSinceLastShot;
        private int damage;
        private Texture2D projectileTexture;
        private List<Projectile> projectiles;
        private bool onCoolDown = false;
        private SoundEffect soundFire;
        

        public Vector2 Position => position;

        public LightningTower(Texture2D texture, SoundEffect s, Vector2 position, float fireRate, Texture2D projectileTexture)
        {
            this.texture = texture;
            this.position = position;
            this.fireRate = fireRate;
            this.timeSinceLastShot = 0f;
            soundFire = s;
            damage = 1;
            this.projectileTexture = projectileTexture;
            this.projectiles = new List<Projectile>();

        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastShot >= fireRate)
            {
                foreach(var enemy in enemies)
                {
                    if(Vector2.Distance(position, enemy.Position) <= range)
                    {
                        soundFire.Play();
                        timeSinceLastShot = 0f;
                        enemy.TakeDamage((int)damage);
                        onCoolDown = true;
                    }
                }
            }
            else
            {
                onCoolDown = false;
            }
            
            projectiles.RemoveAll(p => !p.IsActive);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, onCoolDown ? Color.Cyan * .5f: Color.Cyan);
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }




        private void Shoot(List<Enemy> enemies)
        {
            if (enemies.Count == 0) return;
            Enemy target = null;

            foreach (Enemy e in enemies)
            {
                if (Vector2.Distance(e.Position, position) < range)
                {
                    target = e;
                    break;
                }
            }


            if (target != null)
            {
                Projectile projectile = new Projectile(projectileTexture, new Vector2(position.X + 16, position.Y + 16), target.Position, 500f, 1, target);
                projectiles.Add(projectile);


            }
        }






    }
}
