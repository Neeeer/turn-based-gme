using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


// main logic flow class used in each level where the grid is set up and the turns are carried out.
public class LevelLogic : MonoBehaviour
{

    public Tilemap tilemap;
    public TileHighlighter tileHighlighter;

    private TileSelect tileSelector;
    private Fog fog;

    private Cell[,] cells;

    public lvlEnemies lvlenemies;
    public objectives objectives;
    public ButtonSelector buttonSelect;
    public CharacterMovement characterMovement;

    [SerializeField] private new Camera camera;
    public Inputs inputs;

    private Character currentTurn;

    private int cellxOffset;
    private int cellyOffset;

    public float zAxisyIncrease = (float)0.36;
    private int kills;
    private int turns;

    public Character character1;
    public Character character2;
    public Character character3;
    public Character character4;


    public HealthbarManager healthbarManager;

    public Text phase;

    public Button confirmAbility;


    List<Character> entityOrder;
    List<Character> playersOrder;
    List<Character> enemiesOrder;
    List<Character> players;
    List<Character> enemies;


    private bool movementAction = false;
    [SerializeField] bool attackAction = false;

    private int Turn;

    private BoundsInt bounds;

    private Tile lavaTile;


    [SerializeField] private Transform damagePopup;


    private void Awake()
    {
        Turn = 0;

        // turn order of entities
        enemies = new List<Character>();

        entityOrder = new List<Character>();
        // player units order
        players = new List<Character>();
        playersOrder = new List<Character>();

        // tile intending to move too, otehr classes might need to use it
        MovingToo = new Vector3Int(0, 0, 0);

        // unpassable tile
        lavaTile = Resources.Load<Tile>("isometric tilemap/25-ground-blocks/lava");

       
        TilemapSetup tilemapSetup = new TilemapSetup();
        // get tilemap bounds
        tilemap.CompressBounds();
        bounds = tilemap.cellBounds;

        // x and y offset to be able to convert lowest x and y positions of the tilemap to start at position 0,0
        cellxOffset = Mathf.Abs(bounds.xMin);
        cellyOffset = Mathf.Abs(bounds.yMin);

        // set up the tilemap and each tile cell information
        cells = tilemapSetup.setUpTilemap(tilemap, lavaTile);

        // set up player character party
        setUpParty();


        // class to make sure the correct tile is selected according to elevation
        tileSelector = new TileSelect(this, tilemap, cells);

        // class to calculate which tiles are visible to the player
        fog = new Fog(this, tilemap, cells);

    }



    // Start is called before the first frame update
    void Start()
    {
        // create add enemy to list and corresponding tile position
        addEnemy(lvlenemies.getEnemies());

        // add enemies to turn order
        setUpEntities(enemiesOrder);

        // revel map tiles acording to player units starting positions
        fog.startGameDefog();


        // initialize healthbars;


        // start first turn
        nextTurn();
    }


    // display selected ability on the selected tile
    public void selectAbility()
    {
        tileHighlighter.highlightAbility();
    }


    public void endTurn()
    {

        if (getMovementAction())
        {
            var currentCharacterLocation = getCurrentTurn().Location;

            // remove current turn highlighter
            tileHighlighter.removeCurrentTurnTile(currentCharacterLocation);

            // ckeck if moving too position is selected to display healthbar if needed
            // checkIfSelected(currentCharacterLocation, MovingToo);

            // remove character from tile and place it to the moving too tile, this in the future will be done in the character movement class as it moves along the desired movement path
            cells[currentCharacterLocation.x + getXoffset(), currentCharacterLocation.y + getYoffset()].removeCharacter();
            cells[MovingToo.x + getXoffset(), MovingToo.y + getYoffset()].Character = currentTurn;

            // set character location in character class to moving too tile
            getCurrentTurn().Location = (MovingToo);

            // set current turn highlighter
            tileHighlighter.setCurrentTurnTile(MovingToo);

            // check vision of tiles surounding the character after movement and if the tiles surrounding the before movement position are still visible
            fog.playerMoveRefog(currentCharacterLocation, MovingToo);


            // set up information for attack phase and empty movement lists and atributes
            inputs.emptyMovementList();
            inputs.emptyDirectionList();
            tileHighlighter.emptyHighlightedList();
            attackAction = true;
            
            buttonSelect.resetButton();


            movementAction = false;
            buttonSelect.enableButtons();
            buttonSelect.selectAbility1();
            phase.text = "Action Phase";

        }
        else if (getAttackAction())
        {
            // empty lists used in attack action and set up information for next turn
            tileHighlighter.emptyHighlightedList();
            tileHighlighter.emptyAffectedList();

            movementAction = false;
            attackAction = false;
            buttonSelect.enableConfirmButton();

            tileHighlighter.removeCurrentTurnTile(currentTurn.Location);

            // set animation of the current turn sprite to 0 
            currentTurn.CharGameobject.GetComponent<Animator>().speed = 0;

            Turn++;
            nextTurn();
        }

    }

    public void confirmAction()
    {
        buttonSelect.disableButtons();
        if (getMovementAction())
        {
            //moveme character sprite from a to b
            startMovement();

        }
        else if (getAttackAction())
        {
            // for each tile affected by ability
            StartCoroutine(playAbility());
        }
    }

     IEnumerator playAbility()
    {
        bool finished = true;
        // call play animation method couratine with lerp arrow? 
        if (!finished)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            applyAbility();
        }
    }

    public void applyAbility()
    {
        foreach (Vector3Int s in tileHighlighter.getAffectedPositions())
        {

            var position = s;
            position.z = cells[position.x + getXoffset(), position.y + getYoffset()].zAxis;

            // if it is occupied by a unit
            if (cells[position.x + getXoffset(), position.y + getYoffset()].Occupied)
            {
                Character affectedChar = cells[position.x + getXoffset(), position.y + getYoffset()].Character;

                // use ability on character in the tile
                currentTurn.useAbility(this, affectedChar, buttonSelect.getAbilitySelected());

                // if the unit drops below 0 health
                if (affectedChar.Health <= 0)
                {
                    // unit dies
                    affectedChar.Dead = true;
                    // remove unit sprite
                    affectedChar.CharGameobject.SetActive(false);

                    // remove unit from lists
                    if (affectedChar.GetisPlayer())
                    {
                        playersOrder.Remove(affectedChar);
                    }
                    else
                    {
                        enemiesOrder.Remove(affectedChar);
                        // if killed a enemy update kill count
                        kills++;
                    }
                    entityOrder.Remove(affectedChar);
                    // remove unit from tile
                    cells[affectedChar.Location.x + getXoffset(), affectedChar.Location.y + getYoffset()].removeCharacter();
                }
            }
        }
        endTurn();
    }

    private void startMovement()
    {
        characterMovement.moveCharacter(inputs.getMovementPositions(), MovingToo);
    }

    
    // check if selected tile is within tilemap bounds
    public bool checkBounds(Vector3Int v)
    {
        if (v.x >= bounds.xMin && v.x < bounds.xMax)
        {
            if (v.y >= bounds.yMin && v.y < bounds.yMax)
            {
                return true;
            }
        }
        return false;
    }

    
    // check if tile is passable
    public bool checkIfCanPass(Vector3Int v)
    {
        if (!cells[v.x + getXoffset(), v.y + getYoffset()].Occupied)
        {
            if (cells[v.x + getXoffset(), v.y + getYoffset()].Passable)
            {
                return true;
            }
        }
        return false;
    }

    // transform world coordinates to isometric coordinates
    public Vector3Int getIsometricCoordinates(Vector3 z)
    {
        double tempx = z.x;
        double tempy = z.y;
        z.x = (float)(tempy * 2 + tempx);
        z.y = (float)(tempy * 2 - tempx);

        Vector3Int zInt = Vector3Int.FloorToInt(z);

        return zInt;
    }

    // transform isometric coordinates to world coordinates
    public Vector3 getNonIsometricCoordinates(Vector3Int zInt)
    {

        Vector3 z = zInt;
        float tempx = z.x;
        float tempy = z.y;
        z.x = ((tempx - tempy) / 2);
        z.y = ((tempx + tempy) / 4);

        return z;
    }

    // transform world coordinates to isometric coordinates to find what tile the sprite is standing on
    Vector3Int getIsometricCoordinatesForSprite(Vector3 z)
    {
        double tempx = z.x;
        double tempy = z.y - 0.36 * z.z;
        z.x = (float)(tempy * 2 + tempx - 1);
        z.y = (float)(tempy * 2 - tempx - 1);
        Vector3Int zInt = Vector3Int.RoundToInt(z);
        return zInt;

    }

    // transform isometric coordinates to world coordinates to know where to move sprite to from tile to tile
    public Vector3 getNonIsometricCoordinatesForSprite(Vector3Int zInt)
    {

        Vector3 z = zInt;
        float tempx = z.x + 1;
        float tempy = z.y + 1;
        z.x = ((tempx - tempy) / 2);
        z.y = ((tempx + tempy) / 4);

        return z;
    }

    // set up unit on the tile the sprite is standing on at the start of the level
    public void setUpUnit(Character c)
    {
        Vector3 zz = c.CharGameobject.transform.position;
        Vector3Int zzz = getIsometricCoordinatesForSprite(zz);
        cells[zzz.x + getXoffset(), zzz.y + getYoffset()].Character = c;
        c.Location = (zzz);
    }


    // upon changing current units turn
    public void nextTurn()
    {
        if (Turn >= entityOrder.Count)
        {
            Turn = 0;
            turns++;
        }
        currentTurn = (entityOrder[Turn]);

        // play animation of the current turn unit
        currentTurn.CharGameobject.GetComponent<Animator>().speed = 1;

        // if its a player unit turn
        if (entityOrder[Turn].GetisPlayer() == true)
        {
            // set current turn tile highlighter
            tileHighlighter.setCurrentTurnTile(currentTurn.Location);

            // move chamera to the current turn unit's location
            CameraLoc cameraAccess = camera.GetComponent<CameraLoc>();
            Vector3 too = currentTurn.CharGameobject.transform.position;
            too.z = -10;
            cameraAccess.moveCamera(camera.transform.position, too);

            buttonSelect.setCurrentTurnAbilities();

            // start movement phase
            movementTurn();
        }
        else
        {
            // start ai movement and action
            ai aintelligence = new ai();
            var result = aintelligence.pathFinding(currentTurn, playersOrder, cells, getXoffset(), getYoffset());
            Vector2Int dest = result.Item1;
            Character focus = result.Item2;
            Vector3Int destination = new Vector3Int(dest.x, dest.y, 0);


            destination.z = cells[destination.x, destination.y].zAxis;

            // checkIfSelected(currentTurn.Location, destination);

            // move ai unit
            cells[currentTurn.Location.x + getXoffset(), currentTurn.Location.y + getYoffset()].removeCharacter();
            cells[destination.x, destination.y].Character = currentTurn;


            destination.x = destination.x - getXoffset();
            destination.y = destination.y - getYoffset();
            currentTurn.Location = (destination);

            Vector3 too = getNonIsometricCoordinatesForSprite(destination);


            too.y += zAxisyIncrease * too.z;
            too.y = Mathf.Round(too.y * 100f) / 100f;

            // move sprite to desired location
            currentTurn.CharGameobject.transform.position = too;

            // check if the location the unit moved to is visible by the player
            if (fog.enemyMoveRefog(currentTurn))
            {
                CameraLoc cameraAccess = camera.GetComponent<CameraLoc>();
                Vector3 movingToo = currentTurn.CharGameobject.transform.position;
                // move camera to enemy unit if visible
                cameraAccess.moveCamera(camera.transform.position, movingToo);

            }

            // if unit is in range of attacking after movement
            if (focus != null)
            {
                // use ability
                currentTurn.useAbility(this, focus, 1);

                // if target drops below 0 hp remove unit from list and tile location and delete game object
                if (focus.Health <= 0)
                {
                    focus.Dead = true;
                    focus.CharGameobject.SetActive(false);
                    playersOrder.Remove(focus);

                    entityOrder.Remove(focus);
                    cells[focus.Location.x + getXoffset(), focus.Location.y + getYoffset()].removeCharacter();
                    fog.checkAreaFog(6, focus.Location, new Vector2Int(0, 0), false);

                }
            }
            currentTurn.CharGameobject.GetComponent<Animator>().speed = 0;

            // if player has no units left end the level otherwise play next units turn
            if (playersOrder.Count() == 0)
            {
                endLevel();
                objectives.levelFailed();
            }
            else
            {
                Turn++;
                nextTurn();
            }
        }
    }

    // set up information about units movement and location and highlight tiles it can move too
    private void movementTurn()
    {
        phase.text = "Movement Phase";
        movementAction = true;
        MovingToo = currentTurn.Location;
        int range = currentTurn.MovementRange;
        var location = currentTurn.Location;
        tileHighlighter.highlightTiles(range, location);
    }

    // set up player units in order chosen in the main menu and place them in the levels starting locations
    private void setUpParty()
    {
        // set up characters according to order and unit class choosen
        CharacterSetUp charSetUp = new CharacterSetUp();
        List<string> charList = Player.instance.CharacterList;
        List<Character> characters = charSetUp.setUpCharacter(charList);
        List<string> prefabList = charSetUp.getCharacterPrefabs();

        // for every player character
        for (int i = 0; i< characters.Count; i++)
        {
           // load unit game object
            var Prefab = Resources.Load(prefabList[i]) as GameObject;
            var variableForPrefab = GameObject.Instantiate(Prefab);
            if (i == 0)
            {
                character1 = characters[i];

                character1.CharGameobject = variableForPrefab;
                playersOrder.Add(character1);
               
            }
            else if (i == 1)
            {
                character2 = characters[i];
                character2.CharGameobject = variableForPrefab;
                playersOrder.Add(character2);
            }
            else if (i == 2)
            {
                character3 = characters[i];
                character3.CharGameobject = variableForPrefab;
                playersOrder.Add(character3);
            }
            else if(i == 3)
            {
                character4 = characters[i];
                character4.CharGameobject = variableForPrefab;
                playersOrder.Add(character4);
            }
        }

        // get starting character locations
        lvlenemies.initializeCharacterLocations();
        List<Vector3> charLoc = lvlenemies.getCharacterStartingPositions();

        for (int i = 0;i< playersOrder.Count ;i++ )
        {
            playersOrder[i].CharGameobject.transform.position = charLoc[i];
            setUpUnit(playersOrder[i]);
        }
        players = playersOrder;
    }

    // set up unit turn order by starting from a play unit and having ai units between them
    void setUpEntities(List<Character> ene)
    {
        int charNum = playersOrder.Count;

        int charAdded = 0;
        entityOrder.Add(playersOrder[charAdded]);
        charAdded++;
        int enemyCount = ene.Count;

        float charPerEnemy = (float)enemyCount / charNum;

        for (int i = 0; i < enemyCount; i++)
        {
            if ((float)i / charAdded >= charPerEnemy)
            {
                entityOrder.Add(playersOrder[charAdded]);
                charAdded++;
            }

            entityOrder.Add(ene[i]);
        }

        foreach (Character entity in entityOrder)
        {
            entity.CharGameobject.GetComponent<Animator>().speed = 0;
        }
        enemies = enemiesOrder;
    }


    // if damage is delt or healing is done update health and create a damage popUp
    public void damageDealt(Character cha, int i)
    {
        DamagePopup.Create(damagePopup, cha.CharGameobject.transform.position, i);
        cha.Health -= i;

        healthbarManager.updateHealthBar(cha);
        
    }

    // check if tile selected contains a player to display healthbar ui
    //public void checkIfSelected(Vector3Int from, Vector3Int too)
    //{
    //    if (inputs.getSelectedPosition() == from)
    //    {
    //        if (from != too)
    //        {
    //            healthbar.removeHealthBar();
    //        }
    //    }
    //    else if (inputs.getSelectedPosition() == too)
    //    {
    //        healthbar.updateHealthBar(currentTurn);
    //    }
    //}

    // get player kills
    public int getKills()
    {
        return kills;
    }

    // return turn number
    public int getTurn()
    {
        return turns;
    }

    // end the level
    public void endLevel()
    {
        buttonSelect.disableButtons();
        buttonSelect.disableAbilityHighlights();
        confirmAbility.interactable = (false);
    }

    // add enemies to enemy list and set up units
    private void addEnemy(List<Character> e)
    {
        enemiesOrder = new List<Character>();

        foreach (Character enemy in e)
        {
            enemiesOrder.Add(enemy);
            setUpUnit(enemy);
        }
    }
    
    // halt game inputs
    public void haltUpdate()
    {
        enabled = false;
        inputs.enabled = false;
        inputs.OnDisable();
    }

    // continue game inputs
    public void continueUpdate()
    {
        enabled = true;
        inputs.OnEnable();
        inputs.enabled = true;
    }


    public Character getCurrentTurn()
    {
        return currentTurn;
    }

    public bool getAttackAction()
    {
        return attackAction;
    }

    public bool getMovementAction()
    {
        return movementAction;
    }


    public int getXoffset()
    {
        return cellxOffset;
    }

    public int getYoffset()
    {
        return cellyOffset;
    }

   
    public List<Character> getPlayersOrder()
    {
        return playersOrder;
    }

    public List<Character> getPlayers()
    {
        return players;
    }

    public BoundsInt getBounds()
    {
        return bounds;
    }

    // select proper tile visually since tile might be elevated
    public Vector3Int selectATile(Vector3 location) 
    { 
       Vector3Int tile = tileSelector.getCorrectSelectedPosition(location);
        return tile;
    }

    
    public TileSelect getTileSelector()
    {
        return tileSelector;
    }
    public Cell[,] getCellGrid()
    {
        return cells;
    }
    

    public Vector3Int MovingToo { get; set; }

    public List<Character> getEnemiesOrder()
    {
        return enemiesOrder;
    }

    public List<Vector2> getCameraBoundries()
    {
        return lvlenemies.getCameraBoundries();
    }

}
