using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceCream.Components;
using IceCream.Attributes;
using Microsoft.Xna.Framework;
using IceCream;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlus.Components
{
    [IceComponentAttribute("PhysicsComponent")]
    public class PhysicsComponent : IceComponent
    {
        protected float width;
        protected float height;
        public Body body;
        public Geom geom; 
        

        public PhysicsComponent()
        {

        }

        public override void OnRegister()
        {
            Enabled = true;
        }

        public override void Update(float elapsedTime)
        {

        }

        protected virtual void InitializePhysics(World physics, Vector2 position, float width, float height, float mass)
        {
            
        }
    }
}
