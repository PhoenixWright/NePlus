using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using TiledLib;

namespace NePlus.EngineComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Level : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string mapFilePath;

        Map map;

        List<MapObject> spawnPoints;

        /// <summary>
        /// Loads and manages maps.
        /// </summary>
        /// <param name="game">Game reference.</param>
        /// <param name="mapPath">The relative path to the map.</param>
        public Level(Game game) : base(game)
        {
            spawnPoints = new List<MapObject>();

            Game.Components.Add(this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
        
        public void LoadContent(string mapPath)
        {
            mapFilePath = mapPath;

            map = Game.Content.Load<Map>(mapPath);
            
            // loop through the tile layer and create physics fixtures for the collideable layer of the map
            TileLayer tileLayer = map.GetLayer("Collision") as TileLayer;
            for (int y = 0; y < tileLayer.Height; ++y)
            {
               for (int x = 0; x < tileLayer.Width; ++x)
               {
                    if (tileLayer.Tiles[x, y] != null)
                    {
                        Rectangle rectangle = tileLayer.Tiles[x, y].Source;

                        Vertices vertices = new Vertices();
                        vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Top)));
                        vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Top)));
                        vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Bottom)));
                        vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Bottom)));

                        Fixture fixture = FixtureFactory.CreatePolygon(Engine.Physics.World, vertices, 1.0f);
                        fixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(new Vector2(x * map.TileWidth, y * map.TileHeight));
                        fixture.Body.BodyType = BodyType.Static;
                        fixture.Restitution = 0.0f;                        
                    }
                }
            }

            foreach (MapObject mapObject in map.GetAllObjects())
            {
                if (mapObject.Type == "SpawnPoint")
                {
                    spawnPoints.Add(mapObject);
                }
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

        public Vector2 GetSpawnPoint()
        {
            foreach (MapObject spawn in spawnPoints)
            {
                if (spawn.Name == "InitialSpawnPoint")
                {
                    return new Vector2(spawn.Bounds.X, spawn.Bounds.Y);
                }
            }

            throw new Exception("No InitialSpawnPoint defined in " + mapFilePath);
        }
    }
}
