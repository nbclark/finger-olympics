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
    abstract class BaseGame
    {
        private static Texture2D _unknownTexture = null;
        private static SpriteFont _menuFont = null;
        private static XnaButton _resumeButton;
        private XnaScoreCenter _scoreCenter;
        protected Texture2D _pixel = null;
        protected List<IDisposable> Assets = new List<IDisposable>();

        public BaseGame(GameManager gameManager, string scoreUnits, ScoreCenter.SortDirection direction)
        {
            this.ScoreUnits = scoreUnits;
            this.SortDirection = direction;
            this.GameManager = gameManager;

            if (null == _unknownTexture)
            {
                _unknownTexture = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\logo_unknown");
            }
            this.Logo = _unknownTexture;
            this.Variation = string.Empty;
        }

        void _resumeButton_Click(object sender, EventArgs e)
        {
            _resumeButton.Tag = true;
        }

        protected GameManager GameManager
        {
            get;
            private set;
        }

        public abstract string Title
        {
            get;
        }

        public string Variation
        {
            get;
            set;
        }

        public string ScoreUnits
        {
            get;
            set;
        }

        public ScoreCenter.SortDirection SortDirection
        {
            get;
            set;
        }

        public abstract string Description
        {
            get;
        }

        public virtual Texture2D Logo
        {
            get;
            set;
        }

        protected virtual float IntroDelay
        {
            get { return 2.0f; }
        }

        protected virtual float EndDelay
        {
            get { return 2.0f; }
        }

        protected virtual float PauseDelay
        {
            get { return 0.5f; }
        }
        
        public virtual void Reset()
        {
            //
        }

        public virtual void LoadContent()
        {
            _pixel = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\pixel");
            _menuFont = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");

            _resumeButton = new XnaButton(new Rectangle(0, 0, 800, 480));
            _resumeButton.Text = "Tap Screen to Resume...";
            _resumeButton.Click += new EventHandler(_resumeButton_Click);

            _scoreCenter = new XnaScoreCenter();
            _scoreCenter.Bounds = new Rectangle(0, 0, 800, 480);
            _scoreCenter.ContinueClicked += new EventHandler(_scoreCenter_ContinueClicked);

            this.Assets.Add(_pixel);
        }

        public virtual void UnloadContent()
        {
            foreach (IDisposable asset in this.Assets)
            {
                asset.Dispose();
            }
            this.Assets.Clear();
        }

        public virtual bool HandleBackClick(ref InGameState gameState)
        {
            if (gameState == InGameState.Intro)
            {
                return true;
            }
            else if (gameState == InGameState.Active)
            {
                gameState = InGameState.Pause;
                return true;
            }
            return false;
        }

        public virtual void Update(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            foreach (IDisposable asset in this.Assets)
            {
                asset.Dispose();
            }
            this.Assets.Clear();
        }

        public virtual void Draw(GameTime gameTime)
        {
            //
        }

        private TimeSpan _introStartTime = TimeSpan.MinValue;
        public virtual void UpdateIntro(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (_introStartTime == TimeSpan.MinValue)
            {
                _introStartTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(this.IntroDelay));
            }
            else if (gameTime.TotalGameTime > _introStartTime)
            {
                _introStartTime = TimeSpan.MinValue;
                gameState = InGameState.Active;
            }
        }

        public virtual void DrawIntro(GameTime gameTime)
        {
            FingerGames.Instance.SpriteBatch.Draw(this.Logo, new Rectangle(0, 0, 800, 480), Color.White);
        }

        public virtual void UpdateConfigure(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            gameState = InGameState.Intro;
        }

        public virtual void DrawConfigure(GameTime gameTime)
        {
        }

        private TimeSpan _transitionTime;

        public virtual void UpdatePause(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (justTransitioned)
            {
                _transitionTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(this.PauseDelay));
            }
            else if (gameTime.TotalGameTime > _transitionTime)
            {
                _resumeButton.Tag = false;
                _resumeButton.Update(gameTime);

                if ((bool)_resumeButton.Tag)
                {
                    gameState = InGameState.Active;
                }
            }
        }

        public virtual void DrawPause(GameTime gameTime)
        {
            Draw(gameTime);

            FingerGames.Instance.SpriteBatch.Draw(this._pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 0x7f));
            _resumeButton.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        private void _scoreCenter_ContinueClicked(object sender, EventArgs e)
        {
            ((XnaScoreCenter)sender).Tag = true;
        }

        public virtual void UpdateScore(GameTime gameTime, ref InGameState gameState, bool justTransitioned, bool showSummary)
        {
            if (justTransitioned)
            {
                _transitionTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(1));
                _scoreCenter.Prepare(GameManager, GameManager.GameUnits);
                _scoreCenter.Update(gameTime);
            }
            else
            {
                _scoreCenter.Tag = false;
                _scoreCenter.Update(gameTime);

                if (gameTime.TotalGameTime > _transitionTime)
                {
                    _scoreCenter.EnableContinueButton();

                    if ((bool)_scoreCenter.Tag)
                    {
                        gameState = InGameState.Exit;
                    }
                }
            }
        }

        public virtual void DrawScore(GameTime gameTime, bool showSummary)
        {
            FingerGames.Instance.SpriteBatch.Draw(this._pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 255));
            _scoreCenter.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        public virtual void UpdateEnd(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (justTransitioned)
            {
                _transitionTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(this.EndDelay));
            }
            else if (gameTime.TotalGameTime > _transitionTime)
            {
                gameState = InGameState.ScoreGame;
            }
        }

        public virtual void DrawEnd(GameTime gameTime)
        {
            FingerGames.Instance.SpriteBatch.Draw(this.Logo, new Rectangle(0, 0, 800, 480), Color.White);
            FingerGames.Instance.SpriteBatch.Draw(this._pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 0x7f));
        }
    }
}
