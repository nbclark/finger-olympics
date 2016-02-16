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
    internal class FingerRunnerPlayer
    {
        public Vector2? _leftFootLoc = null, _rightFootLoc = null;
        public List<bool> _footDownMarks = new List<bool>();
        public int _footDownSpeed = 0;
        public Vector2 _position = new Vector2(0, 0);
        public Vector2 _speed = new Vector2(0, 0);
        public Vector2? _hurdleLoc = null;
        public bool _isOnLeft = true;
        public bool _shouldSpeedUp = false;
        public bool _hitHurdle = false;
        public TimeSpan _hitHurdleTime = TimeSpan.Zero;
        public Rectangle _bounds, LeftBox, RightBox, LeftBorder, RightBorder;
        public bool Flip;
        public GamePlayer Player;
        private SoundEffectInstance _leftFootSound, _rightFootSound;

        public string Message = string.Empty;

        public FingerRunnerPlayer(GamePlayer player, Rectangle bounds, bool flip)
        {
            Player = player;
            Flip = flip;

            _bounds = bounds;
            _leftFootLoc = _rightFootLoc = null;
            _position = new Vector2(0, 0);
            _hurdleLoc = null;
            _speed = new Vector2(0, 0);
            _footDownMarks.Clear();
            _isOnLeft = true;
            _shouldSpeedUp = false;

            int topA = (flip) ? 0 : 315;
            int botA = (flip) ? 0 : 320;

            LeftBorder = new Rectangle(bounds.Left, topA, 165, 165);
            RightBorder = new Rectangle(bounds.Left + bounds.Width - 165, topA, 165, 165);
            LeftBox = new Rectangle(bounds.Left, botA, 160, 160);
            RightBox = new Rectangle(bounds.Left + bounds.Width - 160, botA, 160, 160);

            _leftFootSound = FingerRunnerGame._leftFootDrum.CreateInstance();
            _rightFootSound = FingerRunnerGame._rightFootDrum.CreateInstance();
        }

        public bool Update(GameTime gameTime)
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            _leftFootLoc = _rightFootLoc = null;
            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved)
                {
                    if (touchLocation.Position.X >= _bounds.Left && touchLocation.Position.X <= _bounds.Right)
                    {
                        if (touchLocation.Position.X < _bounds.Center.X)
                        {
                            // left foot
                            _leftFootLoc = new Vector2(touchLocation.Position.X, touchLocation.Position.Y);
                        }
                        else
                        {
                            // right foot
                            _rightFootLoc = new Vector2(touchLocation.Position.X, touchLocation.Position.Y);
                        }
                    }
                }
            }

            // We have a velocity that decelerates in the air, and accelerates when we change feet.
            if (_leftFootLoc.HasValue && _rightFootLoc.HasValue)
            {
                // both feet are down, we are transitioning
            }
            else if (_leftFootLoc.HasValue)
            {
                // our left foot is down
                if (!_isOnLeft)
                {
                    _shouldSpeedUp = true;
                    _isOnLeft = true;

                    if (_leftFootSound.State == SoundState.Playing)
                    {
                        _leftFootSound = FingerRunnerGame._rightFootDrum.CreateInstance();
                    }
                    SoundManager.Play(_leftFootSound);
                }
                else
                {
                    _shouldSpeedUp = false;
                }
            }
            else if (_rightFootLoc.HasValue)
            {
                // our left foot is down
                if (_isOnLeft)
                {
                    _shouldSpeedUp = true;
                    _isOnLeft = false;

                    //SoundManager.Stop(_rightFootSound);

                    if (_rightFootSound.State == SoundState.Playing)
                    {
                        _rightFootSound = FingerRunnerGame._rightFootDrum.CreateInstance();
                    }
                    SoundManager.Play(_rightFootSound);
                }
                else
                {
                    _shouldSpeedUp = false;
                }
            }

            _footDownMarks.Add(_shouldSpeedUp);

            if (_shouldSpeedUp)
            {
                _footDownSpeed++;
                _shouldSpeedUp = false;
            }

            if (_footDownMarks.Count > FingerRunnerGame.MaxFootDownMarks)
            {
                bool wasDown = _footDownMarks[0];
                _footDownMarks.RemoveAt(0);

                if (wasDown)
                {
                    _footDownSpeed--;
                }
            }

            _speed.Y = _footDownSpeed * 4;
            _position.Y += _speed.Y / 4;

            // every so often we have a hurdle (every 500 units or so)
            float hurdleOffset = (_position.Y % FingerRunnerGame.HurdleDistance) - (FingerRunnerGame.HurdleDistance - _bounds.Height);

            if (hurdleOffset > -1800)
            {

                int hurdleWidth = (int)((float)_bounds.Width / 800 * FingerRunnerGame.Hurdle.Width);
                int hurdleHeight = (int)((float)_bounds.Width / 800 * FingerRunnerGame.Hurdle.Height);

                _hurdleLoc = new Vector2(_bounds.Center.X - hurdleWidth / 2, (Flip) ? (480 - hurdleOffset - hurdleHeight) : hurdleOffset);
                Rectangle hurdleBox = new Rectangle((int)(_bounds.Center.X - hurdleWidth / 2), (int)hurdleOffset, hurdleWidth, hurdleHeight);


                if (Flip)
                {
                    if ((_hurdleLoc.Value.Y >= 3 * 200) && (_hurdleLoc.Value.Y < 3 * 300))
                    {
                        Message = "Ready...";
                    }
                    else if ((_hurdleLoc.Value.Y >= 2 * 100) && (_hurdleLoc.Value.Y < 3 * 200))
                    {
                        Message = "Set...";
                    }
                    else if ((_hurdleLoc.Value.Y >= 100) && (_hurdleLoc.Value.Y < 2 * 100))
                    {
                        Message = "Jump...";
                    }
                    else
                    {
                        Message = string.Empty;
                    }
                }
                else
                {
                    if ((_hurdleLoc.Value.Y < 480 - 3 * 200) && (_hurdleLoc.Value.Y >= 480 - 3 * 300))
                    {
                        Message = "Ready...";
                    }
                    else if ((_hurdleLoc.Value.Y < 480 - 2 * 100) && (_hurdleLoc.Value.Y >= 480 - 3 * 200))
                    {
                        Message = "Set...";
                    }
                    else if ((_hurdleLoc.Value.Y < 480 - 100) && (_hurdleLoc.Value.Y >= 480 - 2 * 100))
                    {
                        Message = "Jump...";
                    }
                    else
                    {
                        Message = string.Empty;
                    }
                }

                bool hasCollision = false;
                if (_leftFootLoc.HasValue || _rightFootLoc.HasValue)
                {
                    if (Flip)
                    {
                        hasCollision = (_hurdleLoc.Value.Y >= 0) && (_hurdleLoc.Value.Y <= 20);
                    }
                    else
                    {
                        hasCollision = (_hurdleLoc.Value.Y <= 480) && (_hurdleLoc.Value.Y >= 460);
                    }
                }

                if (hasCollision)
                {
                    if (!_hitHurdle)
                    {
                        // assess a penalty
                        _hitHurdle = true;

                        int penalty = _footDownSpeed / 2;
                        _footDownSpeed = _footDownSpeed - penalty;
                        for (int i = 0; i < _footDownMarks.Count && penalty > 0; ++i)
                        {
                            if (_footDownMarks[i])
                            {
                                _footDownMarks[i] = false;
                                penalty--;
                            }
                        }

                        _hitHurdleTime = gameTime.TotalGameTime;
                    }
                }
            }
            else
            {
                _hurdleLoc = null;
            }

            if (_hitHurdle)
            {
                if ((gameTime.TotalGameTime - _hitHurdleTime).TotalSeconds > 1)
                {
                    _hitHurdle = false;
                }
            }

            if (_position.Y > FingerRunnerGame.RaceLength)
            {
                return true;
            }
            return false;
        }
    }
}
