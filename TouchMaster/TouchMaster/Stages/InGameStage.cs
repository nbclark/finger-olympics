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

namespace MobileSrc.FantasticFingerFun
{
    public enum InGameState
    {
        None,
        Configure,
        Intro,
        Active,
        Pause,
        End,
        ScoreGame,
        ScoreSummary,
        Exit
    }
    class InGameStage : BaseStage
    {
        private static InGameStage _instance;
        private BaseGame _activeGame;
        private int _gameIndex = 0;
        private InGameState _gameState = InGameState.Configure;

        public InGameStage(Game game)
            : base(game)
        {
            _instance = this;
        }

        public static InGameStage Instance
        {
            get { return _instance; }
        }

        public override Stage Stage
        {
            get { return MobileSrc.FantasticFingerFun.Stage.InGame; }
        }

        public override void LoadContent()
        {
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _activeGame.UnloadContent();
        }

        public override void Activate()
        {
            _gameState = InGameState.Configure;
            _gameIndex = 0;

            foreach (BaseGame game in FingerGames.Instance.GameManager.Games)
            {
                game.Reset();
            }
            SetActiveGame(FingerGames.Instance.GameManager.Games[0]);
            _justTransitioned = true;
        }

        private void SetActiveGame(BaseGame game)
        {
            _activeGame = game;

            FingerGames.Instance.GameManager.GameName = _activeGame.Title;
            FingerGames.Instance.GameManager.GameVariation = _activeGame.Variation;
            FingerGames.Instance.GameManager.GameUnits = _activeGame.ScoreUnits;
            FingerGames.Instance.GameManager.SortDirection = _activeGame.SortDirection;
        }

        public override bool HandleBackClick()
        {
            InGameState oldState = _gameState;
            bool handled = _activeGame.HandleBackClick(ref _gameState);

            if (oldState != _gameState)
            {
                _justTransitioned = true;
            }

            return handled;
        }

        private bool _justTransitioned = false;
        public override void Update(GameTime gameTime)
        {
            InGameState oldState = _gameState;

            base.Update(gameTime);

            switch (_gameState)
            {
                case InGameState.Configure:
                    {
                        _activeGame.UpdateConfigure(gameTime, ref _gameState, _justTransitioned);
                    }
                    break;
                case InGameState.Intro:
                    {
                        _activeGame.UpdateIntro(gameTime, ref _gameState, _justTransitioned);
                    }
                    break;
                case InGameState.Active:
                    {
                        _activeGame.Update(gameTime, ref _gameState, _justTransitioned);
                    }
                    break;
                case InGameState.Pause:
                    {
                        _activeGame.UpdatePause(gameTime, ref _gameState, _justTransitioned);
                    }
                    break;
                case InGameState.End:
                    {
                        _activeGame.UpdateEnd(gameTime, ref _gameState, _justTransitioned);
                    }
                    break;
                case InGameState.ScoreSummary :
                case InGameState.ScoreGame:
                    {
                        _activeGame.UpdateScore(gameTime, ref _gameState, _justTransitioned, (InGameState.ScoreSummary == _gameState));
                    }
                    break;
                case InGameState.Exit:
                    {
                        if (_gameIndex + 1 < FingerGames.Instance.GameManager.Games.Count)
                        {
                            _gameIndex++;
                            SetActiveGame(FingerGames.Instance.GameManager.Games[_gameIndex]);
                            _gameState = InGameState.Configure;
                        }
                        else
                        {
                            FingerGames.Instance.GoBack();
                        }
                    }
                    break;
            }

            _justTransitioned = (oldState != _gameState);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_justTransitioned)
            {
                return;
            }

            switch (_gameState)
            {
                case InGameState.Configure:
                    {
                        _activeGame.DrawConfigure(gameTime);
                    }
                    break;
                case InGameState.Intro:
                    {
                        _activeGame.DrawIntro(gameTime);
                    }
                    break;
                case InGameState.Active:
                    {
                        _activeGame.Draw(gameTime);
                    }
                    break;
                case InGameState.Pause:
                    {
                        _activeGame.DrawPause(gameTime);
                    }
                    break;
                case InGameState.ScoreGame:
                case InGameState.ScoreSummary:
                    {
                        _activeGame.DrawScore(gameTime, (InGameState.ScoreSummary == _gameState));
                    }
                    break;
                case InGameState.End:
                    {
                        _activeGame.DrawEnd(gameTime);
                    }
                    break;
            }
        }
    }
}
