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

    private bool pickingNextPlayer = false;

    public List<Player> Players => players;
    public Player CurrentPlayer => currentPlayer;

    void Start()
    {
        StartCoroutine(Coroutine_StartMatch());
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
                Player player = AddNewPlayer();

                player.Initialize(
                    gameInfo.names[i][0] != '\u200B' ? gameInfo.names[i] : "Player " + players.Count,
                    gameInfo.colors[i],
                    gameInfo.tankPrefabs[i],
                    gameInfo.controls[i],
                    generateSpawnpoints.GetNewSpawnpoint());
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

        if (!pickingNextPlayer)
            StartCoroutine(Coroutine_PlayerTransition());
    }

    private IEnumerator Coroutine_PlayerTransition()
    {
        pickingNextPlayer = true;
        float delay = currentPlayer.Inventory.SelectedItem.prefab.GetComponent<Projectile>().GetTimeToLive();

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
        if (currentPlayer != null)
            currentPlayer.Unready();

        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
            currentPlayerIndex = 0;

        for (int i = currentPlayerIndex; i < players.Count; i++)
        {
            if (players[i].Tank.gameObject.activeInHierarchy)
            {
                currentPlayerIndex = i;
                currentPlayer = players[currentPlayerIndex];
                currentPlayer.Ready();
                return;
            }
        }
    }

}
