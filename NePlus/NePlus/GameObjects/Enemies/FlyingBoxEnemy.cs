using System;

using Microsoft.Xna.Framework;

using NePlus.Components.GameComponents;
using NePlus.Components.GraphicsComponents;

namespace NePlus.GameObjects.Enemies
{
    public class FlyingBoxEnemy : Enemy
    {
        // state management
        // an "attack" from the rotating box enemy is to dive at the player
        bool attacking;

        int minX = 800;
        int maxX = 1000;
        int minY = 200;
        int maxY = 300;
        Random random = new Random();

        float timeBetweenAttacks = 7.0f;
        float timeSinceLastAttack = 0.0f;

        public FlyingBoxEnemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine, position, shape)
        {
            animation = new Animation(engine, @"Characters\GrayRotatingBox", 128, 128, 4, 4, 16, 9, Global.Animations.Repeat);
            animation.DrawOrder = (int)Global.Layers.AboveLighting;
            animation.Play();

            deathAnimation = new Animation(engine, @"Miscellaneous\Explosion", 512, 512, 3, 4, 9, 20, Global.Animations.PlayOnce);
            deathAnimation.DrawOrder = int.MaxValue - 1;
            deathAnimation.Scale = 0.3f;

            attacking = false;

            enemySound = Engine.Audio.GetCue("WeirdHoverSound");

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                if (!enemySound.IsPlaying && !enemySound.IsDisposed)
                {
                    enemySound.Apply3D(Engine.Player.AudioListener, audioEmitter);
                    enemySound.Play();
                }

                if (timeSinceLastAttack > timeBetweenAttacks)
                {
                    attacking = true;
                }

                // manipulate the physics object to float around and make dives at the player
                if (attacking)
                {
                    // TODO: try to hit the player
                    Attack();
                }
                else
                {
                    timeSinceLastAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // these are the force values to be applied to the enemy for movement
                    float x = 0;
                    float y = 0;

                    // move around the player
                    if (Math.Abs(enemyPhysicsComponent.Position.X - Engine.Player.Position.X) > minX)
                    {
                        if (enemyPhysicsComponent.Position.X > Engine.Player.Position.X)
                        {
                            // then we need to move left
                            x = -50;
                        }
                        else
                        {
                            // move right
                            x = 50;
                        }
                    }
                    else
                    {
                        // just move around randomly
                        // could float up a bit, could not, doesn't matter
                        int randomNumber = random.Next(0, 500);
                        if (randomNumber < 20)
                        {
                            x = -random.Next(20, 40);
                        }

                        if (randomNumber > 480)
                        {
                            x = random.Next(20, 40);
                        }
                    }

                    // float above the player
                    if (Math.Abs((enemyPhysicsComponent.Position.Y - Engine.Player.Position.Y)) < minY)
                    {
                        // absolutely must float upward
                        if (enemyPhysicsComponent.MainFixture.Body.LinearVelocity.Y > -4.0f)
                        {
                            y = -random.Next(25, 50);
                        }
                    }
                    else
                    {
                        // could float up a bit, could not, doesn't matter
                        int randomNumber = random.Next(0, 1000);
                        if (randomNumber < 20)
                        {
                            y = -random.Next(20, 40);
                        }
                    }

                    enemyPhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(x, y));
                }

                if (!enemySound.IsDisposed)
                {
                    enemySound.Apply3D(Engine.Player.AudioListener, audioEmitter);
                }
            }

            base.Update(gameTime);
        }

        private void Attack()
        {
            MoveTowardPlayerX();
            MoveTowardPlayerY();
        }

        private void MoveTowardPlayerX()
        {
            float x;

            if (enemyPhysicsComponent.Position.X > Engine.Player.Position.X)
            {
                // then we need to move left
                x = -10;
            }
            else
            {
                // move right
                x = 10;
            }

            enemyPhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(x, 0));
        }

        private void MoveTowardPlayerY()
        {
            float y;

            if (enemyPhysicsComponent.Position.Y > Engine.Player.Position.Y)
            {
                // move down
                y = -10;
            }
            else
            {
                y = 10;
            }

            enemyPhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(0, y));
        }

        protected override void OnEnemyPlayerCollision()
        {
            // reset attack variables
            timeSinceLastAttack = 0.0f;
            attacking = false;

            base.OnEnemyPlayerCollision();
        }
    }
}
