using FinalGameProject.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace FinalGameProject.Projectiles
{
    public class Projectile
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 targetPosition;
        private float speed;
        private int damage;
        private bool isActive;
        private Enemy target;

        public bool IsActive => isActive;

        public Projectile(Texture2D texture, Vector2 startPosition, Vector2 targetPosition, float speed, int damage, Enemy target)
        {
            this.texture = texture;
            position = startPosition;
            this.targetPosition = targetPosition;
            this.speed = speed;
            this.damage = damage;
            isActive = true;

            this.target = target;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 direction = targetPosition - position;
            direction.Normalize();
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Vector2.Distance(position, targetPosition) < speed * (float)gameTime.ElapsedGameTime.TotalSeconds)
            {
                ApplyDamage(target);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }

        public void ApplyDamage(Enemy enemy)
        {
            if(enemy != null) enemy.TakeDamage(damage);
            isActive = false;
        }
    }
}


