using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Player> players;
    [SerializeField] private Player currentPlayer;
    [SerializeField] private int currentPlayerIndex;
    [SerializeField] private CameraController cameraController;

    private bool pickingNextPlayer = false;

    public List<Player> Players => players;
    public Player CurrentPlayer => currentPlayer;

    void Start()
    {
        StartCoroutine(Coroutine_StartMatch());
    }

    private IEnumerator Coroutine_StartMatch()
    {
        float delay = 1.0f;

        GameInfo gameInfo = GameObject.Find("Game Info").GetComponent<GameInfo>();

        for (int i = 0; i < gameInfo.names.Count; ++i)
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.name = gameInfo.names[i];
            playerInfo.color = gameInfo.colors[i];
            playerInfo.tankType = gameInfo.tankTypes[i];
            playerInfo.control = gameInfo.controls[i];

            AddNewPlayer(playerInfo);
        }

        while (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        currentPlayerIndex = 0;
        currentPlayer = players[currentPlayerIndex];
        currentPlayer.Ready();
        yield return 0;
    }

    public void AddNewPlayer(PlayerInfo playerInfo)
    {
        Player player = Instantiate(playerPrefab).GetComponent<Player>();
        player.Initialize(playerInfo);
        players.Add(player);
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
