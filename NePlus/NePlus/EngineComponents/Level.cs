using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using TiledLib;

using NePlus.GameObjects;

namespace NePlus.EngineComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Level : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string mapFilePath;

        Map map;

        public List<Light> Lights { get; private set; }

        /// <summary>
        /// Loads and manages maps.
        /// </summary>
        /// <param name="game">Game reference.</param>
        /// <param name="mapPath">The relative path to the map.</param>
        public Level(Game game, string mapPath) : base(game)
        {
            mapFilePath = mapPath;
            Lights = new List<Light>();

            Game.Components.Add(this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LoadContent();

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            map = Game.Content.Load<Map>(mapFilePath);
            
            // loop through the collision objects and create physics fixtures for them
            MapObjectLayer collisionLayer = map.GetLayer("CollisionObjects") as MapObjectLayer;            
            foreach (MapObject collisionObject in collisionLayer.Objects)
            {
                // I don't fully understand why using Vector2.Zero works here. It must have something to do with the Bounds already containing the position information
                CreateCollisionRectangle(collisionObject.Bounds, Engine.Physics.PositionToPhysicsWorld(Vector2.Zero));
            }

            // loop through the collision tiles and create physics fixtures for them
            TileLayer tileLayer = map.GetLayer("CollisionTiles") as TileLayer;
            for (int y = 0; y < tileLayer.Height; ++y)
            {
               for (int x = 0; x < tileLayer.Width; ++x)
               {
                    if (tileLayer.Tiles[x, y] != null)
                    {
                        CreateCollisionRectangle(tileLayer.Tiles[x, y].Source, Engine.Physics.PositionToPhysicsWorld(new Vector2(x * map.TileWidth, y * map.TileHeight)));
                    }
                }
            }

            // loop through the light objects and create lights in the game world for them
            MapObjectLayer lightLayer = map.GetLayer("LightObjects") as MapObjectLayer;
            foreach (MapObject lightObject in lightLayer.Objects)
            {
                CreateLight(lightObject);
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Engine.Camera.CameraMatrix);
            map.Draw(Engine.Video.SpriteBatch);
            Engine.Video.SpriteBatch.End();

            base.Draw(gameTime);
        }


        public void CreateCollisionRectangle(Rectangle rectangle, Vector2 position)
        {
            Vertices vertices = new Vertices();
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Top)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Top)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Bottom)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Bottom)));

            Fixture fixture = FixtureFactory.CreatePolygon(Engine.Physics.World, vertices, 1.0f);
            fixture.Body.Position = position;
            fixture.Body.BodyType = BodyType.Static;
            fixture.Restitution = 0.0f;
            // TODO: add collision categories to game
        }

        public void CreateLight(MapObject lightObject)
        {
            Property lightType;
            if (lightObject.Properties.TryGetValue("LightType", out lightType) == false)
            {
                throw new Exception("Failed to retrieve light type from " + lightObject.Name + " in map " + mapFilePath);
            }

            switch (lightType.RawValue)
            {
                case "Gravity":
                    // get gravity value
                    Property gravityValueProperty;
                    if (lightObject.Properties.TryGetValue("GravityValue", out gravityValueProperty) == false)
                    {
                        throw new Exception("Failed to retrieve gravity value from " + lightObject.Name + " in map " + mapFilePath);
                    }

                    float gravityValue = float.Parse(gravityValueProperty.RawValue);

                    Lights.Add(new GravityLight(new Vector2(lightObject.Bounds.X, lightObject.Bounds.Y), gravityValue));
                    break;
                default:
                    throw new Exception("Failed to instantiate light with type of " + lightObject.Name + " in map " + mapFilePath);
            }
        }

        public Vector2 GetSpawnPoint() // TODO: override this function to get the appropriate spawn point based on where the player died
        {
            MapObjectLayer spawnPointLayer = map.GetLayer("SpawnPointObjects") as MapObjectLayer;
            foreach (MapObject spawnPoint in spawnPointLayer.Objects)
            {
                if (spawnPoint.Name == "InitialSpawnPoint")
                {
                    return new Vector2(spawnPoint.Bounds.X, spawnPoint.Bounds.Y);
                }
            }

            throw new Exception("No InitialSpawnPoint defined in " + mapFilePath);
        }
    }
}
