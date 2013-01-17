﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ECSFramework;


//using WorldGeneration.Cave;
using WorldGeneration.Utils;

using Vaerydian.Components;
using Vaerydian.Components.Dbg;
using Vaerydian.Components.Spatials;
using Vaerydian.Components.Utils;
using Vaerydian.Components.Graphical;

namespace Vaerydian.Systems.Draw
{
    class MapSystem : EntityProcessingSystem
    {
        private Dictionary<short, Rectangle> m_RectDict;
        private Texture2D m_Texture;
        private ComponentMapper m_GameMapMapper;
        private ComponentMapper m_ViewportMapper;
        private ComponentMapper m_PositionMapper;
        private ComponentMapper m_MapDebugMapper;
        private ComponentMapper m_GeometryMapper;
        private GameContainer m_Container;
        private Entity m_Camera;
        private Entity m_Player;
        private Entity m_MapDebug;
        private Entity m_Geometry;
        private ViewPort m_ViewPort;
        private SpriteBatch m_SpriteBatch;

        private int m_xStart, m_yStart, m_xFinish, m_yFinish, m_TileSize;

        private Terrain m_Terrain;

        public MapSystem(GameContainer container)
        {
            m_Container = container;
            m_SpriteBatch = m_Container.SpriteBatch;
        }

        public override void initialize()
        {
            m_RectDict = new Dictionary<short, Rectangle>();
            m_GameMapMapper = new ComponentMapper(new GameMap(), e_ECSInstance);
            m_ViewportMapper = new ComponentMapper(new ViewPort(), e_ECSInstance);
            m_PositionMapper = new ComponentMapper(new Position(), e_ECSInstance);
            m_MapDebugMapper = new ComponentMapper(new MapDebug(), e_ECSInstance);
            m_GeometryMapper = new ComponentMapper(new GeometryMap(), e_ECSInstance);
        }
        
        protected override void preLoadContent(Bag<Entity> entities)
        {
            m_Camera = e_ECSInstance.TagManager.getEntityByTag("CAMERA");
            m_Player = e_ECSInstance.TagManager.getEntityByTag("PLAYER");
            m_MapDebug = e_ECSInstance.TagManager.getEntityByTag("MAP_DEBUG");
            m_Geometry = e_ECSInstance.TagManager.getEntityByTag("GEOMETRY");

            m_RectDict.Add(TerrainType.CAVE_FLOOR, new Rectangle(19 * 32, 10 * 32, 32, 32));
            m_RectDict.Add(TerrainType.CAVE_WALL, new Rectangle(18 * 32, 13 * 32, 32, 32));

            //m_Texture = m_Container.ContentManager.Load<Texture2D>("terrain\\various");
            //m_Texture = m_Container.ContentManager.Load<Texture2D>("terrain\\noise");
			m_Texture = m_Container.ContentManager.Load<Texture2D>("terrain\\default");

            m_TileSize = m_RectDict[TerrainType.CAVE_WALL].Width;
        }

        protected override void cleanUp(Bag<Entity> entities) { }

        protected override void process(Entity entity)
        {
            //get map and viewport
            GameMap map = (GameMap) m_GameMapMapper.get(entity);
            m_ViewPort = (ViewPort) m_ViewportMapper.get(m_Camera);
            //GeometryMap geometry = (GeometryMap)m_GeometryMapper.get(m_Geometry);

            //update for current viewport location/dimensions
            
            //grab key viewport info
            Vector2 origin = m_ViewPort.getOrigin();
            Vector2 dimensions = m_ViewPort.getDimensions();// / 2;
            Vector2 pos;

            //updateView(origin, center, m_ViewPort.getDimensions());
            updateView(map.Map,origin,dimensions);

            m_SpriteBatch.Begin();

            //iterate through current viewable tiles
            for (int x = m_xStart; x <= m_xFinish; x++)
            {
                for (int y = m_yStart; y <= m_yFinish; y++)
                {
                    //grab current tile terrain
                    m_Terrain = map.getTerrain(x, y);

                    //ensure its useable
                    if (m_Terrain == null)
                        continue;

                    //calculate position to place tile
                    pos = new Vector2(x*m_TileSize,y*m_TileSize);

                    //m_SpriteBatch.Draw(m_Texture, pos-origin, m_RectDict[m_Terrain.TerrainType], Color.White, 0f, new Vector2(0), new Vector2(1), SpriteEffects.None, 0f);
                    m_SpriteBatch.Draw(m_Texture, pos - origin, null, getColorVariation(m_Terrain), 0f, new Vector2(0), new Vector2(1), SpriteEffects.None, 0f);
                }
            }

            
            
            /*
            try
            {
                MapDebug debug = (MapDebug)m_MapDebugMapper.get(m_MapDebug);


                if (debug.OpenSet != null)
                {
                    for (int i = 0; i < debug.OpenSet.Size; i++)
                    {
                        if (debug.OpenSet[i] == null)
                            continue;
                        pos = debug.OpenSet[i].Data.Position * m_TileSize;

                        m_SpriteBatch.Draw(m_Texture, pos-origin, m_RectDict[TerrainType.CAVE_FLOOR], Color.Orange, 0f,new Vector2(0), new Vector2(1), SpriteEffects.None, 0f);
                    }
                }

                if (debug.Blocking != null)
                {
                    for (int i = 0; i < debug.Blocking.Count; i++)
                    {
                        if (debug.Blocking[i] == null)
                            continue;
                        pos = debug.Blocking[i].Position * m_TileSize;

                        m_SpriteBatch.Draw(m_Texture, pos - origin, m_RectDict[TerrainType.CAVE_FLOOR], Color.Red, 0f, new Vector2(0), new Vector2(1), SpriteEffects.None, 0f);
                    }
                }

                if (debug.ClosedSet != null)
                {
                    for (int i = 0; i < debug.ClosedSet.Count; i++)
                    {
                        if (debug.ClosedSet[i] == null)
                            continue;
                        pos = debug.ClosedSet[i].Position * m_TileSize;

                        m_SpriteBatch.Draw(m_Texture, pos - origin, m_RectDict[TerrainType.CAVE_FLOOR], Color.Yellow, 0f, new Vector2(0), new Vector2(1), SpriteEffects.None, 0f);
                    }
                }

                if (debug.Path != null)
                {
                    for (int i = 0; i < debug.Path.Count; i++)
                    {
                        if (debug.Path[i] == null)
                            continue;
                        pos = debug.Path[i].Position * m_TileSize;

                        m_SpriteBatch.Draw(m_Texture, pos - origin, m_RectDict[TerrainType.CAVE_FLOOR], Color.YellowGreen, 0f, new Vector2(0), new Vector2(1), SpriteEffects.None, 0f);
                    }
                }

            }
            catch (Exception e)
            {
                
            }*/

            m_SpriteBatch.End();
        }

        /// <summary>
        /// updates the tile indexes based on current viewport for the draw loop
        /// </summary>
        private void updateView(Map map, Vector2 origin, Vector2 dimensions)
        {

            m_xStart = (int)origin.X / m_TileSize;
            if (m_xStart <= 0)
                m_xStart = 0;

            m_xFinish = (int)(origin.X + dimensions.X) / m_TileSize;
            if (m_xFinish >= map.XSize-1)//m_ViewPort.getDimensions().X - 1)
                m_xFinish = map.XSize-1;// (int)m_ViewPort.getDimensions().X - 1;

            m_yStart = (int)origin.Y / m_TileSize;
            if (m_yStart <= 0)
                m_yStart = 0;

            m_yFinish = (int)(origin.Y + dimensions.Y) / m_TileSize;
            if (m_yFinish >= map.YSize-1)//m_ViewPort.getDimensions().X - 1)
                m_yFinish = map.YSize-1;// (int)m_ViewPort.getDimensions().X - 1;
        }


        private Color getColorVariation(Terrain terrain)
        {
            Color color = getColor(terrain);

            Vector3 colVec = color.ToVector3();

            colVec.X *= terrain.Variation;
            colVec.Y *= terrain.Variation;
            colVec.Z *= terrain.Variation;


            return new Color(colVec);
        }

        private Color getColor(Terrain terrain)
        {
            switch (terrain.TerrainType)
            {
                case TerrainType.LAND_ARCTIC_DESERT:
                    return new Color(204, 204, 255);
                case TerrainType.LAND_DESERT:
                    return new Color(204, 204, 0);
                case TerrainType.LAND_SCORCHED:
                    return new Color(153, 102, 51);
                case TerrainType.LAND_SNOW_PLAINS:
                    return Color.White;
                case TerrainType.LAND_TUNDRA:
                    return new Color(53, 111, 53);
                case TerrainType.LAND_TAIGA:
                    return new Color(24, 72, 48);
                case TerrainType.LAND_TEMPERATE_GRASSLAND:
                    return new Color(153, 255, 102);
                case TerrainType.LAND_SHRUBLAND:
                    return new Color(102, 153, 0);
                case TerrainType.LAND_SAVANA:
                    return new Color(204, 255, 102);
                case TerrainType.LAND_TEMPERATE_FOREST:
                    return new Color(0, 153, 0);
                case TerrainType.LAND_TROPICAL_FOREST:
                    return new Color(102, 255, 51);
                case TerrainType.LAND_GLACIER:
                    return new Color(153, 255, 204);
                case TerrainType.LAND_MARSH:
                    return new Color(33, 101, 67);
                case TerrainType.LAND_TEMPERATE_RAIN_FOREST:
                    return new Color(0, 102, 0);
                case TerrainType.LAND_HYBOREAN_RIMELAND:
                    return new Color(204, 255, 255);
                case TerrainType.LAND_BOG:
                    return new Color(51, 51, 0);
                case TerrainType.LAND_SWAMP:
                    return new Color(0, 51, 0);
                case TerrainType.LAND_TROPICAL_RAIN_FOREST:
                    return new Color(0, 128, 0);
                case TerrainType.OCEAN_ICE:
                    return new Color(204, 255, 255);
                case TerrainType.OCEAN_COAST:
                    return new Color(255, 255, 153);
                case TerrainType.OCEAN_LITTORAL:
                    return new Color(51, 153, 255);
                case TerrainType.OCEAN_SUBLITTORAL:
                    return new Color(0, 102, 255);
                case TerrainType.OCEAN_ABYSSAL:
                    return new Color(0, 51, 204);
                case TerrainType.MOUNTAIN_FOOTHILL:
                    return new Color(57, 69, 43);
                case TerrainType.MOUNTAIN_LOWLAND:
                    return new Color(79, 95, 59);
                case TerrainType.MOUNTAIN_HIGHLAND:
                    return new Color(115, 123, 105);
                case TerrainType.MOUNTAIN_CASCADE:
                    return new Color(150, 150, 150);
                case TerrainType.MOUNTAIN_DRY_PEAK:
                    return new Color(192, 192, 192);
                case TerrainType.MOUNTAIN_SNOWY_PEAK:
                    return new Color(221, 221, 221);
                case TerrainType.BASE_RIVER:
                    return new Color(0, 102, 102);
                case TerrainType.CAVE_ENTRANCE:
                    return new Color(255, 0, 0);
                case TerrainType.CAVE_WALL:
                    return new Color(120, 54, 29);//64, 64, 64);
                case TerrainType.CAVE_FLOOR:
                    return new Color(211, 158, 78);//128, 128, 128);
                default:
                    return Color.Red;
            }
        }
    }
}
