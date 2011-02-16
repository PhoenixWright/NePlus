using Microsoft.Xna.Framework;

using NePlus.Components.EngineComponents;
using NePlus.ScreenManagement;

namespace NePlus
{
    public class Component
    {
        public Engine Engine;

        // this is the component's draw order
        int drawOrder;

        public Component(Engine engine)
        {
            Engine = engine;
            drawOrder = 0;
        }

        public int DrawOrder
        {
            get { return drawOrder; }
            set
            {
                this.drawOrder = value;

                Engine.PutComponentInOrder(this);
            }
        }

        public virtual void Initialize() { }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }

        public virtual void Dispose(bool disposing)
        {
            Engine.RemoveComponent(this);
        }
    }
}
