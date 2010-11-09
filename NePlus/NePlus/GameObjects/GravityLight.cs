using Microsoft.Xna.Framework;

using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GravityLight : Light
    {
        public float GravityValue { get; private set; }

        public GravityLight(Vector2 position, float gravityValue) : base(Engine.Game, position)
        {
            GravityValue = gravityValue;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
