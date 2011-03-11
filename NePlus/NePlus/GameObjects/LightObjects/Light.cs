using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus;
using NePlus.Components.EngineComponents;
using NePlus.Components.GameComponents;
using NePlus.Components.GraphicsComponents;
using NePlus.Components.PhysicsComponents;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.GameObjects.LightObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    abstract public class Light : Component
    {
        // configuration for the light
        public bool DrawLight { get; set; }
        public bool EffectActive { get; set; }

        public float Angle { get; protected set; }
        public float Fov { get; protected set; }
        public Vector2 Position { get; protected set; }
        public float Range { get; protected set; }

        public bool IsOn { get; set; }

        public LightComponent LightingComponent;
        public PhysicsComponent PhysicsComponent;
        public SensorPhysicsComponent SensorPhysicsComponent;

        // this list is the list of fixtures currently under the light
        protected List<Fixture> AffectedFixtures { get; private set; }
        protected List<Light> AffectedLights { get; private set; }

        public Light(Engine engine, Vector2 position, float fov, float angle, float range, Color color, string motion)
            : base(engine)
        {
            DrawLight = true;
            EffectActive = true;

            Angle = angle;
            Fov = fov;
            IsOn = true;
            Position = position;
            Range = range;

            CreateLightComponent(position, fov, angle, range, color);
            CreatePhysicsComponent(motion);
            CreateSensorPhysicsComponent(GetFarseerVertices(), Position);

            AffectedFixtures = new List<Fixture>();
            AffectedLights = new List<Light>();

            Engine.AddComponent(this);
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

            LightingComponent.Light.Angle = Angle;
            LightingComponent.Light.IsOn = IsOn;
            LightingComponent.Light.Position = Position;
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void CreateLightComponent(Vector2 position, float fov, float angle, float range, Color color)
        {
            LightingComponent = new LightComponent(Engine, position, fov, angle, range, color);
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

            return true;
        }

        public void AfterFixtureCollision(Fixture a, Fixture b)
        {
            AffectedFixtures.Remove(b);
        }
    }
}
