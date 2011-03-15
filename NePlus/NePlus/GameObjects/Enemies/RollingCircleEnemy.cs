using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using NePlus.Components.GameComponents;
using NePlus.GameObjects.LightObjects;

namespace NePlus.GameObjects.Enemies
{
    public class RollingCircleEnemy : Enemy
    {
        Light light;
        float timer;

        public RollingCircleEnemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine, position, shape)
        {
            animation = new Animation(engine, @"Characters\FlickeringCircle", 64, 64, 1, 2, 2, 15, Global.Animations.Repeat);
            animation.Play();

            deathAnimation = new Animation(engine, @"Miscellaneous\Explosion", 512, 512, 3, 4, 9, 20, Global.Animations.PlayOnce);
            deathAnimation.Scale = 0.3f;
            deathAnimation.DrawOrder = int.MaxValue - 1;

            enemySound = Engine.Audio.GetCue("EnterTheVoid");

            light = new Light(engine);
            light.Color = Color.White;
            light.Fov = MathHelper.TwoPi;
            light.Position = position;
            light.Range = 100;

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
            {
                light.IsOn = false;
            }

            timer += gameTime.ElapsedGameTime.Milliseconds;

            light.Position = enemyPhysicsComponent.Position;

            if (timer > 100.0f)
            {
                if (light.Color == Color.White)
                {
                    light.Color = Color.Orange;
                }
                else
                {
                    light.Color = Color.White;
                }

                timer = 0.0f;
            }

            if (!enemySound.IsPlaying && !enemySound.IsDisposed)
            {
                enemySound.Apply3D(Engine.Player.AudioListener, audioEmitter);
                enemySound.Play();
            }

            if (Active)
            {
                if (enemyPhysicsComponent.Position.X > Engine.Player.Position.X)
                {
                    enemyPhysicsComponent.MainFixture.Body.ApplyTorque(-5.0f);
                }
                else if (enemyPhysicsComponent.Position.X < Engine.Player.Position.X)
                {
                    enemyPhysicsComponent.MainFixture.Body.ApplyTorque(5.0f);
                }
            }

            if (!enemySound.IsDisposed)
            {
                enemySound.Apply3D(Engine.Player.AudioListener, audioEmitter);
            }

            base.Update(gameTime);
        }

        public override void Dispose(bool disposing)
        {
            light.Dispose(disposing);
            light = null;

            base.Dispose(disposing);
        }
    }
}
