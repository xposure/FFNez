using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Atma.Systems;
using Atma.Console;
using Atma.Tweens;
using Atma.Timers;
using Atma.BitmapFonts;
using Atma.Analysis;
using Atma.Textures;
using System.Threading;
using System.Runtime.InteropServices;

namespace Atma
{
	public class Core : Game
	{
        public SpriteBatch test;
		/// <summary>
		/// core emitter. emits only Core level events.
		/// </summary>
		public static Emitter<CoreEvents> emitter;

		/// <summary>
		/// enables/disables if we should quit the app when escape is pressed
		/// </summary>
		public static bool exitOnEscapeKeypress = true;

		/// <summary>
		/// enables/disables pausing when focus is lost. No update or render methods will be called if true when not in focus.
		/// </summary>
		public static bool pauseOnFocusLost = true;

		/// <summary>
		/// enables/disables debug rendering
		/// </summary>
		public static bool debugRenderEnabled = false;

		/// <summary>
		/// global access to the graphicsDevice
		/// </summary>
		public static GraphicsDevice graphicsDevice;
		
		/// <summary>
		/// global access to the graphicsDeviceManager
		/// </summary>
		public static GraphicsDeviceManager graphicsManager;

		/// <summary>
		/// The resolution given by ('width', 'height')
		/// </summary>
		public Point defaultResolution;

		/// <summary>
		/// global content manager for loading any assets that should stick around between scenes
		/// </summary>
		public static NezContentManager content;

		/// <summary>
		/// default SamplerState used by Materials. Note that this must be set at launch! Changing it after that time will result in only
		/// Materials created after it was set having the new SamplerState
		/// </summary>
		public static SamplerState defaultSamplerState = SamplerState.PointClamp;

		/// <summary>
		/// default wrapped SamplerState. Determined by the Filter of the defaultSamplerState.
		/// </summary>
		/// <value>The default state of the wraped sampler.</value>
		public static SamplerState defaultWrappedSamplerState { get { return defaultSamplerState.Filter == TextureFilter.Point ? SamplerState.PointWrap : SamplerState.LinearWrap; } }

		/// <summary>
		/// default GameServiceContainer access
		/// </summary>
		/// <value>The services.</value>
		public static GameServiceContainer services { get { return _instance.Services; } }

		/// <summary>
		/// internal flag used to determine if EntitySystems should be used or not
		/// </summary>
		internal static bool entitySystemsEnabled;

		/// <summary>
		/// facilitates easy access to the global Content instance for internal classes
		/// </summary>
		internal static Core _instance;

		#if DEBUG
		internal static long drawCalls;
		TimeSpan _frameCounterElapsedTime = TimeSpan.Zero;
		int _frameCounter = 0;
		string _windowTitle;
		#endif

		Scene _scene;
		Scene _nextScene;
		internal SceneTransition _sceneTransition;

		public bool inScreenTransition => _sceneTransition != null;

		/// <summary>
		/// used to coalesce GraphicsDeviceReset events
		/// </summary>
		ITimer _graphicsDeviceChangeTimer;

		// globally accessible systems
		FastList<IUpdatableManager> _globalManagers = new FastList<IUpdatableManager>();
		CoroutineManager _coroutineManager = new CoroutineManager();
		TimerManager _timerManager = new TimerManager();


		/// <summary>
		/// The currently active Scene. Note that if set, the Scene will not actually change until the end of the Update
		/// </summary>
		public static Scene scene
		{
			get { return _instance._scene; }
			set { _instance._nextScene = value; }
		}

		public static Core instance => _instance;

		public Core( int width = 1280, int height = 720, bool isFullScreen = false, bool enableEntitySystems = true, string windowTitle = "Nez", string contentDirectory = "Content" )
		{
            if(SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            #if DEBUG
            _windowTitle = windowTitle;
			#endif

			_instance = this;
			emitter = new Emitter<CoreEvents>( new CoreEventsComparer() );

			graphicsManager = new GraphicsDeviceManager( this );
			defaultResolution = new Point(width, height);
			graphicsManager.PreferredBackBufferWidth = width;
			graphicsManager.PreferredBackBufferHeight = height;
			graphicsManager.IsFullScreen = isFullScreen;
			graphicsManager.SynchronizeWithVerticalRetrace = true;
			graphicsManager.DeviceReset += onGraphicsDeviceReset;
			graphicsManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

			Screen.initialize( graphicsManager );
			Window.ClientSizeChanged += onGraphicsDeviceReset;
			Window.OrientationChanged += onOrientationChanged;

			Content.RootDirectory = contentDirectory;
			content = new NezGlobalContentManager( Services, Content.RootDirectory );
			IsMouseVisible = true;
			IsFixedTimeStep = false;

			entitySystemsEnabled = enableEntitySystems;

			// setup systems
			_globalManagers.add( _coroutineManager );
			_globalManagers.add( new TweenManager() );
			_globalManagers.add( _timerManager );
			_globalManagers.add( new RenderTarget() );
		}


		void onOrientationChanged( object sender, EventArgs e )
		{
			emitter.emit( CoreEvents.OrientationChanged );
		}


		/// <summary>
		/// this gets called whenever the screen size changes
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void onGraphicsDeviceReset( object sender, EventArgs e )
		{
			// we coalese these to avoid spamming events
			if( _graphicsDeviceChangeTimer != null )
			{
				_graphicsDeviceChangeTimer.reset();
			}
			else
			{
				_graphicsDeviceChangeTimer = schedule( 0.05f, false, this, t =>
				{
					( t.context as Core )._graphicsDeviceChangeTimer = null;
					emitter.emit( CoreEvents.GraphicsDeviceReset );
				} );
			}
		}


		#region Passthroughs to Game

		public static void exit()
		{
			_instance.Exit();
		}

		#endregion


		#region Game overides

		protected override void Initialize()
		{
			base.Initialize();

			// prep the default Graphics system
			graphicsDevice = GraphicsDevice;
			var font = content.Load<BitmapFont>( "atma://Nez.Content.NezDefaultBMFont.xnb" );
			Graphics.instance = new Graphics( font );

            test = new SpriteBatch(Core.graphicsDevice);

            _vertexInfo = new VertexPositionColorTexture4[MAX_SPRITES];
            _textureInfo = new Texture2D[MAX_SPRITES];
            _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), MAX_VERTICES, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, MAX_INDICES, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indexData);

            var s = new VertexPositionColorTexture4();
            s.position0 = new vec3(100, 100, 0);
            s.position1 = new vec3(200, 100, 0);
            s.position3 = new vec3(200, 200, 0);
            s.position2 = new vec3(100, 200, 0);
            s.textureCoordinate0 = new vec2(0.5f);
            s.textureCoordinate1 = new vec2(0.5f);
            s.textureCoordinate2 = new vec2(0.5f);
            s.textureCoordinate3 = new vec2(0.5f);
            s.color0 = Color.Red;
            s.color1 = Color.Green;
            s.color1 = Color.Blue;
            s.color1 = Color.White;
            _vertexInfo[0] = s;
            _vertexBuffer.SetData(0, _vertexInfo, 0, 1, VertexPositionColorTexture4.realStride, SetDataOptions.None);

            _spriteEffect = new SpriteEffect();
            _spriteEffectPass = _spriteEffect.CurrentTechnique.Passes[0];

        }


        protected override void Update( GameTime gameTime )
		{
			//if( pauseOnFocusLost && !IsActive )
			//{
			//	SuppressDraw();
			//	return;
			//}

			//#if DEBUG
			//TimeRuler.instance.startFrame();
			//TimeRuler.instance.beginMark( "update", Color.Green );
			//#endif

			//// update all our systems and global managers
			//Time.update( (float)gameTime.ElapsedGameTime.TotalSeconds );
			//Input.update();

			//for( var i = _globalManagers.length - 1; i >= 0; i-- )
			//	_globalManagers.buffer[i].update();

			//if( exitOnEscapeKeypress && ( Input.isKeyDown( Keys.Escape ) || Input.gamePads[0].isButtonReleased( Buttons.Back ) ) )
			//{
			//	Exit();
			//	return;
			//}

			//if( _scene != null )
			//	_scene.update();

			//if( _scene != _nextScene )
			//{
			//	if( _scene != null )
			//		_scene.end();

			//	_scene = _nextScene;
			//	onSceneChanged();

			//	if( _scene != null )
			//		_scene.begin();
			//}

			//#if DEBUG
			//TimeRuler.instance.endMark( "update" );
			//DebugConsole.instance.update();
			//drawCalls = 0;
			//#endif

			//#if FNA
			//// MonoGame only updates old-school XNA Components in Update which we dont care about. FNA's core FrameworkDispatcher needs
			//// Update called though so we do so here.
			//FrameworkDispatcher.Update();
			//#endif
		}


		protected override void Draw( GameTime gameTime )
		{
            //if( pauseOnFocusLost && !IsActive )
            //	return;

            //#if DEBUG
            //TimeRuler.instance.beginMark( "draw", Color.Gold );

            //// fps counter
            //_frameCounter++;
            //_frameCounterElapsedTime += gameTime.ElapsedGameTime;
            //if( _frameCounterElapsedTime >= TimeSpan.FromSeconds( 1 ) )
            //{
            //	var totalMemory = ( GC.GetTotalMemory( false ) / 1048576f ).ToString( "F" );
            //	Window.Title = string.Format( "{0} {1} fps - {2} MB", _windowTitle, _frameCounter, totalMemory );
            //	_frameCounter = 0;
            //	_frameCounterElapsedTime -= TimeSpan.FromSeconds( 1 );
            //}
            //#endif

            //if( _sceneTransition != null )
            //	_sceneTransition.preRender( Graphics.instance );

            //if( _scene != null )
            //{
            //	_scene.render();

            //	#if DEBUG
            //	if( debugRenderEnabled )
            //		Debug.render();
            //	#endif

            //	// render as usual if we dont have an active SceneTransition
            //	if( _sceneTransition == null )
            //		_scene.postRender();
            //}

            //// special handling of SceneTransition if we have one
            //if( _sceneTransition != null )
            //{
            //	if( _scene != null && _sceneTransition.wantsPreviousSceneRender && !_sceneTransition.hasPreviousSceneRender )
            //	{
            //		_scene.postRender( _sceneTransition.previousSceneRender );
            //		if( _sceneTransition._loadsNewScene )
            //			scene = null;
            //		startCoroutine( _sceneTransition.onBeginTransition() );
            //	}
            //	else if( _scene != null )
            //	{
            //		_scene.postRender();
            //	}

            //	_sceneTransition.render( Graphics.instance );
            //}

            //#if DEBUG
            //TimeRuler.instance.endMark( "draw" );
            //DebugConsole.instance.render();

            //// the TimeRuler only needs to render when the DebugConsole is not open
            //if( !DebugConsole.instance.isOpen )
            //	TimeRuler.instance.render();
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.A)) x -= 0.01f; 
            if (state.IsKeyDown(Keys.D)) x += 0.01f;

            Core.graphicsDevice.SetRenderTarget(null);
            Core.graphicsDevice.Clear(Color.CornflowerBlue);
            //test.Begin(SpriteSortMode.Immediate, transformMatrix: mat4.Translate(x,0,0));
            //test.Draw(Graphics.instance.pixelTexture.texture2D, new Rectangle(0, 200, 100, 100), Color.Red);
            flushBatch();
            //Graphics.instance.batcher.begin(BlendState.Opaque, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null);
            //Graphics.instance.batcher.draw(Graphics.instance.pixelTexture, new Rectangle(0, 0, 1000, 1000), Color.White);
            //Graphics.instance.batcher.end();
            ///test.End();

            #if !FNA
            drawCalls = graphicsDevice.Metrics.DrawCount;
			#endif
			//#endif
		}

        public void flushBatch()
        {
            //graphicsDevice.BlendState = _blendState;
            //graphicsDevice.SamplerStates[0] = _samplerState;
            //graphicsDevice.DepthStencilState = _depthStencilState;
            //graphicsDevice.RasterizerState = _rasterizerState;

            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            var viewport = graphicsDevice.Viewport;

            //            // inlined CreateOrthographicOffCenter
            //#if FNA
            //			_projectionMatrix.M11 = (float)( 2.0 / (double) ( viewport.Width / 2 * 2 - 1 ) );
            //			_projectionMatrix.M22 = (float)( -2.0 / (double) ( viewport.Height / 2 * 2 - 1 ) );
            //#else
            //            _projectionMatrix.M11 = (float)(2.0 / (double)viewport.Width);
            //            _projectionMatrix.M22 = (float)(-2.0 / (double)viewport.Height);
            //#endif

            //            _projectionMatrix.M41 = -1 - 0.5f * _projectionMatrix.M11;
            //            _projectionMatrix.M42 = 1 - 0.5f * _projectionMatrix.M22;

            //            _matrixTransformMatrix = _transformMatrix * _projectionMatrix;// glm.Mul(transformMatrix, _projectionMatrix);
            //                                                                          //mat4.Multiply( ref _transformMatrix, ref _projectionMatrix, out _matrixTransformMatrix );
            //                                                                          //_spriteEffect.setMatrixTransform( ref _matrixTransformMatrix );
            //            var m = mat4.Ortho(0, viewport.Width, viewport.Height, 0, 1, -1);
            //            _spriteEffect.setMatrixTransform(mat4.Identity);

            // we have to Apply here because custom effects often wont have a vertex shader and we need the default SpriteEffect's
            //_spriteEffectPass.Apply();
            drawPrimitives(Graphics.instance.pixelTexture, 0, 1);

        }

        void drawPrimitives(Texture texture, int baseSprite, int batchSize)
        {
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;

            var m = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, -1);
            _spriteEffect.setMatrixTransform(m);
            _spriteEffectPass.Apply();

            graphicsDevice.Textures[0] = texture;
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, baseSprite * 4, 0, batchSize * 2);
        }

        // Buffer objects used for actual drawing
        const int MAX_SPRITES = 2048;
        const int MAX_VERTICES = MAX_SPRITES * 4;
        const int MAX_INDICES = MAX_SPRITES * 6;

        DynamicVertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        // Local data stored before buffering to GPU
        VertexPositionColorTexture4[] _vertexInfo;
        Texture2D[] _textureInfo;

        private float x = 0f;
		static readonly short[] _indexData = generateIndexArray();
        static short[] generateIndexArray()
        {
            var result = new short[MAX_INDICES];
            for (int i = 0, j = 0; i < MAX_INDICES; i += 6, j += 4)
            {
                result[i] = (short)(j);
                result[i + 1] = (short)(j + 1);
                result[i + 2] = (short)(j + 2);
                result[i + 3] = (short)(j + 3);
                result[i + 4] = (short)(j + 2);
                result[i + 5] = (short)(j + 1);
            }
            return result;
        }

        // Default SpriteEffect
        SpriteEffect _spriteEffect;
        EffectPass _spriteEffectPass;


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct VertexPositionColorTexture4 : IVertexType
        {
            public const int realStride = 96;

            VertexDeclaration IVertexType.VertexDeclaration { get { throw new NotImplementedException(); } }

            public vec3 position0; //3
            public Color color0; //1
            public vec2 textureCoordinate0; //2
            public vec3 position1; 
            public Color color1;
            public vec2 textureCoordinate1;
            public vec3 position2;
            public Color color2;
            public vec2 textureCoordinate2;
            public vec3 position3;
            public Color color3;
            public vec2 textureCoordinate3;
        }
        #endregion


        /// <summary>
        /// Called after a Scene ends, before the next Scene begins
        /// </summary>
        void onSceneChanged()
		{
			emitter.emit( CoreEvents.SceneChanged );
			Time.sceneChanged();
			GC.Collect();
		}


		/// <summary>
		/// temporarily runs SceneTransition allowing one Scene to transition to another smoothly with custom effects.
		/// </summary>
		/// <param name="sceneTransition">Scene transition.</param>
		public static T startSceneTransition<T>( T sceneTransition ) where T : SceneTransition
		{
			Assert.isNull( _instance._sceneTransition, "You cannot start a new SceneTransition until the previous one has completed" );
			_instance._sceneTransition = sceneTransition;
			return sceneTransition;
		}


		#region Global Managers

		/// <summary>
		/// adds a global manager object that will have its update method called each frame before Scene.update is called
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <param name="manager">Manager.</param>
		public static void registerGlobalManager( IUpdatableManager manager )
		{
			_instance._globalManagers.add( manager );
		}


		/// <summary>
		/// removes the global manager object
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <param name="manager">Manager.</param>
		public static void unregisterGlobalManager( IUpdatableManager manager )
		{
			_instance._globalManagers.remove( manager );
		}


		/// <summary>
		/// gets the global manager of type T
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T getGlobalManager<T>() where T : class, IUpdatableManager
		{
			for( var i = 0; i < _instance._globalManagers.length; i++ )
			{
				if( _instance._globalManagers.buffer[i] is T )
					return _instance._globalManagers.buffer[i] as T;
			}
			return null;
		}

		#endregion


		#region Systems access

		/// <summary>
		/// starts a coroutine. Coroutines can yeild ints/floats to delay for seconds or yeild to other calls to startCoroutine.
		/// Yielding null will make the coroutine get ticked the next frame.
		/// </summary>
		/// <returns>The coroutine.</returns>
		/// <param name="enumerator">Enumerator.</param>
		public static ICoroutine startCoroutine( IEnumerator enumerator )
		{
			return _instance._coroutineManager.startCoroutine( enumerator );
		}


		/// <summary>
		/// schedules a one-time or repeating timer that will call the passed in Action
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="repeats">If set to <c>true</c> repeats.</param>
		/// <param name="context">Context.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer schedule( float timeInSeconds, bool repeats, object context, Action<ITimer> onTime )
		{
			return _instance._timerManager.schedule( timeInSeconds, repeats, context, onTime );
		}


		/// <summary>
		/// schedules a one-time timer that will call the passed in Action after timeInSeconds
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="context">Context.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer schedule( float timeInSeconds, object context, Action<ITimer> onTime )
		{
			return _instance._timerManager.schedule( timeInSeconds, false, context, onTime );
		}


		/// <summary>
		/// schedules a one-time or repeating timer that will call the passed in Action
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="repeats">If set to <c>true</c> repeats.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer schedule( float timeInSeconds, bool repeats, Action<ITimer> onTime )
		{
			return _instance._timerManager.schedule( timeInSeconds, repeats, null, onTime );
		}


		/// <summary>
		/// schedules a one-time timer that will call the passed in Action after timeInSeconds
		/// </summary>
		/// <param name="timeInSeconds">Time in seconds.</param>
		/// <param name="onTime">On time.</param>
		public static ITimer schedule( float timeInSeconds, Action<ITimer> onTime )
		{
			return _instance._timerManager.schedule( timeInSeconds, false, null, onTime );
		}

		#endregion

	}
}

