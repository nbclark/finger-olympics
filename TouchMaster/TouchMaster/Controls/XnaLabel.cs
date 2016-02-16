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
    class XnaLabel : XnaControl
    {
        private static SpriteFont _defaultFont = null;
        public XnaLabel(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.Text = "null";
            this.Color = Color.White;
            this.BackColor = Color.Transparent;
            this.BorderColor = Color.Transparent;
            this.BorderWidth = 0;
            this.HorizontalAlignment = MobileSrc.FantasticFingerFun.HorizontalAlignment.Center;
            this.VerticalAlignment = MobileSrc.FantasticFingerFun.VerticalAlignment.Middle;
            this.FontZoom = 1.0f;

            if (null == _defaultFont)
            {
                _defaultFont = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");
            }
            this.Font = _defaultFont;
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get;
            set;
        }

        public VerticalAlignment VerticalAlignment
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public Color BackColor
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        public float FontZoom
        {
            get;
            set;
        }

        public SpriteFont Font
        {
            get;
            set;
        }

        public override void Update(GameTime gameTime)
        {
        }

        protected virtual void GetColors(ref Color foreColor, ref Color backColor)
        {
            //
        }

        protected virtual float GetRotation()
        {
            return 0f;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawBorder(spriteBatch, gameTime);

            Rectangle contentBounds = this.Bounds;
            contentBounds.Inflate(-this.BorderWidth, -this.BorderWidth);

            Color backColor = this.BackColor;
            Color foreColor = this.Color;

            GetColors(ref foreColor, ref backColor);

            if (Color.Transparent != backColor)
            {
                spriteBatch.Draw(_pixel, contentBounds, backColor);
            }

            Vector2 textSize = this.Font.MeasureString(this.Text);
            Vector2 location = new Vector2(contentBounds.X, contentBounds.Y);
            Vector2 offset = default(Vector2);
            switch (this.HorizontalAlignment)
            {
                case MobileSrc.FantasticFingerFun.HorizontalAlignment.Center:
                    {
                        location.X = contentBounds.Center.X;
                        offset.X = textSize.X / 2;
                    }
                    break;
                case MobileSrc.FantasticFingerFun.HorizontalAlignment.Right:
                    {
                        location.X = contentBounds.Right;
                        offset.X = textSize.X;
                    }
                    break;
            }

            switch (this.VerticalAlignment)
            {
                case MobileSrc.FantasticFingerFun.VerticalAlignment.Middle:
                    {
                        location.Y = contentBounds.Center.Y;
                        offset.Y = textSize.Y / 2;
                    }
                    break;
                case MobileSrc.FantasticFingerFun.VerticalAlignment.Bottom:
                    {
                        location.Y = contentBounds.Bottom;
                        offset.Y = textSize.Y;
                    }
                    break;
            }

            float rotation = GetRotation();
            spriteBatch.DrawString(this.Font, this.Text, location, foreColor, rotation, offset, this.FontZoom, SpriteEffects.None, 0);
        }
    }
}
