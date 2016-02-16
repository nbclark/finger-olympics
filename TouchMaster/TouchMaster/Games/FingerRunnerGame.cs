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
    class FingerRunnerGame : BaseGame
    {
        enum RaceLengths
        {
            Sprint = 7500,
            Medium = 18000,
            Marathon = 30000
        }

        private Texture2D _track;
        private SpriteFont _font, _fixedFont;

        internal static Texture2D Hurdle, LeftFoot, RightFoot;
        internal static int RaceLength = 30000;
        internal static int HurdleDistance = 2000;
        internal static int MaxFootDownMarks = 100;
        internal static SoundEffect _leftFootDrum;
        internal static SoundEffect _rightFootDrum;

        private TimeSpan _elapsedtime = TimeSpan.Zero;
        private TimeSpan _startTime = TimeSpan.MinValue;
        private List<FingerRunnerPlayer> _runPlayers = new List<FingerRunnerPlayer>();
        private VerticalStackPanel _timePanel = new VerticalStackPanel();
        private XnaLabel _timeLabel = new XnaLabel(new Rectangle(0, 400, 800, 70));
        private XnaLabel _timeLabel2 = new XnaLabel(new Rectangle(0, 400, 800, 70));
        private Texture2D _raceFlags;

        private VerticalStackPanel _configurePanel = new VerticalStackPanel();

        public FingerRunnerGame(GameManager gameManager)
            : base(gameManager, "seconds", ScoreCenter.SortDirection.Ascending)
        {
        }

        protected override float EndDelay
        {
            get
            {
                return 5.0f;
            }
        }

        public override string Title
        {
            get { return "Runner"; }
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override void LoadContent()
        {
            LeftFoot = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\leftfoot");
            RightFoot = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\rightfoot");
            Hurdle = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\hurdle");

            this._track = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\track");
            this._font = this.GameManager.Game.Content.Load<SpriteFont>(@"Fonts\FixedWidthFont");
            this._fixedFont = this._font;

            _leftFootDrum = this.GameManager.Game.Content.Load<SoundEffect>(@"Sounds\FootDrum");
            _rightFootDrum = this.GameManager.Game.Content.Load<SoundEffect>(@"Sounds\FootDrum");

            this.Logo = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\runner");
            this._raceFlags = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\racerflags");

            this.Assets.Add(LeftFoot);
            this.Assets.Add(RightFoot);
            this.Assets.Add(Hurdle);
            this.Assets.Add(_track);
            this.Assets.Add(_leftFootDrum);
            this.Assets.Add(_rightFootDrum);
            this.Assets.Add(Logo);
            this.Assets.Add(_raceFlags);

            this._timePanel.Bounds = new Rectangle(0, 10, 800, 470);
            this._timePanel.FixedSize = true;

            XnaImage flagsImage = new XnaImage(this._raceFlags, new Rectangle(0, 0, this._raceFlags.Width, this._raceFlags.Height));
            XnaLabel completedLabel = new XnaLabel(new Rectangle(0, 0, 800, 90));
            completedLabel.BackColor = new Color(0, 0, 0, 150);
            completedLabel.FontZoom = 1.5f;
            completedLabel.Text = "Race Completed!";

            this._timePanel.AddChild(flagsImage);
            this._timePanel.AddChild(completedLabel);
            this._timePanel.AddChild(_timeLabel);
            this._timePanel.AddChild(_timeLabel2);

            SpriteFont bigFont = this.GameManager.Game.Content.Load<SpriteFont>(@"Fonts\GameFont");

            _configurePanel.Bounds = new Rectangle(20, 0, 780, 480);
            _configurePanel.HorizontalAlignment = HorizontalAlignment.Left;

            XnaLabel label = new XnaLabel(new Rectangle(0, 0, 800, 100));
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.Font = bigFont;
            label.FontZoom = 1.0f;
            label.Text = "Select Race Length..";

            _configurePanel.AddChild(label);

            int index = 0;
            foreach (RaceLengths diff in new RaceLengths[] { RaceLengths.Sprint, RaceLengths.Medium, RaceLengths.Marathon })
            {
                XnaButton raceLengthButton = new XnaButton(new Rectangle(0, 0, 800, 80));
                raceLengthButton.Tag = diff;
                raceLengthButton.HorizontalAlignment = HorizontalAlignment.Left;
                raceLengthButton.Font = bigFont;
                raceLengthButton.FontZoom = 0.8f;
                raceLengthButton.Text = diff.ToString();
                raceLengthButton.Click += new EventHandler(raceLengthButton_Click);

                if (FingerGames.IsTrial && index > 0)
                {
                    raceLengthButton.Color = new Color(110, 110, 110);
                }

                _configurePanel.AddChild(raceLengthButton);
                index++;
            }

            base.LoadContent();
        }

        public override void Reset()
        {
            //_state = RaceState.Waiting;
            _runPlayers.Clear();
            
            int startX = 0;
            int width = 800 / FingerGames.Instance.GameManager.Players.Count;
            bool flip = false;
            foreach (GamePlayer player in FingerGames.Instance.GameManager.Players)
            {
                _runPlayers.Add(new FingerRunnerPlayer(player, new Rectangle(startX, 0, width, 480), flip));
                startX += width;
                flip = !flip;
            }

            RaceLength = (int)FingerGames.Randomizer.Next(10000, 30000);
            HurdleDistance = (int)FingerGames.Randomizer.Next(1000, 5000);
            _startTime = TimeSpan.MinValue;
        }

        public override bool HandleBackClick(ref InGameState gameState)
        {
            return base.HandleBackClick(ref gameState);
        }

        /// <summary>
        /// Take N samples. Speed is number of alternating taps over that period
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (justTransitioned)
            {
                //SoundManager.Play(this._runningSoundInstace);
            }
            foreach (FingerRunnerPlayer runner in _runPlayers)
            {
                if (runner.Update(gameTime))
                {
                    // this player has won
                    gameState = InGameState.End;
                    //SoundManager.Stop(this._runningSoundInstace);
                }
            }

            if (gameState == InGameState.End)
            {
                foreach (FingerRunnerPlayer runner in _runPlayers)
                {
                    runner.Player.Score = (_elapsedtime.TotalSeconds);
                }
            }
            else
            {
                TouchCollection touchCollection = TouchPanel.GetState();

                int activeTouches = 0;
                TouchLocation activeTouch = default(TouchLocation);
                foreach (TouchLocation touchLocation in touchCollection)
                {
                    if (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved)
                    {
                        activeTouch = touchLocation;
                        activeTouches++;
                    }
                }

                if (activeTouches > 0)
                {
                    if (_startTime == TimeSpan.MinValue)
                    {
                        _startTime = gameTime.TotalGameTime;
                        _elapsedtime = TimeSpan.Zero;
                    }
                }
                _elapsedtime = _elapsedtime.Add(gameTime.ElapsedGameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            int startX = 0;
            int width = 800 / FingerGames.Instance.GameManager.Players.Count;
            int playerCount = FingerGames.Instance.GameManager.Players.Count;

            for (int iPlayer = 0; iPlayer < FingerGames.Instance.GameManager.Players.Count; ++iPlayer)
            {
                FingerRunnerPlayer player = _runPlayers[iPlayer];
                bool flip = (iPlayer % 2) == 1;

                int repeatCount = (int)Math.Ceiling((float)480 / _track.Height);
                float yOffset = (player._position.Y % _track.Height);

                if (flip)
                {
                    yOffset = -yOffset;
                }

                Vector2 source = new Vector2(startX, -_track.Height + yOffset);

                for (int i = 0; i < repeatCount + 2; ++i)
                {
                    FingerGames.Instance.SpriteBatch.Draw(_track, new Rectangle((int)source.X, (int)source.Y, width, (int)_track.Height), Color.White);
                    source.Y += _track.Height;
                }

                if (player._hurdleLoc.HasValue)
                {
                    Rectangle hurdleBox = new Rectangle((int)player._hurdleLoc.Value.X, (int)player._hurdleLoc.Value.Y, Hurdle.Width, Hurdle.Height);
                    if (playerCount > 0)
                    {
                        hurdleBox.Width = hurdleBox.Width / 2;
                    }
                    FingerGames.Instance.SpriteBatch.Draw(Hurdle, player._hurdleLoc.Value, Color.White);
                }

                Color transBlack = new Color(0, 0, 0, 0x3f);

                int topA = (flip) ? 0 : 315;
                int botA = (flip) ? 0 : 320;

                FingerGames.Instance.SpriteBatch.Draw(_pixel, player.LeftBorder, transBlack);
                FingerGames.Instance.SpriteBatch.Draw(_pixel, player.RightBorder, transBlack);
                FingerGames.Instance.SpriteBatch.Draw(_pixel, player.LeftBox, transBlack);
                FingerGames.Instance.SpriteBatch.Draw(_pixel, player.RightBox, transBlack);

                if (player._rightFootLoc.HasValue)
                {
                    FingerGames.Instance.SpriteBatch.Draw((flip) ? LeftFoot : RightFoot, new Vector2((int)player._rightFootLoc.Value.X, (int)player._rightFootLoc.Value.Y), null, Color.White, (flip) ? (float)Math.PI : 0f, new Vector2(LeftFoot.Width / 2, LeftFoot.Height / 2), 1.0f, SpriteEffects.None, 0);
                }
                if (player._leftFootLoc.HasValue)
                {
                    FingerGames.Instance.SpriteBatch.Draw((flip) ? RightFoot : LeftFoot, new Vector2((int)player._leftFootLoc.Value.X , (int)player._leftFootLoc.Value.Y), null, Color.White, (flip) ? (float)Math.PI : 0f, new Vector2(LeftFoot.Width / 2, LeftFoot.Height / 2), 1.0f, SpriteEffects.None, 0);
                }

                string message = (player._hitHurdle) ? "HIT HURDLE!" : player.Message;
                if (!string.IsNullOrEmpty(message))
                {
                    Vector2 fontSize = _font.MeasureString(message);
                    FingerGames.Instance.SpriteBatch.DrawString(_font, message, new Vector2(player._bounds.Center.X, (player.Flip) ? 480 : 0), Color.White, (flip) ? (float)Math.PI : 0f, new Vector2(fontSize.X / 2, 0), 1.0f, SpriteEffects.None, 0f);
                }

                message = string.Format("{0:0.00}s", _elapsedtime.TotalSeconds);
                Vector2 fontSize2 = _fixedFont.MeasureString(message);
                FingerGames.Instance.SpriteBatch.DrawString(_fixedFont, message, new Vector2(player._bounds.Center.X, (player.Flip) ? 240 : 240), Color.White, (flip) ? (float)Math.PI : 0f, new Vector2(fontSize2.X / 2, fontSize2.Y / 2), 0.6f, SpriteEffects.None, 0f);

                message = string.Format("{0:0.00}mph", player._speed.Y / 10);
                Vector2 fontSize3 = _fixedFont.MeasureString(message);
                FingerGames.Instance.SpriteBatch.DrawString(_fixedFont, message, new Vector2(player._bounds.Center.X, (player.Flip) ? 240 - fontSize2.Y / 3 : 240 + fontSize2.Y / 3), Color.White, (flip) ? (float)Math.PI : 0f, new Vector2(fontSize3.X / 2, fontSize3.Y / 2), 0.6f, SpriteEffects.None, 0f);

                message = string.Format("{0} left", RaceLength - player._position.Y);
                fontSize3 = _fixedFont.MeasureString(message);
                FingerGames.Instance.SpriteBatch.DrawString(_fixedFont, message, new Vector2(player._bounds.Center.X, (player.Flip) ? 240 + fontSize2.Y / 3 : 240 - fontSize2.Y / 3), Color.White, (flip) ? (float)Math.PI : 0f, new Vector2(fontSize3.X / 2, fontSize3.Y / 2), 0.6f, SpriteEffects.None, 0f);

                startX += width;
            }
            base.Draw(gameTime);
        }

        public override void DrawEnd(GameTime gameTime)
        {
            int repeatCount = (int)Math.Ceiling((float)480 / _track.Height);

            Vector2 source = new Vector2(0, 0);
            for (int i = 0; i < repeatCount + 2; ++i)
            {
                FingerGames.Instance.SpriteBatch.Draw(_track, new Rectangle((int)source.X, (int)source.Y, _track.Width, _track.Height), Color.White);
                source.Y += _track.Height;
            }
            _timePanel.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        public override void UpdateEnd(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            base.UpdateEnd(gameTime, ref gameState, justTransitioned);

            if (justTransitioned)
            {
                this._timeLabel.Text = string.Format("{0:0000.00} seconds", FingerGames.Instance.GameManager.Players[0].Score);
                if (FingerGames.Instance.GameManager.Players.Count > 1)
                {
                    this._timeLabel2.Text = string.Format("{0:0000.00} seconds", FingerGames.Instance.GameManager.Players[1].Score);
                    this._timeLabel.Bounds = new Rectangle(0, 0, 800, 35);
                    this._timeLabel2.Bounds = new Rectangle(0, 0, 800, 35);
                    this._timeLabel.FontZoom = 0.5f;
                    this._timeLabel2.FontZoom = 0.5f;
                    this._timePanel.ForceLayout();
                }
                else
                {
                    this._timeLabel2.Text = "";
                    this._timeLabel.Bounds = new Rectangle(0, 0, 800, 70);
                    this._timeLabel.FontZoom = 1;
                    this._timePanel.ForceLayout();
                }
            }

            if (gameState == InGameState.ScoreGame)
            {
                if (justTransitioned)
                {
                    // we need to rank our people
                    List<GamePlayer> players = new List<GamePlayer>(FingerGames.Instance.GameManager.Players);
                    players.Sort(new Comparison<GamePlayer>(CompareScores));

                    double topScore = players.Count * 1000;
                    for (int i = 0; i < players.Count; ++i)
                    {
                        players[i].OverallScore += topScore;
                        topScore -= 1000;
                    }
                }
            }
        }

        private int CompareScores(GamePlayer a, GamePlayer b)
        {
            return (a.Score.CompareTo(b.Score));
        }

        private bool _needsConfigure = false;
        void raceLengthButton_Click(object sender, EventArgs e)
        {
            XnaButton button = (XnaButton)sender;
            RaceLengths length = (RaceLengths)button.Tag;
            _needsConfigure = false;

            if (FingerGames.IsTrial && (length == RaceLengths.Medium || length == RaceLengths.Marathon))
            {
                FingerGames.Instance.SetStage(Stage.Trial);
            }
            else
            {
                RaceLength = (int)length;

                HurdleDistance = (int)FingerGames.Randomizer.Next(RaceLength / 10, RaceLength / 2);
                GameManager.GameVariation = length.ToString();
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
