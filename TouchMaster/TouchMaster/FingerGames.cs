using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FingerGames : Microsoft.Xna.Framework.Game
    {
        private static FingerGames _instance;
        private static Random _random = new Random();
        private static readonly int ANIDLength = 32;
        private static readonly int ANIDOffset = 2;

        public static FingerGames Instance
        {
            get { return _instance; }
        }

        public static bool IsTrial
        {
            get { return Guide.IsTrialMode; }
        }

        public static Random Randomizer
        {
            get { return _random; }
        }

        public static string GetWindowsLiveAnonymousID()
        {
            string result = string.Empty;
            object anid;
            if (Microsoft.Phone.Info.UserExtendedProperties.TryGetValue("ANID", out anid))
            {
                if (anid != null && anid.ToString().Length >= (ANIDLength + ANIDOffset))
                {
                    result = anid.ToString().Substring(ANIDOffset, ANIDLength);
                }
            }

            return result;
        }


        private GraphicsDeviceManager _graphics;
        private Dictionary<Stage, BaseStage> _gameStages = new Dictionary<Stage, BaseStage>();
        private BaseStage _activeStage = null;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel = null;
        private Texture2D _arrow = null;
        private GameManager _gameManager = null;
        private bool _isTransitioning = false;
        private int _transitionCount = 0;
        private byte _transitionOpacity = 0;
        private const int _transitionLength = 20;
        private const int _halfTransitionLength = _transitionLength / 2;
        private Stage _newStage;
        private Stack<Stage> _stageQueue = new Stack<Stage>();

        private Texture2D _captureBar, _captureNet, _captureOrb;

        public FingerGames()
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
            
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            _gameStages.Add(Stage.InGame, new InGameStage(this));
            _gameStages.Add(Stage.Intro, new IntroStage(this));
            _gameStages.Add(Stage.SelectGame, new SelectGameStage(this));
            _gameStages.Add(Stage.Trial, new TrialStage(this));
            _gameStages.Add(Stage.Help, new HelpStage(this));
            _activeStage = _gameStages[Stage.Intro];

            _gameManager = new GameManager(GetWindowsLiveAnonymousID(), this, new GamePlayer[] { new GamePlayer(0, "Default") });
        }

        internal GameManager GameManager
        {
            get { return _gameManager; }
        }

        internal SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }
        
        public void SetStage(Stage stage)
        {
            SetStage(stage, true);
        }

        public void SetStage(Stage stage, bool setHistory)
        {
            _newStage = stage;
            _isTransitioning = true;
            _transitionCount = _transitionLength;

            if (setHistory)
            {
                _stageQueue.Push(_activeStage.Stage);
            }
        }

        public void GoBack()
        {
            if (_activeStage.Stage == Stage.Intro || _stageQueue.Count == 0)
            {
                this.Exit();
            }
            else
            {
                Stage stage = _stageQueue.Pop();
                this.SetStage(stage, false);
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(this.GraphicsDevice);
            // TODO: use this.Content to load your game content here
            foreach (BaseStage stage in _gameStages.Values)
            {
                stage.LoadContent();
            }
            _pixel = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\pixel");
            _arrow = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\arrow");


            _captureBar = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\capture_bar");
            _captureOrb = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\capture_orb");
            _captureNet = FingerGames.Instance.Content.Load<Texture2D>(@"Textures\capture_net");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
            // TODO: use this.Content to load your game content here
            foreach (BaseStage stage in _gameStages.Values)
            {
                stage.UnloadContent();
            }
            _pixel.Dispose();
            _arrow.Dispose();
        }

        private void KeyboardComplete(IAsyncResult result)
        {
            Settings.Instance.GamerTag = Guide.EndShowKeyboardInput(result);
            Settings.Instance.Save();
        }

        private bool _isKeyboardShown = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!_isKeyboardShown)
            {
                _isKeyboardShown = true;

                if (string.IsNullOrEmpty(Settings.Instance.GamerTag))
                {
                    Guide.BeginShowKeyboardInput(PlayerIndex.One, "Enter Your High Score Name", "Please enter the name you want used for high scores.", "", new AsyncCallback(KeyboardComplete), null);
                }
            }
            if (!_isTransitioning)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    if (_activeStage.HandleBackClick())
                    {
                        //
                    }
                    else
                    {
                        GoBack();
                    }
                }

                _activeStage.Update(gameTime);
            }
            else
            {
                if (_transitionCount == _halfTransitionLength)
                {
                    _activeStage.Deactivate();
                    _activeStage = _gameStages[_newStage];
                    _activeStage.Activate();
                    _activeStage.Update(gameTime);
                }
                else if (_transitionCount > _halfTransitionLength)
                {
                    _transitionOpacity = (byte)(0xff - ((_transitionCount - _halfTransitionLength) * (0xff / (float)_halfTransitionLength)));
                }
                else
                {
                    _transitionOpacity = (byte)(((_transitionCount) * (0xff / (float)_halfTransitionLength)));
                }

                if (_transitionCount <= 0)
                {
                    _isTransitioning = false;
                }

                _transitionCount--;
            }
            base.Update(gameTime);

            if (iterCount++ == 40)
            {
                iterCount = 0;
                for (int i = 0; i < _orbNext.Length; ++i)
                {
                    _orbNext[i].X = Randomizer.Next(-80, 80);
                    _orbNext[i].Y = Randomizer.Next(-80, 80);
                }
            }
            
            offset.X = float.MaxValue;
            offset.Y = float.MaxValue;
            for (int i = 0; i < _orbNext.Length; ++i)
            {
                //_orbSpots[i].X = (float)Math.Max(0, Math.Min(800, _orbSpots[i].X + _orbNext[i].X / 40));
                //_orbSpots[i].Y = (float)Math.Max(0, Math.Min(480, _orbSpots[i].Y + _orbNext[i].Y / 40));

                offset.X = Math.Min(offset.X, _orbSpots[i].X);
                offset.Y = Math.Min(offset.Y, _orbSpots[i].Y);
            }

            return;
        }

        Vector2 offset = new Vector2(0, 0);

        int iterCount = 0;

        private Vector2[] _orbSpots = new Vector2[] { new Vector2(50, 50), new Vector2(700, 150), new Vector2(0, 480) };
        private Vector2[] _orbNext = new Vector2[] { new Vector2(50, 50), new Vector2(700, 150), new Vector2(0, 480) };
        private BasicEffect baseEffect = null;

        int Sort(Vector2 a)
        {
            return 0;
        }

        int animCount = 0;
        bool countUp = true;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {/*
            if (null == baseEffect)
            {
                baseEffect = new BasicEffect(GraphicsDevice);
                baseEffect.World = Matrix.Identity;
                baseEffect.View = Matrix.Identity;
                baseEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1f);
                baseEffect.VertexColorEnabled = true;
                baseEffect.TextureEnabled = true;
                baseEffect.Texture = _captureNet;
            }

            if (countUp)
            {
                //animCount += 8;
            }
            else
            {
                //animCount -= 8;
            }

            if (animCount == 256)
            {
                //animCount -= 8;
                countUp = false;
            }
            if (animCount == 0)
            {
                //animCount += 8;
                countUp = true;
            }
            //_orbSpots.OrderBy(new Func<Vector2, int>(Sort));
            GraphicsDevice.Clear(Color.Black);

            this._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, baseEffect);
            VertexPositionTexture[] verts = new VertexPositionTexture[3];

            // ABC
            // ACB

            verts[0].Position = new Vector3(_orbSpots[0], 0);
            verts[1].Position = new Vector3(_orbSpots[1], 0);
            verts[2].Position = new Vector3(_orbSpots[2], 0);
            verts[0].TextureCoordinate = new Vector2(0, .25f);
            verts[1].TextureCoordinate = new Vector2(0, .75f);
            verts[2].TextureCoordinate = new Vector2(1, .5f);

            List<VertexPositionColorTexture> colors = new List<VertexPositionColorTexture>();

            List<short> indices = new List<short>();
            Vector2 prevSpot = _orbSpots[_orbSpots.Length - 1];
            
            foreach (Vector2 orbSpot in _orbSpots)
            {
                VertexPositionColorTexture pos = new VertexPositionColorTexture();
                pos.Position = new Vector3(orbSpot, 0);
                pos.Color = new Color(255, 255, 255);
                pos.TextureCoordinate = verts[2].TextureCoordinate;

                colors.Add(pos);
            }

            for (int ind = 0; ind < _orbSpots.Length; ++ind)
            {
                Vector2 vec = _orbSpots[ind];

                int orthogInd = (ind + 1) % _orbSpots.Length;
                Vector2 orthog = _orbSpots[orthogInd];

                for (int i = 1; i < 16; ++i)
                {
                    VertexPositionColorTexture pos = new VertexPositionColorTexture();
                    pos.Position = new Vector3(vec + i * (prevSpot - vec) / 16, 0);
                    pos.Color = new Color(255 - 0, 255 - 0, 255 - 0);
                    pos.TextureCoordinate = verts[i%2].TextureCoordinate;

                    colors.Add(pos);
                    if (i % 3 == 0)
                    {
                        indices.Add((short)orthogInd);
                        indices.Add((short)(colors.Count - 1));
                        indices.Add((short)(colors.Count - 2));
                    }
                }

                prevSpot = vec;
            }

            
            foreach (EffectPass pass in baseEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, colors.ToArray(), 0, colors.Count, indices.ToArray(), 0, indices.Count / 3);
                //GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 1);
            }
            this._spriteBatch.End();

            this._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //this._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);

            //


            prevSpot = _orbSpots[_orbSpots.Length - 1];
            foreach (Vector2 orbSpot in _orbSpots)
            {
                double rotationAngle = Math.Atan2(prevSpot.Y - orbSpot.Y, prevSpot.X - orbSpot.X);
                double distance = Math.Sqrt(Math.Pow(prevSpot.Y - orbSpot.Y, 2) + Math.Pow(prevSpot.X - orbSpot.X, 2));
                this._spriteBatch.Draw(_captureBar, new Rectangle((int)orbSpot.X, (int)orbSpot.Y, (int)distance, _captureBar.Height), null, Color.White, (float)rotationAngle, new Vector2(0, _captureBar.Height / 2), SpriteEffects.None, 0);

                prevSpot = orbSpot;
            }

            Vector2 orbSize = new Vector2(_captureOrb.Width, _captureOrb.Height);
            foreach (Vector2 orbSpot in _orbSpots)
            {
                this._spriteBatch.Draw(_captureOrb, orbSpot - orbSize / 2, Color.White);
            }

            this._spriteBatch.End();
            return;
            */
            GraphicsDevice.Clear(Color.Black);
            this._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _activeStage.Draw(gameTime);
            base.Draw(gameTime);

            if (_isTransitioning)
            {
                this._spriteBatch.Draw(this._pixel, new Rectangle(0, 0, 800, 480), new Color(0,0,0, _transitionOpacity));
            }

            if (Settings.Instance.ArrowsEnabled)
            {
                // calculate the rotation angle
                TouchCollection touchCollection = TouchPanel.GetState();
                foreach (TouchLocation touchLocation in touchCollection)
                {
                    if (touchLocation.State == TouchLocationState.Pressed || touchLocation.State == TouchLocationState.Moved)
                    {
                        double angle = Math.Atan2(touchLocation.Position.Y - 240, touchLocation.Position.X - 400);
                        this._spriteBatch.Draw(this._arrow, new Vector2(touchLocation.Position.X, touchLocation.Position.Y), null, new Color(255, 255, 255, 180), (float)angle, new Vector2(_arrow.Width, _arrow.Height / 2), 2.0f, SpriteEffects.None, 0);
                    }
                }
            }

            this._spriteBatch.End();

            return;
        }
    }
}
