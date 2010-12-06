using NePlusEngine.EngineComponents;

namespace NePlusEngine
{
    public class Component
    {
        public Engine Engine;

        // this is used to tell if the component has been loaded
        bool loaded;

        // this is the component's draw order
        int drawOrder;

        public Component(Engine engine)
        {
            Engine = engine;
            loaded = false;
            drawOrder = 0;
        }

        public void LoadComponent()
        {
            if (!loaded)
                Load();

            loaded = true;
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

        protected virtual void Load()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }
    }
}
