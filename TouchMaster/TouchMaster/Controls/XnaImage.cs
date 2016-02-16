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
    class XnaImage : XnaControl
    {
        public XnaImage(Texture2D image, Rectangle bounds)
            : base()
        {
            this.Bounds = bounds;
            this.Image = image;
            this.BlendColor = Color.White;
        }

        public Texture2D Image
        {
            get;
            set;
        }

        public Color BlendColor
        {
            get;
            set;
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(this.Image, this.Bounds, this.BlendColor);
        }

        public override void Update(GameTime gameTime)
        {
            //
        }
    }
}
