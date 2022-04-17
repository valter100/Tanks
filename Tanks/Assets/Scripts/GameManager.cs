using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<string> playerNames;
    [SerializeField] List<Tank> tanks;
    [SerializeField] Color[] tankColors;
    [SerializeField] int currentPlayerIndex;
    [SerializeField] int playerCount;
    [SerializeField] Tank currentTank;

    void Start()
    {
        GameObject[] allTanksInScene = GameObject.FindGameObjectsWithTag("Tank");
        
        foreach(GameObject tank in allTanksInScene)
        {
            tanks.Add(tank.GetComponent<Tank>());
        }

        for(int i = 0; i < tanks.Count; i++)
        {
            tanks[i].AssignPlayer(i, playerNames[i], tankColors[i]);
        }

        currentPlayerIndex = 0;
        currentTank = tanks[currentPlayerIndex];
        currentTank.ReadyTank();
    }

    public void NextPlayer()
    {
        if(tanks.Count == 1)
        {
            Debug.Log(currentTank.GetPlayerName() + " Wins!");
            //current tank wins
            //End game
        }

        currentTank.UnreadyTank();

        currentPlayerIndex++;
        if(currentPlayerIndex >= tanks.Count)
            currentPlayerIndex=0;

        currentTank = tanks[currentPlayerIndex];
        currentTank.ReadyTank();
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    public Tank GetCurrentTank()
    {
        return tanks[currentPlayerIndex];
    }

    public void Fire()
    {
        if (currentTank.CanFire())
            currentTank.Fire();
    }

    public void RemoveTankFromList(Tank tank)
    {
        tanks.Remove(tank);
        Destroy(tank.gameObject);
    }
}
