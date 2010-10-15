using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using IceCream;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace NePlus
{

    public class Game : IceCream.Game
    {
        IceScene scene;

        public World PhysicsSimulator { get; private set; }

        public Game()
        {
            PhysicsSimulator = new World(Vector2.UnitY * 500);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            scene = SceneManager.LoadScene("Content/TestScene.icescene");
        }

        protected override void Update(GameTime gameTime)
        {
            PhysicsSimulator.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }    
    }
}
