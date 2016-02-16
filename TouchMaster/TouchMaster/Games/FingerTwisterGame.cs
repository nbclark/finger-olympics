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
    class FingerTwisterGame : BaseGame
    {
        enum TwisterDifficulty
        {
            Beginner,
            Intermediate,
            Master
        }

        enum TwisterState
        {
            Waiting,
            PlayerInstruction,
            Player2Instruction,
            Feedback
        }

        internal static SoundEffect PopSound = null;

        private Color[] _colors = new Color[] { Color.Green, Color.Red, Color.Blue, Color.Yellow, Color.Orange, Color.White, Color.Turquoise, Color.Fuchsia, Color.Brown, Color.LightBlue, Color.LightGreen, Color.Pink,
        Color.Purple, Color.DarkOrange, Color.HotPink};
        private SpriteFont _font;
        private TwisterCircle[] _circles = new TwisterCircle[15];
        private TwisterState _state = TwisterState.Waiting;
        private string _message;
        private TimeSpan _messageTime;
        private int _newIndex = -1;
        private List<TwisterDigit> _digits = new List<TwisterDigit>();
        private TwisterDigit _activeDigit = null;
        private XnaLabel _winnerLabel = new XnaLabel(new Rectangle(0, 0, 800, 480));

        private VerticalStackPanel _configurePanel = new VerticalStackPanel();

        public FingerTwisterGame(GameManager gameManager)
            : base(gameManager, "moves", ScoreCenter.SortDirection.Descending)
        {
            _digits.Add(new TwisterDigit(gameManager.Players[0], 0));
            if (gameManager.Players.Count > 1)
            {
                _digits.Add(new TwisterDigit(gameManager.Players[1], 0));
                _digits.Add(new TwisterDigit(gameManager.Players[0], 1));
                _digits.Add(new TwisterDigit(gameManager.Players[1], 1));
            }
            else
            {
                _digits.Add(new TwisterDigit(gameManager.Players[0], 1));
                _digits.Add(new TwisterDigit(gameManager.Players[0], 2));
            }
            _activeDigit = _digits[0];
        }

        public override string Title
        {
            get { return "Twister"; }
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override void LoadContent()
        {
            this.Logo = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\twister");
            this._font = this.GameManager.Game.Content.Load<SpriteFont>(@"Fonts\MenuFont");
            SpriteFont bigFont = this.GameManager.Game.Content.Load<SpriteFont>(@"Fonts\GameFont");
            PopSound = this.GameManager.Game.Content.Load<SoundEffect>(@"Sounds\POP2");

            this.Assets.Add(Logo);
            this.Assets.Add(PopSound);

            _configurePanel.Bounds = new Rectangle(20, 0, 780, 480);
            _configurePanel.HorizontalAlignment = HorizontalAlignment.Left;

            XnaLabel label = new XnaLabel(new Rectangle(0, 0, 800, 100));
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Font = bigFont;
            label.FontZoom = 1.0f;
            label.Text = "Select Difficulty..";

            _configurePanel.AddChild(label);

            foreach (TwisterDifficulty diff in new TwisterDifficulty[] { TwisterDifficulty.Beginner, TwisterDifficulty.Intermediate, TwisterDifficulty.Master })
            {
                XnaButton difficultyButton = new XnaButton(new Rectangle(0, 0, 800, 80));
                difficultyButton.Tag = diff;
                difficultyButton.HorizontalAlignment = HorizontalAlignment.Left;
                difficultyButton.Font = bigFont;
                difficultyButton.FontZoom = 0.8f;
                difficultyButton.Text = diff.ToString();
                difficultyButton.Click += new EventHandler(difficultyButton_Click);

                if (FingerGames.IsTrial && diff != TwisterDifficulty.Intermediate)
                {
                    difficultyButton.Color = new Color(110, 110, 110);
                }

                _configurePanel.AddChild(difficultyButton);
            }

            base.LoadContent();
        }

        public override void Reset()
        {
            _state = TwisterState.Waiting;

            _digits.Clear();
            _digits.Add(new TwisterDigit(FingerGames.Instance.GameManager.Players[0], 0));
            if (FingerGames.Instance.GameManager.Players.Count > 1)
            {
                _digits.Add(new TwisterDigit(FingerGames.Instance.GameManager.Players[1], 0));
                _digits.Add(new TwisterDigit(FingerGames.Instance.GameManager.Players[0], 1));
                _digits.Add(new TwisterDigit(FingerGames.Instance.GameManager.Players[1], 1));
            }
            else
            {
                _digits.Add(new TwisterDigit(FingerGames.Instance.GameManager.Players[0], 1));
                _digits.Add(new TwisterDigit(FingerGames.Instance.GameManager.Players[0], 2));
            }
            _activeDigit = _digits[0];

            foreach (GamePlayer player in FingerGames.Instance.GameManager.Players)
            {
                player.Score = 0;
            }

            _newIndex = -1;

            base.Reset();
        }

        /// <summary>
        /// Take N samples. Speed is number of alternating taps over that period
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            int activeTouches = 0;

            Dictionary<int, TouchLocation> touches = new Dictionary<int, TouchLocation>();
            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved)
                {
                    touches.Add(touchLocation.Id, touchLocation);
                    activeTouches++;
                }
            }

            if (_state == TwisterState.Waiting)
            {
                _state = TwisterState.PlayerInstruction;
            }
            else
            {
                // If we pull off of a circle (when not transitioning) we lose
                foreach (TwisterCircle circle in _circles)
                {
                    if (circle.Digit != null && !circle.IsBlinking)
                    {
                        if (!touches.ContainsKey(circle.Digit.TouchId))
                        {
                            bool found = false;
                            foreach (TouchLocation touchLoc in touchCollection)
                            {
                                Point searchPoint = new Point((int)touchLoc.Position.X, (int)touchLoc.Position.Y);
                                if (circle.Contains(searchPoint))
                                {
                                    //if (touches.ContainsKey(touchLoc.Id))
                                    //{
                                    //    //
                                    //}
                                    //else
                                    {
                                        circle.Digit.TouchId = touchLoc.Id;
                                        found = true;
                                    }
                                }
                            }

                            if (!found)
                            {
                                circle.MissingCount++;

                                if (circle.MissingCount > 20)
                                {
                                    // game over
                                    _winnerLabel.Text = string.Format("{0} Loses ({1} touches)!", circle.Digit.Player.Name, circle.Digit.Player.Score);
                                    gameState = InGameState.End;
                                }
                            }
                        }
                        else
                        {
                            TouchLocation location = touches[circle.Digit.TouchId];
                            Point searchPoint = new Point((int)location.Position.X, (int)location.Position.Y);
                            if (!circle.Contains(searchPoint))
                            {
                                // game over
                                _winnerLabel.Text = string.Format("{0} Loses ({1} touches)!", circle.Digit.Player.Name, circle.Digit.Player.Score);
                                gameState = InGameState.End;
                            }
                            else
                            {
                                circle.MissingCount = 0;
                            }
                        }
                    }
                }

                if (_state == TwisterState.Feedback)
                {
                    if ((gameTime.TotalGameTime - _messageTime).TotalSeconds > 2)
                    {
                        _state = TwisterState.PlayerInstruction;
                    }
                }
                else if (_state == TwisterState.PlayerInstruction)
                {
                    if (_newIndex < 0)
                    {
                        int circleIndex = 0;
                        List<int> inactiveIndices = new List<int>();
                        foreach (TwisterCircle circle in _circles)
                        {
                            if (null == circle.Digit)
                            {
                                inactiveIndices.Add(circleIndex);
                            }
                            circleIndex++;
                        }
                        _newIndex = inactiveIndices[FingerGames.Randomizer.Next(0, inactiveIndices.Count())];

                        if (null != _activeDigit.ActiveCircle)
                        {
                            _activeDigit.ActiveCircle.IsBlinking = true;
                        }
                        _circles[_newIndex].IsBlinking = true;
                        _circles[_newIndex].MissingCount = 0;
                    }
                    else
                    {
                        Dictionary<int, TwisterDigit> digitTouches = new Dictionary<int, TwisterDigit>();

                        foreach (TwisterDigit digit in _digits)
                        {
                            if (digit.TouchId != 0)
                            {
                                if (!touches.ContainsKey(digit.TouchId))
                                {
                                    // digit picked up his finger -- game over
                                    //return true;
                                }
                                else if (!digitTouches.ContainsKey(digit.TouchId))
                                {
                                    digitTouches.Add(digit.TouchId, digit);
                                }
                            }
                        }
                        foreach (TouchLocation location in touches.Values)
                        {
                            if (!digitTouches.ContainsKey(location.Id))
                            {
                                // new id
                                _activeDigit.TouchId = location.Id;
                                break;
                            }
                        }

                        if (_activeDigit.TouchId != 0 && touches.ContainsKey(_activeDigit.TouchId))
                        {
                            // Our finger is down
                            TouchLocation loc = touches[_activeDigit.TouchId];

                            if (_circles[_newIndex].Contains(new Point((int)loc.Position.X, (int)loc.Position.Y)))
                            {
                                // we got it...
                                if (null != _activeDigit.ActiveCircle)
                                {
                                    _activeDigit.ActiveCircle.Digit = null;
                                    _activeDigit.ActiveCircle.IsBlinking = false;
                                }
                                _circles[_newIndex].Digit = _activeDigit;
                                _circles[_newIndex].IsBlinking = false;
                                _circles[_newIndex].MissingCount = 0;
                                _activeDigit.ActiveCircle = _circles[_newIndex];

                                _activeDigit.Player.Score++;
                                float playerMod = (GameManager.Players.IndexOf(_activeDigit.Player) == 0) ? -0.5f : 0.5f;
                                SoundManager.Play(PopSound, 1.0f, playerMod, playerMod);

                                _newIndex = -1;
                                int index = _digits.IndexOf(_activeDigit);
                                index = (index + 1) % _digits.Count();

                                _activeDigit = _digits[index];

                                _state = TwisterState.Feedback;
                                _message = "Well Done!";
                                _messageTime = gameTime.TotalGameTime;
                            }
                        }
                    }
                }
            }

            foreach (TwisterCircle circle in _circles)
            {
                circle.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (TwisterCircle circle in _circles)
            {
                circle.Draw(FingerGames.Instance.SpriteBatch, gameTime);
            }

            string data = string.Empty;

            switch (_state)
            {
                case TwisterState.Waiting:
                case TwisterState.PlayerInstruction:
                    {
                        data = string.Format("{0}'s turn", _activeDigit.Player.Name);
                    }
                    break;
                case TwisterState.Feedback:
                    {
                        data = _message;
                    }
                    break;
            }

            Vector2 fontSize = this._font.MeasureString(data);
            Vector2 location = new Vector2(5, 240 + fontSize.X / 2);
            FingerGames.Instance.SpriteBatch.DrawString(_font, data, location, Color.White, 1 * (float)-Math.PI / 2, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

            base.Draw(gameTime);
        }

        public override void UpdateIntro(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            gameState = InGameState.Active;
        }

        public override void UpdateEnd(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            base.UpdateEnd(gameTime, ref gameState, justTransitioned);
        }

        public override void DrawEnd(GameTime gameTime)
        {
            _winnerLabel.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        private bool _needsConfigure = false;
        void difficultyButton_Click(object sender, EventArgs e)
        {
            XnaButton button = (XnaButton)sender;
            TwisterDifficulty diff = (TwisterDifficulty)button.Tag;
            _needsConfigure = false;

            if (FingerGames.IsTrial && (diff == TwisterDifficulty.Beginner || diff == TwisterDifficulty.Master))
            {
                FingerGames.Instance.SetStage(Stage.Trial);
            }
            else
            {
                int colCount = 5;
                int rowCount = 3;
                int yOffset = 24;
                int width, height;

                switch (diff)
                {
                    case TwisterDifficulty.Beginner:
                        {
                            colCount = 3;
                            rowCount = 2;
                        }
                        break;
                    case TwisterDifficulty.Intermediate:
                        {
                            colCount = 5;
                            rowCount = 3;
                        }
                        break;
                    case TwisterDifficulty.Master:
                        {
                            colCount = 6;
                            rowCount = 4;
                        }
                        break;
                }

                GameManager.GameVariation = diff.ToString();

                width = (800 - 80) / colCount;
                height = width;
                yOffset = (480 - width * rowCount) / 2;

                double halfCol = (colCount - 1) / 2;
                double widthOffset = (width / 10);

                _circles = new TwisterCircle[colCount * rowCount];
                int index = 0;
                for (int x = 0; x < colCount; ++x)
                {
                    for (int y = 0; y < rowCount; ++y)
                    {
                        Rectangle bounds = new Rectangle((int)(80 + x * width + (y-halfCol) * widthOffset), yOffset + y * height, width, height);
                        _circles[index] = new TwisterCircle(this.GameManager.Game, bounds, _colors[index % _colors.Length]);

                        index++;
                    }
                }
                foreach (TwisterCircle circle in _circles)
                {
                    circle.Digit = null;
                    circle.SetIntroAnim(TimeSpan.FromMilliseconds(FingerGames.Randomizer.Next(0, 2000)), FingerGames.Randomizer.Next(100, 1600));
                }
            }
        }

        public override void UpdateConfigure(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (justTransitioned)
            {
                _needsConfigure = true;
            }
            else if (!_needsConfigure)
            {
                gameState = InGameState.Intro;
            }
            else
            {
                _configurePanel.Update(gameTime);
            }
        }

        public override void DrawConfigure(GameTime gameTime)
        {
            _configurePanel.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }
    }
}
