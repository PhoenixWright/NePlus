using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NePlus.Krypton.Lights;

namespace NePlus.Krypton
{
    public enum LightMapSize
    {
        Full = 1,
        Fourth = 2,
        Eighth = 4,
    }

    public enum CullMode
    {
        None = 0,
        Clockwise = 1,
        CounterClockwise = 2,
    }

    /// <summary>
    /// A GPU-based 2D lighting engine, wrapped up in a DrawableGameComponent
    /// </summary>
    public class KryptonEngine : Component
    {
        // The Krypton Effect
        private string mEffectAssetName;
        private Effect mEffect;
        private CullMode mCullMode = CullMode.CounterClockwise;

        // The goods
        private List<ShadowHull> mHulls = new List<ShadowHull>();
        private List<ILight2D> mLights = new List<ILight2D>();

        // World View Projection matrix, and it's min and max view bounds
        private Matrix mWVP = Matrix.Identity;
        private bool mSpriteBatchCompatabilityEnabled = false;
        private Vector2 mViewMin = Vector2.One * float.MinValue;
        private Vector2 mViewMax = Vector2.One * float.MaxValue;

        // Blur
        private bool mBlurEnable = false;
        private float mBluriness = 1;
        private RenderTarget2D mMapBlur;

        // Light maps
        private RenderTarget2D mMapTemp;
        private RenderTarget2D mMapFinal;
        private Color mAmbientColor = new Color(35,35,35);
        private LightMapSize mLightMapSize = LightMapSize.Full;

        public KryptonRenderHelper RenderHelper { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating how Krypton should cull geometry. The default value is CullMode.CounterClockwise
        /// </summary>
        public CullMode CullMode
        {
            get { return this.mCullMode; }
            set { this.mCullMode = value; }
        }

        /// <summary>
        /// The collection of lights krypton uses to render shadows
        /// </summary>
        public List<ILight2D> Lights { get { return this.mLights; } }

        /// <summary>
        /// The collection of hulls krypton uses to render shadows
        /// </summary>
        public List<ShadowHull> Hulls { get { return this.mHulls; } }

        /// <summary>
        /// The matrix used to draw the light map. This should match your scene.
        /// </summary>
        public Matrix Matrix
        {
            set { this.mWVP = value; }
            get { return this.mWVP; }
        }

        /// <summary>
        /// Gets or sets a value indicating weither or not to use SpriteBatch's matrix drawing lightmaps
        /// </summary>
        public bool SpriteBatchCompatablityEnabled
        {
            get { return this.mSpriteBatchCompatabilityEnabled; }
            set { this.mSpriteBatchCompatabilityEnabled = value; }
        }

        /// <summary>
        /// Not used yet - will be used for culling
        /// </summary>
        public Vector2 ViewMin
        {
            get { return this.mViewMin; }
            set { this.mViewMin = value; }
        }

        /// <summary>
        /// Not used yet - will be used for culling
        /// </summary>
        public Vector2 ViewMax
        {
            get { return this.mViewMin; }
            set { this.mViewMax = value; }
        }

        /// <summary>
        /// Ambient color of the light map. Lights + AmbientColor = Final 
        /// </summary>
        public Color AmbientColor
        {
            get { return this.mAmbientColor; }
            set { this.mAmbientColor = value; }
        }

        /// <summary>
        /// Gets or sets the value used to determine light map size
        /// </summary>
        public LightMapSize LightMapSize
        {
            get { return this.mLightMapSize; }
            set
            {
                if (this.mLightMapSize != value)
                {
                    this.mLightMapSize = value;
                    this.DisposeRenderTargets();
                    this.CreateRenderTargets();
                }
            }
        }

        public bool BlurEnable
        {
            get { return this.mBlurEnable; }
            set { this.mBlurEnable = value; }
        }

        public float Bluriness
        {
            get { return this.mBluriness; }
            set { this.mBluriness = Math.Max(0, value); }
        }

        public float BlurFactorU { set { this.mEffect.Parameters["BlurFactorU"].SetValue(value); } }
        public float BlurFactorV { set { this.mEffect.Parameters["BlurFactorV"].SetValue(value); } }

        /// <summary>
        /// Constructs a new instance of krypton
        /// </summary>
        /// <param name="game">Your game object</param>
        /// <param name="effectAssetName">The asset name of Krypton's effect file, which should be included in your content project</param>
        public KryptonEngine(Engine engine, string effectAssetName)
            : base(engine)
        {
            this.mEffectAssetName = effectAssetName;
        }

        /// <summary>
        /// Initialized Krpyton, and hooks itself to the Graphcs Device
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Engine.Video.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Engine.Video.GraphicsDevice.DeviceReset -= new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
        }

        void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            this.DisposeRenderTargets();
            this.CreateRenderTargets();
        }

        /// <summary>
        /// Load's the graphics related content required to draw light maps
        /// </summary>
        public override void LoadContent()
        {
            // This needs to better handle content loading...
            // if the window is resized, Krypton needs to notice.
            this.mEffect = Engine.Content.Load<Effect>(this.mEffectAssetName);
            RenderHelper = new KryptonRenderHelper(Engine.Video.GraphicsDevice, this.mEffect);

            this.CreateRenderTargets();
        }

        public override void UnloadContent()
        {
            this.DisposeRenderTargets();
        }

        private void CreateRenderTargets()
        {
            var targetWidth = Engine.Video.GraphicsDevice.Viewport.Width / (int)(this.mLightMapSize);
            var targetHeight = Engine.Video.GraphicsDevice.Viewport.Height / (int)(this.mLightMapSize);

            this.mMapTemp = new RenderTarget2D(Engine.Video.GraphicsDevice, targetWidth, targetHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
            this.mMapBlur = new RenderTarget2D(Engine.Video.GraphicsDevice, targetWidth, targetHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
            this.mMapFinal = new RenderTarget2D(Engine.Video.GraphicsDevice, targetWidth, targetHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
        }

        private void DisposeRenderTargets()
        {
            KryptonEngine.TryDispose(this.mMapTemp);
            KryptonEngine.TryDispose(this.mMapBlur);
            KryptonEngine.TryDispose(this.mMapFinal);
        }

        private static void TryDispose(IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = null;
            }
        }

        /// <summary>
        /// Draws the light map to the current render target
        /// </summary>
        /// <param name="gameTime">N/A - Required</param>
        public override void Draw(GameTime gameTime)
        {
            this.LightMapPresent();
        }

        /// <summary>
        /// Prepares the light map to be drawn (pre-render)
        /// </summary>
        public void LightMapPrepare()
        {
            // Prepare and set the matrix
            var viewWidth = Engine.Video.GraphicsDevice.ScissorRectangle.Width;
            var viewHeight = Engine.Video.GraphicsDevice.ScissorRectangle.Height;

            KryptonEngine.MatrixPrepareAndSet(this.mWVP, this.mSpriteBatchCompatabilityEnabled, this.mEffect.Parameters["Matrix"], viewWidth, viewHeight);

            // Obtain the original rendering states
            var originalRenderTargets = Engine.Video.GraphicsDevice.GetRenderTargets();

            Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapFinal);
            Engine.Video.GraphicsDevice.Clear(this.mAmbientColor);

            // Render Light Maps
            foreach (var light in this.mLights)
            {
                if (light.IsOn)
                {
                    // Draw the light and shadows
                    Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapTemp);
                    Engine.Video.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Black, 0, 0);
                    Engine.Video.GraphicsDevice.BlendState = BlendState.Opaque;

                    Engine.Video.GraphicsDevice.RasterizerState = KryptonEngine.RasterizerStateGetFromCullMode(this.mCullMode);

                    light.Draw(this.RenderHelper);
                    light.DrawShadows(this.RenderHelper, this.mHulls);

                    // Add the temp light map
                    Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapFinal);
                    RenderHelper.DrawTextureToTarget(this.mMapTemp, this.mLightMapSize, BlendTechnique.Add);
                }
            }

            if (this.mBlurEnable)
            {
                Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapBlur);
                Engine.Video.GraphicsDevice.Clear(Color.Black);
                this.RenderHelper.BlurTextureToTarget(this.mMapFinal, LightMapSize.Full, BlurTechnique.Horizontal, this.mBluriness);
                Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapFinal);
                this.RenderHelper.BlurTextureToTarget(this.mMapBlur, LightMapSize.Full, BlurTechnique.Vertical, this.mBluriness);
            }

            // Reset to the original rendering states
            Engine.Video.GraphicsDevice.SetRenderTargets(originalRenderTargets);
        }

        /// <summary>
        /// Sets a matrix parameter of an effect, according to a user defined matrix, and (optionally) the default SpriteBatch matrix
        /// </summary>
        /// <param name="matrixWVP">User-defined matrix</param>
        /// <param name="useSpriteBatchMatrix">Is the SpriteBatch matrix?</param>
        /// <param name="effectParameter">The effect's Matrix parameter</param>
        /// <param name="width">The render target width</param>
        /// <param name="height">The render target height</param>
        private static void MatrixPrepareAndSet(Matrix matrixWVP, bool useSpriteBatchMatrix, EffectParameter effectParameter, float width, float height)
        {
            Matrix matrixSpriteBatch = Matrix.Identity;

            if (useSpriteBatchMatrix)
            {
                float num2 = (width > 0) ? (1f / ((float)width)) : 0f;
                float num = (height > 0) ? (-1f / ((float)height)) : 0f;

                matrixSpriteBatch = new Matrix
                {
                    M11 = num2 * 2f,
                    M22 = num * 2f,
                    M33 = 1f,
                    M44 = 1f,
                    M41 = -1f - num2,
                    M42 = 1f - num
                };
            }

            effectParameter.SetValue(matrixWVP * matrixSpriteBatch);
        }
        /// <summary>
        /// Retrieves a rasterize state by using the cull mode as a lookup
        /// </summary>
        /// <param name="cullMode"></param>
        /// <returns></returns>
        private static RasterizerState RasterizerStateGetFromCullMode(Krypton.CullMode cullMode)
        {
            switch (cullMode)
            {
                case (CullMode.CounterClockwise):
                    return RasterizerState.CullCounterClockwise;

                case (CullMode.Clockwise):
                    return RasterizerState.CullClockwise;

                default:
                    return RasterizerState.CullNone;
            }
        }

        /// <summary>
        /// Presents the light map to the current render target
        /// </summary>
        public void LightMapPresent()
        {
            RenderHelper.DrawTextureToTarget(this.mMapFinal, this.mLightMapSize, BlendTechnique.Multiply);
        }
    }
}