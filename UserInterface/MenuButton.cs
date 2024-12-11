using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace FinalGameProject.UserInterface
{
    public class MenuButton
    {
        private Texture2D texture;
        private Vector2 position;
        private Rectangle bounds;
        private string label;
        private SpriteFont font;
        private Action onClick;
        private bool isHovering;
        public bool clicked;

        public MenuButton(Texture2D texture, Vector2 position, string label, SpriteFont font, Action onClick)
        {
            this.texture = texture;
            this.position = position;
            this.label = label;
            this.font = font;

            this.onClick = onClick;
            bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            isHovering = bounds.Contains(mouseState.Position);

            if(isHovering && mouseState.LeftButton == ButtonState.Pressed)
            {
                onClick?.Invoke();
                clicked = true;
            }
            else
            {
                clicked = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var color = isHovering ? Color.Gray : Color.Gray * .5f;
            spriteBatch.Draw(texture, position, color);
            spriteBatch.DrawString(font, label, new Vector2(position.X - 15, position.Y -texture.Height), Color.White);
        }


    }
}
