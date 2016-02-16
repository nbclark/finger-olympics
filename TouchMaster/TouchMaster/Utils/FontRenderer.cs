using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    class FontRenderer
    {
        public void RenderNumber(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            float maxWidth = 0;
            float charCount = 0;
            foreach (char ch in text)
            {
                if (ch >= '0' && ch <= '9')
                {
                    Vector2 size = font.MeasureString(ch.ToString());
                    maxWidth = Math.Max(size.X, maxWidth);
                    charCount++;
                }
                else
                {
                    charCount += 0.5f;
                }
            }

            spriteBatch.DrawString(font, text, position, color, rotation , origin, scale, effects, layerDepth);
        }
    }
}
