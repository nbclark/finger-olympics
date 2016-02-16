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

namespace MobileSrc.FantasticFingerFun.Games
{
    class TwisterCircle
    {
        private static Texture2D _circle = null;
        private int _animationIndex = 0;
        private static int _animationCount = 40;
        private static int _halfAnimationCount = _animationCount / 2;
        private bool _isBlinking = false;
        private bool _playedSound = false;
        private int _introAnimCount = 0;
        private float _introAnimIndex = 0;
        private TimeSpan _introAnimLag;
        private TimeSpan _startIntroAnim;
        private SoundEffectInstance _popInstance;

        public TwisterCircle(Game game, Rectangle bounds, Color color)
        {
            this.Digit = null;
            this.Bounds = bounds;
            this.Color = color;
            this.MissingCount = 0;

            if (null == _circle)
            {
                _circle = game.Content.Load<Texture2D>(@"Textures\circle");
            }

            _popInstance = FingerTwisterGame.PopSound.CreateInstance();
        }
        public int MissingCount
        {
            get;
            set;
        }
        public Rectangle Bounds
        {
            get;
            set;
        }
        public Color Color
        {
            get;
            set;
        }
        public TwisterDigit Digit
        {
            get;
            set;
        }
        public void SetIntroAnim(TimeSpan animLag, int count)
        {
            _introAnimLag = animLag;
            _introAnimCount = count;
            _introAnimIndex = 1;
            _startIntroAnim = TimeSpan.MaxValue;
            _animationIndex = 0;
            _isBlinking = false;
            _playedSound = false;
        }
        public bool IsIntroAnimating
        {
            get { return (_introAnimIndex < _introAnimCount); }
        }
        public bool IsBlinking
        {
            get { return _isBlinking; }
            set
            {
                _isBlinking = value;
                if (_isBlinking)
                {
                    _animationIndex = 0;
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            if (TimeSpan.MaxValue == _startIntroAnim)
            {
                _startIntroAnim = gameTime.TotalGameTime.Add(_introAnimLag);
            }
            else if (gameTime.TotalGameTime < _startIntroAnim)
            {
                //
            }
            else if (this.IsIntroAnimating)
            {
                _introAnimIndex *= 1.2F;

                {
                    if (!_playedSound || _introAnimIndex > _introAnimCount)
                    {
                        _playedSound = true;
                        _popInstance.Volume = 0.5f;
                        _popInstance.Pitch = (float)FingerGames.Randomizer.Next(-100, 100) / 100.0f;
                        _popInstance.Pan = (float)FingerGames.Randomizer.Next(-100, 100) / 100.0f;
                        SoundManager.Play(_popInstance);
                    }
                }
            }
            else if (this.IsBlinking)
            {
                this._animationIndex = (this._animationIndex + 1) % _animationCount;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle inflatedBounds = this.Bounds;
            Color color = this.Color;

            if (gameTime.TotalGameTime < _startIntroAnim)
            {
                return;
            }
            else if (this.IsIntroAnimating)
            {
                float scale = 1 - ((float)_introAnimIndex / _introAnimCount);
                int circleWidth = _circle.Width;

                int scaleSize = (int)(200 * scale);
                inflatedBounds.Inflate(scaleSize, scaleSize);

                color.A = 0x7f;
            }
            else if (this.IsBlinking)
            {
                int animIndex = _animationIndex;

                if (animIndex > _halfAnimationCount)
                {
                    animIndex = _animationCount - animIndex;
                }

                int inflation = (int)(animIndex * 2);
                inflatedBounds.Inflate(inflation, inflation);
            }

            spriteBatch.Draw(_circle, inflatedBounds, color);

            if (null != this.Digit)
            {
                //inflatedBounds.Inflate(-20, -20);
                //spriteBatch.Draw(_circle, inflatedBounds, Color.Black);
            }
        }

        public bool Contains(Point pos)
        {
            return this.Bounds.Contains(pos);
        }
    }
}
