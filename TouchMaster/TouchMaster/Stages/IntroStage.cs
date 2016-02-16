using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    class IntroStage : BaseStage
    {
        private Texture2D _introTitle, _introLogo;
        private SpriteFont _font;
        private XnaImage _introImage;

        public IntroStage(Game game)
            : base(game)
        {
        }

        public override Stage Stage
        {
            get { return MobileSrc.FantasticFingerFun.Stage.Intro; }
        }

        public override void LoadContent()
        {
            _introLogo = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\IntroLogo");
            _introTitle = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\IntroTitle");
            _font = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");

            this.Assets.Add(_introLogo);
            this.Assets.Add(_introTitle);

            _introImage = new XnaImage(_introLogo, new Rectangle(800 - _introLogo.Width, 480 - _introLogo.Height, _introLogo.Width, _introLogo.Height));
            this.Children.Add(_introImage);

            XnaImage titleImage = new XnaImage(_introTitle, new Rectangle(400 - _introTitle.Width / 2, 5, _introTitle.Width, _introTitle.Height));
            this.Children.Add(titleImage);

            VerticalStackPanel panel = new VerticalStackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Left;
            panel.Bounds = new Rectangle(titleImage.Bounds.X, 90, 100, 400);

            XnaButton singlePlayerButton = new XnaButton(new Rectangle(0, 0, 310, 54));
            singlePlayerButton.BorderWidth = 0;
            singlePlayerButton.HorizontalAlignment = HorizontalAlignment.Left;
            singlePlayerButton.Text = "Single Player";
            singlePlayerButton.Font = _font;
            singlePlayerButton.BackColor = Color.Transparent;
            singlePlayerButton.Click += new EventHandler(singlePlayerButton_Click);

            XnaButton multiPlayerButton = new XnaButton(new Rectangle(0, 0, 310, 54));
            multiPlayerButton.BorderWidth = 0;
            multiPlayerButton.HorizontalAlignment = HorizontalAlignment.Left;
            multiPlayerButton.Text = "Two Player";
            multiPlayerButton.Font = _font;
            multiPlayerButton.BackColor = Color.Transparent;
            multiPlayerButton.Click += new EventHandler(multiPlayerButton_Click);

            XnaButton soundToggle = new XnaButton(new Rectangle(0, 0, 310, 54));
            soundToggle.Tag = !Settings.Instance.SoundsEnabled;
            soundToggle.BorderWidth = 0;
            soundToggle.HorizontalAlignment = HorizontalAlignment.Left;
            soundToggle.Font = _font;
            soundToggle.BackColor = Color.Transparent;
            soundToggle.Click += new EventHandler(soundToggle_SelectionChanged);
            soundToggle_SelectionChanged(soundToggle, null);

            XnaButton fingerArrowToggle = new XnaButton(new Rectangle(0, 0, 310, 54));
            fingerArrowToggle.Tag = !Settings.Instance.ArrowsEnabled;
            fingerArrowToggle.BorderWidth = 0;
            fingerArrowToggle.HorizontalAlignment = HorizontalAlignment.Left;
            fingerArrowToggle.Font = _font;
            fingerArrowToggle.BackColor = Color.Transparent;
            fingerArrowToggle.Click += new EventHandler(fingerArrowToggle_SelectionChanged);
            fingerArrowToggle_SelectionChanged(fingerArrowToggle, null);

            panel.AddChild(singlePlayerButton);
            panel.AddChild(multiPlayerButton);
            panel.AddChild(fingerArrowToggle);
            panel.AddChild(soundToggle);

            XnaButton exitButton = new XnaButton(new Rectangle(titleImage.Bounds.Right - 310, 480 - 64, 310, 64));
            exitButton.BorderWidth = 0;
            exitButton.HorizontalAlignment = HorizontalAlignment.Right;
            exitButton.Text = "Exit";
            exitButton.Font = _font;
            exitButton.BackColor = Color.Transparent;
            exitButton.Click += new EventHandler(exitButton_Click);

            XnaButton helpButton = new XnaButton(new Rectangle(titleImage.Bounds.X, 480 - 64, 310, 64));
            helpButton.BorderWidth = 0;
            helpButton.HorizontalAlignment = HorizontalAlignment.Left;
            helpButton.Text = "Help";
            helpButton.Font = _font;
            helpButton.BackColor = Color.Transparent;
            helpButton.Click += new EventHandler(helpButton_Click);

            this.Children.Add(panel);
            this.Children.Add(exitButton);
            this.Children.Add(helpButton);

            base.LoadContent();
        }

        void helpButton_Click(object sender, EventArgs e)
        {
            FingerGames.Instance.SetStage(FantasticFingerFun.Stage.Help);
        }

        void soundToggle_SelectionChanged(object sender, EventArgs e)
        {
            XnaButton button = (XnaButton)sender;
            bool isSelected = !((bool)button.Tag);

            if (isSelected)
            {
                button.Text = "sound: on";
            }
            else
            {
                button.Text = "sound: off";
            }
            Settings.Instance.SoundsEnabled = isSelected;
            Settings.Instance.Save();

            button.Tag = isSelected;
        }

        void fingerArrowToggle_SelectionChanged(object sender, EventArgs e)
        {
            XnaButton button = (XnaButton)sender;
            bool isSelected = !((bool)button.Tag);

            if (isSelected)
            {
                button.Text = "arrows: on";
            }
            else
            {
                button.Text = "arrows: off";
            }
            Settings.Instance.ArrowsEnabled = isSelected;
            Settings.Instance.Save();

            button.Tag = isSelected;
        }

        void singlePlayerButton_Click(object sender, EventArgs e)
        {
            FingerGames.Instance.GameManager.SetSinglePlayer();
            FingerGames.Instance.SetStage(MobileSrc.FantasticFingerFun.Stage.SelectGame);
        }

        void multiPlayerButton_Click(object sender, EventArgs e)
        {
            FingerGames.Instance.GameManager.SetMultiPlayer();
            FingerGames.Instance.SetStage(MobileSrc.FantasticFingerFun.Stage.SelectGame);
        }

        void difficultyButton_Click(object sender, EventArgs e)
        {
            //
        }

        void exitButton_Click(object sender, EventArgs e)
        {
            Game.Exit();
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
