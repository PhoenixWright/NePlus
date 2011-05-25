using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
using NePlus.GameObjects.LightObjects;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.GameObjects
{
    public class PlayerEnemy : Enemy
    {
        // components
        public Light light { get; private set; }
        public EnemyPlayerPhysicsComponent PhysicsComponent { get; private set; }

        // variables/state management
        private Vector2 airMovementForce;
        private double airTimer;
        private double airMovementWindow;
        private List<Bullet> bullets;
        private double bulletTimer;
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
        Sprite playerEnemyStandingRight;
        Sprite playerEnemyArmShootingRight;
        Sprite playerEnemyCrouchingRight;
        Sprite playerEnemyJumpingRight;
        Sprite playerEnemyFallingRight;
        Animation playerEnemyWalkingRight;

        Random random;

        public PlayerEnemy(Engine engine, Vector2 position)
            : base(engine, position)
        {
            DrawOrder = (int)Global.Layers.Player;
            Health = 100;

            deathAnimation = new Animation(engine, @"Miscellaneous\Explosion", 512, 512, 3, 4, 9, 20, Global.Animations.PlayOnce);
            deathAnimation.DrawOrder = int.MaxValue - 1;
            deathAnimation.Scale = 0.3f;

            PhysicsComponent = new EnemyPlayerPhysicsComponent(Engine, position);
            PhysicsComponent.MainFixture.Body.LinearDamping = 2.0f;
            PhysicsComponent.MainFixture.OnCollision += EnemyOnCollision;
            PhysicsComponent.WheelFixture.OnCollision += EnemyOnCollision;
            enemyPhysicsComponent = PhysicsComponent;

            airMovementForce = new Vector2(1.0f, 0.0f);
            airMovementWindow = 3000.0d;
            bullets = new List<Bullet>();
            bulletTimer = double.MaxValue / 2;
            releasedFire = true;
            groundCache = new List<Fixture>();
            wallCache = new List<Fixture>();

            Position = position;

            light = new Light(engine);
            light.Color = Color.White;
            light.Fov = MathHelper.TwoPi;
            light.Position = position;
            light.Range = 250;
            light.ShadowType = Krypton.Lights.ShadowType.Illuminated;

            PhysicsComponent = new EnemyPlayerPhysicsComponent(Engine, position);

            PhysicsComponent.WheelFixture.OnCollision += PlayerEnemyOnCollision;
            PhysicsComponent.WheelFixture.OnSeparation += PlayerEnemyOnSeparation;

            // load visuals
            playerEnemyArmShootingRight = new Sprite(engine, @"Characters\PlayerEnemy\EnemyArmShootingRight");
            playerEnemyArmShootingRight.DrawOrder = DrawOrder;
            playerEnemyStandingRight = new Sprite(engine, @"Characters\PlayerEnemy\EnemyStandingRight");
            playerEnemyStandingRight.DrawOrder = DrawOrder;
            playerEnemyCrouchingRight = new Sprite(engine, @"Characters\PlayerEnemy\EnemyCrouchingRight");
            playerEnemyCrouchingRight.DrawOrder = DrawOrder;
            playerEnemyJumpingRight = new Sprite(engine, @"Characters\PlayerEnemy\EnemyJumpingRight");
            playerEnemyJumpingRight.DrawOrder = DrawOrder;
            playerEnemyFallingRight = new Sprite(engine, @"Characters\PlayerEnemy\EnemyFallingRight");
            playerEnemyFallingRight.DrawOrder = DrawOrder;
            playerEnemyWalkingRight = new Animation(engine, @"Characters\PlayerEnemy\EnemyWalkingRight", 88, 132, 2, 4, 8, 4, Global.Animations.Repeat);
            playerEnemyWalkingRight.DrawOrder = DrawOrder;

            random = new Random();

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                UpdateBloom();

                bulletTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                // update whether or not the PlayerEnemy can fire
                UpdateProjectiles();

                Position = PhysicsComponent.Position;
                light.Position = Position + new Vector2(0, 25);

                if (OnGround)
                {
                    airTimer = 0.0f;

                    if (Crouching)
                    {
                        Crouching = random.Next(100) > 5;
                    }
                    else
                    {
                        Crouching = random.Next(100) > 95;
                    }
                }
                else
                {
                    airTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                // check to see if the PlayerEnemy is even allowed to jump before we do anything
                if (!Crouching && OnGround)
                {
                    if (random.Next(100) > 90)
                    {
                        // using world center as the point to apply force to; this makes the point of the force application the center of the fixture
                        PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(0.0f, -80.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
                    }
                }

                if (Engine.Player.Position.X < Position.X)
                {
                    LastDirection = Global.Directions.Left;

                    if (!Crouching)
                    {
                        PhysicsComponent.MoveLeft();

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
                    else
                    {
                        PhysicsComponent.StopMoving();
                        Walking = false;
                    }
                }
                else if (Engine.Player.Position.X > Position.X)
                {
                    LastDirection = Global.Directions.Right;

                    if (!Crouching)
                    {
                        PhysicsComponent.MoveRight();

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
                }
                else
                {
                    PhysicsComponent.StopMoving();
                    Walking = false;
                }

                UpdateAllArt();
            }
            else
            {
                HideAllArt();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.SpriteBatch.End();
        }

        private void UpdateBloom()
        {
            if (Health == 100)
            {
            }
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

            bool firePressed = random.Next(100) > 95;

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
                    float bulletX = 0.0f;
                    float bulletY = 0.0f;
                    float bulletAngle = 0.0f;

                    // determine bullet position and direction
                    switch (LastDirection)
                    {
                        case Global.Directions.Left:
                            bulletPosition = Position + new Vector2(-40.0f, 9.0f);
                            bulletX = -16.0f;
                            break;
                        case Global.Directions.Right:
                            bulletPosition = Position + new Vector2(40.0f, 9.0f);
                            bulletX = 16.0f;
                            break;
                        default:
                            bulletPosition = Position;
                            break;
                    }

                    // determine bullet vertical movement and angle
                    if (Engine.Player.Position.Y > Position.Y + 20)
                    {
                        bulletY = -13.0f;
                        bulletAngle = 90.0f;
                    }

                    if (Crouching)
                    {
                        bulletPosition.Y += 40.0f;
                    }

                    Bullet bullet = new Bullet(Engine, bulletPosition, new Vector2(bulletX, bulletY), bulletAngle, Global.CollisionCategories.EnemyBullet);
                    bullets.Add(bullet);
                    bulletTimer = 0.0f;

                    Cue lazer = Engine.Audio.GetCue("LazerShot");
                    lazer.Play();
                }
                else
                {
                    releasedFire = true;
                }
            }
        }

        private void ChangeArtDirectionLeft()
        {
            playerEnemyArmShootingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerEnemyStandingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerEnemyCrouchingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerEnemyJumpingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerEnemyFallingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
            playerEnemyWalkingRight.SpriteEffect = SpriteEffects.FlipHorizontally;
        }

        private void ChangeArtDirectionRight()
        {
            playerEnemyArmShootingRight.SpriteEffect = SpriteEffects.None;
            playerEnemyStandingRight.SpriteEffect = SpriteEffects.None;
            playerEnemyCrouchingRight.SpriteEffect = SpriteEffects.None;
            playerEnemyJumpingRight.SpriteEffect = SpriteEffects.None;
            playerEnemyFallingRight.SpriteEffect = SpriteEffects.None;
            playerEnemyWalkingRight.SpriteEffect = SpriteEffects.None;
        }

        private void HideAllArt()
        {
            playerEnemyArmShootingRight.Visible = false;
            playerEnemyStandingRight.Visible = false;
            playerEnemyCrouchingRight.Visible = false;
            playerEnemyJumpingRight.Visible = false;
            playerEnemyFallingRight.Visible = false;
            playerEnemyWalkingRight.Stop();
        }

        private void UpdateAllArt()
        {
            // update art positions
            Vector2 artOffsetVector = new Vector2(0.0f, 25.0f);
            playerEnemyStandingRight.Position = Position + artOffsetVector;
            playerEnemyCrouchingRight.Position = Position + artOffsetVector;
            playerEnemyJumpingRight.Position = Position + artOffsetVector;
            playerEnemyFallingRight.Position = Position + artOffsetVector;
            playerEnemyWalkingRight.Position = Position + artOffsetVector;
            
            if (Crouching)
            {
                playerEnemyArmShootingRight.Position = Position + artOffsetVector + new Vector2(0.0f, 35.0f);
            }
            else
            {
                playerEnemyArmShootingRight.Position = Position + artOffsetVector;
            }

            // arm angle
            if (Engine.Input.IsButtonDown(Global.Configuration.GetButtonConfig("GameControls", "UpButton")) || Engine.Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "UpKey")))
            {
                if (LastDirection == Global.Directions.Right)
                {
                    playerEnemyArmShootingRight.Angle = -MathHelper.PiOver4;

                    // offset to right to account for angle
                    playerEnemyArmShootingRight.Position += new Vector2(5.0f, 0.0f);
                }
                else
                {
                    playerEnemyArmShootingRight.Angle = MathHelper.PiOver4;

                    playerEnemyArmShootingRight.Position += new Vector2(-5.0f, 0.0f);
                }
            }
            else
            {
                playerEnemyArmShootingRight.Angle = 0.0f;
            }

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
                if (!playerEnemyWalkingRight.Playing)
                {
                    HideAllArt();
                    playerEnemyWalkingRight.Play();
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
                        playerEnemyCrouchingRight.Visible = true;
                    }
                    else
                    {
                        playerEnemyStandingRight.Visible = true;
                    }
                }
                else
                {
                    // check physics object to see if we're going up or down
                    if (PhysicsComponent.MainFixture.Body.LinearVelocity.Y > 0.0f)
                    {
                        // going down
                        playerEnemyFallingRight.Visible = true;
                    }
                    else
                    {
                        // going up
                        playerEnemyJumpingRight.Visible = true;
                    }
                }
            }

            if (bulletTimer < 250.0d)
            {
                playerEnemyArmShootingRight.Visible = true;
            }
            else
            {
                playerEnemyArmShootingRight.Visible = false;
            }
        }

        private void ResetPlayerEnemy()
        {
            PhysicsComponent.ResetPlayerPosition(Engine.Level.GetSpawnPoint());
            groundCache.Clear();
            wallCache.Clear();
            OnGround = false;
            OnWall = false;
            PhysicsComponent.MainFixture.OnCollision += PlayerEnemyOnCollision;
            PhysicsComponent.WheelFixture.OnCollision += PlayerEnemyOnCollision;
            PhysicsComponent.WheelFixture.OnSeparation += PlayerEnemyOnSeparation;
        }

        private bool PlayerEnemyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (!((fixtureB.CollisionFilter.CollisionCategories & (Category)Global.CollisionCategories.Light) == (Category)Global.CollisionCategories.Light)
                && !((fixtureB.CollisionFilter.CollisionCategories & (Category)Global.CollisionCategories.Enemy) == (Category)Global.CollisionCategories.Enemy))
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
            }

            if ((fixtureB.CollisionFilter.CollisionCategories & (Category)Global.CollisionCategories.PlayerBullet) == (Category)Global.CollisionCategories.PlayerBullet)
            {
                // decrement enemy health
                Health -= 50;
            }

            return true;
        }

        private void PlayerEnemyOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (!((fixtureB.CollisionFilter.CollisionCategories & (Category)Global.CollisionCategories.Light) == (Category)Global.CollisionCategories.Light))
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
}
