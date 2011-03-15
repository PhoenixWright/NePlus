using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus.Components.PhysicsComponents;

namespace NePlus.GameObjects.LightObjects
{
    public abstract class EffectLight : Light
    {
        public bool EffectActive { get; set; }

        // this list is the list of fixtures currently under the light
        protected List<Fixture> AffectedFixtures { get; private set; }
        protected List<EffectLight> AffectedLights { get; private set; }

        public PhysicsComponent PhysicsComponent;
        public SensorPhysicsComponent SensorPhysicsComponent;

        public EffectLight(Engine engine)
            : base(engine)
        {
            EffectActive = true;

            AffectedFixtures = new List<Fixture>();
            AffectedLights = new List<EffectLight>();

            Engine.AddComponent(this);
        }

        public void Activate(string motion)
        {
            CreatePhysicsComponent(motion);
            CreateSensorPhysicsComponent(GetFarseerVertices(), Position);
        }

        public override void Update(GameTime gameTime)
        {
            if (PhysicsComponent != null)
            {
                Angle = PhysicsComponent.Angle + MathHelper.TwoPi / 4;
                Position = PhysicsComponent.Position;
            }

            if (SensorPhysicsComponent != null)
            {
                SensorPhysicsComponent.SensorFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(Position);
                SensorPhysicsComponent.SensorFixture.Body.Rotation = Angle - MathHelper.TwoPi / 4;
            }

            base.Update(gameTime);
        }

        private void CreatePhysicsComponent(string motionType)
        {
            switch (motionType)
            {
                case "None":
                    break;
                case "Pendulum":
                    Point pivotPoint = new Point((int)Math.Round(Position.X), (int)Math.Round(Position.Y - 200));
                    Point weightPoint = new Point((int)Math.Round(Position.X + 200), (int)Math.Round(Position.Y));
                    PhysicsComponent = new PendulumPhysicsComponent(Engine, pivotPoint, weightPoint);
                    break;
                default:
                    throw new Exception("Physics component type not recognized");
            }
        }

        private void CreateSensorPhysicsComponent(List<Vector2> vertices, Vector2 position)
        {
            SensorPhysicsComponent = new SensorPhysicsComponent(Engine, vertices, position);
            SensorPhysicsComponent.SensorFixture.OnCollision += OnFixtureCollision;
            SensorPhysicsComponent.SensorFixture.OnSeparation += AfterFixtureCollision;
        }

        public List<Vector2> GetFarseerVertices()
        {
            // create a list of vectors
            List<Vector2> vertices = new List<Vector2>();

            Vector2 a = Vector2.Zero;

            // the second vector is the first endpoint, which should take into account angle and range, where angle takes into account where the light is aimed
            Vector2 b = Engine.Physics.PositionToPhysicsWorld(new Vector2(Range / 1.5f, Range / 1.5f));
            b = XnaHelper.RotateVector2(b, Angle - MathHelper.PiOver2 + 0.17f, a);

            // the third vector is the second endpoint, which should take into account angle, range, and the light's "fov", or the light's interior angle
            Vector2 c = XnaHelper.RotateVector2(b, Fov, Vector2.Zero);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            return vertices;
        }

        public List<Vector2> GetVertices()
        {
            // create a list of vectors
            List<Vector2> vertices = new List<Vector2>();

            // the first vector is the light's position
            Vector2 a = Position;

            // the second vector is the first endpoint, which should take into account angle and range, where angle takes into account where the light is aimed
            Vector2 b = Position + new Vector2(Range / 1.5f, Range / 1.5f);
            b = XnaHelper.RotateVector2(b, Angle - MathHelper.PiOver2 + 0.17f, a);

            // the third vector is the second endpoint, which should take into account angle, range, and the light's "fov", or the light's interior angle
            Vector2 c = XnaHelper.RotateVector2(b, Fov, a);

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);

            return vertices;
        }

        public bool PositionInLight(Vector2 position)
        {
            // if the angle isn't zero, then it's a cone-shaped light TODO: this isn't actually true
            if (Angle != 0)
            {
                return XnaHelper.IsPointInsideTriangle(GetVertices(), position);
            }

            return false;
        }

        public bool OnFixtureCollision(Fixture a, Fixture b, Contact c)
        {
            AffectedFixtures.Add(b);
            OnLightEntry(b);

            return true;
        }

        public void AfterFixtureCollision(Fixture a, Fixture b)
        {
            AffectedFixtures.Remove(b);
            OnLightExit(b);
        }

        protected virtual void OnLightEntry(Fixture fixture)
        {
        }

        protected virtual void OnLightExit(Fixture fixture)
        {
        }
    }
}
