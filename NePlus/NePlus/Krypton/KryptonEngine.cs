using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NePlus.Krypton.Lights;

namespace NePlus.Krypton
{
    /// <summary>
    /// A GPU-based 2D lighting engine, wrapped up in a DrawableGameComponent
    /// </summary>
    public class KryptonEngine : Component
    {
        // The Krypton Effect
        private string mEffectAssetName;
        private Effect mEffect;

        // The goods
        private List<ShadowHull> mHulls = new List<ShadowHull>();
        private List<ILight2D> mLights = new List<ILight2D>();

        // World View Projection matrix, and it's min and max view bounds
        private Matrix mWVP = Matrix.Identity;
        private Vector2 mViewMin = Vector2.One * float.MinValue;
        private Vector2 mViewMax = Vector2.One * float.MaxValue;

        // Light maps
        private RenderTarget2D mMapTemp;
        private RenderTarget2D mMapBlur;
        private RenderTarget2D mMapFinal;
        private Color mAmbientColor = new Color(35, 35, 35);

        public KryptonRenderHelper RenderHelper { get; private set; }

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
            set
            {
                mWVP = value;
                mEffect.Parameters["Matrix"].SetValue(value);
            }
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
            get
            {
                return this.mAmbientColor;
            }
            set
            {
                this.mAmbientColor = value;
                this.mEffect.Parameters["AmbientColor"].SetValue(new Vector4(value.R, value.G, value.B, value.A) / 255f);
            }
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
        /// Load's the graphics related content required to draw light maps
        /// </summary>
        public override void LoadContent()
        {
            // This needs to better handle content loading...
            // if the window is resized, Krypton needs to notice.
            this.mEffect = Engine.Content.Load<Effect>(this.mEffectAssetName);
            RenderHelper = new KryptonRenderHelper(Engine.Video.GraphicsDevice, this.mEffect);

            this.mEffect.Parameters["Matrix"].SetValue(Matrix.Identity);

            this.mMapTemp = new RenderTarget2D(Engine.Video.GraphicsDevice, Engine.Video.GraphicsDevice.Viewport.Width, Engine.Video.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
            this.mMapBlur = new RenderTarget2D(Engine.Video.GraphicsDevice, Engine.Video.GraphicsDevice.Viewport.Width, Engine.Video.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PlatformContents);
            this.mMapFinal = new RenderTarget2D(Engine.Video.GraphicsDevice, Engine.Video.GraphicsDevice.Viewport.Width, Engine.Video.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
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
            // Obtain the original rendering states
            var originalRenderTargets = Engine.Video.GraphicsDevice.GetRenderTargets();

            Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapFinal);
            Engine.Video.GraphicsDevice.Clear(Color.Black);

            // Render Light Maps
            foreach (var light in this.mLights)
            {
                if (light.IsOn)
                {
                    // Draw the light and shadows
                    Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapTemp);
                    Engine.Video.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Black, 0, 0);
                    Engine.Video.GraphicsDevice.BlendState = BlendState.Opaque;

                    light.Draw(this.RenderHelper);
                    light.DrawShadows(this.RenderHelper, this.mHulls);

                    // Add the temp light map
                    Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapFinal);
                    RenderHelper.DrawTextureToTarget(this.mMapTemp, BlendTechnique.Add);
                }
            }

            if (true) // blur or not ?
            {
                Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapBlur);
                Engine.Video.GraphicsDevice.Clear(Color.Black);
                this.RenderHelper.BlurTextureToTarget(this.mMapFinal, BlurTechnique.Horizontal);

                Engine.Video.GraphicsDevice.SetRenderTarget(this.mMapFinal);
                this.RenderHelper.BlurTextureToTarget(this.mMapBlur, BlurTechnique.Vertical);
            }

            // Reset to the original rendering states
            Engine.Video.GraphicsDevice.SetRenderTargets(originalRenderTargets);
        }

        /// <summary>
        /// Presents the light map to the current render target
        /// </summary>
        public void LightMapPresent()
        {
            RenderHelper.DrawTextureToTarget(this.mMapFinal, BlendTechnique.Multiply);

            //Stream stream = File.OpenWrite("test.png");
            //mMapFinal.SaveAsPng(stream, mMapFinal.Width, mMapFinal.Height);
        }
    }
}