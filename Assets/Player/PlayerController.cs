
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerOrder;
    [SerializeField] private List<int> onlyIndex;
    [SerializeField] private GameConductor theConductor;
    [SerializeField] private Toggle[] theHand;
    [SerializeField] private TableUI artDict;
    
    public Button submitButton;
    public Button discardButton;
    
    /*debug*/
    //[SerializeField] private List<string> myHand;

    
    
    
    private GameConductor.Player thePlayer;
    void OnEnable()
    {
        //Register Button Events
        submitButton.onClick.AddListener(() => ButtonTrySubmit());
        discardButton.onClick.AddListener(() => ButtonTryDiscard());
        theConductor.onPlayerHandsChanged += DrawHand;
    }

    private void ButtonTrySubmit()
    {
        Debug.Log($"Player{thePlayer.PlayerOrder} tried submit");
        List<int> selected = DetectSelectedTiles();
        if (selected.Count < 1) return;
        theConductor.SubmitSelectedTile(thePlayer, selected);
        ResetSelectedTiles();
    }
    private void ButtonTryDiscard()
    {
        Debug.Log($"Player{thePlayer.PlayerOrder} tried discard");
        List<int> selected = DetectSelectedTiles();
        theConductor.DiscardSelectedTile(thePlayer, selected);
        ResetSelectedTiles();
    }
    
    private List<int> DetectSelectedTiles()
    {
        List<int> selected = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            if (theHand[i].isOn)
            {
                selected.Add(i);
            }
        }

        return selected;
    }
    
    private void ResetSelectedTiles()
    {
        for (int i = 0; i < 8; i++)
        {
            theHand[i].isOn = false;
        }
    }
    void OnDisable()
    {
        //Un-Register Button Events
        submitButton.onClick.RemoveAllListeners();
        discardButton.onClick.RemoveAllListeners();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void DrawHand()
    {
        if (thePlayer == null)
        {
            GetPlayer();
        }
        List<string> names = thePlayer.PlayerHand;
        int i = 0;
        while (i < names.Count)
        {
            theHand[i].gameObject.SetActive(true);
            theHand[i].GetComponentInChildren<Image>().sprite = artDict.GetTileSprite(names[i]);
            i++;
        }
        while(i < 8)
        {
            theHand[i].gameObject.SetActive(false);
            i++;
        }
    }
    
    private void GetPlayer()
    {
        thePlayer = theConductor.GetPlayer(playerOrder);
    }
   
}
