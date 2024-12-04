using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalGameProject.Enemies
{
    public interface IEnemy
    {
        public bool IsDead { get; }

        public Vector2 Position { get; }

        public void TakeDamage(int damage);


         



    }
}
