using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    private static StoryManager instance;

    [Header("Story UI")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private StoryChoice storyChoice;
    [SerializeField] private Button continueButton;

    //this is awful, should probably just make the choice buttons via script at start
    [SerializeField] private StoryChoice storyChoice0;
    [SerializeField] private StoryChoice storyChoice1;
    [SerializeField] private StoryChoice storyChoice2;
    [SerializeField] private StoryChoice storyChoice3;
    [SerializeField] private StoryChoice storyChoice4;

    [SerializeField] private GameObject spareChoiceButtonContainer;

    private Story story;
    private bool playerInChoice;
    private List<StoryChoice> spareChoiceButtons;
    private List<StoryChoice> activeChoiceButtons;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Dialogue Manager exists in the scene");
        }
        instance = this;
    }

    public static StoryManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        spareChoiceButtons = new List<StoryChoice>();
        spareChoiceButtons.Add(storyChoice0);
        spareChoiceButtons.Add(storyChoice1);
        spareChoiceButtons.Add(storyChoice2);
        spareChoiceButtons.Add(storyChoice3);
        spareChoiceButtons.Add(storyChoice4);

        activeChoiceButtons = new List<StoryChoice>();


        playerInChoice = false;
        story = new Story(inkJSON.text);
        ContinueStory();
    }

    private void Update()
    {
        LayoutRebuilder.MarkLayoutForRebuild(storyPanel.GetComponent<RectTransform>());
    }

    public void ContinueStory()
    {
        if (story.canContinue)
        {
            TextMeshProUGUI tempText = Instantiate(storyText);
            tempText.text = story.Continue();
            tempText.transform.SetParent(storyPanel.transform, false);
            continueButton.transform.SetSiblingIndex(storyPanel.transform.childCount - 1);
            continueButton.Select();
        }
        else if (story.currentChoices.Count > 0 && !playerInChoice)
        {
            continueButton.gameObject.SetActive(false);
            playerInChoice = true;
            int buttonIndex = 0;

            foreach (Choice choice in story.currentChoices)
            {
                //probably add in a check to make sure there are spare choices available
                StoryChoice choiceButton = spareChoiceButtons[0];
                //set choice index for choiceButton to index
                activeChoiceButtons.Add(choiceButton);
                spareChoiceButtons.Remove(choiceButton);

                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                choiceButton.gameObject.SetActive(true);
                choiceButton.SetChoiceIndex(buttonIndex);
                choiceButton.transform.SetParent(storyPanel.transform, false);

                buttonIndex++;

            }
            activeChoiceButtons[0].GetComponent<Button>().Select();
        }
        else 
        {
            Debug.Log("story cannot continue");
            //storyText.text = "The End";
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        foreach (StoryChoice button in activeChoiceButtons)
        {
            button.transform.SetParent(spareChoiceButtonContainer.transform, false);
            button.gameObject.SetActive(false);
            spareChoiceButtons.Add(button);
        }
        activeChoiceButtons.Clear();

        story.ChooseChoiceIndex(choiceIndex);
        playerInChoice = false;
        ContinueStory();
        continueButton.gameObject.SetActive(true);
    }
}
