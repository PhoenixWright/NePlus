using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IceCream.Components;
using IceCream.Attributes;
using Microsoft.Xna.Framework;
using IceCream;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.Components
{
    [IceComponentAttribute("PhysicsComponent")]
    public class PhysicsComponent : IceComponent
    {

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

    }
}
