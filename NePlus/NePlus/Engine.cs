using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NePlus.Components.EngineComponents;
using NePlus.Krypton;
using NePlus.Krypton.Lights;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus
{
    public class Engine
    {
        List<Component> components;

        public ContentManager Content { get; private set; }

        public Audio Audio { get; private set; }
        public Camera2D Camera { get; private set; }
        public InputState Input { get; private set; }
        public Lighting Lighting { get; private set; }
        public Physics Physics { get; private set; }
        public Video Video { get; private set; }

        public SpriteBatch SpriteBatch { get; private set; }

        // test stuff
        //Texture2D background;
        public KryptonEngine krypton;
        PointLight light = new PointLight();
        ShadowHull shadowHull = ShadowHull.CreateRectangle(Vector2.One);

        public Engine(ContentManager content)
        {
            components = new List<Component>();

            Content = content;

            Audio = new Audio(this);
            Input = new InputState();
            Physics = new Physics(this);
            Video = new Video(this);

            // these things require video
            Camera = new Camera2D(this);
            
            // lighting needs to know the camera matrix
            //Lighting = new Lighting(this);

            SpriteBatch = new SpriteBatch(Global.GraphicsDeviceManager.GraphicsDevice);

            // test stuff
            krypton = new KryptonEngine(this, @"Lighting\KryptonEffect");
            krypton.AmbientColor = new Color(65, 65, 65);
            krypton.Matrix = Camera.CameraMatrix;
            krypton.SpriteBatchCompatablityEnabled = true;
            //krypton.Matrix = Matrix.CreateOrthographic(Video.GraphicsDevice.Viewport.Width / 10, Video.GraphicsDevice.Viewport.Height / 10, 0, 1);
            krypton.Initialize();
            krypton.LoadContent();
            light.Range = 40;
            light.Texture = LightTextureBuilder.CreatePointLight(Video.GraphicsDevice, 512);
            light.Position = new Vector2(0, 0);
            light.Color = Color.White;
            krypton.Lights.Add(light);
            shadowHull.Position = new Vector2(10, 10);
            krypton.Hulls.Add(shadowHull);
        }

        public void LoadContent(Game game)
        {
            foreach (Component c in components)
                c.LoadContent();
        }

        public void UnloadContent()
        {
            foreach (Component c in components)
                c.UnloadContent();
        }

        public void Pause()
        {
            Audio.Pause();
        }

        public void Update(GameTime gameTime)
        {
            krypton.Matrix = Camera.CameraMatrix;
            Input.Update(gameTime);
            Physics.Update(gameTime);

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraUpKey")))
            {
                Camera.Position += new Vector2(0.0f, -10.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraDownKey")))
            {
                Camera.Position += new Vector2(0.0f, 10.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraLeftKey")))
            {
                Camera.Position += new Vector2(-10.0f, 0.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "CameraRightKey")))
            {
                Camera.Position += new Vector2(10.0f, 0.0f);
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "ZoomInKey")))
            {
                Camera.Zoom += 0.01f;
            }

            if (Input.IsKeyDown(Global.Configuration.GetKeyConfig("GameControls", "ZoomOutKey")))
            {
                Camera.Zoom -= 0.01f;
            }

            // test stuff
            krypton.Update(gameTime);

            foreach (Component c in components)
                c.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            // putting this here because it seems to blank out stuff if I don't
            //Lighting.KryptonEngine.LightMapPrepare();

            // test stuff
            Video.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            krypton.LightMapPrepare();

            Video.GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.CameraMatrix);
            //SpriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            SpriteBatch.End();

            foreach (Component c in components)
                c.Draw(gameTime);

            // test stuff
            krypton.Draw(gameTime);

            //Lighting.DebugDraw();
            Physics.Draw(gameTime);
        }

        public void AddComponent(Component Component)
        {
            if (!components.Contains(Component))
            {
                components.Add(Component);
                Component.Initialize();
                Component.LoadContent();
                PutComponentInOrder(Component);
            }
        }

        // The components are stored in their draw order, so it is easy to loop 
        // through them and draw them in the correct order without having to sort
        // them every time they are drawn
        public void PutComponentInOrder(Component component)
        {
            if (components.Contains(component))
            {
                components.Remove(component);

                int i = 0;

                // Iterate through the components in order until we find one with
                // a higher or equal draw order, and insert the component at that
                // position.
                for (i = 0; i < components.Count; i++)
                    if (components[i].DrawOrder >= component.DrawOrder)
                        break;

                components.Insert(i, component);
            }
        }

        public void RemoveComponent(Component Component)
        {
            if (Component != null && components.Contains(Component))
            {
                components.Remove(Component);
            }
        }
    }
}
