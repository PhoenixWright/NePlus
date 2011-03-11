using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus;
using NePlus.Components.EngineComponents;
using NePlus.Components.GameComponents;
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

        // variables/state management
        private Vector2 airMovementForce;
        private double airTimer;
        private double airMovementWindow;
        private List<Bullet> bullets;
        private double lastGameTime;
        private bool releasedFire { get; set; }

        public bool Crouching { get; private set; }
        public Global.Directions LastDirection = Global.Directions.Right;
        public bool OnGround { get; private set; }
        public bool OnWall { get; private set; }
        public bool Walking { get; private set; }
        private List<Fixture> groundCache;
        private List<Fixture> wallCache;

        public Vector2 Position { get; private set; }

        // animations
        Sprite playerStandingRight;
        Sprite playerCrouchingRight;
        Sprite playerJumpingRight;
        Sprite playerFallingRight;
        Animation playerWalkingRight;

        public Player(Engine engine, Vector2 position)
            : base(engine)
        {
            airMovementForce = new Vector2(1.0f, 0.0f);
            airMovementWindow = 3000.0d;
            bullets = new List<Bullet>();
            releasedFire = true;
            groundCache = new List<Fixture>();
            wallCache = new List<Fixture>();

            Position = position;

            LightComponent = new LightComponent(engine, position, MathHelper.TwoPi, 0, 250, Color.White);
            PhysicsComponent = new PlayerPhysicsComponent(Engine, position);

            PhysicsComponent.WheelFixture.OnCollision += PlayerOnCollision;
            PhysicsComponent.WheelFixture.OnSeparation += PlayerOnSeparation;

            // load visuals
            playerStandingRight = new Sprite(engine, @"Characters\Player\PlayerStandingRight");
            playerCrouchingRight = new Sprite(engine, @"Characters\Player\PlayerCrouchingRight");
            playerJumpingRight = new Sprite(engine, @"Characters\Player\PlayerJumpingRight");
            playerFallingRight = new Sprite(engine, @"Characters\Player\PlayerFallingRight");
            playerWalkingRight = new Animation(engine, @"Characters\Player\PlayerWalkingRight", 88, 132, 2, 4, 8, 4, Global.Animations.Repeat);

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            double timeSinceLastUpdate = gameTime.ElapsedGameTime.TotalMilliseconds - lastGameTime;

            // update whether or not the player can fire
            UpdateProjectiles();

            // check if the player needs to be reset
            if (Position.Y > 2000.0f)
            {
                ResetPlayer();
            }

            Position = PhysicsComponent.Position;
            LightComponent.Light.Position = Position + new Vector2(0, 25);

            if (OnGround)
            {
                airTimer = 0.0f;
                Crouching = Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "DownButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "DownKey"));
            }
            else
            {
                airTimer += timeSinceLastUpdate;
            }

            // check to see if the player is even allowed to jump before we do anything
            if (!Crouching && OnGround)
            {
                if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "JumpButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "JumpKey")))
                {
                    // using world center as the point to apply force to; this makes the point of the force application the center of the fixture
                    PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(0.0f, -80.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
                }
            }

            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "LeftButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "LeftKey")))
            {
                PhysicsComponent.MoveLeft();
                LastDirection = Global.Directions.Left;

                if (!OnGround)
                {
                    // apply a bit of force in the air if the air timer is still under the air movement window
                    if (airTimer < airMovementWindow)
                    {
                        PhysicsComponent.MainFixture.Body.ApplyForce(-airMovementForce);
                    }
                }

                // we're only walking if we're on the ground
                Walking = OnGround;
            }
            else if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "RightButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "RightKey")))
            {
                PhysicsComponent.MoveRight();
                LastDirection = Global.Directions.Right;

                if (!OnGround)
                {
                    // apply a bit of force in the air if the air timer is still under the air movement window
                    if (airTimer < airMovementWindow)
                    {
                        PhysicsComponent.MainFixture.Body.ApplyForce(airMovementForce);
                    }
                }

                // we're only walking if we're on the ground
                Walking = OnGround;
            }
            else
            {
                PhysicsComponent.StopMoving();
                Walking = false;
            }

            UpdateAllArt();

            lastGameTime = gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.SpriteBatch.End();
        }

        private void UpdateProjectiles()
        {
            int idx = 0;
            while (idx < bullets.Count)
            {
                if (bullets[idx].Disposed)
                {
                    bullets.RemoveAt(idx);
                    continue;
                }

                ++idx;
            }

            bool firePressed = Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "FireButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "FireKey"));

            if (!releasedFire)
            {
                if (!firePressed)
                {
                    releasedFire = true;
                }
            }

            bool canFire = releasedFire && bullets.Count < 2;

            // handle firing projectiles
            if (canFire)
            {
                if (firePressed)
                {
                    releasedFire = false;

                    Vector2 bulletPosition;

                    //determine bullet position
                    switch (LastDirection)
                    {
                        case Global.Directions.Left:
                            bulletPosition = Position + new Vector2(-40.0f, 0.0f);
                            break;
                        case Global.Directions.Right:
                            bulletPosition = Position + new Vector2(40.0f, 0.0f);
                            break;
                        default:
                            bulletPosition = Position;
                            break;
                    }

                    if (Crouching)
                    {
                        bulletPosition.Y += 40.0f;
                    }

                    // create bullet
                    Bullet bullet = new Bullet(Engine, bulletPosition, LastDirection, Global.CollisionCategories.PlayerBullet);
                    bullets.Add(bullet);
                }
                else
                {
                    releasedFire = true;
                }
            }
        }

        private void ChangeArtDirectionLeft()
        {
            playerStandingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerCrouchingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerJumpingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerFallingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerWalkingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
        }

        private void ChangeArtDirectionRight()
        {
            playerStandingRight.SpriteEffect = SpriteEffects.None;
            playerCrouchingRight.SpriteEffect = SpriteEffects.None;
            playerJumpingRight.SpriteEffect = SpriteEffects.None;
            playerFallingRight.SpriteEffect = SpriteEffects.None;
            playerWalkingRight.SpriteEffect = SpriteEffects.None;
        }

        private void HideAllArt()
        {
            playerStandingRight.Visible = false;
            playerCrouchingRight.Visible = false;
            playerJumpingRight.Visible = false;
            playerFallingRight.Visible = false;
            playerWalkingRight.Stop();
        }

        private void UpdateAllArt()
        {
            // update art positions
            Vector2 artOffsetVector = new Vector2(0.0f, 25.0f);
            playerStandingRight.Position = Position + artOffsetVector;
            playerCrouchingRight.Position = Position + artOffsetVector;
            playerJumpingRight.Position = Position + artOffsetVector;
            playerFallingRight.Position = Position + artOffsetVector;
            playerWalkingRight.Position = Position + artOffsetVector;

            // update art directions
            if (LastDirection == Global.Directions.Right)
            {
                ChangeArtDirectionRight();
            }
            else
            {
                ChangeArtDirectionLeft();
            }

            // set states appropriately
            if (Walking)
            {
                if (!playerWalkingRight.Playing)
                {
                    HideAllArt();
                    playerWalkingRight.Play();
                }
            }
            else
            {
                // hide everything and stop animations
                HideAllArt();

                if (OnGround)
                {
                    if (Crouching)
                    {
                        // unhide crouching sprite
                        playerCrouchingRight.Visible = true;
                    }
                    else
                    {
                        playerStandingRight.Visible = true;
                    }
                }
                else
                {
                    // check physics object to see if we're going up or down
                    if (PhysicsComponent.MainFixture.Body.LinearVelocity.Y > 0.0f)
                    {
                        // going down
                        playerFallingRight.Visible = true;
                    }
                    else
                    {
                        // going up
                        playerJumpingRight.Visible = true;
                    }
                }
            }
        }

        private void ResetPlayer()
        {
            PhysicsComponent.ResetPlayerPosition(Engine.Level.GetSpawnPoint());
            groundCache.Clear();
            wallCache.Clear();
            OnGround = false;
            OnWall = false;
            PhysicsComponent.WheelFixture.OnCollision += PlayerOnCollision;
            PhysicsComponent.WheelFixture.OnSeparation += PlayerOnSeparation;
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
