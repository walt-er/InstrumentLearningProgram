﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MidiJack;

public class InstrumentButtonManager_playthenote : MonoBehaviour
{
    public GameObject buttonList;
    public int[] instrumentButtonPattern;

    private InstrumentButtonBehavior[] buttonListArray;

    private int randomValue;
    public string randomNote;
    public bool randomNoteDetector = false;

    private Dictionary<string, int[]> masterDictionary = new Dictionary<string, int[]> { };
    public PatternMasterList patternMasterList = new PatternMasterList();

    public Text displayRandomNote;
    public Text displayScore;

    private int displayScoreInt = 0;

    void Start()
    {
        buttonListArray = buttonList.GetComponentsInChildren<InstrumentButtonBehavior>();
        Debug.Log("buttonListArray is built");
        instrumentButtonPattern = new int[buttonListArray.Length];
        Debug.Log($"Buttonlist is {buttonListArray.Length} in length");
        masterDictionary = patternMasterList.CreateFluteDictionary();
        Debug.Log($"Master List Created");
        displayScore.text = "Score: " + displayScoreInt;
        GetRandomNote();
        Debug.Log($"Random Note is " + randomNote);
    }
    void Update()
    {
        if(!randomNoteDetector)
        {
            GetInstrumentButtonPattern();
            randomNoteDetector = IsInputAKnownPattern(instrumentButtonPattern, masterDictionary.ElementAt(randomValue).Value);
        }
        else
        {
            displayRandomNote.text = "Correct!";
            displayScoreInt += 10;
            displayScore.text = "Score: " + displayScoreInt;
            StateSwapper();
            Time.timeScale = 0;
        }
    }

    void GetRandomNote()
    {
        randomValue = Random.Range(0, masterDictionary.Count);
        randomNote = masterDictionary.ElementAt(randomValue).Key;
        displayRandomNote.text = "Note: " + randomNote;
    }
    void GetInstrumentButtonPattern()
    {
        int patternPosition = 0;
        foreach (InstrumentButtonBehavior button in buttonListArray)
        {
            //velocity at 0 mean note is released, anything else is pressed
            float velocity = MidiMaster.GetKey(button.midiValue);
            if (velocity > 0)
            {
                instrumentButtonPattern[patternPosition] = 1;
                button.state = "press";
            }
            else
            {
                instrumentButtonPattern[patternPosition] = 0;
                button.state = "release";
            }
            patternPosition++;
        }
    }

    bool IsInputAKnownPattern(int[] inputPattern, int[] randomNote)
    {
        if (inputPattern.SequenceEqual(randomNote))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void StateSwapper()
    {
        //isPattern true turns buttons green (that are pressed) while keeping released buttons blue
        foreach (InstrumentButtonBehavior button in buttonListArray)
        {
            if (button.state == "press")
            {
                button.state = "combo";
            }
        }
    }
}
