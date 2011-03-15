using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlus.Components.EngineComponents;

namespace NePlus
{
    public static class Global
    {
        public enum Animations
        {
            PlayOnce,
            Repeat
        }

        public enum CollisionCategories
        {
            Player = Category.Cat1,
            PlayerBullet = Category.Cat2,
            Enemy = Category.Cat3,
            EnemyBullet = Category.Cat4,
            Light = Category.Cat5,
            Structure = Category.Cat6
        }

        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }

        public enum Layers
        {
            Background,
            Player,
            Enemies,
            Projectiles,
            Lighting,
            AboveLighting
        }

        public enum Shapes
        {
            Circle,
            Square
        }

        public static Configuration Configuration { get; private set; }
        public static Game Game { get; private set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        public static void Initialize(Game game, GraphicsDeviceManager gdm)
        {
            Configuration = new Configuration();
            Game = game;
            GraphicsDeviceManager = gdm;
        }
    }
}
