using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.IO;

namespace MobileSrc.FantasticFingerFun.Games
{
    class XnaRaceTrackTile : XnaButton
    {
        private FingerRacerGame.TilePiece[][] _boardTiles;

        public XnaRaceTrackTile(Dictionary<FingerRacerGame.TilePiece, Texture2D> textureMap, string name, string mapData)
            : base(new Rectangle())
        {
            this.TextureMap = textureMap;
            this.Name = name;

            string[] rows = mapData.Split('\n');

            _boardTiles = new FingerRacerGame.TilePiece[rows.Length][];

            // Let us load the map
            int y = 0;
            foreach (string row in rows)
            {
                string[] cols = row.Trim().Split(' ');

                _boardTiles[y] = new FingerRacerGame.TilePiece[cols.Length];

                int x = 0;
                foreach (string cell in cols)
                {
                    if (y < _boardTiles.Length && x < _boardTiles[0].Length)
                    {
                        _boardTiles[y][x] = (FingerRacerGame.TilePiece)Convert.ToInt32(cell);
                    }
                    x++;
                }
                y++;
            }
        }

        public string Name
        {
            get;
            set;
        }

        private Dictionary<FingerRacerGame.TilePiece, Texture2D> TextureMap
        {
            get;
            set;
        }

        public FingerRacerGame.TilePiece[][] BoardTiles
        {
            get { return _boardTiles; }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int tileWidth = TextureMap[FingerRacerGame.TilePiece.Vertical].Bounds.Width;

            float scaledWidth = (float)this.Bounds.Width / BoardTiles[0].Length;
            float scaledHeight = (float)this.Bounds.Height / BoardTiles.Length;

            float xRatio = scaledWidth / tileWidth;
            float yRatio = scaledHeight / tileWidth;

            int backTileX = (int)(80 * xRatio);
            int backTileY = (int)(80 * yRatio);

            Color tileColor = new Color(0xff, 0xff, 0xff, (this.IsDown) ? 0x7f : 0xff);

            for (int xPos = this.Bounds.Left; xPos < this.Bounds.Right; xPos += backTileX)
            {
                for (int yPos = this.Bounds.Top; yPos < this.Bounds.Bottom; yPos += backTileY)
                {
                    FingerGames.Instance.SpriteBatch.Draw(TextureMap[FingerRacerGame.TilePiece.Grass], new Rectangle(xPos, yPos, backTileX, backTileY), tileColor);
                }
            }

            float startY = this.Bounds.Top;
            for (int y = 0; y < BoardTiles.Length; ++y)
            {
                float startX = this.Bounds.Left;
                for (int x = 0; x < BoardTiles[0].Length; ++x)
                {
                    if (BoardTiles[y][x] != FingerRacerGame.TilePiece.Grass)
                    {
                        FingerGames.Instance.SpriteBatch.Draw(TextureMap[BoardTiles[y][x]], new Rectangle((int)startX, (int)startY, (int)scaledWidth, (int)scaledHeight), tileColor);
                    }
                    startX += scaledWidth;
                }
                startY += scaledHeight;
            }
        }
    }
}
