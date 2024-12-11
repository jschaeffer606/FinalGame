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
    public class BuildMenu
    {
        private List<MenuButton> buttons;
        private SpriteFont font;

        public bool Selecting;


        public BuildMenu(SpriteFont font)
        {
            this.font = font;
            buttons = new List<MenuButton>();
        }


        public void AddButton(Texture2D texture, Vector2 position, string label,Action onClick)
        {
            buttons.Add(new MenuButton(texture, position, label, font, onClick));

        }
        
        public void Update(GameTime gameTime)
        {
            bool isclicked = false;
            foreach(var button in buttons)
            {
                button.Update(gameTime);
                if(button.clicked == true)
                {
                    isclicked = true;
                    
                }
            }
            Selecting = isclicked;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var button in buttons)
            {
                button.Draw(spriteBatch);
            }


        }



    }
}
