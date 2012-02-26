﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ECSFramework;

using Vaerydian.Components;
using Vaerydian.Components.Debug;
using Vaerydian.Behaviors;
using Vaerydian.Utils;

using WorldGeneration.Cave;
using WorldGeneration.World;
using WorldGeneration.Utils;

using DeenGames.Utils.AStarPathFinder;


namespace Vaerydian.Factories
{
    class EntityFactory
    {
        private ECSInstance e_EcsInstance;
        private static GameContainer e_Container;
        private Random rand = new Random();

        public EntityFactory(ECSInstance ecsInstance, GameContainer container)
        {
            e_EcsInstance = ecsInstance;
            e_Container = container;
        }

        public EntityFactory(ECSInstance ecsInstance) 
        {
            e_EcsInstance = ecsInstance;
        }

        public void createPlayer()
        {
            Entity e = e_EcsInstance.create();
            e_EcsInstance.EntityManager.addComponent(e, new Position(new Vector2(576f, 360f),new Vector2(12.5f)));
            //e_EcsInstance.EntityManager.addComponent(e, new Position(new Vector2(0, 0), new Vector2(12.5f)));
            e_EcsInstance.EntityManager.addComponent(e, new Velocity(4f));
            e_EcsInstance.EntityManager.addComponent(e, new Controllable());
            e_EcsInstance.EntityManager.addComponent(e, new Sprite("characters\\lord_lard_sheet", "characters\\normals\\lord_lard_sheet_normals",32,32,0,0));
            e_EcsInstance.EntityManager.addComponent(e, new CameraFocus(100));
            e_EcsInstance.EntityManager.addComponent(e, new MapCollidable());
            e_EcsInstance.EntityManager.addComponent(e, new Heading());
            e_EcsInstance.EntityManager.addComponent(e, createLight(false, 200, new Vector3(576, 360, 80), 0.7f, new Vector4(1f, 1f, 1f, 1f)));
            e_EcsInstance.EntityManager.addComponent(e, new Transform());

            e_EcsInstance.TagManager.tagEntity("PLAYER", e);

            e_EcsInstance.refresh(e);

        }

        public void createCamera()
        {
            Entity e = e_EcsInstance.create();
            e_EcsInstance.EntityManager.addComponent(e, new Velocity(5f));
            //e_EcsInstance.EntityManager.addComponent(e, new ViewPort(new Vector2(576, 360f), new Vector2(1152, 720)));
            e_EcsInstance.EntityManager.addComponent(e, new ViewPort(new Vector2(0, 0), new Vector2(768, 480)));

            e_EcsInstance.TagManager.tagEntity("CAMERA", e);

            e_EcsInstance.refresh(e);
           
        }

        public void createMousePointer()
        {
            Entity e = e_EcsInstance.create();
            e_EcsInstance.EntityManager.addComponent(e, new Position(new Vector2(0), new Vector2(24)));
            e_EcsInstance.EntityManager.addComponent(e, new Sprite("reticle","reticle_normal",48,48,0,0));
            e_EcsInstance.EntityManager.addComponent(e, new MousePosition());
            e_EcsInstance.EntityManager.addComponent(e, createLight(true, 150, new Vector3(576, 360, 100), 0.9f, new Vector4(1f, 1f, 1f, 1f)));
            e_EcsInstance.EntityManager.addComponent(e, new Transform());

            e_EcsInstance.TagManager.tagEntity("MOUSE", e);

            e_EcsInstance.refresh(e);
        }

        public void createFollower(Vector2 position, Entity target, float distance)
        {
            Entity e = e_EcsInstance.create(); 

            e_EcsInstance.EntityManager.addComponent(e, new Position(position, new Vector2(12.5f)));
            e_EcsInstance.EntityManager.addComponent(e, new Velocity(4f));
            e_EcsInstance.EntityManager.addComponent(e, new Sprite("characters\\herr_von_speck_sheet", "characters\\normals\\herr_von_speck_sheet_normals",32,32,0,0));
            e_EcsInstance.EntityManager.addComponent(e, new AiBehavior(new SimpleFollowBehavior(e, target, distance, e_EcsInstance)));
            e_EcsInstance.EntityManager.addComponent(e, new MapCollidable());
            e_EcsInstance.EntityManager.addComponent(e, new Heading());
            e_EcsInstance.EntityManager.addComponent(e, new Transform());

            Health health = new Health(50);
            health.RecoveryAmmount = 2;
            health.RecoveryRate = 500;
            e_EcsInstance.EntityManager.addComponent(e, health);

            Interactable interact = new Interactable();
            interact.Interactions.Add(InteractionTypes.PROJECTILE_COLLIDABLE);
            interact.Interactions.Add(InteractionTypes.DAMAGEABLE);
            e_EcsInstance.EntityManager.addComponent(e, interact);


            e_EcsInstance.refresh(e);

        }

        public void createCave()
        {
            Entity e = e_EcsInstance.create();
            CaveEngine ce = new CaveEngine();

            Random rand = new Random(41);

            ce.generateCave(100, 100, rand.Next(100), 4, 0.5);
            e_EcsInstance.EntityManager.addComponent(e, new GameMap(ce.getMap()));

            e_EcsInstance.TagManager.tagEntity("MAP", e);

            e_EcsInstance.refresh(e);
        }


        public void CreateTestMap()
        {
            Map map = new Map(100,100);

            Random rand = new Random();

            //byte[,] byteMap = new byte[100, 100];

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Terrain terrain = new Terrain();
                    if (rand.Next(100) > 90)
                    {
                        terrain.TerrainType = TerrainType.CAVE_WALL;
                        terrain.IsBlocking = true;
                        //byteMap[i, j] = PathFinderHelper.BLOCKED_TILE;
                    }
                    else
                    {
                        terrain.TerrainType = TerrainType.CAVE_FLOOR;
                        //byteMap[i, j] = PathFinderHelper.EMPTY_TILE;
                    }
                    map.Terrain[i, j] = terrain;
                }
            }

            GameMap gameMap = new GameMap(map);

            //gameMap.ByteMap = byteMap;

            Entity e = e_EcsInstance.create();
            e_EcsInstance.EntityManager.addComponent(e, gameMap);

            e_EcsInstance.TagManager.tagEntity("MAP", e);

            e_EcsInstance.refresh(e);
        }

        public void createMapDebug()
        {
            Entity e = e_EcsInstance.create();

            e_EcsInstance.EntityManager.addComponent(e, new MapDebug());

            e_EcsInstance.TagManager.tagEntity("MAP_DEBUG", e);

            e_EcsInstance.refresh(e);

        }


        public Entity createGeometryMap()
        {
            Entity e = e_EcsInstance.create();

            GeometryMap geometry = new GeometryMap();

            PresentationParameters pp = e_Container.SpriteBatch.GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            geometry.ColorMap = new RenderTarget2D(e_Container.SpriteBatch.GraphicsDevice, width, height);
            geometry.NormalMap = new RenderTarget2D(e_Container.SpriteBatch.GraphicsDevice, width, height);
            geometry.DepthMap = new RenderTarget2D(e_Container.SpriteBatch.GraphicsDevice, width, height);
            geometry.ShadingMap = new RenderTarget2D(e_Container.SpriteBatch.GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            geometry.AmbientColor = new Vector4(.1f, .1f, .1f, .1f);

            e_EcsInstance.EntityManager.addComponent(e, geometry);

            e_EcsInstance.TagManager.tagEntity("GEOMETRY", e);

            e_EcsInstance.refresh(e);

            return e;
        }

        public Light createLight(bool enabled, int radius, Vector3 position, float power, Vector4 color)
        {
            Light light = new Light();

            light.IsEnabled = enabled;
            light.LightRadius = radius;
            light.Position = position;
            light.ActualPower = power;
            light.Color = color;

            return light;
        }

        public void createStandaloneLight(bool enabled, int radius, Vector3 position, float power, Vector4 color)
        {
            Entity e = e_EcsInstance.create();

            e_EcsInstance.EntityManager.addComponent(e, createLight(enabled, radius, position, power, color));
            e_EcsInstance.EntityManager.addComponent(e, new Position(new Vector2(position.X, position.Y),Vector2.Zero));

            e_EcsInstance.refresh(e);
        }

        public void createRandomLight()
        {
            Entity e = e_EcsInstance.create();

            

            Vector3 pos = new Vector3(rand.Next(100*25), rand.Next(100*25), 50);

            //1152, 720
            e_EcsInstance.EntityManager.addComponent(e, createLight(true, 
                                                        rand.Next(100)+100,
                                                        pos, 
                                                        (float)rand.NextDouble()*.5f+0.5f,
                                                        new Vector4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble())));
            e_EcsInstance.EntityManager.addComponent(e, new Position(new Vector2(pos.X, pos.Y), Vector2.Zero));

            e_EcsInstance.refresh(e);

        }

        public void createCollidingProjectile(Vector2 start, Vector2 heading, float velocity, int duration, Light light, Transform transform, Entity originator)
        {
            Entity e = e_EcsInstance.create();

            e_EcsInstance.EntityManager.addComponent(e, new Position(start,new Vector2(16)));
            e_EcsInstance.EntityManager.addComponent(e, new Heading(heading));
            e_EcsInstance.EntityManager.addComponent(e, new Sprite("projectile", "projectile", 32, 32, 0, 0));
            e_EcsInstance.EntityManager.addComponent(e, new Velocity(velocity));
            
            Projectile projectile = new Projectile(duration);
            projectile.Originator = originator;
            e_EcsInstance.EntityManager.addComponent(e, projectile);
            
            e_EcsInstance.EntityManager.addComponent(e, new MapCollidable());
            e_EcsInstance.EntityManager.addComponent(e, transform);

            if (light != null)
                e_EcsInstance.EntityManager.addComponent(e, light);

            e_EcsInstance.refresh(e);

        }

        public void createProjectile(Vector2 start, Vector2 heading, float velocity, int duration, Light light, Transform transform, Entity originator)
        {
            Entity e = e_EcsInstance.create();

            e_EcsInstance.EntityManager.addComponent(e, new Position(start, new Vector2(16)));
            e_EcsInstance.EntityManager.addComponent(e, new Heading(heading));
            e_EcsInstance.EntityManager.addComponent(e, new Sprite("projectile", "projectile", 32, 32, 0, 0));
            e_EcsInstance.EntityManager.addComponent(e, new Velocity(velocity));

            Projectile projectile = new Projectile(duration);
            projectile.Originator = originator;
            e_EcsInstance.EntityManager.addComponent(e, projectile);
            
            e_EcsInstance.EntityManager.addComponent(e, transform);
            //e_EcsInstance.EntityManager.addComponent(e, new MapCollidable());

            if (light != null)
                e_EcsInstance.EntityManager.addComponent(e, light);

            e_EcsInstance.refresh(e);

        }

        public void createSpatialPartition(Vector2 ul, Vector2 lr, int tiers)
        {
            Entity e = e_EcsInstance.create();

            SpatialPartition spatial = new SpatialPartition();

            spatial.QuadTree = new QuadTree<Entity>(ul, lr);
            spatial.QuadTree.buildQuadTree(tiers);

            e_EcsInstance.EntityManager.addComponent(e, spatial);

            e_EcsInstance.TagManager.tagEntity("SPATIAL", e);

            e_EcsInstance.refresh(e);
        }

    }
}
