using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tanks
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] List<string> playerNames;
        [SerializeField] List<TankType> tankTypes;
        [SerializeField] List<GameObject> tankPrefabs;
        //[SerializeField] Dictionary<TankType, GameObject> tankPrefabs;
        [SerializeField] List<Tank> spawnedTanks;
        [SerializeField] List<Color> playerColors;
        [SerializeField] int currentPlayerIndex;
        [SerializeField] int playerCount;
        [SerializeField] Tank currentTank;
        [SerializeField] CameraController cameraController;
        [SerializeField] GameInfo gameInfo;
        [SerializeField] TMP_Text playerNameText;
        [SerializeField] GenerateSpawnpoints gameSpawnpoints;
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
            playerNames = new List<string>();
            spawnedTanks = new List<Tank>();
            playerColors = new List<Color>();
            playerNameText = GameObject.Find("GUI").transform.Find("CurrentPlayer").GetComponent<TMP_Text>();
            gameInfo = GameObject.Find("Game Info").GetComponent<GameInfo>();
            StartCoroutine(Coroutine_StartMatch());
        }

        private IEnumerator Coroutine_StartMatch()
        {
            float delay = 1.0f;

            RetrieveInfo();

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

        void RetrieveInfo()
        {
            for (int i = 0; i < gameInfo.names.Count; ++i)
            {
                playerNames.Add(gameInfo.names[i].ToString());
                tankTypes.Add(gameInfo.tankTypes[i]);
                playerColors.Add(gameInfo.colors[i]);
            }
        }

        void SpawnTanks()
        {
            for (int i = 0; i < gameInfo.names.Count; ++i)
            {
                Tank spawnedTank = tankPrefabs[0].GetComponent<Tank>();
                spawnedTanks.Add(spawnedTank);
                gameSpawnpoints.GenerateTanks(spawnedTanks.Count, spawnedTank.gameObject);
            }


            //for(int i = 0; i < tankTypes.Count; ++i)
            //{
            //    if(tankPrefabs.ContainsKey(tankTypes[i]))
            //    {
            //        Tank spawnedTank = tankPrefabs[tankTypes[i]].GetComponent<Tank>();
            //        spawnedTanks.Add(spawnedTank);
            //    }
            //}
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

        public void Fire()
        {
            if (currentTank.CanFire())
                currentTank.Fire();
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
    }

}
