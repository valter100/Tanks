using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<string> playerNames;
    [SerializeField] List<Tank> tanks;
    [SerializeField] List<Tank> instatiatedTanks;
    [SerializeField] List<Color> playerColors;
    [SerializeField] int currentPlayerIndex;
    [SerializeField] int playerCount;
    [SerializeField] Tank currentTank;
    [SerializeField] CameraController cameraController;

    [SerializeField] TMP_Text playerNameText;
    float timeUntilNextPlayer;
    float nextPlayerTimer;

    bool pickingNextPlayer = false;

    //enum GameState
    //{
    //    Start,
    //    Turn,
    //    Transition,
    //    End
    //}

    void Start()
    {
        playerNameText = GameObject.Find("GUI").transform.Find("CurrentPlayer").GetComponent<TMP_Text>();

        StartCoroutine(Coroutine_StartMatch());
    }

    private IEnumerator Coroutine_StartMatch()
    {
        float delay = 1.0f;

        foreach(Tank tank in tanks)
        {
            tank.AssignPlayer();
        }

        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }


        currentPlayerIndex = 0;
        currentTank = tanks[currentPlayerIndex];
        playerNameText.text = "Current player: " + currentTank.GetPlayerName();
        currentTank.ReadyTank();
        yield return 0;
    }


    public void StartPlayerTransition()
    {
        if (tanks.Count == 1)
        {
            Debug.Log(currentTank.GetPlayerName() + " Wins!");
            //current tank wins
            //End game
        }

        if (!pickingNextPlayer)
            StartCoroutine(Coroutine_PlayerTransition());
    }
    private IEnumerator Coroutine_PlayerTransition()
    {
        pickingNextPlayer = true;
        float delay = currentTank.GetCurrentProjectile().GetTimeToLive();

        while (delay > 0.0f)
        {
            GameObject firedProjectile = GameObject.FindGameObjectWithTag("Projectile");
            delay -= Time.deltaTime;

            if (firedProjectile == null && delay > 1.5f)
                delay = 1.5f;

            yield return null;
        }

        SetNextPlayer();
        pickingNextPlayer = false;
        yield return 0;
    }

    public void SetNextPlayer()
    {
        if (currentTank)
            currentTank.UnreadyTank();

        currentPlayerIndex++;
        if (currentPlayerIndex >= tanks.Count)
            currentPlayerIndex = 0;

        for (int i = currentPlayerIndex; i < tanks.Count; i++)
        {
            if (tanks[i].gameObject.activeInHierarchy)
            {
                currentPlayerIndex = i;
                currentTank = tanks[currentPlayerIndex];
                playerNameText.text = "Current player: " + currentTank.GetPlayerName();
                currentTank.ReadyTank();
                return;
            }
        }
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    public List<Tank> GetTankList()
    {
        return tanks;
    }

    public Tank GetCurrentTank()
    {
        return tanks[currentPlayerIndex];
    }

    public void RemoveTankFromList(PlayerTank tank)
    {
        tanks.Remove(tank);
    }

    public string AssignName()
    {
        string newName = playerNames[0];
        playerNames.RemoveAt(0);
        return newName;
    }

    public Color AssignColor()
    {
        Color newColor = playerColors[0];
        playerColors.RemoveAt(0);
        return newColor;
    }

    public void AddInstantiatedTank(Tank newTank)
    {
        instatiatedTanks.Add(newTank);
    }
}
