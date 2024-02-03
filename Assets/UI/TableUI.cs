using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TableUI : MonoBehaviour
{
    
    [SerializeField] private Sprite A;
    [SerializeField] private Sprite B;
    [SerializeField] private Sprite C;
    [SerializeField] private Sprite D;
    [SerializeField] private Sprite E;
    [SerializeField] private Sprite F;
    [SerializeField] private Sprite G;
    [SerializeField] private Sprite H;
    [SerializeField] private Sprite I;
    [SerializeField] private Sprite J;
    [SerializeField] private Sprite K;
    [SerializeField] private Sprite NineA;
    [SerializeField] private Sprite NineB;
    [SerializeField] private Sprite EightA;
    [SerializeField] private Sprite EightB;
    [SerializeField] private Sprite SevenA;
    [SerializeField] private Sprite SevenB;
    [SerializeField] private Sprite Six;
    [SerializeField] private Sprite FiveA;
    [SerializeField] private Sprite FiveB;
    [SerializeField] private Sprite Three;
    [SerializeField] private Sprite Back;
    
    [SerializeField] private GameObject DisplayZone0;
    [SerializeField] private GameObject DisplayZone1;
    [SerializeField] private GameObject DisplayZone2;
    [SerializeField] private GameObject DisplayZone3;
    private List<Image[]> cellArrayList;
    private Image[] cellArray0; //there should be 4 
    private Image[] cellArray1; //there should be 4 
    private Image[] cellArray2; //there should be 4 
    private Image[] cellArray3; //there should be 4 
    private Dictionary<string, Sprite> spriteDictionary;
    
    [SerializeField] private Image[] arrows;
    [SerializeField] private Button startGameButton;
    private void Awake()
    {
        spriteDictionary = new Dictionary<string, Sprite>()
        {
            // civil
            {"A", A},
            {"B", B},
            {"C", C},
            {"D", D},
            {"E", E},
            {"F", F},
            {"G", G},
            {"H", H},
            {"I", I},
            {"J", J},
            {"K", K},
            // military
            {"NineA", NineA},
            {"NineB", NineB},
            {"EightA", EightA},
            {"EightB", EightB},
            {"SevenA", SevenA},
            {"SevenB", SevenB},
            {"Six", Six},
            {"FiveA", FiveA},
            {"FiveB", FiveB},
            {"Three", Three}
        };
        
        cellArray0 = DisplayZone0.GetComponentsInChildren<Image>();
        cellArray1 = DisplayZone1.GetComponentsInChildren<Image>();
        cellArray2 = DisplayZone2.GetComponentsInChildren<Image>();
        cellArray3 = DisplayZone3.GetComponentsInChildren<Image>();
        cellArrayList = new List<Image[]>() { cellArray0, cellArray1, cellArray2, cellArray3 };
    }

    private void Start()
    {
        startGameButton.onClick.AddListener(CallStartGame);
    }

    public Sprite GetTileSprite(string tileName)
    {
        if (spriteDictionary.ContainsKey(tileName))
        {
            return spriteDictionary[tileName];
        }
        else
        {
            throw new Exception("no tile name");
        }
        
    }
    private void OnEnable()
    {
        GetComponent<GameConductor>().onSuccessfulRemove += DisplayRemovedTiles;
        GetComponent<GameConductor>().onRoundStart += ClearDisplayZone;
        GetComponent<GameConductor>().onActivePlayerChanged += ChangeArrowDirection;
        GetComponent<GameConductor>().onGameEnded += EnableStartGameButton;
    }

    private void OnDisable()
    {
        GetComponent<GameConductor>().onSuccessfulRemove -= DisplayRemovedTiles;
        GetComponent<GameConductor>().onRoundStart -= ClearDisplayZone;
        GetComponent<GameConductor>().onGameEnded -= EnableStartGameButton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DisplayRemovedTiles(int playerOrder, int numberOfItems, List<string> tileNames)
    {
        //tile names can be null
        int cellIndex = 0;
        Image[] cellArray = cellArrayList[playerOrder];
        while (cellIndex < numberOfItems)
        {
            Sprite theSprite = tileNames == null ? Back : spriteDictionary[tileNames[cellIndex]]; //if tileNames == null, display the back side of tile
            cellArray[cellIndex].sprite = theSprite;
            cellArray[cellIndex].enabled = true;
            cellIndex++;
        }

        // rest will be disable
        while (cellIndex < 4)
        {
            cellArray[cellIndex].enabled = false;
            cellIndex++;
        }
    }
    
    
    private void ClearDisplayZone()
    {
        foreach (Image[] arr in cellArrayList)
        {
            foreach (Image image in arr)
            {
                image.enabled = false;
            }
        }
    }

    private void ChangeArrowDirection(int position)
    {
        foreach (Image arrow in arrows)
        {
            arrow.enabled = false;
        }

        arrows[position].enabled = true;
    }

    private void CallStartGame()
    {
        GetComponent<GameConductor>().StartGame();
        startGameButton.gameObject.SetActive(false);
    }
    private void EnableStartGameButton()
    {
        startGameButton.gameObject.SetActive(true);
    }
}
