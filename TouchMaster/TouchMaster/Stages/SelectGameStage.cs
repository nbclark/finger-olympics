using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    class SelectGameStage : BaseStage
    {
        private List<BaseGame> _games = new List<BaseGame>();
        public SelectGameStage(Game game)
            : base(game)
        {
        }

        public override Stage Stage
        {
            get { return MobileSrc.FantasticFingerFun.Stage.SelectGame; }
        }

        public override void LoadContent()
        {
            SpriteFont font = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");
            
            HorizontalStackPanel horizontalPanel = new HorizontalStackPanel();
            horizontalPanel.VerticalAlignment = VerticalAlignment.Top;
            horizontalPanel.FixedSize = false;
            horizontalPanel.Bounds = new Rectangle(0, 0, 800, 480);
            horizontalPanel.Padding = new Rectangle(10, 10, 10, 10);

            foreach (BaseGame game in new BaseGame[] { new Games.FingerRacerGame(FingerGames.Instance.GameManager), new Games.FingerRunnerGame(FingerGames.Instance.GameManager), new Games.FingerTwisterGame(FingerGames.Instance.GameManager) })
            {
                _games.Add(game);

                game.LoadContent();
                XnaImageButton imageButton = new XnaImageButton(game.Logo, new Rectangle(5, 5, 240, 240));
                imageButton.Tag = game;
                imageButton.BackColor = Color.Gray;
                imageButton.Click += new EventHandler(imageButton_Click);

                XnaButton textButton = new XnaButton(new Rectangle(5, 5, 240, 60));
                textButton.BorderWidth = 0;
                textButton.BackColor = Color.Transparent;
                textButton.Text = game.Title;
                textButton.Font = font;
                textButton.Tag = game;
                textButton.Click += new EventHandler(imageButton_Click);

                VerticalStackPanel panel = new VerticalStackPanel();
                panel.Bounds = new Rectangle(0, 0, 240, 0);
                panel.HorizontalAlignment = HorizontalAlignment.Center;
                panel.AddChild(imageButton);
                panel.AddChild(textButton);

                horizontalPanel.AddChild(panel);
            }

            VerticalStackPanel vertPanel = new VerticalStackPanel();
            vertPanel.Bounds = new Rectangle(0, 0, 800, 0);
            vertPanel.HorizontalAlignment = HorizontalAlignment.Center;
            vertPanel.AddChild(horizontalPanel);

            XnaButton playAllText = new XnaButton(new Rectangle(0, 480 - 60, 800, 60));
            playAllText.BorderWidth = 0;
            playAllText.BackColor = Color.Transparent;
            playAllText.Text = "Play All Games";
            playAllText.Font = font;
            playAllText.Tag = null;
            playAllText.Click += new EventHandler(imageButton_Click);

            this.Children.Add(vertPanel);
            this.Children.Add(playAllText);

            base.LoadContent();
        }

        void imageButton_Click(object sender, EventArgs e)
        {
            XnaButton button = sender as XnaButton;

            if (null != button.Tag)
            {
                FingerGames.Instance.GameManager.SetGames(button.Tag as BaseGame);
            }
            else
            {
                FingerGames.Instance.GameManager.SetGames(_games.ToArray());
            }
            FingerGames.Instance.SetStage(MobileSrc.FantasticFingerFun.Stage.InGame, true);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
