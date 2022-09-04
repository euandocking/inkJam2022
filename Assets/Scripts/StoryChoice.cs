using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryChoice : MonoBehaviour
{
    [SerializeField] private StoryManager storyManager;

    private int choiceIndex;

    public void SetChoiceIndex(int index)
    {
        choiceIndex = index;
    }

    public void MakeChoice()
    {
        storyManager.MakeChoice(choiceIndex);
    }
}
