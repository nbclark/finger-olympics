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
    class XnaButton : XnaLabel
    {
        public event EventHandler Click;
        private bool _isDown = false;

        public XnaButton(Rectangle bounds)
            : base(bounds)
        {
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
                    if (null != Click)
                    {
                        Click(this, new EventArgs());
                    }
                }
            }
            _isDown = isDown;
        }

        protected override void GetColors(ref Color foreColor, ref Color backColor)
        {
            foreColor.A = (byte)((this.IsDown) ? 0x7f : 0xff);
            if (backColor != Color.Transparent)
            {
                backColor.A = foreColor.A;
            }
        }

        protected override float GetRotation()
        {
            return (this.IsDown) ? (float)(Math.PI / 32) : 0;
        }
    }
}
