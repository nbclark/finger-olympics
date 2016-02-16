using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileSrc.FantasticFingerFun
{
    public enum Stage
    {
        Intro,
        SelectGame,
        StartGame,
        InGame,
        EndGame,
        HighScore,
        Help,
        Trial
    }

    abstract class BaseStage
    {
        protected List<IDisposable> Assets = new List<IDisposable>();

        public BaseStage(Game game)
        {
            this.Game = game;
            this.Children = new List<XnaControl>();
        }

        protected Game Game
        {
            get;
            private set;
        }

        protected List<XnaControl> Children
        {
            get;
            private set;
        }

        public abstract Stage Stage
        {
            get;
        }

        public virtual void LoadContent()
        {
        }

        public virtual bool HandleBackClick()
        {
            return false;
        }

        public virtual void UnloadContent()
        {
            foreach (IDisposable asset in this.Assets)
            {
                asset.Dispose();
            }
            this.Assets.Clear();
        }

        public virtual void Activate()
        {
            //
        }

        public virtual void Deactivate()
        {
            //
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (XnaControl control in this.Children)
            {
                control.Draw(FingerGames.Instance.SpriteBatch, gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (XnaControl control in this.Children)
            {
                control.Update(gameTime);
            }
        }
    }
}
