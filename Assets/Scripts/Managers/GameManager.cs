using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GamePhase
{
    Start,
    Lobby,
    Upgrade,
    Combat
}
public class GameManager : MonoBehaviour
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Start;

    public static GameManager Instance { get; private set; }

    private List<GameObject> playerList = new List<GameObject>();
    private int playersReady;
    private List<GameObject> redTeam = new List<GameObject>();
    private List<GameObject> blueTeam = new List<GameObject>();
    private bool redCanSpawn = true, blueCanSpawn = true;

    private List<GameObject> crystalList = new List<GameObject>();
    private List<GameObject> redCrystals = new List<GameObject>();
    private List<GameObject> blueCrystals = new List<GameObject>();

    public Vector3 redSpawnPoint, blueSpawnPoint, lobbySpawnPoint;

    private int redScore = 0, blueScore = 0;
    private bool inLobby = false;

    private List<UpgradeCard> upgradeCards = new List<UpgradeCard>();
    private System.Random rng = new System.Random(); // Random generator for better randomness
    private Sprite currCardVisual;
    public delegate void GameEvent();
    public static event GameEvent UpgradeEvent, CombatPhaseStart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUpgradeCards();
        TransitionToNextPhase();
        redSpawnPoint = GameObject.Find("RedSpawn").transform.position;
        blueSpawnPoint = GameObject.Find("BlueSpawn").transform.position;
        lobbySpawnPoint = GameObject.Find("LobbySpawn").transform.position;
    }

    private void InitializeUpgradeCards()
    {
        currCardVisual = Resources.Load<Sprite>("CardVisuals/AsolUpgrade");
        upgradeCards.Add(new UpgradeCard(currCardVisual, typeof(AsolUpgrade), "Surround yourself with 2 orbs of death, each dealing 10 damage"));
        currCardVisual = Resources.Load<Sprite>("CardVisuals/ShieldUpgrade");
        upgradeCards.Add(new UpgradeCard(currCardVisual, typeof(ShieldUpgrade), "Surround yourself with a forcefield which can absorb 100 damage"));
        currCardVisual = Resources.Load<Sprite>("CardVisuals/RegenUpgrade");
        upgradeCards.Add(new UpgradeCard(currCardVisual, typeof(RegenUpgrade), "Heal 10 health every 5 seconds"));
    }

    public List<UpgradeCard> GetRandomUpgradeCards(int numberOfCards)
    {
        List<UpgradeCard> randomCards = new List<UpgradeCard>();

        if (upgradeCards.Count <= numberOfCards)
        {
            // If there are fewer or equal cards than requested, return duplicates of all cards
            foreach (UpgradeCard card in upgradeCards)
            {
                randomCards.Add(card.Clone());
            }
            return randomCards;
        }

        List<int> selectedIndexes = new List<int>();
        while (selectedIndexes.Count < numberOfCards)
        {
            int randomIndex = rng.Next(upgradeCards.Count); // Get a random index

            if (!selectedIndexes.Contains(randomIndex)) // Ensure we don't select the same card twice
            {
                selectedIndexes.Add(randomIndex);
                randomCards.Add(upgradeCards[randomIndex].Clone()); // Add a clone of the selected card
            }
        }

        return randomCards;
    }

    public void StartUpgradePhase()
    {
        Debug.Log("Upgrade phase started.");
        CurrentPhase = GamePhase.Upgrade;
        UpgradeEvent?.Invoke();
        playersReady = playerList.Count;
        // Add logic specific to the Upgrade Phase
        // For example: enable upgrade UI, stop combat logic, etc.
    }

    public void PlayerReadyUp()
    {
        playersReady--;
        if (playersReady == 0)
        {
            TransitionToNextPhase();
        }
    }

    public void StartCombatPhase()
    {
        Debug.Log("Combat phase started.");
        CombatPhaseStart?.Invoke();
        CurrentPhase = GamePhase.Combat;
        InitializePlayers();
        InitializeCrystals();

        // Add logic specific to the Combat Phase
        // For example: spawn enemies, enable combat logic, etc.
    }

    public void StartLobbyPhase()
    {
        Debug.Log("Lobby phase STARTED");
        CurrentPhase = GamePhase.Lobby;
        inLobby = true;
        GameObject.Find("PlayerManager").GetComponent<PlayerInputManager>().EnableJoining();
        foreach (GameObject p in playerList)
        {
            p.SetActive(true);
            p.GetComponent<PlayerClass>().Respawn(lobbySpawnPoint, 0);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0) && inLobby == true)
        {
            inLobby = false;
            StartUpgradePhase();
        }

    }

    public void TransitionToNextPhase()
    {
        if (CurrentPhase == GamePhase.Upgrade)
        {
            StartCombatPhase();
        }
        else if (CurrentPhase == GamePhase.Start)
        {
            StartLobbyPhase();
        }
        else if (CurrentPhase == GamePhase.Combat)
        {
            if( redScore >= 3 || blueScore >= 3)
            {
                StartLobbyPhase();
            }
            else
            {
                StartUpgradePhase();
            }
        }
        else if (CurrentPhase == GamePhase.Lobby)
        {
            redScore = 0;
            blueScore = 0;
            GameObject.Find("PlayerManager").GetComponent<PlayerInputManager>().DisableJoining();
            StartCombatPhase();
        }
    }

    private void InitializePlayers() {
        redTeam.Clear();
        blueTeam.Clear();
        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<PlayerClass>().GetTeam() == teamName.red)
            {
                redTeam.Add(player);
                player.GetComponent<PlayerClass>().Respawn(redSpawnPoint.position, 0);
            }
            if (player.GetComponent<PlayerClass>().GetTeam() == teamName.blue)
            {
                blueTeam.Add(player);
                player.GetComponent<PlayerClass>().Respawn(blueSpawnPoint.position, 0);
            }
        }
    }
    public void AddPlayer(GameObject player)
    {
        playerList.Add(player);
    }

    public void PlayerDie(GameObject player)  ////DO THISS NEEEEEEEEEEEEEEEEXT just redo
    {
        player.SetActive(false);
        if (player.GetComponent<PlayerClass>().GetTeam() == teamName.red)
        {
            if (redCanSpawn)
            {
                player.GetComponent<PlayerClass>().Respawn(redSpawnPoint.position, 5);
            }
            else
            {
                redTeam.Remove(player);
                if (redTeam.Count == 0)
                {
                    blueScore += 1;
                    TransitionToNextPhase();
                }
            }
        }
        if (player.GetComponent<PlayerClass>().GetTeam() == teamName.blue)
        {
            if (blueCanSpawn)
            {
                player.GetComponent<PlayerClass>().Respawn(blueSpawnPoint.position, 5);
            }
            else
            {
                blueTeam.Remove(player);
                if (blueTeam.Count == 0)
                {
                    redScore += 1;
                    TransitionToNextPhase();
                }
            }
        }
    }

    private void InitializeCrystals()
    {
        redCrystals.Clear();
        blueCrystals.Clear();
        redCanSpawn = true; blueCanSpawn = true;
        foreach (GameObject c in crystalList)
        {
            if (c.GetComponent<CrystalScript>().GetTeam() == teamName.red)
            {
                redCrystals.Add(c);
            }
            if (c.GetComponent<CrystalScript>().GetTeam() == teamName.blue)
            {
                blueCrystals.Add(c);
            }
            c.GetComponent<CrystalScript>().Spawn();
        }

    }

    public void AddCrystal(GameObject crystal)
    {
        crystalList.Add(crystal);
    }

    public void DestroyCrystal(GameObject crystal)
    {
        crystal.SetActive(false);
        if (crystal.GetComponent<CrystalScript>().GetTeam() == teamName.red)
        {
            redTeam.Remove(crystal);
            if (redTeam.Count == 0)
            {
                redCanSpawn = false;
            }
        }
        if (crystal.GetComponent<CrystalScript>().GetTeam() == teamName.blue)
        {
            blueTeam.Remove(crystal);
            if (blueTeam.Count == 0)
            {
                blueCanSpawn = false;
            }
        }
    }

    public teamName AssignTeam(GameObject player)
    {
        if (redTeam.Count <= blueTeam.Count)
        {
            redTeam.Add(player);
            return teamName.red;
        }
        else
        {
            blueTeam.Add(player);
            return teamName.blue;
        }
    }
}
