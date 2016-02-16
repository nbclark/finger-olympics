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
    class XnaScoreCenter : XnaContainer
    {
        public event EventHandler ContinueClicked;

        private static SpriteFont _menuFont;
        private bool _isScoreLoading = false;
        private bool _showContinueButton = false;
        private XnaLabel _loadingLabel;
        private XnaButton _continueButton, _totalLabel, _scoreLabel;
        private HorizontalStackPanel _horizontalScorePanel;
        private VerticalStackPanel _scorePanel, _nameScorePanel, _scoreScorePanel, _totalScorePanel;

        public XnaScoreCenter()
        {
            if (null == _menuFont)
            {
                _menuFont = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");
            }

            _continueButton = new XnaButton(new Rectangle(0, 400, 800, 80));
            _continueButton.Text = "Tap Here to Continue...";
            _continueButton.Click += new EventHandler(_continueButton_Click);

            _horizontalScorePanel = new HorizontalStackPanel();
            _horizontalScorePanel.VerticalAlignment = VerticalAlignment.Top;
            _horizontalScorePanel.Bounds = new Rectangle(0, 0, 800, 400);
            _horizontalScorePanel.Padding = new Rectangle(0, 0, 0, 0);

            _nameScorePanel = new VerticalStackPanel();
            _nameScorePanel.HorizontalAlignment = HorizontalAlignment.Left;
            _nameScorePanel.FixedSize = true;
            _nameScorePanel.Bounds = new Rectangle(0, 0, 300, 400);
            _nameScorePanel.BorderColor = Color.White;
            _nameScorePanel.BorderWidth = 1;

            _scoreScorePanel = new VerticalStackPanel();
            _scoreScorePanel.FixedSize = true;
            _scoreScorePanel.Bounds = new Rectangle(0, 0, 250, 400);
            _scoreScorePanel.BorderColor = Color.White;
            _scoreScorePanel.BorderWidth = 1;

            _totalScorePanel = new VerticalStackPanel();
            _totalScorePanel.FixedSize = true;
            _totalScorePanel.Bounds = new Rectangle(0, 0, 250, 400);
            _totalScorePanel.BorderColor = Color.White;
            _totalScorePanel.BorderWidth = 1;

            XnaLabel nameLabel = new XnaButton(new Rectangle(0, 0, 300, 70));
            nameLabel.Text = "Name";
            nameLabel.FontZoom = 1.25f;
            nameLabel.Font = _menuFont;
            nameLabel.HorizontalAlignment = HorizontalAlignment.Left;

            _nameScorePanel.AddChild(nameLabel);

            _scoreLabel = new XnaButton(new Rectangle(0, 0, 250, 70));
            _scoreLabel.Text = "Score";
            _scoreLabel.FontZoom = 1.25f;
            _scoreLabel.Font = _menuFont;
            _scoreLabel.HorizontalAlignment = HorizontalAlignment.Center;

            _scoreScorePanel.AddChild(_scoreLabel);

            _totalLabel = new XnaButton(new Rectangle(0, 0, 250, 70));
            _totalLabel.Text = "Overall";
            _totalLabel.FontZoom = 1.25f;
            _totalLabel.Font = _menuFont;
            _totalLabel.HorizontalAlignment = HorizontalAlignment.Center;

            _totalScorePanel.AddChild(_totalLabel);

            _horizontalScorePanel.AddChild(_nameScorePanel);
            _horizontalScorePanel.AddChild(_scoreScorePanel);
            _horizontalScorePanel.AddChild(_totalScorePanel);

            _scorePanel = new VerticalStackPanel();
            _scorePanel.Bounds = new Rectangle(0, 0, 800, 480);
            _scorePanel.FixedSize = true;

            _scorePanel.AddChild(_horizontalScorePanel);
            _scorePanel.AddChild(_continueButton);

            _loadingLabel = new XnaLabel(new Rectangle(0, 0, 800, 480));
            _loadingLabel.Text = "Loading...";

            _continueButton.Click += new EventHandler(_continueButton_Click);
        }

        void _continueButton_Click(object sender, EventArgs e)
        {
            if (null != ContinueClicked)
            {
                ContinueClicked(this, e);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_isScoreLoading)
            {
                _loadingLabel.Update(gameTime);
            }
            else
            {
                _scorePanel.Update(gameTime);

                if (_showContinueButton)
                {
                    _continueButton.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_isScoreLoading)
            {
                _loadingLabel.Draw(spriteBatch, gameTime);
            }
            else
            {
                _scorePanel.Draw(spriteBatch, gameTime);

                if (_showContinueButton)
                {
                    _continueButton.Draw(spriteBatch, gameTime);
                }
            }
        }

        public void EnableContinueButton()
        {
            _showContinueButton = true;
        }

        public void Prepare(GameManager gameManager, string scoreUnits)
        {
            _scoreLabel.Text = scoreUnits;
            _showContinueButton = false;

            while (_nameScorePanel.Children.Count > 1)
            {
                _nameScorePanel.Children.RemoveAt(1);
            }
            while (_scoreScorePanel.Children.Count > 1)
            {
                _scoreScorePanel.Children.RemoveAt(1);
            }
            while (_totalScorePanel.Children.Count > 1)
            {
                _totalScorePanel.Children.RemoveAt(1);
            }

            if (gameManager.Players.Count > 1)
            {
                _isScoreLoading = false;
                _totalLabel.Text = "Overall";
                foreach (GamePlayer player in gameManager.Players)
                {
                    XnaLabel nameLabel = new XnaButton(new Rectangle(0, 0, 300, 60));
                    nameLabel.Text = player.Name;
                    nameLabel.Font = _menuFont;
                    nameLabel.HorizontalAlignment = HorizontalAlignment.Left;

                    _nameScorePanel.AddChild(nameLabel);

                    XnaButton scoreLabel = new XnaButton(new Rectangle(0, 0, 250, 60));
                    scoreLabel.Text = string.Format("{0:0000.0}", player.Score);
                    scoreLabel.Font = _menuFont;
                    scoreLabel.HorizontalAlignment = HorizontalAlignment.Center;

                    _scoreScorePanel.AddChild(scoreLabel);

                    XnaButton totalLabel = new XnaButton(new Rectangle(0, 0, 250, 60));
                    totalLabel.Text = string.Format("{0:0000.0}", player.OverallScore);
                    totalLabel.Font = _menuFont;
                    totalLabel.HorizontalAlignment = HorizontalAlignment.Center;

                    _totalScorePanel.AddChild(totalLabel);
                }
            }
            else
            {
                _totalLabel.Text = "Rank";
                _isScoreLoading = true;
                ScoreCenterManager.RegisterScore(gameManager.GameName, gameManager.GameVariation, (float)gameManager.Players[0].Score, gameManager.SortDirection, new EventHandler<ScoreCenter.RegisterScoreCompletedEventArgs>(ScoresReady));
            }
        }

        private void ScoresReady(object sender, ScoreCenter.RegisterScoreCompletedEventArgs e)
        {
            _isScoreLoading = false;

            if (null != e)
            {
                foreach (ScoreCenter.ScoreItem item in e.Result)
                {
                    float scale = (item.IsLast) ? 1.0f : .8f;
                    int height = (item.IsLast) ? 45 : 35;
                    Color foreColor = (item.IsLast) ? Color.LightBlue : Color.White;

                    XnaButton nameLabel = new XnaButton(new Rectangle(0, 0, 300, height));
                    nameLabel.FontZoom = scale;
                    nameLabel.Color = foreColor;
                    nameLabel.Text = item.User;
                    nameLabel.Font = _menuFont;
                    nameLabel.HorizontalAlignment = HorizontalAlignment.Left;

                    _nameScorePanel.AddChild(nameLabel);

                    XnaButton scoreLabel = new XnaButton(new Rectangle(0, 0, 250, height));
                    scoreLabel.FontZoom = scale;
                    scoreLabel.Color = foreColor;
                    scoreLabel.Text = string.Format("{0:0000.0}", item.Score);
                    scoreLabel.Font = _menuFont;
                    scoreLabel.HorizontalAlignment = HorizontalAlignment.Center;

                    _scoreScorePanel.AddChild(scoreLabel);

                    XnaButton totalLabel = new XnaButton(new Rectangle(0, 0, 250, height));
                    totalLabel.FontZoom = scale;
                    totalLabel.Color = foreColor;
                    totalLabel.Text = string.Format("#{0}", item.Index);
                    totalLabel.Font = _menuFont;
                    totalLabel.HorizontalAlignment = HorizontalAlignment.Center;

                    _totalScorePanel.AddChild(totalLabel);
                }
            }
        }
    }
}
