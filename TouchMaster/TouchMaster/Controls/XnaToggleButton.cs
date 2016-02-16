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
    class XnaToggleButton : XnaControl
    {
        public event EventHandler SelectionChanged;
        private bool _isDown = false;

        public XnaToggleButton(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.Text = "hi";
            this.Color = Color.White;
            this.BackColor = Color.Orange;
            this.BorderColor = Color.Green;
            this.BorderWidth = 10;
            this.HorizontalAlignment = MobileSrc.FantasticFingerFun.HorizontalAlignment.Center;
            this.VerticalAlignment = MobileSrc.FantasticFingerFun.VerticalAlignment.Middle;
        }

        public string Text
        {
            get;
            set;
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

        public SpriteFont Font
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get;
            set;
        }

        protected bool IsDown
        {
            get { return _isDown; }
        }

        public override void Update(GameTime gameTime)
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            bool isDown = false;
            bool hasActive = false;
            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved)
                {
                    if (this.Bounds.Contains(new Point((int)touchLocation.Position.X, (int)touchLocation.Position.Y)))
                    {
                        isDown = true;
                        break;
                    }
                    else
                    {
                        hasActive = true;
                    }
                }
            }

            if (_isDown && !isDown)
            {
                // click event
                if (!hasActive)
                {
                    this.IsSelected = !this.IsSelected;
                    if (null != SelectionChanged)
                    {
                        SelectionChanged(this, new EventArgs());
                    }
                }
            }
            _isDown = isDown;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle contentBounds = this.Bounds;
            contentBounds.Inflate(-this.BorderWidth, -this.BorderWidth);

            Color backColor = this.BackColor;
            Color foreColor = this.Color;

            foreColor.A =  backColor.A = (byte)((this.IsDown) ? 0x7f : 0xff);

            if (this.BackColor != Color.Transparent)
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

            float rotation = (this.IsSelected) ? (float)(Math.PI / 16) : 0;
            float scale = (this.IsSelected) ? 1.5f : 1.0f;

            spriteBatch.DrawString(this.Font, this.Text, location, foreColor, rotation, offset, scale, SpriteEffects.None, 0);
        }
    }
}
