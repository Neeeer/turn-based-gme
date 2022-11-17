using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarManager : MonoBehaviour
{
    private List<Healthbar> HBarList;

    public Healthbar healthbar1;
    public Healthbar healthbar2;
    public Healthbar healthbar3;
    public Healthbar healthbar4;



    public MiniHealthBarUI miniHealthUI;


    public LevelLogic levelLogic;
    private List<Character> playersOrder;

    private void Awake()
    {
        HBarList = new List<Healthbar>();

        HBarList.Add(healthbar1);
        HBarList.Add(healthbar2);
        HBarList.Add(healthbar3);
        HBarList.Add(healthbar4);

    }

    // Start is called before the first frame update
    void Start()
    {
        playersOrder = levelLogic.getPlayers();
        for (int i = 0; i < playersOrder.Count ;i++ )
        {
            Healthbar Hbar = HBarList[i];
            Character cha = playersOrder[i];

            Hbar.assignCharacterToHbar(cha);
            Hbar.updateHealthBar(cha);
            Hbar.setCharactrerName();

        }

    }


    public void updateHealthBar(Character cha)
    {

        foreach (Healthbar bar in HBarList)
        {

            if (bar.getCharacter() == cha )
            {
                bar.updateHealthBar(cha);
            }
        }
        
    }
}
