using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlus;
using NePlus.Components.EngineComponents;
using NePlus.Components.GraphicsComponents;
using NePlus.Components.PhysicsComponents;
using NePlus.GameComponents;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.GameObjects
{
    public class Player : Component
    {
        // components
        public ParticleEffectComponent ParticleEffectComponent { get; private set; }
        public PlayerPhysicsComponent PhysicsComponent { get; private set; }

        // variables
        public Vector2 Position { get; private set; }
        private Texture2D texture;

        public Player(Engine engine, Vector2 position) : base(engine)
        {
            Position = position;

            texture = Engine.Content.Load<Texture2D>(@"Characters\TestSquare");

            //ParticleEffectComponent = new ParticleEffectComponent(engine, "someName", Position);
            //PhysicsComponent = new RectanglePhysicsComponent(Engine, texture.Bounds, position, true);
            PhysicsComponent = new PlayerPhysicsComponent(Engine, position, true);
            PhysicsComponent.MainFixture.CollisionFilter.CollidesWith = FarseerPhysics.Dynamics.Category.Cat1;

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            Position = PhysicsComponent.Position;
            //Engine.Camera.Position = Position;

            // check if the player is even able to jump before we check for input
            //if (PhysicsComponent.MainFixture.Body.
                if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "JumpButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "JumpKey")))
                {
                    // using world center as the point to apply force to; this makes the point of the force application the center of the fixture
                    PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(0.0f, -10.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
                }

            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "LeftButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "LeftKey")))
            {
                PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(-4.0f, 0.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
            }

            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "RightButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "RightKey")))
            {
                PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(4.0f, 0.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
            }

            //ParticleEffectComponent.Position = this.Position;
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.SpriteBatch.Draw(texture, Position, null, Color.White, PhysicsComponent.MainFixture.Body.Rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            Engine.SpriteBatch.End();
        }
    }
}
