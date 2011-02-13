using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

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
        public LightComponent LightComponent { get; private set; }
        public PlayerPhysicsComponent PhysicsComponent { get; private set; }

        // variables
        public bool OnGround { get; private set; }
        public bool OnWall { get; private set; }
        private List<Fixture> groundCache;
        private List<Fixture> wallCache;

        public Vector2 Position { get; private set; }

        public Player(Engine engine, Vector2 position)
            : base(engine)
        {
            groundCache = new List<Fixture>();
            wallCache = new List<Fixture>();

            Position = position;

            LightComponent = new LightComponent(engine, position, MathHelper.TwoPi, 0, 250, Color.CornflowerBlue);
            PhysicsComponent = new PlayerPhysicsComponent(Engine, position, true);

            PhysicsComponent.WheelFixture.OnCollision += PlayerOnCollision;
            PhysicsComponent.WheelFixture.OnSeparation += PlayerOnSeparation;

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            // check if the player needs to be reset
            if (Position.Y > 2000.0f)
            {
                PhysicsComponent.ResetPlayerPosition(Engine.Level.GetSpawnPoint());
                groundCache.Clear();
                wallCache.Clear();
                OnGround = false;
                OnWall = false;
                PhysicsComponent.WheelFixture.OnCollision += PlayerOnCollision;
                PhysicsComponent.WheelFixture.OnSeparation += PlayerOnSeparation;
            }

            Position = PhysicsComponent.Position;
            LightComponent.Light.Position = Position + new Vector2(0, 25);

            // check to see if the player is even allowed to jump before we do anything
            if (OnGround)
            {
                if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "JumpButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "JumpKey")))
                {
                    // using world center as the point to apply force to; this makes the point of the force application the center of the fixture
                    PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(0.0f, -100.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
                }
            }

            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "LeftButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "LeftKey")))
            {
                PhysicsComponent.MoveLeft();

                if (!OnGround)
                {
                    if (Math.Abs(PhysicsComponent.MainFixture.Body.LinearVelocity.X) < 10.0f)
                    {
                        PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(-1.5f, 0.0f));
                    }
                }
            }
            else if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "RightButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "RightKey")))
            {
                PhysicsComponent.MoveRight();

                if (!OnGround)
                {
                    PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(1.5f, 0.0f));
                }
            }
            else
            {
                PhysicsComponent.StopMoving();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.SpriteBatch.End();
        }

        private bool PlayerOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Vector2 down = new Vector2(0.0f, 1.0f);

            Manifold manifold;
            contact.GetManifold(out manifold);

            float angle = Math.Abs(Vector2.Dot(manifold.LocalNormal, down));

            if (angle > 0.99f)
            {
                OnGround = true;
                groundCache.Add(fixtureB);
            }

            if (angle < 0.15f && angle > -0.15f)
            {
                OnWall = true;
            }

            return true;
        }

        private void PlayerOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (groundCache.Contains(fixtureB))
            {
                groundCache.Remove(fixtureB);

                if (groundCache.Count == 0)
                {
                    OnGround = false;
                }
            }

            if (wallCache.Contains(fixtureB))
            {
                wallCache.Remove(fixtureB);

                if (wallCache.Count == 0)
                {
                    OnWall = false;
                }
            }
        }
    }
}