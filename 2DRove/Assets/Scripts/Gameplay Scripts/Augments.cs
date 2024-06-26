using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Augments : MonoBehaviour
{
    public Button[] augmentCards;
    public Button rerollButton;
    public Button augmentDebugButton;
    public Button augmentOverlayCloseButton;
    public Canvas augmentOverlayCanvas;
    public Canvas augmentListOverlay;
    public static List<AugmentObject> chosenAugments;
    private List<AugmentObject> shufflePool;
    private List<AugmentObject> currentChoiceAugments;

    void Start()
    {
        // Shuffle pool for augment selection
        shufflePool = new List<AugmentObject>();
        //List of augments chosen by the randomizer
        currentChoiceAugments = new List<AugmentObject>();
        // List of augments chosen by the player
        chosenAugments = new List<AugmentObject>();
        //List of augments chosen by the player
        rerollButton.onClick.AddListener(Reroll);
        // Augment debug button
        augmentDebugButton.onClick.AddListener(showAugmentOverlay);
        // Augment overlay close button
        augmentOverlayCloseButton.onClick.AddListener(closeAugmentOverlay);
        // initial reroll call to populate the first three augments
        Reroll();
    }
    // Shows augment display
    void showAugmentOverlay()
    {
        pause();
        if(augmentListOverlay.gameObject.activeSelf)
        {
            augmentListOverlay.gameObject.SetActive(false);
        }
        augmentOverlayCanvas.gameObject.SetActive(true);
    }
    // Closes augment display 
    void closeAugmentOverlay()
    {
        augmentOverlayCanvas.gameObject.SetActive(false);
        unpause();
    }
    //Adds augment to list of chosen augments
    void addAugment(AugmentObject augmentClicked)
    {
        // Add the selected augment to the list of chosen augments
        chosenAugments.Add(augmentClicked);
        // Remove the selected augment from the list of current choice augments
        augmentOverlayCanvas.gameObject.SetActive(false);
        //Removes chosen augments
        switch(augmentClicked.augmentRarity)
        {
            case "Common":
                if(!augmentClicked.isStackable)
                    AugmentInstantiator.commonAugmentDictionary.Remove(augmentClicked.augmentName);
                break;
            case "Rare":
                if(!augmentClicked.isStackable)
                    AugmentInstantiator.rareAugmentDictionary.Remove(augmentClicked.augmentName);
                break;
            case "Epic":
                if(!augmentClicked.isStackable)
                    AugmentInstantiator.epicAugmentDictionary.Remove(augmentClicked.augmentName);
                break;
            case "Legendary":
                if(!augmentClicked.isStackable)
                    AugmentInstantiator.legendaryAugmentDictionary.Remove(augmentClicked.augmentName);
                break;
        }
        AugmentInstantiator.callAugmentMethod(augmentClicked);
        Reroll();
        unpause();
    }

    void pause()
    {
        Time.timeScale = 0;
    }

    void unpause()
    {
        Time.timeScale = 1;
    }

    // Populates augments from the list of choice augments available
    void Reroll()
    {
        currentChoiceAugments.Clear();

        // Create a new list for the shuffle pool
        List<AugmentObject> shufflePool = new List<AugmentObject>();

        // Add all augments to the shuffle pool
        shufflePool.AddRange(AugmentInstantiator.commonAugmentDictionary.Values);
        
        // Add other augments to the shuffle pool based on their rarity
        System.Random rand = new System.Random();
        foreach(var augment in AugmentInstantiator.rareAugmentDictionary.Values)
        {
            if(rand.NextDouble() < 0.20) // 20% chance
            {
                shufflePool.Add(augment);
            }
        }
        foreach(var augment in AugmentInstantiator.epicAugmentDictionary.Values)
        {
            if(rand.NextDouble() < 0.10) // 10% chance
            {
                shufflePool.Add(augment);
            }
        }
        foreach(var augment in AugmentInstantiator.legendaryAugmentDictionary.Values)
        {
            if(rand.NextDouble() < 0.05) // 5% chance
            {
                shufflePool.Add(augment);
            }
        }
        //Shuffle the augments
        for(int i = 0; i < shufflePool.Count; i++)
        {
            AugmentObject temp = shufflePool[i];
            int randomIndex = Random.Range(i, shufflePool.Count);
            shufflePool[i] = shufflePool[randomIndex];
            shufflePool[randomIndex] = temp;
        }

        // Assign the first three augments to the buttons
        for(int i = 0; i < 3; i++)
        {
            currentChoiceAugments.Add(shufflePool[i]);
            Sprite newSprite = null;
            switch(currentChoiceAugments[i].augmentRarity)
            {
                case "Common":
                    augmentCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Augments/Rarity/common");
                    newSprite = Resources.Load<Sprite>("Augments/Rarity/common_clicked");
                    break;
                case "Rare":
                    augmentCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Augments/Rarity/rare");
                    newSprite = Resources.Load<Sprite>("Augments/Rarity/rare_clicked");
                    break;
                case "Epic":
                    augmentCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Augments/Rarity/epic");
                    newSprite = Resources.Load<Sprite>("Augments/Rarity/epic_clicked");
                    break;
                case "Legendary":
                    augmentCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Augments/Rarity/legendary");
                    newSprite = Resources.Load<Sprite>("Augments/Rarity/legendary_clicked");
                    break;
            }
            // Get the Button component
            Button button = augmentCards[i].GetComponent<Button>();
            // Create a new SpriteState
            SpriteState spriteState = new SpriteState();

            //Set the highlighted, pressed, and selected sprites
            spriteState.highlightedSprite = newSprite;
            spriteState.pressedSprite = newSprite;
            spriteState.selectedSprite = newSprite;
            //Apply the new SpriteState to the button
            button.spriteState = spriteState;

            // Finds child image object
            Transform childImage = augmentCards[i].transform.Find("Augment Image");
            Image nestedImage = childImage.GetComponent<Image>();
            // Changes the icon image to fit the augment
            nestedImage.sprite = shufflePool[i].AugmentImage;
            //Changes text to fit augment
            augmentCards[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = shufflePool[i].augmentName;
            //Removes all previous listeners
            augmentCards[i].onClick.RemoveAllListeners();
            //Adds new listener to button
            int index = i;
            augmentCards[i].onClick.AddListener(() => addAugment(shufflePool[index]));
        }
    }
}
