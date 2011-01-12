using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using NePlus.Components.EngineComponents;

namespace NePlus
{
    public class Engine
    {
        // game reference
        public Game Game { get; private set; }
        // game time
        public GameTime GameTime { get; private set; }

        // content reference
        public ContentManager Content { get; private set; }

        public Audio Audio { get; private set; }
        public Camera Camera { get; private set; }
        public Configuration Configuration { get; private set; }
        public Input Input { get; private set; }
        public Physics Physics { get; private set; }
        public Video Video { get; private set; }

        // components
        List<Component> components;

        public Engine(Game game, GraphicsDeviceManager gdm)
        {
            components = new List<Component>();

            Game = game;

            Content = game.Content;
            Content.RootDirectory = "Content";

            Audio = new Audio(this);
            Video = new Video(gdm);
            Camera = new Camera(new Vector2(Video.Width, Video.Height));
            Configuration = new Configuration();
            Input = new Input();
            Physics = new Physics(this);
        }

        public void LoadContent(Game game)
        {
            Video.LoadContent(game);
        }

        public void Update(GameTime gameTime)
        {            
            GameTime = gameTime;

            Audio.Update();
            Input.Update();
            Camera.Update();
            Physics.Update();

            // Copy the list of components so the game won't crash if the original
            // is modified while updating
            List<Component> updating = new List<Component>();

            foreach (Component c in components)
                updating.Add(c);

            foreach (Component c in updating)
                c.Update();
        }

        public void Draw()
        {
            foreach (Component c in components)
                c.Draw();

            Physics.Draw();
        }

        public void AddComponent(Component Component)
        {
            if (!components.Contains(Component))
            {
                components.Add(Component);
                Component.LoadComponent();
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
