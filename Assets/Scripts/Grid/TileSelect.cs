using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelect 
{
    Tilemap tilemap;
    Cell[,] cells;
    LevelLogic levelLogic;

    private float zAxisyIncrease = (float)0.36;

    public TileSelect(LevelLogic g, Tilemap map, Cell[,] cellsMap)
    {
        levelLogic = g;
        cells = cellsMap;
        tilemap = map;
    }



    public Vector3Int getCorrectSelectedPosition(Vector3 loc)
    {


        Vector3 selectWorldPosition = Camera.main.ScreenToWorldPoint(loc);
        Vector3Int tilePos = levelLogic.getIsometricCoordinates(selectWorldPosition);
        Vector3Int otherTilePos = levelLogic.getIsometricCoordinates(selectWorldPosition);
        Vector3Int underTilePos = levelLogic.getIsometricCoordinates(selectWorldPosition);

        Vector3 tileWorldPosition = levelLogic.getNonIsometricCoordinates(tilePos);


        List<Vector3Int> colums = new List<Vector3Int>();

        if (levelLogic.checkBounds(tilePos))
        {
            tilePos.z = cells[tilePos.x + levelLogic.getXoffset(), tilePos.y + levelLogic.getYoffset()].zAxis;


            if (tilePos.z == 0)
            {
                return tilePos;
            }


            if (selectWorldPosition.x - tileWorldPosition.x < 0)
            {
                otherTilePos.x += -3;
                otherTilePos.y += -2;

                underTilePos.x += -2;
                underTilePos.y += -2;

            }
            else
            {
                otherTilePos.x += -3;
                otherTilePos.y += -3;

                underTilePos.x += -2;
                underTilePos.y += -3;
            }

            if (levelLogic.checkBounds(otherTilePos))
            {
                otherTilePos.z = cells[otherTilePos.x + levelLogic.getXoffset(), otherTilePos.y + levelLogic.getYoffset()].zAxis;

                colums.Add(otherTilePos);

            }
            if (levelLogic.checkBounds(underTilePos))
            {
                underTilePos.z = cells[underTilePos.x + levelLogic.getXoffset(), underTilePos.y + levelLogic.getYoffset()].zAxis;

                colums.Add(underTilePos);
            }

            while (underTilePos.x != tilePos.x)
            {

                otherTilePos.x++;
                otherTilePos.y++;

                underTilePos.x++;
                underTilePos.y++;


                if (levelLogic.checkBounds(otherTilePos))
                {
                    otherTilePos.z = cells[otherTilePos.x + levelLogic.getXoffset(), otherTilePos.y + levelLogic.getYoffset()].zAxis;
                    colums.Add(otherTilePos);
                }
                if (levelLogic.checkBounds(underTilePos))
                {
                    underTilePos.z = cells[underTilePos.x + levelLogic.getXoffset(), underTilePos.y + levelLogic.getYoffset()].zAxis;
                    colums.Add(underTilePos);
                }

            }

            float lastz = 0;

            foreach (Vector3Int i in colums)
            {

                Vector3 tempTile = levelLogic.getNonIsometricCoordinates(i);
                float tempx = Mathf.Abs(selectWorldPosition.x - tempTile.x);
                float tempy = Mathf.Abs(selectWorldPosition.y - (tempTile.y + zAxisyIncrease * tempTile.z + tilemap.cellSize.y / 2));


                if (tempx < tilemap.cellSize.x / 2)
                {
                    if (tempy < tilemap.cellSize.y / 2)
                    {
                        if ((tilemap.cellSize.x / 2 - tempx) * 0.5 >= tempy)
                        {
                            return i;
                        }
                    }
                }

                if (lastz < tempTile.z)
                {
                    if (selectWorldPosition.y - (tempTile.y + zAxisyIncrease * tempTile.z + tilemap.cellSize.y / 2) <= -tilemap.cellSize.y / 2)
                    {
                        return i;
                    }
                }
                lastz = tempTile.z;
            }
        }
        else
        {
            tileWorldPosition = levelLogic.getNonIsometricCoordinates(tilePos);

            if (selectWorldPosition.x - tileWorldPosition.x < 0)
            {
                otherTilePos.x += -1;
            }
            else
            {
                otherTilePos.y += -1;
            }

            underTilePos.x--;
            underTilePos.y--;


            for (int i = 0; i < 3; i++)
            {
                if (levelLogic.checkBounds(otherTilePos))
                {
                    otherTilePos.z = cells[otherTilePos.x + levelLogic.getXoffset(), otherTilePos.y + levelLogic.getYoffset()].zAxis;

                    Vector3 tempTile = levelLogic.getNonIsometricCoordinates(otherTilePos);
                    float tempx = Mathf.Abs(selectWorldPosition.x - tempTile.x);
                    float tempy = Mathf.Abs(selectWorldPosition.y - (tempTile.y + zAxisyIncrease * tempTile.z + tilemap.cellSize.y / 2));


                    if (tempx < tilemap.cellSize.x / 2)
                    {
                        if (tempy < tilemap.cellSize.y / 2)
                        {
                            if ((tilemap.cellSize.x / 2 - tempx) * 0.5 >= tempy)
                            {
                                return otherTilePos;
                            }
                        }
                    }
                }

                if (levelLogic.checkBounds(underTilePos))
                {
                    underTilePos.z = cells[underTilePos.x + levelLogic.getXoffset(), underTilePos.y + levelLogic.getYoffset()].zAxis;

                    Vector3 tempTile = levelLogic.getNonIsometricCoordinates(underTilePos);
                    float tempx = Mathf.Abs(selectWorldPosition.x - tempTile.x);
                    float tempy = Mathf.Abs(selectWorldPosition.y - (tempTile.y + zAxisyIncrease * tempTile.z + tilemap.cellSize.y / 2));


                    if (tempx < tilemap.cellSize.x / 2)
                    {
                        if (tempy < tilemap.cellSize.y / 2)
                        {
                            if ((tilemap.cellSize.x / 2 - tempx) * 0.5 >= tempy)
                            {
                                return underTilePos;
                            }
                        }
                    }
                }
                otherTilePos.x--;
                otherTilePos.y--;

                underTilePos.x--;
                underTilePos.y--;
            }
        }
        
        return tilePos;

    }
}
