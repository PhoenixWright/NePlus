using System;

using Microsoft.Xna.Framework;

using NePlus.Components.GameComponents;
using NePlus.Components.GraphicsComponents;

namespace NePlus.GameObjects.Enemies
{
    public class RotatingBoxEnemy : Enemy
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

        public RotatingBoxEnemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine, position, shape)
        {
            animation = new Animation(engine, @"Characters\GrayRotatingBox", 128, 128, 4, 4, 16, 9);
            animation.DrawOrder = int.MaxValue - 1;

            attacking = false;

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                attacking = true;
            }

            // manipulate the physics object to float around and make dives at the player
            if (attacking)
            {
                // TODO: try to hit the player
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
                    y = -random.Next(25, 50);
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

            base.Update(gameTime);
        }
    }
}
