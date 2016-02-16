using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    class TrialStage : BaseStage
    {
        private Texture2D _introLogo;
        private SpriteFont _font;
        private XnaImage _introImage;
        private Texture2D _pixel = null;

        public TrialStage(Game game)
            : base(game)
        {
        }

        public override Stage Stage
        {
            get { return MobileSrc.FantasticFingerFun.Stage.Trial; }
        }

        public override void LoadContent()
        {
            _introLogo = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\IntroLogo");
            _pixel = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\pixel");
            _font = FingerGames.Instance.Content.Load<SpriteFont>(@"Fonts\MenuFont");

            _introImage = new XnaImage(_introLogo, new Rectangle(800 - _introLogo.Width, 480 - _introLogo.Height, _introLogo.Width, _introLogo.Height));
            _introImage.BlendColor = new Color(60,60,60);

            VerticalStackPanel panel = new VerticalStackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Bounds = new Rectangle(10, 100, 780, 300);

            XnaLabel trialLabel1 = new XnaLabel(new Rectangle(0, 0, 800, 40));
            trialLabel1.Font = _font;
            trialLabel1.FontZoom = 0.6f;
            trialLabel1.Text = "You are running the trial version of Fantastic Finger Fun.";

            XnaLabel trialLabel2 = new XnaLabel(new Rectangle(0, 0, 800, 40));
            trialLabel2.Font = _font;
            trialLabel2.FontZoom = 0.6f;
            trialLabel2.Text = "The full version unlocks more race tracks,";

            XnaLabel trialLabel3 = new XnaLabel(new Rectangle(0, 0, 800, 40));
            trialLabel3.Font = _font;
            trialLabel3.FontZoom = 0.6f;
            trialLabel3.Text = "game modes, and challenges. To continue using";

            XnaLabel trialLabel4 = new XnaLabel(new Rectangle(0, 0, 800, 40));
            trialLabel4.Font = _font;
            trialLabel4.FontZoom = 0.6f;
            trialLabel4.Text = "the Trial mode, click the back button.";

            panel.AddChild(trialLabel1);
            panel.AddChild(trialLabel2);
            panel.AddChild(trialLabel3);
            panel.AddChild(trialLabel4);

            XnaButton purchaseButton = new XnaButton(new Rectangle(0, 400, 800, 80));
            purchaseButton.BorderWidth = 0;
            purchaseButton.FontZoom = 0.8f;
            purchaseButton.HorizontalAlignment = HorizontalAlignment.Center;
            purchaseButton.Text = "Click Here To Purchase Fantastic Finger Fun...";
            purchaseButton.Font = _font;
            purchaseButton.Click += new EventHandler(purchaseButton_Click);

            this.Children.Add(_introImage);
            this.Children.Add(panel);
            this.Children.Add(purchaseButton);

            base.LoadContent();
        }

        void purchaseButton_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.MarketplaceDetailTask searchTask = new Microsoft.Phone.Tasks.MarketplaceDetailTask();
            searchTask.Show();
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
            FingerGames.Instance.SpriteBatch.Draw(this._pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 0));
            base.Draw(gameTime);
        }
    }
}
