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
using System.Xml;
using System.IO;

namespace MobileSrc.FantasticFingerFun.Games
{
    class FingerRacerGame : BaseGame
    {
        private SpriteFont _font;
        private Dictionary<TilePiece, Texture2D> _textureMap = new Dictionary<TilePiece, Texture2D>();
        private Texture2D _car;
        private Texture2D _raceFlags;
        private Point _carPosition = new Point(0, 0);
        private Point _carOffset = new Point(0, 0);
        private Point _prevPosition = new Point(-1, -1);
        private TimeSpan _messageTime = TimeSpan.MinValue;
        private TimeSpan _startTime = TimeSpan.MinValue;
        private TimeSpan _elapsedtime = TimeSpan.Zero;
        private int _completedLaps = 0;
        private float _decimalWidth = 0;
        private float _carAngle = 0;
        private float _prevCarAngle = 0;
        private int _playerIndex = 0;
        private SoundEffectInstance _engineIdle;
        private SoundEffectInstance _carScreech;
        private SoundEffectInstance _carHorn;
        private VerticalStackPanel _timePanel = new VerticalStackPanel();
        private XnaLabel _timeLabel = new XnaLabel(new Rectangle(0, 400, 800, 70));
        private VerticalStackPanel _mapPanel = new VerticalStackPanel();

        private static int RequiredLaps = 10;

        internal enum TilePiece
        {
            Start = 0,
            Grass = 1,
            Horizontal = 2,
            Vertical = 3,
            TopLeft = 4,
            TopRight = 5,
            BottomLeft = 6,
            BottomRight = 7
        }

        private TilePiece[][] _boardTiles = new TilePiece[][]
        {
            new TilePiece[] { TilePiece.Start,    TilePiece.Horizontal, TilePiece.TopRight,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass   },
            new TilePiece[] { TilePiece.Vertical,   TilePiece.Grass,      TilePiece.Vertical,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass   },
            new TilePiece[] { TilePiece.Vertical,   TilePiece.Grass,      TilePiece.BottomLeft, TilePiece.Horizontal, TilePiece.Horizontal, TilePiece.TopRight,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass   },
            new TilePiece[] { TilePiece.Vertical,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Vertical,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass   },
            new TilePiece[] { TilePiece.Vertical,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Vertical,   TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass   },
            new TilePiece[] { TilePiece.BottomLeft, TilePiece.Horizontal, TilePiece.Horizontal, TilePiece.Horizontal, TilePiece.Horizontal, TilePiece.BottomRight,TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass,      TilePiece.Grass   },
        };

        Dictionary<string, string> LoadedMaps = new Dictionary<string, string>();

        static string[] MapNames = new string[]
            {
                "Beginner",
                "TheBigB",
                "Mmm",
                "Staircase",
                "Gardens",
                "SideToSide",
                "Nascar",
                "Elevator",
                "Windy"
            };

        public FingerRacerGame(GameManager gameManager)
            : base(gameManager, "seconds", ScoreCenter.SortDirection.Ascending)
        {
        }

        public override string Title
        {
            get { return "Racer"; }
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override void LoadContent()
        {
            this._font = this.GameManager.Game.Content.Load<SpriteFont>(@"Fonts\GameFont");

            this._engineIdle = this.GameManager.Game.Content.Load<SoundEffect>(@"Sounds\Engine").CreateInstance();
            this._carScreech = this.GameManager.Game.Content.Load<SoundEffect>(@"Sounds\pop").CreateInstance();
            this._carHorn = this.GameManager.Game.Content.Load<SoundEffect>(@"Sounds\Horn").CreateInstance();

            this.Logo = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\racer");
            this._car = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\car");
            this._raceFlags = this.GameManager.Game.Content.Load<Texture2D>(@"Textures\racerflags");
            
            _textureMap.Add(TilePiece.Grass, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\grass"));
            _textureMap.Add(TilePiece.TopLeft, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\tl"));
            _textureMap.Add(TilePiece.TopRight, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\tr"));
            _textureMap.Add(TilePiece.BottomLeft, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\bl"));
            _textureMap.Add(TilePiece.BottomRight, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\br"));
            _textureMap.Add(TilePiece.Horizontal, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\h"));
            _textureMap.Add(TilePiece.Vertical, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\v"));
            _textureMap.Add(TilePiece.Start, this.GameManager.Game.Content.Load<Texture2D>(@"Textures\tl"));


            this.Assets.Add(_engineIdle);
            this.Assets.Add(_carScreech);
            this.Assets.Add(_carHorn);
            this.Assets.Add(Logo);
            this.Assets.Add(_car);
            this.Assets.Add(_raceFlags);

            foreach (IDisposable asset in _textureMap.Values)
            {
                this.Assets.Add(asset);
            }

            this._engineIdle.Volume = this._carScreech.Volume = this._carHorn.Volume = 0.5f;
            this._engineIdle.IsLooped = true;


            _mapPanel.Bounds = new Rectangle(0, 0, 800, 480);

            int mapCount = 0;
            HorizontalStackPanel panel = null;

            foreach (string mapName in MapNames)
            {
                string mapData = this.GameManager.Game.Content.Load<string>(@"RacerMaps\" + mapName).Trim();
                LoadedMaps.Add(mapName, mapData);

                if (null == panel)
                {
                    panel = new HorizontalStackPanel();
                    panel.VerticalAlignment = VerticalAlignment.Middle;
                    panel.FixedSize = false;
                    panel.Bounds = new Rectangle(0, 0, 800, 180);

                    _mapPanel.AddChild(panel);
                }

                VerticalStackPanel vertPanel = new VerticalStackPanel();
                vertPanel.Bounds = new Rectangle(0, 0, 200, 150);

                XnaRaceTrackTile tile = new XnaRaceTrackTile(_textureMap, mapName, mapData);
                tile.Tag = tile;
                tile.Bounds = new Rectangle(0, 0, 200, 120);
                tile.Click += new EventHandler(mapTile_Click);

                XnaButton button = new XnaButton(new Rectangle(0, 0, 250, 30));
                button.Tag = tile;
                button.Text = mapName;
                button.FontZoom = 0.5f;
                button.Click += new EventHandler(mapTile_Click);

                if (FingerGames.IsTrial && mapCount > 2)
                {
                    button.Color = new Color(110, 110, 110);
                }

                vertPanel.AddChild(tile);
                vertPanel.AddChild(button);

                panel.AddChild(vertPanel);

                mapCount++;

                if ((mapCount % 3) == 0)
                {
                    panel = null;
                }
            }
            _mapPanel.ForceLayout();

            float maxX = 0;
            for (int i = 0; i < 10; ++i)
            {
                maxX = Math.Max(maxX, _font.MeasureString(i.ToString()).X);
            }

            _decimalWidth = maxX * 1.05f / 2;

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

            base.LoadContent();
        }

        public override void Reset()
        {
            _playerIndex = 0;
            _loadedMap = false;

            ResetPlayer();
        }

        private void ResetPlayer()
        {
            _messageTime = TimeSpan.MinValue;
            _startTime = TimeSpan.MinValue;
            _completedLaps = 0;

            int y = 0;
            foreach (TilePiece[] col in _boardTiles)
            {
                int x = 0;
                foreach (TilePiece cell in col)
                {
                    if (cell == TilePiece.Start)
                    {
                        _carPosition = new Point(x, y);
                    }
                    x++;
                }
                y++;
            }

            _carOffset = new Point(0, 0);
            _prevPosition = new Point(-1, -1);
            this._carAngle = (float)(Math.PI * 3 / 4.0f);
        }

        private bool IsMoveValid(Point curr, Point next)
        {
            TilePiece currPiece = _boardTiles[curr.Y][curr.X];
            TilePiece nextPiece = _boardTiles[next.Y][next.X];

            // We need to allow "jumping" the corner
            if (next.X > curr.X && next.Y > curr.Y)
            {
                Point topRight = new Point(curr.X + 1, curr.Y);
                Point botLeft = new Point(curr.X, curr.Y + 1);

                if (IsMoveValid(curr, topRight) && IsMoveValid(topRight, next))
                {
                    return true;
                }
                if (IsMoveValid(curr, botLeft) && IsMoveValid(botLeft, next))
                {
                    return true;
                }
            }
            else if (next.X > curr.X && next.Y < curr.Y)
            {
                Point topRight = new Point(curr.X, curr.Y - 1);
                Point botLeft = new Point(curr.X + 1, curr.Y);

                if (IsMoveValid(curr, topRight) && IsMoveValid(topRight, next))
                {
                    return true;
                }
                if (IsMoveValid(curr, botLeft) && IsMoveValid(botLeft, next))
                {
                    return true;
                }
            }
            else if (next.X < curr.X && next.Y > curr.Y)
            {
                Point topRight = new Point(curr.X, curr.Y + 1);
                Point botLeft = new Point(curr.X - 1, curr.Y);

                if (IsMoveValid(curr, topRight) && IsMoveValid(topRight, next))
                {
                    return true;
                }
                if (IsMoveValid(curr, botLeft) && IsMoveValid(botLeft, next))
                {
                    return true;
                }
            }
            else if (next.X < curr.X && next.Y < curr.Y)
            {
                Point topRight = new Point(curr.X - 1, curr.Y);
                Point botLeft = new Point(curr.X, curr.Y - 1);

                if (IsMoveValid(curr, topRight) && IsMoveValid(topRight, next))
                {
                    return true;
                }
                if (IsMoveValid(curr, botLeft) && IsMoveValid(botLeft, next))
                {
                    return true;
                }
            }
            else if (next.X > curr.X)
            {
                switch (currPiece)
                {
                    case TilePiece.Start:
                    case TilePiece.TopLeft:
                    case TilePiece.Horizontal:
                    case TilePiece.BottomLeft:
                        {
                            return (nextPiece == TilePiece.Horizontal || nextPiece == TilePiece.TopRight || nextPiece == TilePiece.BottomRight);
                        }
                    case TilePiece.BottomRight:
                    case TilePiece.TopRight:
                    case TilePiece.Vertical:
                        {
                            return false;
                        }
                }
            }
            else if (next.X < curr.X)
            {
                switch (currPiece)
                {
                    case TilePiece.Start:
                    case TilePiece.Horizontal:
                    case TilePiece.BottomRight:
                    case TilePiece.TopRight:
                        {
                            return (nextPiece == TilePiece.Horizontal || nextPiece == TilePiece.TopLeft || nextPiece == TilePiece.BottomLeft || nextPiece == TilePiece.Start);
                        }
                    case TilePiece.TopLeft:
                    case TilePiece.BottomLeft:
                    case TilePiece.Vertical:
                        {
                            return false;
                        }
                }
            }
            else if (next.Y > curr.Y)
            {
                switch (currPiece)
                {
                    case TilePiece.Start:
                    case TilePiece.TopLeft:
                    case TilePiece.TopRight:
                    case TilePiece.Vertical:
                        {
                            return (nextPiece == TilePiece.Vertical || nextPiece == TilePiece.BottomLeft || nextPiece == TilePiece.BottomRight);
                        }
                    case TilePiece.Horizontal:
                    case TilePiece.BottomLeft:
                    case TilePiece.BottomRight:
                        {
                            return false;
                        }
                }
            }
            else if (next.Y < curr.Y)
            {
                switch (currPiece)
                {
                    case TilePiece.BottomLeft:
                    case TilePiece.BottomRight:
                    case TilePiece.Vertical:
                        {
                            return (nextPiece == TilePiece.Vertical || nextPiece == TilePiece.TopLeft || nextPiece == TilePiece.TopRight || nextPiece == TilePiece.Start);
                        }
                    case TilePiece.Start:
                    case TilePiece.TopLeft:
                    case TilePiece.TopRight:
                    case TilePiece.Horizontal:
                        {
                            return false;
                        }
                }
            }
            return false;
        }

        public override bool HandleBackClick(ref InGameState gameState)
        {
            SoundManager.Stop(this._engineIdle);
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
                SoundManager.Play(this._engineIdle);
            }
            if (_messageTime == TimeSpan.MinValue)
            {
                _messageTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(1.5));
            }

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

            Point touchPoint = new Point((int)activeTouch.Position.X, (int)activeTouch.Position.Y);

            bool isActive = false;

            if (activeTouches > 0)
            {
                int y = 0;
                foreach (TilePiece[] col in _boardTiles)
                {
                    int x = 0;
                    foreach (TilePiece cell in col)
                    {
                        Rectangle bounds = new Rectangle(x * 80, y * 80, 80, 80);

                        if (cell == TilePiece.Grass && bounds.Contains(touchPoint))
                        {
                            return; // false
                        }

                        if (bounds.Contains(touchPoint))
                        {
                            isActive = true;

                            int xDiff = Math.Abs(x - _carPosition.X);
                            int yDiff = Math.Abs(y - _carPosition.Y);
                            int coordDiff = xDiff + yDiff;

                            if (coordDiff == 1 || (xDiff == 1 && yDiff == 1))
                            {
                                if (_prevPosition.X != x || _prevPosition.Y != y)
                                {
                                    // we also need to make sure our move is valid

                                    if (IsMoveValid(_carPosition, new Point(x, y)))
                                    {
                                        _prevPosition = _carPosition;
                                        _prevCarAngle = _carAngle;
                                        _carPosition = new Point(x, y);

                                        switch (cell)
                                        {
                                            case TilePiece.Start:
                                            case TilePiece.TopLeft:
                                                {
                                                    _carAngle = (float)((_carPosition.X < _prevPosition.X) ? Math.PI * 3 / 4 : -Math.PI / 4);
                                                }
                                                break;
                                            case TilePiece.Horizontal:
                                                {
                                                    _carAngle = (float)((_carPosition.X > _prevPosition.X) ? 0 : Math.PI);
                                                }
                                                break;
                                            case TilePiece.Vertical:
                                                {
                                                    _carAngle = (float)((_carPosition.Y > _prevPosition.Y) ? Math.PI / 2 : -Math.PI / 2);
                                                }
                                                break;
                                            case TilePiece.TopRight:
                                                {
                                                    _carAngle = (float)((_carPosition.Y < _prevPosition.Y) ? -Math.PI * 3 / 4 : Math.PI / 4);
                                                }
                                                break;
                                            case TilePiece.BottomLeft:
                                                {
                                                    _carAngle = (float)((_carPosition.X < _prevPosition.X) ? -Math.PI * 3 / 4 : Math.PI / 4);
                                                }
                                                break;
                                            case TilePiece.BottomRight:
                                                {
                                                    _carAngle = (float)((_carPosition.Y < _prevPosition.Y) ? Math.PI * 3 / 4 : -Math.PI / 4);
                                                }
                                                break;
                                        }

                                        if (cell == TilePiece.TopLeft || cell == TilePiece.TopRight || cell == TilePiece.BottomLeft || cell == TilePiece.BottomRight)
                                        {
                                            SoundManager.Play(this._carScreech);
                                        }

                                        if (cell == TilePiece.Start)
                                        {
                                            _messageTime = TimeSpan.MinValue;
                                            _completedLaps++;

                                            if (RequiredLaps > _completedLaps)
                                            {
                                                SoundManager.Play(this._carHorn);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        x++;
                    }
                    y++;
                }
            }

            if (isActive)
            {
                // Break up the cell into
                float xOffset = Math.Min(40, Math.Max(-40, _carPosition.X * 80 + 40 - touchPoint.X));
                float yOffset = Math.Min(40, Math.Max(-40, _carPosition.Y * 80 + 40 - touchPoint.Y));

                switch (_boardTiles[_carPosition.Y][_carPosition.X])
                {
                    case TilePiece.Start:
                    case TilePiece.TopLeft:
                    case TilePiece.TopRight:
                    case TilePiece.BottomLeft:
                    case TilePiece.BottomRight:
                        {
                            yOffset = Math.Max(-10, Math.Min(10, yOffset));
                            xOffset = Math.Max(-10, Math.Min(10, xOffset));
                        }
                        break;
                    case TilePiece.Horizontal:
                        {
                            yOffset = 0;
                        }
                        break;
                    case TilePiece.Vertical:
                        {
                            xOffset = 0;
                        }
                        break;
                }
                _carOffset = new Point((int)xOffset, (int)yOffset);
            }

            if (_completedLaps == RequiredLaps)
            {
                SoundManager.Stop(this._engineIdle);
                FingerGames.Instance.GameManager.Players[_playerIndex].Score = _elapsedtime.TotalSeconds;
                gameState = InGameState.End;
                // sweet!
            }

            return; // false
        }

        public override void Draw(GameTime gameTime)
        {
            for (int xPos = 0; xPos < 800; xPos += 160)
            {
                for (int yPos = 0; yPos < 480; yPos += 160)
                {
                    FingerGames.Instance.SpriteBatch.Draw(_textureMap[TilePiece.Grass], new Vector2(xPos, yPos), Color.White);
                }
            }

            int y = 0;
            foreach (TilePiece[] col in _boardTiles)
            {
                int x = 0;
                foreach (TilePiece cell in col)
                {
                    Vector2 pos = new Vector2(x * 80, y * 80);
                    if (cell != TilePiece.Grass)
                    {
                        FingerGames.Instance.SpriteBatch.Draw(_textureMap[cell], pos, Color.White);
                    }
                    x++;
                }
                y++;
            }

            FingerGames.Instance.SpriteBatch.Draw(_car, new Rectangle(_carPosition.X * 80 + 40 - _carOffset.X, _carPosition.Y * 80 + 40 - _carOffset.Y, _car.Width, _car.Height), null, new Color(255, 255, 255, 255), _carAngle, new Vector2(_car.Width / 2, _car.Height / 2), SpriteEffects.None, 0);

            if (_startTime != TimeSpan.MinValue)
            {
                string time = string.Format("{0:0000.0}", (_elapsedtime).TotalSeconds);

                float fontWidth = (5.5f * _decimalWidth);
                float startX = 400 - fontWidth / 2;

                FingerGames.Instance.SpriteBatch.Draw(_pixel, new Rectangle((int)startX - 4, 0, (int)fontWidth + 8, 35), new Color(0, 0, 0, 100));

                foreach (char dec in time)
                {
                    string str = dec.ToString();
                    float charWidth = (dec != '.') ? _decimalWidth : _decimalWidth / 2;
                    Vector2 fontSize = this._font.MeasureString(str);

                    FingerGames.Instance.SpriteBatch.DrawString(_font, str, new Vector2(startX + (charWidth) / 2, 0), Color.White, 0, new Vector2(fontSize.X / 2, 0), 0.5f, SpriteEffects.None, 0);

                    startX += charWidth;
                }
            }

            if (_messageTime > gameTime.TotalGameTime)
            {
                string data = string.Format("{0} Laps Remaining", RequiredLaps - _completedLaps);

                if (RequiredLaps - _completedLaps == 1)
                {
                    data = "Final Lap!";
                }

                Vector2 fontSize = this._font.MeasureString(data);

                FingerGames.Instance.SpriteBatch.Draw(_pixel, new Rectangle(0, 240 - (int)(.75 * fontSize.Y), 800, (int)(1.5 * fontSize.Y)), new Color(0, 0, 0, 180));
                FingerGames.Instance.SpriteBatch.DrawString(_font, data, new Vector2(400, 240) - fontSize / 2, Color.White);
            }
        }

        XnaLabel _introLabel = new XnaLabel(new Rectangle(0, 150, 800, 180));

        protected override float IntroDelay
        {
            get
            {
                return 4.0f;
            }
        }

        public override void UpdateIntro(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (justTransitioned)
            {
                _introLabel.Text = "Get Ready " + FingerGames.Instance.GameManager.Players[_playerIndex].Name;
                _introLabel.FontZoom = 1.5f;
                _introLabel.BackColor = new Color(0, 0, 0, 180);
            }
            base.UpdateIntro(gameTime, ref gameState, justTransitioned);
        }

        public override void DrawIntro(GameTime gameTime)
        {
            for (int xPos = 0; xPos < 800; xPos += 160)
            {
                for (int yPos = 0; yPos < 480; yPos += 160)
                {
                    FingerGames.Instance.SpriteBatch.Draw(_textureMap[TilePiece.Grass], new Vector2(xPos, yPos), Color.White);
                }
            }
            _introLabel.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        private bool _loadedMap = false;
        void mapTile_Click(object sender, EventArgs e)
        {
            XnaButton button = (XnaButton)sender;
            XnaRaceTrackTile tile = button.Tag as XnaRaceTrackTile;

            if (Array.IndexOf(MapNames, tile.Name) > 2 && FingerGames.IsTrial)
            {
                FingerGames.Instance.SetStage(Stage.Trial);
            }
            else
            {
                _boardTiles = tile.BoardTiles;
                GameManager.GameVariation = tile.Name;
                _loadedMap = true;
            }
        }

        public override void UpdateConfigure(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            if (justTransitioned)
            {
                _loadedMap = false;
            }
            else if (_loadedMap)
            {
                gameState = InGameState.Intro;
            }
            else
            {
                _mapPanel.Update(gameTime);
            }
        }

        public override void DrawConfigure(GameTime gameTime)
        {
            _mapPanel.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        protected override float EndDelay
        {
            get
            {
                return 4.0f;
            }
        }

        public override void DrawEnd(GameTime gameTime)
        {
            for (int xPos = 0; xPos < 800; xPos += 160)
            {
                for (int yPos = 0; yPos < 480; yPos += 160)
                {
                    FingerGames.Instance.SpriteBatch.Draw(_textureMap[TilePiece.Grass], new Vector2(xPos, yPos), Color.White);
                }
            }
            _timePanel.Draw(FingerGames.Instance.SpriteBatch, gameTime);
        }

        public override void UpdateEnd(GameTime gameTime, ref InGameState gameState, bool justTransitioned)
        {
            base.UpdateEnd(gameTime, ref gameState, justTransitioned);

            if (justTransitioned)
            {
                this._timeLabel.Text = string.Format("{0:0000.00} seconds", FingerGames.Instance.GameManager.Players[_playerIndex].Score);
            }

            if (gameState == InGameState.ScoreGame)
            {
                if (_playerIndex + 1 < FingerGames.Instance.GameManager.Players.Count)
                {
                    _playerIndex++;
                    ResetPlayer();
                    gameState = InGameState.Intro;
                }
                else if (justTransitioned)
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
    }
}
