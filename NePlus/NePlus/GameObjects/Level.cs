using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using TiledLib;

using NePlus.GameObjects;
using NePlus.GameObjects.LightObjects;

using NePlus;
using NePlus.Components.EngineComponents;
using NePlus.Components.GraphicsComponents;
using NePlus.Krypton;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.GameObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Level : Component
    {
        public List<Light> Lights { get; private set; }
        
        private string mapFilePath;
        private Map map;
        private List<ParticleEffectComponent> levelParticleEffects;

        /// <summary>
        /// Loads and manages maps.
        /// </summary>
        /// <param name="game">Game reference.</param>
        /// <param name="mapPath">The relative path to the map.</param>
        public Level(Engine engine, string mapPath) : base(engine)
        {
            mapFilePath = mapPath;
            Lights = new List<Light>();
            levelParticleEffects = new List<ParticleEffectComponent>();
        }

        public override void LoadContent()
        {
            map = Engine.Content.Load<Map>(mapFilePath);
            
            // loop through the map properties and handle them
            foreach (Property property in map.Properties)
            {
                switch (property.Name)
                {
                    case "BackgroundTrack":
                        Engine.Audio.PlaySound("Rain");
                        break;
                    case "ParticleEffect":
                        CreateParticleEffect(property.RawValue);
                        break;
                    default:
                        throw new Exception("Map property " + property.Name + " not recognized in map file " + mapFilePath);
                }
            }

            // loop through the collision objects and create physics fixtures for them
            MapObjectLayer collisionLayer = map.GetLayer("CollisionObjects") as MapObjectLayer;            
            foreach (MapObject collisionObject in collisionLayer.Objects)
            {
                // I don't fully understand why using Vector2.Zero works here. It must have something to do with the Bounds already containing the position information
                CreateCollisionRectangle(collisionObject.Bounds, Engine.Physics.PositionToPhysicsWorld(Vector2.Zero));

                // create the shadow hull for the platform
                CreateRectangleShadowHull(new Vector2(collisionObject.Bounds.Center.X, collisionObject.Bounds.Center.Y), collisionObject.Bounds);
            }

            // loop through the collision tiles and create physics fixtures for them
            TileLayer tileLayer = map.GetLayer("CollisionTiles") as TileLayer;
            if (Global.Configuration.GetBooleanConfig("Debug", "ShowDebugCollisionTiles"))
            {
                tileLayer.Opacity = 100;
            }
            else
            {
                tileLayer.Opacity = 0;
            }
            for (int y = 0; y < tileLayer.Height; ++y)
            {
               for (int x = 0; x < tileLayer.Width; ++x)
               {
                    if (tileLayer.Tiles[x, y] != null)
                    {
                        // create the physics object for the tile
                        CreateCollisionRectangle(tileLayer.Tiles[x, y].Source, Engine.Physics.PositionToPhysicsWorld(new Vector2(x * map.TileWidth, y * map.TileHeight)));

                        // create the shadow for the tile new Vector2(x * map.TileWidth, y * map.TileHeight)
                        CreateRectangleShadowHull(new Vector2(x * map.TileWidth + tileLayer.Tiles[x, y].Source.Width / 2,
                                                              y * map.TileHeight + tileLayer.Tiles[x, y].Source.Height / 2),
                                                  tileLayer.Tiles[x, y].Source);
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

        public override void Draw(GameTime gameTime)
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            map.Draw(Engine.SpriteBatch);
            Engine.SpriteBatch.End();
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
            Vector2 position = new Vector2(lightObject.Bounds.Center.X, lightObject.Bounds.Center.Y);

            Property lightAngle;
            if (lightObject.Properties.TryGetValue("LightAngle", out lightAngle) == false)
            {
                lightAngle = new Property("LightAngle", "3.14");
            }
            float lightAngleValue = float.Parse(lightAngle.RawValue);

            Property lightColorString;
            if (lightObject.Properties.TryGetValue("LightColor", out lightColorString) == false)
            {
                throw new Exception("Failed to retrieve light color from " + lightObject.Name + " in map " + mapFilePath);
            }
            // use reflection to get a Color from a string
            PropertyInfo colorProperty = typeof(Color).GetProperty(lightColorString.RawValue);
            Color lightColor = (Color)colorProperty.GetValue(null, null);

            Property lightFov;
            if (lightObject.Properties.TryGetValue("LightFov", out lightFov) == false)
            {
                lightFov = new Property("LightFov", "6.28");
            }
            float lightFovValue = float.Parse(lightFov.RawValue);

            Property lightMotion;
            if (lightObject.Properties.TryGetValue("LightMotion", out lightMotion) == false)
            {
                throw new Exception("Failed to retrieve light motion from " + lightObject.Name + " in map " + mapFilePath);
            }

            Property lightRange;
            if (lightObject.Properties.TryGetValue("LightRange", out lightRange) == false)
            {
                throw new Exception("Failed to retrieve light range from " + lightObject.Name + " in map " + mapFilePath);
            }
            float lightRangeValue = float.Parse(lightRange.RawValue);

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

                    Lights.Add(new GravityLight(Engine, position, lightFovValue, lightAngleValue, lightRangeValue, lightColor, lightMotion.RawValue, gravityValue));
                    break;
                    
                case "Null":
                    Lights.Add(new NullLight(Engine, position, lightFovValue, lightAngleValue, lightRangeValue, lightColor, lightMotion.RawValue, Lights));
                    break;

                default:
                    throw new Exception("Failed to instantiate light with type of " + lightObject.Name + " in map " + mapFilePath);
            }
        }

        public void CreateParticleEffect(string particleEffectName)
        {
            levelParticleEffects.Add(new ParticleEffectComponent(Engine, particleEffectName, new Vector2(Engine.Video.GraphicsDevice.Viewport.Width / 2, 0)));
        }

        public void CreatePlatform(Vector2 position, Rectangle rectangle, bool castsShadow)
        {
            // create the shadow hull for the platform
            CreateRectangleShadowHull(position, rectangle);
        }

        public void CreateRectangleShadowHull(Vector2 position, Rectangle rectangle)
        {
            ShadowHull shadowHull = ShadowHull.CreateRectangle(new Vector2(rectangle.Width, rectangle.Height));
            shadowHull.Position = position;

            Engine.Lighting.Krypton.Hulls.Add(shadowHull);
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