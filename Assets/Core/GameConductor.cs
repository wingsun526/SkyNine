using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;


public class GameConductor : MonoBehaviour //singleton
{
    public Player Player0 { get; private set; }
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    public Player Player3 { get; private set; }
    

    private List<Player> listOfPlayers;
    private Random rng;
    private Coroutine onGoingRound;


    public event Action onRoundStart;
    public event Action onPlayerHandsChanged;
    public event Action<int> onActivePlayerChanged;
    public event Action<int, int, List<string>> onSuccessfulRemove;
    public event Action onGameEnded;



    

    public Player GetPlayer(int order)
    {
        if(listOfPlayers == null)
        {
            throw new Exception("list not instantiate yet");
        }

        return listOfPlayers[order];
    }

    public class Player
    {
        public int PlayerOrder { get;}
        private int _tricksWon = 0;
        public List<string> PlayerHand { get; set; }

        public Player(int order)
        {
            PlayerOrder = order;
            
        }

        
        public void AddTricksWon(int number)
        {
            _tricksWon += number;
        }
        public int GetTricksWon()
        {
            return _tricksWon;
        }
        
    }

    private Player CurrentActivePlayer { get; set; }
    private bool _currentActivePlayerMadeTheMove = false;

    // Start is called before the first frame update
    void Awake()
    {
        rng = new Random();
        
        // init player order when constructing
        Player0 = new Player(0);
        Player1 = new Player(1);
        Player2 = new Player(2);
        Player3 = new Player(3);
        
        //distribute 
        listOfPlayers = new List<Player>() {Player0 ,Player1, Player2, Player3};

    }

    private void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() // A Game consist of multiple rounds
    {
        if(onGoingRound != null)
        {
            throw new Exception("there is a gaming still on going");
        }
        //Shuffle and Distribute/populate pool
        string[] pool = Judge.GetSkyNinePool();
        Shuffle(rng, pool);
        Deal(pool, listOfPlayers);
        
        
        onGoingRound = StartCoroutine(StartRound(0));
    }
    
    private List<string> _currentRoundWinningTrick;
    private Player _currentRoundWinningPlayer;
    private void UpdateWinningTrickAndPlayer(Player thePlayer, List<string> winningTrick)
    {
        _currentRoundWinningTrick = winningTrick;
        _currentRoundWinningPlayer = thePlayer;
    }

    private void ResetRound()
    {
        _currentRoundWinningTrick = null;
        _currentRoundWinningPlayer = null;
        if (onRoundStart != null)
        {
            onRoundStart();
        }
    }

    IEnumerator StartRound(int roundStarter) // this argument would be PlayerOrder
    {
        int roundOrder = 0;
        ResetRound();
        
        while (roundOrder < 4)
        {
            int nextPlayer = (roundOrder + roundStarter) % 4;
            NextPlayerIsGoingToBe(nextPlayer);
            
            _currentActivePlayerMadeTheMove = false; //submit button can change this with ref/out ?
            
            // when in final round, skip player who has no winning yet.
            if (CurrentActivePlayer.PlayerHand.Count == 1 && CurrentActivePlayer.GetTricksWon() == 0)
            {
                RemoveTilesFromPlayerHand(CurrentActivePlayer, new List<int>(){0});
                //CurrentActivePlayer.PlayerHand.RemoveAt(0);
                roundOrder++;
                continue;
            }

            while (_currentActivePlayerMadeTheMove == false)
            {
                yield return null;
            }

            roundOrder++;
        }
        EndRound();

        yield return new WaitForSeconds(1f);
        // if there are still more round
        if(_currentRoundWinningPlayer.PlayerHand.Count != 0)
        {
            int nextRoundStarter = _currentRoundWinningPlayer.PlayerOrder;
            onGoingRound = StartCoroutine(StartRound(nextRoundStarter));
        }
        else
        {
            Debug.Log($"winner is player{_currentRoundWinningPlayer.PlayerOrder}");
            if (onGameEnded != null) onGameEnded();
            onGoingRound = null;
        }
        
    }

    

    private void EndRound()
    {
        //add winnings to best players
        _currentRoundWinningPlayer.AddTricksWon(_currentRoundWinningTrick.Count);
        
        
    }


    private void NextPlayerIsGoingToBe(int roundOrder)
    {
        CurrentActivePlayer = listOfPlayers[roundOrder];

        if (onActivePlayerChanged != null) onActivePlayerChanged(roundOrder);

        // enable player control function() / disable player control function()

        //display arrow pointing to active player
    }
    
    private bool TryBeatWinningTrick(List<int> submissionValues, Judge.TrickType submissionType)
    {
        //if no winning trick is present
        if (_currentRoundWinningTrick == null) return true;

        List<int> winningTrickValue = GetValuesOfTiles(_currentRoundWinningTrick);
        Judge.TrickType winningTrickType = Judge.DetectTrickType(winningTrickValue);

        return Judge.IsTrickStronger(winningTrickValue, winningTrickType, submissionValues, submissionType);


    }

    public void SubmitSelectedTile(Player thePlayer, List<int> indices) // a button // beware of clicking button in quick succession
    {
        if (thePlayer != CurrentActivePlayer)
        {
            throw new Exception("not your turn");
        }

        //convert to actual tiles from index
        List<string> tileNames = new List<string>();
        foreach (int i in indices)
        {
            if (thePlayer.PlayerHand[i] == null)
            {
                throw new Exception();
            }
            tileNames.Add(thePlayer.PlayerHand[i]);
        }
        
        List<int> tileValues = GetValuesOfTiles(tileNames);
        // check if submission is a valid trick type
        var type = Judge.DetectTrickType(tileValues);
        
        if(type == null)
        {
            Debug.Log("unknown trick type");
            return;
        }
        
        // when trick type is valid
        bool canBeat = TryBeatWinningTrick(tileValues, type);

        if (canBeat)
        {
            // Function current made valid move();
            UpdateWinningTrickAndPlayer(thePlayer, tileNames);
            RemoveTilesFromPlayerHand(thePlayer, indices, tileNames);
            _currentActivePlayerMadeTheMove = true;
        }
    }
    
    public void DiscardSelectedTile(Player thePlayer, List<int> index)
    {
        if (thePlayer != CurrentActivePlayer)
        {
            Debug.Log("not your turn");
            return;
        }
        if (_currentRoundWinningTrick == null)
        {
            Debug.Log("you are free to play whatever");
            return;
        }
        if(_currentRoundWinningTrick.Count != index.Count)
        {
            Debug.Log("wrong number of tiles");
            return;
        }

        _currentActivePlayerMadeTheMove = true;
        
        RemoveTilesFromPlayerHand(thePlayer, index);
        

    }
    
    private void RemoveTilesFromPlayerHand(Player thePlayer, List<int> indices, List<string> tileNames = null) /* does not check for submission validity */
    {
        foreach(int i in indices.OrderByDescending(v => v)) /*look at this sorting method if something goes wrong*/
        {
            thePlayer.PlayerHand.RemoveAt(i);
        }
        //Remove Tiles are place on display zone
        if (onSuccessfulRemove != null)
        {
            onSuccessfulRemove(thePlayer.PlayerOrder, indices.Count, tileNames);
        }
        if (onPlayerHandsChanged != null) onPlayerHandsChanged();
        //storing discarded tricks for AI use later
    }

    
    
    #region Tiles

    private static void Shuffle(Random rng, string[] array) /*Fisher-Yates algorithm*/
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
    
    private void Deal(string[] pool, List<Player> players)
    {
        // empty player hand
        foreach (Player player in players)
        {
            player.PlayerHand = new List<string>();
        }
        
        //deal
        for (int i = 0; i < pool.Length; i++)
        {
            players[i % 4].PlayerHand.Add(pool[i]);
        }
        
        // Sort
        
        
        if (onPlayerHandsChanged != null) onPlayerHandsChanged();
    }

    private List<int> GetValuesOfTiles(List<string> theTileList)
    {
        if (theTileList.Count == 0)
        {
            throw new Exception("empty list");
        }
        
        List<int> theValues = new List<int>();
        foreach (string nameOfTile in theTileList)
        {
            theValues.Add(Judge.GetTileValue(nameOfTile));
        }

        return theValues;
    }

    #endregion

    
    public void LogCurrentGameStats()
    {
        Debug.Log($"Current Active Player: {CurrentActivePlayer.PlayerOrder}");
        if (_currentRoundWinningTrick != null)
        {
            Debug.Log("current winning trick is");
            foreach (string name in _currentRoundWinningTrick)
            {
                Debug.Log($"{name}");
            }
            Debug.Log($"from player {_currentRoundWinningPlayer.PlayerOrder}");
        }
        else
        {
            Debug.Log("no one played yet");
        }
    }

    
}