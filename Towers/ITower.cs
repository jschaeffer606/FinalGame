using FinalGameProject.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalGameProject.Towers
{
    public interface ITower
    {
        public Vector2 Position { get; }

        public void Update(GameTime gameTime, List<Enemy> enemies);

        public void Draw(SpriteBatch spriteBatch);





    }
}
