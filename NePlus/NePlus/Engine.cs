using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NePlus.Components.EngineComponents;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus
{
    public class Engine
    {
        List<Component> components;

        public ContentManager Content { get; private set; }

        public Audio Audio { get; private set; }
        public Camera Camera { get; private set; }
        public InputState Input { get; private set; }
        public Lighting Lighting { get; private set; }
        public Physics Physics { get; private set; }
        public Video Video { get; private set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public Engine(ContentManager content)
        {
            components = new List<Component>();

            Content = content;

            Audio = new Audio(this);
            Camera = new Camera(this, new Vector2(Global.Configuration.GetIntConfig("Video", "Width"), Global.Configuration.GetIntConfig("Video", "Height")));
            Input = new InputState();            
            Physics = new Physics(this);
            Video = new Video(this);

            // lighting requires video
            Lighting = new Lighting(this);
            
            SpriteBatch = new SpriteBatch(Global.GraphicsDeviceManager.GraphicsDevice);
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
            Input.Update(gameTime);
            Physics.Update(gameTime);

            foreach (Component c in components)
                c.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Video.GraphicsDevice.Clear(Color.Black);

            foreach (Component c in components)
                c.Draw(gameTime);

            Physics.Draw(gameTime);
            Lighting.DebugDraw();
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
