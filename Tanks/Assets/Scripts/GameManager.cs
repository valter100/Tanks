using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<string> playerNames;
    [SerializeField] List<Color> playerColors;
    [SerializeField] List<TankType> playerTankTypes;

    [SerializeField] GameInfo gameInfo;
    [SerializeField] GenerateSpawnpoints gameSpawnpoints;
    [SerializeField] List<GameObject> tankPrefabs;
    [SerializeField] List<Tank> spawnedTanks;

    [SerializeField] int currentPlayerIndex;
    [SerializeField] Tank currentTank;
    [SerializeField] CameraController cameraController;

    [SerializeField] TMP_Text playerNameText;
    float timeUntilNextPlayer;
    float nextPlayerTimer;

    bool pickingNextPlayer = false;

    void Start()
    {
        playerNames = new List<string>();
        playerColors = new List<Color>();
        playerTankTypes = new List<TankType>();
        spawnedTanks = new List<Tank>();

        playerNameText = GameObject.Find("GUI").transform.Find("CurrentPlayer").GetComponent<TMP_Text>();
        gameInfo = GameObject.Find("Game Info").GetComponent<GameInfo>();
        StartCoroutine(Coroutine_StartMatch());
    }

    private IEnumerator Coroutine_StartMatch()
    {
        float delay = 1.0f;

        GetInfo();
        SpawnTanks();

        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }


        currentPlayerIndex = 0;
        currentTank = spawnedTanks[currentPlayerIndex];
        playerNameText.text = "Current player: " + currentTank.GetPlayerName();
        currentTank.ReadyTank();
        yield return 0;
    }

    void GetInfo()
    {
        for(int i = 0; i < gameInfo.names.Count; ++i)
        {
            playerNames.Add(gameInfo.names[i]);
            playerColors.Add(gameInfo.colors[i]);
            playerTankTypes.Add(gameInfo.tankTypes[i]);
        }
    }

    void SpawnTanks()
    {
        for (int i = 0; i < gameInfo.names.Count; ++i)
        {
            GameObject spawnedTank = tankPrefabs[0];
            gameSpawnpoints.GenerateTank(spawnedTank);
        }
    }

    public void StartPlayerTransition()
    {
        if (spawnedTanks.Count == 1)
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
        if (currentPlayerIndex >= spawnedTanks.Count)
            currentPlayerIndex = 0;

        for (int i = currentPlayerIndex; i < spawnedTanks.Count; i++)
        {
            if (spawnedTanks[i].gameObject.activeInHierarchy)
            {
                currentPlayerIndex = i;
                currentTank = spawnedTanks[currentPlayerIndex];
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
        return spawnedTanks;
    }

    public Tank GetCurrentTank()
    {
        return spawnedTanks[currentPlayerIndex];
    }

    public void RemoveTankFromList(Tank tank)
    {
        spawnedTanks.Remove(tank);
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
        spawnedTanks.Add(newTank);
    }
}
