using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NePlus.EngineComponents;

namespace NePlus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.GameComponent
    {
        public Camera Camera { get; private set; }
        public Input Input { get; private set; }
        public Physics Physics { get; private set; }
        public Video Video { get; private set; }

        public Engine(Game game)
            : base(game)
        {
            Video = new Video(game);
            
            Camera = new Camera(new Vector2(Video.Width, Video.Height));
            Input = new Input();
            Physics = new Physics(game);
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {


            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Input.Update();
            Camera.Update(Input);
            Physics.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
