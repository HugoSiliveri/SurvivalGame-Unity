using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGenerator : MonoBehaviour
{
    Dictionary<int, Tile> tileset;
    Dictionary<int, AnimatedTile> tilesetAnimated;

    public Texture2D beachTileset;
    public Tilemap tilemap;

    Dictionary<Vector2, string> tileBiome = new Dictionary<Vector2, string>();

    public void GenerateTileMap(float[,] heightMap, TerrainType[] regions)
    {
        CreateTileset();
        CreateMap(heightMap, regions);
        ReplaceWithBitmask(heightMap, regions);
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, Tile>();
        Sprite[] sprites = Resources.LoadAll<Sprite>(beachTileset.name);
        for(int i = 0; i < sprites.Length; i++)
        {
            Tile t = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
            t.sprite = sprites[i];
            tileset.Add(i, t);
        }

        tilesetAnimated = new Dictionary<int, AnimatedTile>();
        AnimatedTile[] tilesAnimated = Resources.LoadAll<AnimatedTile>("Animations/");
       
        for(int i = 0; i < tilesAnimated.Length; i++)
        {
            AnimatedTile at = tilesAnimated[i];
            at.m_AnimationStartTime = Time.time;
            tilesetAnimated.Add(i, at);
           
        }

    }

    void CreateMap(float[,] heightMap, TerrainType[] regions)
    {

        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int tile_id = GetIdUsingPerlin(heightMap[x, y], regions);

                Vector2 v = new Vector2(x, y);
                if (tile_id == 20)
                {
                    tileBiome.Add(v, "water");
                }
                else if (tile_id == 8)
                {
                    tileBiome.Add(v, "sand");
                }
                else
                {
                    tileBiome.Add(v, "grass");
                }

                CreateTile(tile_id, x, y);
                
            }
        }
    }

   int GetIdUsingPerlin(float perlinValue, TerrainType[] regions)
   {
        if (perlinValue <= regions[0].height)
        {
            return 20;
        }
        else if (perlinValue <= regions[1].height)
        {
            return 8;
        }
        else
        {
            return 6;
        }
    }

    void CreateTile(int tileID, int x, int y)
    {

        Vector3Int vector3Int = new Vector3Int(x, y, 0);
        Tile at = tileset[tileID];
        tilemap.SetTile(vector3Int, at);
    }

    private void UpdateTile(int tileID, int x, int y, bool isAnimated)
    {
        if (isAnimated)
        {
            Vector3Int vector3Int = new Vector3Int(x, y, 0);
            AnimatedTile at = tilesetAnimated[tileID];
            tilemap.SetTile(vector3Int, at);            
        }
        else {
            Vector3Int vector3Int = new Vector3Int(x, y, 0);
            Tile at = tileset[tileID];
            tilemap.SetTile(vector3Int, at);
        }
    }

    void ReplaceWithBitmask(float[,] heightMap, TerrainType[] regions)
    {
        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        int north;
        int south;
        int west;
        int east;
        int NE;
        int SE;
        int SW;
        int NW;

        int mask;

        float perlinValue;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (0 <= x - 1 && x + 1 < mapWidth && 0 <= y - 1 && y + 1 < mapHeight)
                {
                    perlinValue = heightMap[x, y];

                    if (perlinValue <= regions[0].height)
                    {
                        north = (tileBiome[new Vector2(x - 1, y)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;
                        south = (tileBiome[new Vector2(x + 1, y)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;
                        west = (tileBiome[new Vector2(x, y - 1)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;
                        east = (tileBiome[new Vector2(x, y + 1)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;


                        mask = north + west * 2 + east * 4 + south * 8;
                        
                        switch (mask)
                        {
                            case 3:
                                UpdateTile(7, x, y, true);
                                break;
                            case 1:
                                UpdateTile(5, x, y, true);
                                break; 
                            case 5:
                                UpdateTile(0, x, y, true);
                                break;
                            case 2:
                                UpdateTile(8, x, y, true);
                                break;
                            case 4:
                                UpdateTile(1, x, y, true);
                                break;
                            case 10:
                                UpdateTile(9, x, y, true);
                                break;
                            case 8:
                                UpdateTile(6, x, y, true);
                                break;
                            case 12:
                                UpdateTile(2, x, y, true);
                                break;
                            case 13:
                                UpdateTile(12, x, y, true);
                                break;
                            case 14:
                                UpdateTile(16, x, y, true);
                                break;
                            case 7:
                                UpdateTile(13, x, y, true);
                                break;
                            case 11:
                                UpdateTile(17, x, y, true);
                                break;

                            default:
                                NE = (tileBiome[new Vector2(x - 1, y + 1)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;
                                SE = (tileBiome[new Vector2(x + 1, y + 1)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;
                                SW = (tileBiome[new Vector2(x + 1, y - 1)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;
                                NW = (tileBiome[new Vector2(x - 1, y - 1)] == tileBiome[new Vector2(x, y)]) ? 0 : 1;

                                mask = NW + north * 2 + NE * 4 + west * 8 + east * 16 + SW * 32 + south * 64 + SE * 128;

                                switch (mask)
                                {
                                    case 1:
                                        UpdateTile(4, x, y, true);
                                        break;
                                    case 4:
                                        UpdateTile(11, x, y, true);
                                        break;
                                    case 32:
                                        UpdateTile(3, x, y, true);
                                        break;
                                    case 128:
                                        UpdateTile(10, x, y, true);
                                        break;
                                    case 5:
                                        UpdateTile(15, x, y, true);
                                        break;
                                    case 132:
                                        UpdateTile(14, x, y, true);
                                        break;
                                    case 160:
                                        UpdateTile(18, x, y, true);
                                        break;
                                    case 33:
                                        UpdateTile(19, x, y, true);
                                        break;
                                }

                                break;
                        }

                    }

                    else if (perlinValue <= regions[1].height)
                    {
                        north = (tileBiome[new Vector2(x - 1, y)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x - 1, y)] == "water") ? 0 : 1;
                        south = (tileBiome[new Vector2(x + 1, y)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x + 1, y)] == "water") ? 0 : 1;
                        west = (tileBiome[new Vector2(x, y - 1)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x, y - 1)] == "water") ? 0 : 1;
                        east = (tileBiome[new Vector2(x, y + 1)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x, y + 1)] == "water") ? 0 : 1;


                        mask = north + west * 2 + east * 4 + south * 8;

                        switch (mask)
                        {
                            case 3:
                                UpdateTile(12, x, y, false);
                                break;
                            case 1:
                                UpdateTile(7, x, y, false);
                                break;
                            case 5:
                                UpdateTile(3, x, y, false);
                                break;
                            case 2:
                                UpdateTile(1, x, y, false);
                                break;
                            case 4:
                                UpdateTile(10, x, y, false);
                                break;
                            case 10:
                                UpdateTile(13, x, y, false);
                                break;
                            case 8:
                                UpdateTile(5, x, y, false);
                                break;
                            case 12:
                                UpdateTile(4, x, y, false);
                                break;
                            case 13:
                                UpdateTile(29, x, y, false);
                                break;
                            case 14:
                                UpdateTile(37, x, y, false);
                                break;
                            case 7:
                                UpdateTile(30, x, y, false);
                                break;
                            case 11:
                                UpdateTile(38, x, y, false);
                                break;

                            default:
                                NE = (tileBiome[new Vector2(x - 1, y + 1)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x - 1, y + 1)] == "water") ? 0 : 1;
                                SE = (tileBiome[new Vector2(x + 1, y + 1)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x + 1, y + 1)] == "water") ? 0 : 1;
                                SW = (tileBiome[new Vector2(x + 1, y - 1)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x + 1, y - 1)] == "water") ? 0 : 1;
                                NW = (tileBiome[new Vector2(x - 1, y - 1)] == tileBiome[new Vector2(x, y)] || tileBiome[new Vector2(x - 1, y - 1)] == "water") ? 0 : 1;

                                mask = NW + north * 2 + NE * 4 + west * 8 + east * 16 + SW * 32 + south * 64 + SE * 128;

                                switch (mask)
                                {
                                    case 1:
                                        UpdateTile(2, x, y, false);
                                        break;
                                    case 4:
                                        UpdateTile(11, x, y, false);
                                        break;
                                    case 32:
                                        UpdateTile(0, x, y, false);
                                        break;
                                    case 128:
                                        UpdateTile(9, x, y, false);
                                        break;
                                    case 5:
                                        UpdateTile(32, x, y, false);
                                        break;
                                    case 132:
                                        UpdateTile(31, x, y, false);
                                        break;
                                    case 160:
                                        UpdateTile(39, x, y, false);
                                        break;
                                    case 33:
                                        UpdateTile(40, x, y, false);
                                        break;

                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}