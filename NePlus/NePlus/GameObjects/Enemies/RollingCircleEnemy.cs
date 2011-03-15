using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using NePlus.Components.GameComponents;

namespace NePlus.GameObjects.Enemies
{
    public class RollingCircleEnemy : Enemy
    {
        public RollingCircleEnemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine, position, shape)
        {
            animation = new Animation(engine, @"Characters\FlickeringCircle", 64, 64, 1, 2, 2, 15, Global.Animations.Repeat);
            animation.Play();

            enemySound = Engine.Audio.GetCue("EnterTheVoid");

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
            {
                Dispose(true);
                return;
            }

            if (!enemySound.IsPlaying && !enemySound.IsDisposed)
            {
                enemySound.Apply3D(Engine.Player.AudioListener, audioEmitter);
                enemySound.Play();
            }

            if (enemyPhysicsComponent.Position.X > Engine.Player.Position.X)
            {
                enemyPhysicsComponent.MainFixture.Body.ApplyTorque(-5.0f);
            }
            else if (enemyPhysicsComponent.Position.X < Engine.Player.Position.X)
            {
                enemyPhysicsComponent.MainFixture.Body.ApplyTorque(5.0f);
            }

            if (!enemySound.IsDisposed)
            {
                enemySound.Apply3D(Engine.Player.AudioListener, audioEmitter);
            }

            base.Update(gameTime);
        }
    }
}
