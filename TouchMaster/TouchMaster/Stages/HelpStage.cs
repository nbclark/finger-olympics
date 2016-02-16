using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    class HelpStage : BaseStage
    {
        private Texture2D _introLogo;
        private SpriteFont _font;
        private XnaImage _introImage;
        private Texture2D _pixel = null;

        public HelpStage(Game game)
            : base(game)
        {
        }

        public override Stage Stage
        {
            get { return MobileSrc.FantasticFingerFun.Stage.Help; }
        }

        public override void LoadContent()
        {
            _introLogo = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\IntroLogo");
            _pixel = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\pixel");
            _font = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");

            this.Assets.Add(_introLogo);
            this.Assets.Add(_pixel);

            _introImage = new XnaImage(_introLogo, new Rectangle(800 - _introLogo.Width, 480 - _introLogo.Height, _introLogo.Width, _introLogo.Height));
            _introImage.BlendColor = new Color(60,60,60);

            VerticalStackPanel panel = new VerticalStackPanel();
            panel.Padding = new Rectangle(0, 0, 0, 0);
            panel.HorizontalAlignment = HorizontalAlignment.Left;
            panel.Bounds = new Rectangle(10, 10, 780, 470);

            Dictionary<string, string> helpSupport = new Dictionary<string, string>();
            helpSupport.Add("Racer", "Get the fastest time by using your finger to drag your car around the track. The car will follow your finger, unless your finger leaves the track.");
            helpSupport.Add("Runner", "Alternate tapping your fingers in each darkened box to propel the runner. Jump the hurdles by removing both fingers from the screen as the hurdle crosses the bottom of the screen.");
            helpSupport.Add("Twister", "Start by placing a different finger on the pusling circles. You must then move the indicated finger to the new pulsing circle. The first player to pick up a pinned finger loses.");

            foreach (string game in helpSupport.Keys)
            {
                XnaLabel gameNameLabel = new XnaLabel(new Rectangle(10, 0, 780, 48));
                gameNameLabel.HorizontalAlignment = HorizontalAlignment.Left;
                gameNameLabel.VerticalAlignment = VerticalAlignment.Middle;
                gameNameLabel.Font = _font;
                gameNameLabel.FontZoom = 1.0f;
                gameNameLabel.Text = game;
                panel.AddChild(gameNameLabel);

                List<string> segments = new List<string>();

                string newWord = "";
                foreach (string word in helpSupport[game].Split(' '))
                {
                    if (_font.MeasureString(newWord + " " + word).X * .6 < 750)
                    {
                        newWord += " " + word;
                    }
                    else
                    {
                        XnaLabel helpLabel = new XnaLabel(new Rectangle(0, 0, 800, 28));
                        helpLabel.HorizontalAlignment = HorizontalAlignment.Left;
                        helpLabel.VerticalAlignment = VerticalAlignment.Top;
                        helpLabel.Font = _font;
                        helpLabel.FontZoom = 0.6f;
                        helpLabel.Text = newWord.Trim();
                        panel.AddChild(helpLabel);

                        newWord = word;
                    }
                }

                if (newWord.Length > 0)
                {
                    XnaLabel helpLabel = new XnaLabel(new Rectangle(0, 0, 800, 30));
                    helpLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    helpLabel.VerticalAlignment = VerticalAlignment.Top;
                    helpLabel.Font = _font;
                    helpLabel.FontZoom = 0.6f;
                    helpLabel.Text = newWord.Trim();
                    panel.AddChild(helpLabel);
                }
            }

            this.Children.Add(_introImage);
            this.Children.Add(panel);

            base.LoadContent();
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
            FingerGames.Instance.SpriteBatch.Draw(this._pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 0));
            base.Draw(gameTime);
        }
    }
}
