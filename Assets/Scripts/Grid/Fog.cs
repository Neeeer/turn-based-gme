using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fog 
{
    Tilemap tilemap;
    Cell[,] cells;
    private Tile fog;
    LevelLogic levelLogic;

    public Fog(LevelLogic g, Tilemap map, Cell[,] cellsMap)
    {
        levelLogic = g;
        cells = cellsMap;
        tilemap = map;
        fog =  Resources.Load<Tile>("isometric tilemap/arrows/fog"); 
    }


    public void checkAreaFog(int range, Vector3Int location, Vector2Int direction, bool movingFromOrToo)
    {
        if (range > 0)
        {
            List<Vector2Int> directions = new List<Vector2Int>();

            if (direction.x == 0)
            {
                if (direction.y == 1)
                {
                    setTileFog(range, location, new Vector2Int(1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(-1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, 1), movingFromOrToo);
                }
                else if (direction.y == -1)
                {
                    setTileFog(range, location, new Vector2Int(1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(-1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, -1), movingFromOrToo);
                }
                else
                {
                    setTileFog(range, location, new Vector2Int(1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(-1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, 1), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, -1), movingFromOrToo);
                }
            }
            else
            {
                if (direction.x == 1)
                {
                    setTileFog(range, location, new Vector2Int(1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, 1), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, -1), movingFromOrToo);
                }
                else
                {
                    setTileFog(range, location, new Vector2Int(-1, 0), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, 1), movingFromOrToo);
                    setTileFog(range, location, new Vector2Int(0, -1), movingFromOrToo);
                }
            }
        }
    }

    public void setTileFog(int range, Vector3Int location, Vector2Int direction, bool movingFromOrToo)
    {
        var checkk = location;
        bool vision = false;

        checkk.x += direction.x;

        checkk.y += direction.y;

        if (levelLogic.checkBounds(checkk))
        {
            if (movingFromOrToo == true)
            {
                Vector3Int loc = new Vector3Int(checkk.x, checkk.y, location.z);
                loc.z = cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].zAxis + 4;

                tilemap.SetTile(loc, null);
                cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].Fog = false;
            }
            else
            {

                foreach (Character cha in levelLogic.getPlayersOrder())
                {
                    if (Mathf.Abs(cha.Location.x - checkk.x) + Mathf.Abs(cha.Location.y - checkk.y) <= 6)
                    {
                        vision = true;
                        break;
                    }
                }


                if (!vision)
                {

                    Vector3Int loc = new Vector3Int(checkk.x, checkk.y, location.z);

                    loc.z = cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].zAxis + 4;
                    if (tilemap.HasTile(new Vector3Int(loc.x, loc.y, 0)))
                    {

                        tilemap.SetTile(loc, fog);
                        tilemap.SetTileFlags(loc, TileFlags.None);
                        Color color = new Color(1.0f, 0.4f, 0.0f, 0.5f);
                        tilemap.SetColor(loc, color);
                        cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].Fog = true;
                    }

                }
            }
            checkAreaFog(range - 1, checkk, direction, movingFromOrToo);

        }
    }


    public void playerMoveRefog(Vector3Int from, Vector3Int to)
    {

        Vector3Int tempLoc = from;
        var locations = new List<KeyValuePair<Vector3Int, Vector2Int>>();
        checkAreaFog(6, from, new Vector2Int(0, 0), false);
        checkAreaFog(6, to, new Vector2Int(0, 0), true);

    }

    public bool enemyMoveRefog(Character c)
    {
       // Debug.Log("start");
        Vector3Int loc = c.Location;
        // Debug.Log("loc" + loc);
        if (cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].Fog == true)
        {
            Renderer tempSprite = c.CharGameobject.GetComponent<SpriteRenderer>();
            tempSprite.enabled = false;
            return false;
        }
        else
        {

           // Debug.Log("made it");
            Renderer tempSprite = c.CharGameobject.GetComponent<SpriteRenderer>();
            tempSprite.enabled = true;
            return true;
        }
    }

    public void startGameDefog()
    {
        for (int x = levelLogic.getBounds().xMin; x < levelLogic.getBounds().xMax; x++)
        {

            for (int y = levelLogic.getBounds().yMin; y < levelLogic.getBounds().yMax; y++)
            {
                var px = levelLogic.getXoffset() + x;
                var py = levelLogic.getYoffset() + y;


                Vector3Int location = new Vector3Int(px, py, cells[px, py].zAxis + 4);
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {

                    Vector3Int loc = new Vector3Int(x, y, location.z);

                    tilemap.SetTile(loc, fog);
                    tilemap.SetTileFlags(loc, TileFlags.None);
                    Color color = new Color(1.0f, 0.4f, 0.0f, 0.5f);
                    tilemap.SetColor(loc, color);

                    cells[px, py].Fog = true;
                }
            }
        }
        foreach (Character i in levelLogic.getPlayersOrder())
        {
            Vector3Int loc = i.Location;

            checkAreaFog(6, loc, new Vector2Int(0, 0), true);

        }
        checkVisibleUnits();
    }

    public void checkVisibleUnits()
    {
        foreach (Character e in levelLogic.getEnemiesOrder())
        {
            if (cells[e.Location.x + levelLogic.getXoffset(), e.Location.y + levelLogic.getYoffset()].Fog == true)
            {
                e.CharGameobject.GetComponent<SpriteRenderer>().enabled = false;
               
            }
        }

    }
}
