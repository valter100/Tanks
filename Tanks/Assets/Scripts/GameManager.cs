using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Player> players;
    [SerializeField] private Player currentPlayer;
    [SerializeField] private int currentPlayerIndex;

    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject airDropPrefab;
    [SerializeField] private List<AirDrop> airDrops;

    [Header("Values")]
    [SerializeField] private bool inPlayerTransition = false;
    [SerializeField] private bool paused;

    public bool Paused => paused;
    public List<Player> Players => players;
    public Player CurrentPlayer => currentPlayer;

    void Start()
    {
        paused = false;
        StartCoroutine(Coroutine_StartMatch());
    }

    private void Update()
    {
        if (paused)
            return;

        if (currentPlayer != null)
        {
            currentPlayer.ManualUpdate();
        }
    }

    public void SetPaused(bool paused)
    {
        this.paused = paused;
        Time.timeScale = paused ? 0 : 1;
        AudioListener.pause = paused;
    }

    private IEnumerator Coroutine_StartMatch()
    {
        GameObject gameInfoObject = GameObject.Find("Game Info");

        if (gameInfoObject != null)
        {
            GameInfo gameInfo = gameInfoObject.GetComponent<GameInfo>();
            GenerateSpawnpoints generateSpawnpoints = GameObject.Find("Map").GetComponent<GenerateSpawnpoints>();

            for (int i = 0; i < gameInfo.names.Count; ++i)
            {
                Vector3 spawnPoint = generateSpawnpoints.GetNewSpawnpoint();
                if (spawnPoint == Vector3.zero)
                {
                    i--;
                    yield return null;
                }
                Player player = AddNewPlayer();
                player.Initialize(
                    gameInfo.names[i][0] != '\u200B' ? gameInfo.names[i] : "Player " + players.Count,
                    gameInfo.colors[i],
                    gameInfo.tankPrefabs[i],
                    gameInfo.controls[i],
                    spawnPoint);
            }
        }

        else
        {
            Debug.LogWarning("No Game Info Game Object found. Make sure to launch the game through the Menu scene.");
        }

        float delay = 1.0f;
        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        if (players.Count != 0)
        {
            currentPlayerIndex = 0;
            currentPlayer = players[currentPlayerIndex];
            currentPlayer.Ready();
        }
        
        yield return 0;
    }

    public Player AddNewPlayer()
    {
        Player player = Instantiate(playerPrefab, transform).GetComponent<Player>();
        players.Add(player);
        return player;
    }

    public void StartPlayerTransition()
    {
        if (players.Count == 1)
        {
            Debug.Log(currentPlayer.Info.name + " Wins!");
        }

        else if (players.Count == 0)
        {
            Debug.Log("Draw");
        }

        if (!inPlayerTransition)
            StartCoroutine(Coroutine_PlayerTransition());
    }

    private IEnumerator Coroutine_PlayerTransition()
    {
        inPlayerTransition = true;
        float delay;

        try { delay = currentPlayer.Inventory.SelectedItem.usable.GetComponent<Projectile>().GetTimeToLive(); }
        catch { delay = 1.5f; }

        while (delay > 0.0f)
        {
            GameObject firedProjectile = GameObject.FindGameObjectWithTag("Projectile");
            delay -= Time.deltaTime;

            if (firedProjectile == null && delay > 1.5f)
                delay = 1.5f;

            yield return null;
        }

        SetNextPlayer();
        inPlayerTransition = false;

        if (Random.Range(0f, 1f) < 0.3f)
            CreateNewAirDrop();

        yield return 0;
    }

    public void SetNextPlayer()
    {
        // Unready current player
        if (currentPlayer != null)
            currentPlayer.Unready();

        // Deactivate destoryed players
        foreach (Player player in players)
        {
            if (player.gameObject.activeInHierarchy && player.Tank.Destroyed())
                player.gameObject.SetActive(false);
        }

        // Find next player
        for (int i = 0; i < players.Count; ++i)
        {
            ++currentPlayerIndex;
            currentPlayerIndex %= players.Count;

            if (players[currentPlayerIndex].gameObject.activeInHierarchy)
                break;
        }

        // Ready next player
        currentPlayer = players[currentPlayerIndex];
        currentPlayer.Ready();
    }

    public void CreateNewAirDrop()
    {
        GenerateSpawnpoints generateSpawnpoints = GameObject.Find("Map").GetComponent<GenerateSpawnpoints>();
        AirDrop airDrop = Instantiate(airDropPrefab, generateSpawnpoints.GetNewSpawnpoint(), Quaternion.identity).GetComponent<AirDrop>();
        airDrop.transform.position += new Vector3(0, 10, 0);
        airDrops.Add(airDrop);
    }
}
