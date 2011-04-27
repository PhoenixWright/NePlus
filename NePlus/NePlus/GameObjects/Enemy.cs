using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus.Components.GameComponents;
using NePlus.Components.PhysicsComponents;
using NePlus.GameObjects.LightObjects;

namespace NePlus.GameObjects
{
    public class Enemy : Component
    {
        // audio
        protected AudioEmitter audioEmitter;
        protected Cue enemySound;

        // components
        protected Animation animation;
        protected PhysicsComponent enemyPhysicsComponent;
        protected Animation deathAnimation;
        protected Light deathLight;

        public bool Active { get; private set; }
        public bool Dead { get; private set; }
        public int Health { get; protected set; }

        public Enemy(Engine engine, Vector2 position)
            : base(engine)
        {
            audioEmitter = new AudioEmitter();

            deathLight = new Light(engine);
            deathLight.Color = Color.Orange;
            deathLight.Fov = MathHelper.TwoPi;
            deathLight.IsOn = false;
            deathLight.Range = 200;

            Health = 100;

            engine.AddComponent(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Health <= 0)
            {
                Dead = true;
            }

            if (!Dead)
            {
                Active = Math.Abs(enemyPhysicsComponent.Position.X - Engine.Player.Position.X) < 1000.0d;

                audioEmitter.Position = new Vector3(enemyPhysicsComponent.Position, 0);
                deathAnimation.Position = enemyPhysicsComponent.Position;
                deathLight.Position = enemyPhysicsComponent.Position;

                if (animation != null && enemyPhysicsComponent != null)
                {
                    animation.Position = enemyPhysicsComponent.Position;
                    animation.Angle = enemyPhysicsComponent.Angle;
                }
            }
            else
            {
                if (deathAnimation != null)
                {
                    if (!deathAnimation.Playing && !deathAnimation.Disposed)
                    {
                        animation.Stop();
                        deathAnimation.Play();
                        deathLight.IsOn = true;
                    }
                    else if (deathAnimation.Playing)
                    {
                        if (deathAnimation.Progress > 0.5f)
                        {
                            deathLight.IsOn = false;
                        }
                    }
                    else
                    {
                        Dispose(true);
                    }
                }
            }

            base.Update(gameTime);
        }

        protected bool EnemyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // check if the player collided with the enemy
            if (fixtureB.CollisionFilter.IsInCollisionCategory((Category)Global.CollisionCategories.Player))
            {
                // player collided with enemy
                OnEnemyPlayerCollision();
            }

            // check if a bullet hit the enemy
            if (fixtureB.CollisionFilter.IsInCollisionCategory((Category)Global.CollisionCategories.PlayerBullet))
            {
                // bullet hit enemy
                Health = 0;
            }

            return true;
        }

        public override void Dispose(bool disposing)
        {
            if (enemySound != null)
            {
                enemySound.Stop(AudioStopOptions.Immediate);
                enemySound.Dispose();
                enemySound = null;
            }

            if (animation != null)
            {
                animation.Dispose(disposing);
                animation = null;
            }

            if (deathAnimation != null)
            {
                deathAnimation.Dispose(disposing);
                deathAnimation = null;
            }

            if (deathLight != null)
            {
                deathLight.Dispose(disposing);
                deathLight = null;
            }

            enemyPhysicsComponent.Dispose(true);
            enemyPhysicsComponent = null;

            base.Dispose(disposing);
        }

        protected virtual void OnEnemyPlayerCollision() { }
    }
}
