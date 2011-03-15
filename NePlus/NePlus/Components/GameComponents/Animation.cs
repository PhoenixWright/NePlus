using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.Components.GameComponents
{
    /// <summary>
    /// The point of this class is to manage a spritesheet and draw the correct frames at the correct times.
    /// </summary>
    public class Animation : Component
    {
        // sprite sheet characteristics
        private Texture2D spriteSheet;
        private string filePath;
        private int frameWidth;
        private int frameHeight;
        private int rows;
        private int columns;
        private int frameCount;
        private int framesPerSecond;
        private float timePerFrame;
        private List<Rectangle> frames;
        private Vector2 spriteOrigin;
        Global.Animations type;

        // state information
        private int currentFrame;
        float totalElapsedTime;
        public bool Playing { get; private set; }

        public float Angle { get; set; }
        public Vector2 Position { get; set; }
        public float Progress
        {
            get
            {
                return (float)currentFrame / (float)frameCount;
            }
        }
        public float Scale { get; set; }
        public float Speed { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        /// <summary>
        /// Constructs an animation from information about the spritesheet.
        /// </summary>
        /// <param name="engine">The game engine reference.</param>
        /// <param name="spriteSheetFilePath">The spritesheet filepath.</param>
        /// <param name="frameWidth">The width of a frame in the spritesheet.</param>
        /// <param name="frameHeight">The height of a frame in the spritesheet.</param>
        /// <param name="rows">The number of rows in the spritesheet.</param>
        /// <param name="cols">The number of columns in the spritesheet.</param>
        /// <param name="frameCount">The number of frames to play in the spritesheet.</param>
        /// <param name="framesPerSecond">The number of frames to display per second.</param>
        /// <param name="type">The type of animation (repeat, play once, etc.).</param>
        public Animation(Engine engine, string spriteSheetFilePath, int frameWidth, int frameHeight,
                         int rows, int cols, int frameCount, int framesPerSecond, Global.Animations type)
            : base(engine)
        {
            this.filePath = spriteSheetFilePath;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.rows = rows;
            this.columns = cols;
            this.frameCount = frameCount;
            this.framesPerSecond = framesPerSecond;
            this.type = type;

            timePerFrame = 1.0f / framesPerSecond;
            spriteOrigin = new Vector2(frameWidth / 2, frameHeight / 2);

            frames = new List<Rectangle>();

            this.Scale = 1.0f;
            this.Speed = 1.0f;
            SpriteEffect = SpriteEffects.None;

            // defaults to not playing
            Playing = false;

            engine.AddComponent(this);
        }

        public override void LoadContent()
        {
            spriteSheet = Engine.Content.Load<Texture2D>(filePath);

            // fill out the list of rectangles
            for (int rowIdx = 0; rowIdx < rows; ++rowIdx)
            {
                for (int colIdx = 0; colIdx < columns; ++colIdx)
                {
                    frames.Add(new Rectangle(colIdx * frameWidth, rowIdx * frameHeight, frameWidth, frameHeight));

                    if (frames.Count == frameCount)
                    {
                        break;
                    }
                }

                if (frames.Count == frameCount)
                {
                    break;
                }
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Playing)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                totalElapsedTime += elapsed;

                if (totalElapsedTime > timePerFrame * (1.0 / Speed))
                {
                    ++currentFrame;

                    // check to see if we're done with a oneshot animation
                    if (type == Global.Animations.PlayOnce)
                    {
                        if (currentFrame == frameCount)
                        {
                            // get rid of the animation object
                            Dispose(true);
                            Playing = false;
                            return;
                        }
                    }

                    currentFrame = currentFrame % frameCount;

                    // reset the total elapsed time
                    totalElapsedTime -= timePerFrame;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Playing)
            {
                Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
                Engine.SpriteBatch.Draw(spriteSheet, Position, frames[currentFrame], Color.White, Angle, spriteOrigin, Scale, SpriteEffect, 1.0f);
                Engine.SpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public void Play()
        {
            Playing = true;
        }

        public void Stop()
        {
            Playing = false;

            totalElapsedTime = 0.0f;
            currentFrame = 0;
        }
    }
}
