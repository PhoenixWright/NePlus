using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Krypton.Lights;

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
    public class Light : Component
    {
        Light2D light;

        public float Angle
        {
            get
            {
                return light.Angle;
            }
            set
            {
                light.Angle = value;
            }
        }
        public Color Color
        {
            get
            {
                return light.Color;
            }
            set
            {
                light.Color = value;
            }
        }
        public float Fov
        {
            get
            {
                return light.Fov;
            }
            set
            {
                light.Fov = value;
            }
        }
        public float Intensity
        {
            get
            {
                return light.Intensity;
            }
            set
            {
                light.Intensity = value;
            }
        }
        public bool IsOn
        {
            get
            {
                return light.IsOn;
            }
            set
            {
                light.IsOn = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return light.Position;
            }
            set
            {
                light.Position = value;
            }
        }
        public float Range
        {
            get
            {
                return light.Range;
            }
            set
            {
                light.Range = value;
            }
        }

        public Light(Engine engine)
            : base(engine)
        {
            light = new Light2D();

            light.Texture = Engine.Lighting.PointLightTexture;

            Engine.Lighting.Krypton.Lights.Add(light);

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Dispose(bool disposing)
        {
            Engine.Lighting.Krypton.Lights.Remove(light);
            light = null;

            base.Dispose(disposing);
        }
    }
}
