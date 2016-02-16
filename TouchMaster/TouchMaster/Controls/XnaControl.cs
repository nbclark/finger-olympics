using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }
    enum VerticalAlignment
    {
        Top,
        Middle,
        Bottom
    }

    abstract class XnaControl : XnaObject
    {
        protected static Texture2D _pixel = null;
        protected Rectangle _bounds = new Rectangle();
        protected XnaControl()
        {
            if (null == _pixel)
            {
                _pixel = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\pixel");
            }
            this.BorderWidth = 0;
        }

        public Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                ForceLayout();
            }
        }

        public Color BorderColor
        {
            get;
            set;
        }

        public int BorderWidth
        {
            get;
            set;
        }

        public virtual void ForceLayout()
        {
            //
        }

        public object Tag
        {
            get;
            set;
        }

        public void DrawBorder(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
        {
            Color borderColor = this.BorderColor;
            if (this.BorderWidth > 0)
            {
                // Draw top border
                spriteBatch.Draw(_pixel, new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.BorderWidth), borderColor);
                // Draw bottom border
                spriteBatch.Draw(_pixel, new Rectangle(this.Bounds.X, this.Bounds.Bottom - this.BorderWidth, this.Bounds.Width, this.BorderWidth), borderColor);
                // Draw left border
                spriteBatch.Draw(_pixel, new Rectangle(this.Bounds.X, this.Bounds.Y, this.BorderWidth, this.Bounds.Height), borderColor);
                // Draw right border
                spriteBatch.Draw(_pixel, new Rectangle(this.Bounds.Right - this.BorderWidth, this.Bounds.Y, this.BorderWidth, this.Bounds.Height), borderColor);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawBorder(spriteBatch, gameTime);
        }
    }

    abstract class XnaContainer : XnaControl
    {
        public XnaContainer()
        {
            this.Children = new List<XnaControl>();
            this.Padding = new Rectangle(5, 5, 5, 5);
            this.FixedSize = true;
        }

        public void AddChild(XnaControl control)
        {
            this.Children.Add(control);
            this.ForceLayout();
        }

        public void ClearChildren()
        {
            this.Children.Clear();
            this.ForceLayout();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (XnaControl control in this.Children)
            {
                control.Update(gameTime);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            foreach (XnaControl control in this.Children)
            {
                control.Draw(spriteBatch, gameTime);
            }
        }

        public List<XnaControl> Children
        {
            get;
            private set;
        }

        public Rectangle Padding
        {
            get;
            set;
        }

        public bool FixedSize
        {
            get;
            set;
        }
    }

    class VerticalStackPanel : XnaContainer
    {
        public VerticalStackPanel()
        {
            this.HorizontalAlignment = HorizontalAlignment.Center;
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get;
            set;
        }

        public override void ForceLayout()
        {
            int maxX = 0, maxY = 0;
            int y = this.Bounds.Top;
            foreach (XnaControl child in this.Children)
            {
                child.Bounds = new Rectangle(this.Bounds.X + this.Padding.X, y, child.Bounds.Width, child.Bounds.Height);
                y += this.Padding.Y + child.Bounds.Height;

                maxX = Math.Max(child.Bounds.Right, maxX);
                maxY = Math.Max(child.Bounds.Bottom, maxY);
            }

            if (!this.FixedSize)
            {
                _bounds = new Rectangle(this.Bounds.X, this.Bounds.Y, maxX - this.Bounds.X, maxY - this.Bounds.Y);
            }
            else
            {
                //_bounds = new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, maxY - this.Bounds.Y);
            }

            foreach (XnaControl child in this.Children)
            {
                int x = child.Bounds.X;
                switch (this.HorizontalAlignment)
                {
                    case MobileSrc.FantasticFingerFun.HorizontalAlignment.Center:
                        {
                            x = (this.Bounds.X + (this.Bounds.Width - child.Bounds.Width) / 2);
                        }
                        break;
                    case MobileSrc.FantasticFingerFun.HorizontalAlignment.Right:
                        {
                            x = this.Bounds.Right - child.Bounds.Width;
                        }
                        break;
                }
                child.Bounds = new Rectangle(x, child.Bounds.Y, child.Bounds.Width, child.Bounds.Height);
            }
        }
    }

    class HorizontalStackPanel : XnaContainer
    {
        public HorizontalStackPanel()
        {
            this.VerticalAlignment = VerticalAlignment.Middle;
        }

        public VerticalAlignment VerticalAlignment
        {
            get;
            set;
        }

        public override void ForceLayout()
        {
            int maxX = 0, maxY = 0;
            int x = this.Bounds.Left;
            foreach (XnaControl child in this.Children)
            {
                child.Bounds = new Rectangle(x, this.Bounds.Y + this.Padding.Y, child.Bounds.Width, child.Bounds.Height);
                x += this.Padding.X + child.Bounds.Width;

                maxX = Math.Max(child.Bounds.Right, maxX);
                maxY = Math.Max(child.Bounds.Bottom, maxY);
            }

            if (!this.FixedSize)
            {
                _bounds = new Rectangle(this.Bounds.X, this.Bounds.Y, maxX - this.Bounds.X, maxY - this.Bounds.Y);
            }
            else
            {
                //_bounds = new Rectangle(this.Bounds.X, this.Bounds.Y, maxX - this.Bounds.X, this.Bounds.Height);
            }

            foreach (XnaControl child in this.Children)
            {
                int y = child.Bounds.Y;
                switch (this.VerticalAlignment)
                {
                    case MobileSrc.FantasticFingerFun.VerticalAlignment.Middle:
                        {
                            y = (this.Bounds.Y + (this.Bounds.Height - child.Bounds.Height) / 2);
                        }
                        break;
                    case MobileSrc.FantasticFingerFun.VerticalAlignment.Bottom:
                        {
                            y = this.Bounds.Bottom - child.Bounds.Height;
                        }
                        break;
                }
                child.Bounds = new Rectangle(child.Bounds.X, y, child.Bounds.Width, child.Bounds.Height);
            }
        }
    }
}
