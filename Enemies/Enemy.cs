using FinalGameProject.Towers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalGameProject.Enemies
{

    public class Enemy
    {
        private Texture2D texture;
        private Vector2 position;
        private float velocity;
        private int health;

        public bool isDead;
        private List<Vector2> waypoints;
        private int currentWaypointIndex;

        public Vector2 Position { get => position; }
        public bool ReachedEnd { get; private set; }


        public Enemy(Texture2D texture, Vector2 position, float speed, int health, List<Vector2> waypoints)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = speed;
            this.health = health;
            this.currentWaypointIndex = 0;
            this.waypoints = waypoints;
            isDead = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!ReachedEnd) { MoveTowardsWaypoint(gameTime); }

        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, position, new Rectangle(0,0,32,32), Color.White,0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
        }

       


        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                // Handle enemy death
                isDead = true;

            }
        }

        private void MoveTowardsWaypoint(GameTime gameTime)
        {
            if (currentWaypointIndex < waypoints.Count)
            {
                Vector2 targetPosition = waypoints[currentWaypointIndex];
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Vector2.Distance(position, targetPosition) < velocity * (float)gameTime.ElapsedGameTime.TotalSeconds)
                {
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Count)
                        { ReachedEnd = true; }
                }
            }
        }
    }
}
