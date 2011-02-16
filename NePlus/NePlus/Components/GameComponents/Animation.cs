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

        // state information
        private int currentFrame;
        float totalElapsedTime;

        public float Angle { get; set; }
        public Vector2 Position { get; set; }

        public Animation(Engine engine, string spriteSheetFilePath, int frameWidth, int frameHeight,
                         int rows, int cols, int frameCount, int framesPerSecond)
            : base(engine)
        {
            this.filePath = spriteSheetFilePath;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.rows = rows;
            this.columns = cols;
            this.frameCount = frameCount;
            this.framesPerSecond = framesPerSecond;

            timePerFrame = 1.0f / framesPerSecond;
            spriteOrigin = new Vector2(frameWidth / 2, frameHeight / 2);

            frames = new List<Rectangle>();

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
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalElapsedTime += elapsed;

            if (totalElapsedTime > timePerFrame)
            {
                ++currentFrame;
                currentFrame = currentFrame % frameCount;

                // reset the total elapsed time
                totalElapsedTime -= timePerFrame;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.SpriteBatch.Draw(spriteSheet, Position, frames[currentFrame], Color.White, Angle, spriteOrigin, 1.0f, SpriteEffects.None, 1.0f);
            Engine.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
