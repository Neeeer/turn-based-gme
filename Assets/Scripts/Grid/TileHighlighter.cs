using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHighlighter :MonoBehaviour
{
    public Tilemap tilemap;
    Cell[,] cells;
    List<Vector3Int> highlightedPositions;
    
    private List<Vector3Int> affectedPositions;
    private List<Vector2Int> highlightedAbility;
    public LevelLogic levelLogic;
    public ButtonSelector abilitySelector;
    private Tile highlightedp;
    private Tile selectedp;
    public Inputs inputs;

    public void Awake()
    {


        selectedp = Resources.Load<Tile>("isometric tilemap/arrows/selectedPosition");
        highlightedp = Resources.Load<Tile>("isometric tilemap/arrows/highlightedTile");

        highlightedAbility = new List<Vector2Int>();
        affectedPositions = new List<Vector3Int>();
        highlightedPositions = new List<Vector3Int>();

    }

    public void Start()
    {
        cells = levelLogic.getCellGrid();
    }

    public void highlightTiles(int range, Vector3Int location)
    {
        highlightRangeLoop(range, location, new Vector2Int(0,0));
    }



    private void highlightRangeLoop(int range, Vector3Int location, Vector2Int direction)
    {
        if (range >= 0)
        {
            int attackAngle = abilitySelector.getAttackAngle();
            var isNot = true;
            foreach (Vector3Int v in highlightedPositions)
            {
                if (v.x == location.x && v.y == location.y)
                {
                    isNot = false;
                }
            }
            if (isNot)
            {

                location.z = cells[location.x + levelLogic.getXoffset(), location.y + levelLogic.getYoffset()].zAxis;
                if (levelLogic.checkBounds(location))
                {
                    if (tilemap.HasTile(location))
                    {

                        highlightATile(location);
                    }
                }
            }

            if (levelLogic.getAttackAction() && attackAngle != 0)
            {
                if (attackAngle == 3)
                {
                    highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(1, 1));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 1));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(1, -1));
                    highlightAdjacentCellCheck(range, location, new Vector2Int(-1, -1));
                }
                else if (attackAngle == 2)
                {

                    if (direction.x == 1)
                    {
                        if (direction.y == 1)
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(1, 1));
                        }
                        else
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(1, -1));
                        }
                    }
                    else if (direction.x == -1)
                    {
                        if (direction.y == 1)
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 1));
                        }
                        else
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(-1, -1));
                        }
                    }
                    else
                    {
                        highlightAdjacentCellCheck(range, location, new Vector2Int(1, 1));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(1, -1));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 1));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(-1, -1));
                    }
                }
                else
                {


                    if (direction.x == 0)
                    {
                        if (direction.y == 1)
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                        }
                        else if (direction.y == -1)
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                        }
                        else
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                            highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                            highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                            highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                        }
                    }
                    else
                    {
                        if (direction.x == 1)
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                        }
                        else
                        {
                            highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                        }
                    }
                }
            }
            else
            {

                if (direction.x == 0)
                {
                    if (direction.y == 1)
                    {
                        highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                    }
                    else if (direction.y == -1)
                    {
                        highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                    }
                    else
                    {
                        highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                    }
                }
                else
                {
                    if (direction.x == 1)
                    {
                        highlightAdjacentCellCheck(range, location, new Vector2Int(1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                    }
                    else
                    {
                        highlightAdjacentCellCheck(range, location, new Vector2Int(-1, 0));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, 1));
                        highlightAdjacentCellCheck(range, location, new Vector2Int(0, -1));
                    }
                }
            }
        }
    }

    private void highlightATile(Vector3Int location)
    {
        location.z = cells[location.x + levelLogic.getXoffset(), location.y + levelLogic.getYoffset()].zAxis + 1;


        tilemap.SetTile(location, getHighlightedp());
        tilemap.SetTileFlags(location, TileFlags.None);
        Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        tilemap.SetColor(location, color);

        highlightedPositions.Add(location);
    }

    private bool highlightAdjacentCellCheck(int range, Vector3Int location, Vector2Int direction)
    {
        var checkk = location;


        checkk.x += direction.x;

        checkk.y += direction.y;


        if (levelLogic.checkBounds(checkk))
        {
            if (levelLogic.getAttackAction())
            {
                highlightRangeLoop(range - 1, checkk, direction);
            }
            else
            {
                if (levelLogic.checkIfCanPass(checkk))
                {
                    highlightRangeLoop(range - 1, checkk, direction);
                }
            }
        }

        return false;
    }

    public List<Vector2Int> getDisplayAbilityDirection(Vector3Int character, Vector3Int position, List<Vector2Int> highlightedAbility, int attackAngle)
    {
        int difx = ((int)(position.x - character.x));
        int dify = ((int)(position.y - character.y));
        List<Vector2Int> ability = new List<Vector2Int>();

        if (attackAngle == 2)
        {
            if (difx > 0)
            {
                if (dify > 0)
                {
                    ability = highlightedAbility;
                }
                else
                {
                    for (int i = 0; i < highlightedAbility.Count; i++)
                    {
                        Vector2Int temp = highlightedAbility[i];
                        temp.y = -temp.y;
                        ability.Add(temp);
                    }
                }
            }
            else
            {
                if (dify > 0)
                {
                    for (int i = 0; i < highlightedAbility.Count; i++)
                    {
                        Vector2Int temp = highlightedAbility[i];
                        temp.x = -temp.x;
                        ability.Add(temp);
                    }
                }
                else
                {
                    for (int i = 0; i < highlightedAbility.Count; i++)
                    {
                        Vector2Int temp = highlightedAbility[i];
                        temp.x = -temp.x;
                        temp.y = -temp.y;
                        ability.Add(temp);
                    }
                }
            }
        }
        else
        {

            if (dify == 0)
            {
                if (difx > 0)
                {

                    ability = highlightedAbility;
                }
                else
                {
                    for (int i = 0; i < highlightedAbility.Count; i++)
                    {
                        ability.Add(-highlightedAbility[i]);

                    }
                }
            }
            else
            {
                if (dify > 0)
                {
                    for (int i = 0; i < highlightedAbility.Count; i++)
                    {
                        Vector2Int temp = highlightedAbility[i];
                        int tempx = temp.x;
                        temp.x = temp.y;
                        temp.y = tempx;
                        ability.Add(temp);
                    }
                }
                else
                {
                    for (int i = 0; i < highlightedAbility.Count; i++)
                    {
                        Vector2Int temp = highlightedAbility[i];
                        int tempx = temp.x;
                        temp.x = -temp.y;
                        temp.y = -tempx;
                        ability.Add(temp);
                    }
                }
            }
        }
        return ability;
    }

    public void displayAbility(List<Vector2Int> d)
    {
        Vector3Int selectPos = inputs.getSelectedPosition();

        if (abilitySelector.getAttackAngle() > 10)
        {
            bool found = false;
            Vector3Int direction = inputs.getSelectedPosition() - levelLogic.getCurrentTurn().Location;
            direction.z = 0;

            for (int range = abilitySelector.getAttackAngle() - 10; range > 0; range--)
            {
                if (levelLogic.checkBounds(selectPos))
                {
                    selectPos.z = cells[selectPos.x + levelLogic.getXoffset(), selectPos.y + levelLogic.getYoffset()].zAxis;

                    if (tilemap.HasTile(selectPos))
                    {

                        if (cells[selectPos.x + levelLogic.getXoffset(), selectPos.y + levelLogic.getYoffset()].Occupied)
                        {
                            found = true;
                            break;
                        }

                    }
                }
                selectPos += direction;
            }
            if (!found)
            {
                return;
            }
        }

        foreach (Vector2Int v in d)
        {
            var loc = selectPos;

            loc.x += v.x;
            loc.y += v.y;
            if (levelLogic.checkBounds(loc))
            {
                loc.z = cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].zAxis;
                if (tilemap.HasTile(loc))
                {
                    displayAffectedTile(loc);
                }
            }
        }
    }

    private void displayAffectedTile(Vector3Int loc)
    {
        loc.z += 1;
        tilemap.SetTile(loc, getHighlightedp());
        tilemap.SetTileFlags(loc, TileFlags.None);
        Color color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
        tilemap.SetColor(loc, color);

        getAffectedPositions().Add(loc);
    }

    public void unHighlightStandingPosition(Vector3Int loc)
    {
        loc.z = cells[loc.x + levelLogic.getXoffset(), loc.y + levelLogic.getYoffset()].zAxis + 1;
        tilemap.SetTile(loc, null);
        highlightedPositions.RemoveAt(0);
    }

    public void highlightAbility()
    {
        emptyHighlightedList();
        emptyAffectedList();
        Vector3Int loc = levelLogic.getCurrentTurn().Location;
        if (abilitySelector.getAttackRange() == 0)
        {
            highlightATile(loc);
        }
        else
        {
            highlightTiles(abilitySelector.getAttackRange(), loc);
            unHighlightStandingPosition(loc);
        }
        highlightedAbility = abilitySelector.getSelectedAbility();

       
    }

   

    public void setCurrentTurnTile(Vector3Int loc)
    {
        loc.z += 3;
        tilemap.SetTile(loc, selectedp);


        tilemap.SetTileFlags(loc, TileFlags.None);
        Color color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        tilemap.SetColor(loc, color);
    }

    public void removeCurrentTurnTile(Vector3Int loc)
    {
        loc.z += 3;
        tilemap.SetTile(loc, null);
    }



    public void emptyHighlightedList()
    {
        for (int i = 0; i < highlightedPositions.Count;)
        {
            tilemap.SetTile(highlightedPositions[0], null);
            highlightedPositions.RemoveAt(0);
        }
    }

    public void emptyAffectedList()
    {
        for (int i = 0; i < affectedPositions.Count;)
        {
            tilemap.SetTile(affectedPositions[0], null);
            affectedPositions.RemoveAt(0);
        }
    }

    public List<Vector3Int> getHighlightedPositions()
    {
        return highlightedPositions;
    }




    public List<Vector3Int> getAffectedPositions()
    {
        return affectedPositions;
    }

    public Tile getHighlightedp()
    {
        return highlightedp;
    }

}
