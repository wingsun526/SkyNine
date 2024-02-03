using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Judge
{
    private static readonly string[] skyNinePool = {"A","A","B","B","C","C","D","D","E","E","F","F","G","G","H","H","I","I","J","J","K","K","NineA","NineB","EightA","EightB","SevenA","SevenB","Six","FiveA","FiveB","Three"};
    private static readonly Dictionary<string, int> tileValueDictionary = new Dictionary<string, int>()
    {
        // civil
        {"A", 2},
        {"B", 3},
        {"C", 5},
        {"D", 11},
        {"E", 13},
        {"F", 17},
        {"G", 19},
        {"H", 23},
        {"I", 29},
        {"J", 31},
        {"K", 37},
        // military
        {"NineA", -2},
        {"NineB", -2},
        {"EightA", -3},
        {"EightB", -3},
        {"SevenA", -5},
        {"SevenB", -5},
        {"Six", -7},
        {"FiveA", -11},
        {"FiveB", -11},
        {"Three", -13},
    };

    public static readonly TrickType BigFour = new TrickType( "big four", true, new int[]{ 16, 81, 625, 14641});
    public static readonly TrickType CivilThree = new TrickType( "civil three",false, new int[]{ -8, -27, -125, -1331 });
    public static readonly TrickType MilitaryThree = new TrickType( "military three", true,new int[]{ 8, 27, 125, 1331 });
    public static readonly TrickType MixedPair = new TrickType( "mixed pair", false,new int[]{ -4, -9, -25, -121});
    public static readonly TrickType MilitaryPair = new TrickType("military pair", true);
    public static readonly TrickType CivilPair = new TrickType("civil pair", true);
    public static readonly TrickType Supreme = new TrickType("supreme", true); // does not matter
    public static readonly TrickType SingleMilitary = new TrickType("Single Military", false);
    public static readonly TrickType SingleCivil = new TrickType("Single Civil", true);


    public static string[] GetSkyNinePool()
    {
        string[] tempArray = new string[32];
        skyNinePool.CopyTo(tempArray, 0);
        return tempArray;
    }
    
    
    // Rules

    public static bool IsTrickStronger(List<int> winningTrick, TrickType winningType, List<int> newTrick, TrickType newTrickType)
    {
        if (winningType == null || newTrickType == null)
        {
            throw new Exception("trick type is null");
        }
        
        if (winningType != newTrickType)
        {
            Debug.Log("wrong trick type");
            return false;
        }

        int winningProduct = CalTrickProduct(winningTrick);
        int newProduct = CalTrickProduct(newTrick);
        
        if (winningType.IsSmallToLarge)
        {
            if (newProduct < winningProduct) return true;
        }
        else
        {
            if (newProduct > winningProduct) return true;
        }
        
        Debug.Log("winning trick is still greater");
        return false;
    }
    
    public static TrickType DetectTrickType(List<int> suitList) // product of entries must match entry in "pool", // Assuming all individual suit are valid and no extra duplicate.  
    {
        //check for suit number = 0
        int numberOfSuits = suitList.Count;
        if (numberOfSuits <= 0 || numberOfSuits > 4)
        {
            Debug.Log("must be between a combination of 1 to 4 suits");
            return null;
        }

        int suitsProduct = CalTrickProduct(suitList);
        
        
        if (numberOfSuits == 4)
        {
            if (BigFour.PossibleCombination.Contains(suitsProduct))
            {
                return BigFour;
            }
        } 
        else if (numberOfSuits == 3)
        {
            if(CivilThree.PossibleCombination.Contains(suitsProduct)) // add >0 to reduce search time
            {
                return CivilThree;
            }
            else if(MilitaryThree.PossibleCombination.Contains(suitsProduct))
            {
                return MilitaryThree;
            }
        }
        else if(numberOfSuits == 2)
        {
            if(suitList[0] == suitList[1]) //pair
            {
                if (suitList[0] < 0) //military pair
                {
                    return MilitaryPair;
                }
                else // civil pair
                {
                    return CivilPair;
                }
            }
            if(MixedPair.PossibleCombination.Contains(suitsProduct))
            {
                return MixedPair;
            }
            if(suitsProduct == 91)
            {
                return Supreme;
            }
        }
        else if(numberOfSuits == 1)
        {
            if (suitList[0] < 0) return SingleMilitary;
            if (suitList[0] > 0) return SingleCivil;
        }
        return null;
    }

    public class TrickType
    {
        public int[] PossibleCombination { get;}
        public string TrickName { get; }
        
        public bool IsSmallToLarge { get; }

        public TrickType(string name, bool isSmallToLarge, int[] combination = null)
        {
            PossibleCombination = combination;
            TrickName = name;
            IsSmallToLarge = isSmallToLarge;
        }
    }

    public static int GetTileValue(string tile)
    {
        if(!tileValueDictionary.ContainsKey(tile))
        {
            throw new Exception("cannot get the value of this tile");
        }

        return tileValueDictionary[tile];
    }
    private static int CalTrickProduct(List<int> suitList)
    {
        int suitsProduct = 1;
        foreach (int suit in suitList)
        {
            suitsProduct *= suit;
        }
        return suitsProduct;
    }
    
}


