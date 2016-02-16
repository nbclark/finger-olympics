using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MobileSrc.FantasticFingerFun
{
    class XnaImageButton : XnaButton
    {
        public XnaImageButton(Texture2D image, Rectangle bounds)
            : base(bounds)
        {
            this.Bounds = bounds;
            this.Image = image;
        }

        public Texture2D Image
        {
            get;
            set;
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawBorder(spriteBatch, gameTime);
            spriteBatch.Draw(_pixel, this.Bounds, this.BackColor);
            spriteBatch.Draw(this.Image, this.Bounds, new Color(0xff, 0xff, 0xff, (this.IsDown) ? 0x7f : 0xff));
        }
    }
}
