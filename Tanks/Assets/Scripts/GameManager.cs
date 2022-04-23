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
    [SerializeField] CameraController cameraController;

    float timeUntilNextPlayer;
    float nextPlayerTimer;

    //enum GameState
    //{
    //    Start,
    //    Turn,
    //    Transition,
    //    End
    //}

    void Start()
    {
        GameObject[] allTanksInScene = GameObject.FindGameObjectsWithTag("Tank");
        
        foreach (GameObject tank in allTanksInScene)
        {
            tanks.Add(tank.GetComponent<Tank>());
        }

        for (int i = 0; i < tanks.Count; i++)
        {
            tanks[i].AssignPlayer(i, playerNames[i], tankColors[i]);
        }

        StartCoroutine(Coroutine_StartMatch());
    }

    private IEnumerator Coroutine_StartMatch()
    {
        float delay = 1.0f;
        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        currentPlayerIndex = 0;
        currentTank = tanks[currentPlayerIndex];
        currentTank.ReadyTank();
        yield return 0;
    }

    private IEnumerator Coroutine_NextPlayer()
    {
        float delay = currentTank.GetCurrentProjectile().GetTimeToLive();

        while (delay > 0.0f)
        {
            GameObject firedProjectile = GameObject.FindGameObjectWithTag("Projectile");
            delay -= Time.deltaTime;

            if (firedProjectile == null && delay > 2)
                delay = 1.5f;

            yield return null;
        }

        currentPlayerIndex++;
        if (currentPlayerIndex >= tanks.Count)
            currentPlayerIndex = 0;

        currentTank = tanks[currentPlayerIndex];

        currentTank.ReadyTank();
        yield return 0;
    }

    public void NextPlayer()
    {
        if (tanks.Count == 1)
        {
            Debug.Log(currentTank.GetPlayerName() + " Wins!");
            //current tank wins
            //End game
        }

        if(currentTank)
            currentTank.UnreadyTank();

        StartCoroutine(Coroutine_NextPlayer());
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
