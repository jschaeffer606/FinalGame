using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
namespace FinalGameProject
{
    public class TileMap
    {

        /// <summary>
        /// The dimensions of the tileset and hte map
        /// </summary>
        int _tileWidth, _tileHeight, _mapWidth, _mapHeight;

        /// <summary>
        /// the tileset texture
        /// </summary>
        Texture2D _tilesetTexture;
        /// <summary>
        /// The tile info in the tileset
        /// </summary>
        Rectangle[] _tiles;

        /// <summary>
        /// The tile map data
        /// </summary>
        int[] _map;

        string _filename;

        private List<Vector2> waypoints;


        public TileMap(string filename, List<Vector2> waypoints)
        {
            _filename = filename;
            this.waypoints = waypoints;
        }

        public void LoadContent(ContentManager content)
        {
            string data = File.ReadAllText(Path.Join(content.RootDirectory, _filename));

            var lines = data.Split('\n');

            var tilesetFilename = lines[0].Trim();
            _tilesetTexture = content.Load<Texture2D>(tilesetFilename);

            var secondLine = lines[1].Split(',');

            _tileWidth = int.Parse(secondLine[0]);
            _tileHeight = int.Parse(secondLine[1]);

            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileHeight;
            _tiles = new Rectangle[tilesetColumns * tilesetRows];

            for (int x = 0; x < tilesetRows; x++)
            {
                for (int y = 0; y < tilesetColumns; y++)
                {
                    int index = y + x;
                    _tiles[index] = new Rectangle(
                        y * _tileWidth,
                        x * _tileHeight,
                        _tileWidth,
                        _tileHeight
                        );


                }
            }

            //Third line is the mapsize
            var thirdLine = lines[2].Split(',');
            _mapWidth = int.Parse(thirdLine[0]);
            _mapHeight = int.Parse(thirdLine[1]);

            ///Now we can create our map
            var fourthLine = lines[3].Split(',');
            _map = new int[_mapWidth * _mapHeight];
            /* for (int i = 0; i < _mapWidth * _mapHeight; i++)
             {
                 _map[i] = int.Parse(fourthLine[i]);

             }*/

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            for (int y = 0; y < 480; y+=32)
            {
                for (int x = 0; x < 800; x+= 32)
                {

                    spriteBatch.Draw(_tilesetTexture, new Vector2(x,y), _tiles[1], Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, .95f);


                }

            }
            DrawTiledPath(spriteBatch);
        }


        public void DrawTiledPath(SpriteBatch _spriteBatch)
        {
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                DrawTiledLine(_spriteBatch, waypoints[i], waypoints[i + 1], 32);
            }
        }
        private void DrawTiledLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, int tileSize)
        {
            Vector2 direction = point2 - point1;
            float length = direction.Length();
            direction.Normalize();
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            int tileCount = (int)(length / tileSize);

            Vector2 origin = Vector2.Zero; 

            for (int i = 0; i < tileCount; i++)
            {
                Vector2 position = point1 + direction * tileSize * i;
                spriteBatch.Draw(_tilesetTexture, position, _tiles[0], Color.White, 0f, origin, 1.0f, SpriteEffects.None, .9f);
            }
        }




    }
}
