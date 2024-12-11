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
        private int hitTimer;
        

        public bool isDead;
        private List<Vector2> waypoints;
        private int currentWaypointIndex;

        public Vector2 Position { get => position; }
        public bool ReachedEnd { get; private set; }

        public bool IsHit { get; private set; } = false;
        private Rectangle spriteBounds;

        public Enemy(Texture2D texture, Vector2 position, float speed, int health, List<Vector2> waypoints)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = speed;
            this.health = health;
            this.currentWaypointIndex = 0;
            this.waypoints = waypoints;
            this.hitTimer = 5;
            spriteBounds = DetermineBounds(health);
            isDead = false;
        }

        public Rectangle DetermineBounds(int hp)
        {
            if(health <= 6)
            {

                return new Rectangle(0, (health - 1) * 32, 32, 32);


            }
            else if(health <=8)
            {
                return new Rectangle(0, 6 * 32, 32, 32);
            }
            else
            {
                return new Rectangle(0, 7 * 32, 32, 32);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!ReachedEnd) { MoveTowardsWaypoint(gameTime); }
            if (IsHit)
            {
                if (hitTimer == 0)
                {
                    IsHit = false;
                    hitTimer = 5;
                }
                else
                {
                    hitTimer--;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, spriteBounds, IsHit ? Color.White*.5f : Color.Red,0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
        }

       


        public void TakeDamage(int damage)
        {
            health -= damage;
            IsHit = true;
            hitTimer = 5;
            if (health <= 0)
            {
                // Handle enemy death
                isDead = true;
                

            }
            spriteBounds = DetermineBounds(health);
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
