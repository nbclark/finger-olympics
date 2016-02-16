using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MobileSrc.FantasticFingerFun
{
    class GameManager
    {
        private List<GamePlayer> _players = new List<GamePlayer>();
        private List<BaseGame> _games = new List<BaseGame>();

        public GameManager(string anid, Game game, IEnumerable<GamePlayer> players)
        {
            this.AnonymousId = anid;
            this.Game = game;
            this._players.AddRange(players);
        }
        public string AnonymousId
        {
            get;
            set;
        }
        public string GameName
        {
            get;
            set;
        }
        public string GameVariation
        {
            get;
            set;
        }
        public string GameUnits
        {
            get;
            set;
        }
        public ScoreCenter.SortDirection SortDirection
        {
            get;
            set;
        }
        public Game Game
        {
            get;
            private set;
        }
        public void SetSinglePlayer()
        {
            _players.Clear();
            _players.Add(new GamePlayer(0, "Player 1"));
        }
        public void SetMultiPlayer()
        {
            _players.Clear();
            _players.Add(new GamePlayer(0, "Player 1"));
            _players.Add(new GamePlayer(1, "Player 2"));
        }
        public void SetGames(params BaseGame[] games)
        {
            _games.Clear();
            _games.AddRange(games);
        }
        public ReadOnlyCollection<GamePlayer> Players
        {
            get { return _players.AsReadOnly(); }
        }
        public ReadOnlyCollection<BaseGame> Games
        {
            get { return _games.AsReadOnly(); }
        }
    }
}
