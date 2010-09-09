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

namespace NePlus
{

    public class NePlus : IceCream.Game
    {
        IceScene scene;
        public NePlus()
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            scene = SceneManager.LoadScene("Content/TestScene.icescene");
        }

    }
}
