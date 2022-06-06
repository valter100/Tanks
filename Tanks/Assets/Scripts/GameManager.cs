using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Player> players;
    [SerializeField] private int currentPlayerIndex;

    /// <summary>
    /// The current player that is also active (controllable).
    /// </summary>
    [SerializeField] private Player activePlayer;

    /// <summary>
    /// The current player, may or may not be active.
    /// </summary>
    [SerializeField] private Player currentPlayer;
    
    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject airDropPrefab;
    [SerializeField] private List<AirDrop> airDrops;
    [SerializeField] private GUIManager guiManager;

    [Header("Values")]
    [SerializeField] private bool inPlayerTransition = false;
    [SerializeField] private bool paused;

    public bool Paused => paused;
    public List<Player> Players => players;

    /// <summary>
    /// The current player that is also active (controllable).
    /// </summary>
    public Player ActivePlayer => activePlayer;

    /// <summary>
    /// The current player, may or may not be active.
    /// </summary>
    public Player CurrentPlayer => currentPlayer;

    void Start()
    {
        StartCoroutine(Coroutine_StartMatch());
    }

    private void Update()
    {
        if (paused)
            return;

        if (activePlayer != null)
        {
            if (!inPlayerTransition && !activePlayer.gameObject.activeInHierarchy)
                StartPlayerTransition();

            else
                activePlayer.ManualUpdate();
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
        activePlayer = null;
        currentPlayer = null;
        currentPlayerIndex = -1;
        inPlayerTransition = false;
        SetPaused(false);

        map.GetComponent<GenerateMap>().GenerateRandomMap();

        GameObject gameInfoObject = GameObject.Find("Game Info");

        if (gameInfoObject == null)
        {
            Debug.Log("No Game Info found. Using Test Game Info instead.");
            gameInfoObject = GameObject.Find("Test Game Info");
        }

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

        float delay = 1.0f;
        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        if (players.Count != 0)
        {
            StartPlayerTransition();
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
        if (!inPlayerTransition)
        {
            StartCoroutine(Coroutine_PlayerTransition());
        }
    }

    private IEnumerator Coroutine_PlayerTransition()
    {
        inPlayerTransition = true;

        float delay;
        try { delay = activePlayer.Inventory.SelectedItem.usable.GetComponent<Projectile>().GetTimeToLive(); }
        catch { delay = 2f; }

        if (activePlayer != null)
            activePlayer.Unready();

        activePlayer = null;
        currentPlayer = null;

        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;

            if (delay > 2f && GameObject.FindGameObjectWithTag("Projectile") == null)
                delay = 2f;

            yield return null;
        }

        StartCoroutine(Coroutine_SetNextPlayer());
        inPlayerTransition = false;

        if (Random.Range(0f, 1f) < 1/3f)
            CreateNewAirDrop();

        yield return 0;
    }

    public IEnumerator Coroutine_SetNextPlayer()
    {
        if (GetAlivePlayers() < 2)
        {
            StartCoroutine(Coroutine_EndGame());
            yield break;
        }

        // Find next player
        for (int i = 0; i < players.Count; ++i)
        {
            ++currentPlayerIndex;
            currentPlayerIndex %= players.Count;

            if (players[currentPlayerIndex].gameObject.activeInHierarchy)
                break;
        }

        // Set current player
        currentPlayer = players[currentPlayerIndex];
        currentPlayer.Ready();

        // Delay if bot
        if (players[currentPlayerIndex].Info.control != Control.Player)
            for (float delay = 1.5f; delay > 0f; delay -= Time.deltaTime)
                yield return null;

        // Set active player
        activePlayer = players[currentPlayerIndex];
        yield return 0;
    }

    public void CreateNewAirDrop()
    {
        GenerateSpawnpoints generateSpawnpoints = GameObject.Find("Map").GetComponent<GenerateSpawnpoints>();
        AirDrop airDrop = Instantiate(airDropPrefab, generateSpawnpoints.GetNewSpawnpoint(), Quaternion.identity).GetComponent<AirDrop>();
        airDrop.transform.position += new Vector3(0, 10, 0);
        airDrops.Add(airDrop);
    }

    public void RestartMatch()
    {
        for (int i = players.Count - 1; i >= 0; --i)
            Destroy(players[i].gameObject);
        players.Clear();

        for (int i = airDrops.Count - 1; i >= 0; --i)
            Destroy(airDrops[i].gameObject);
        airDrops.Clear();

        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Projectile"))
            Destroy(gameObject);

        StopAllCoroutines();
        cameraController.Restart();
        MessagesManager.ClearAllMessages();
        StartCoroutine(Coroutine_StartMatch());
    }

    private IEnumerator Coroutine_EndGame()
    {
        MessagesManager.ClearAllMessages();
        currentPlayer = null;

        for (float delay = 1f; delay > 0f; delay -= Time.deltaTime)
            yield return null;

        if (GetAlivePlayers() != 1)
            guiManager.EndScreen.SetText("Draw!");

        else
        {
            int i = -1;
            while (!players[++i].gameObject.activeSelf) ;
            guiManager.EndScreen.SetText(players[i].Info.name + " wins!");
        }

        guiManager.EndScreen.SetOpen(true);
        yield return 0;
    }

    private int GetAlivePlayers()
    {
        int alivePlayers = 0;

        foreach (Player player in players)
            if (player.gameObject.activeSelf)
                ++alivePlayers;

        return alivePlayers;
    }
}
