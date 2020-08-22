using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FullCard : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text loreText;
    public TMP_Text manaCostText;
    public Image art;
    public Image lockedSprite;
    public Button addButton;

    // private bool locked;

    public void SetCard(Card card, CardPanel cardPanel) {
        nameText.text = card.name;
        descriptionText.text = card.description;
        manaCostText.text = card.manaCost.ToString();
        loreText.text = card.lore.ToString();
        art.sprite = card.sprite;

        addButton.onClick.AddListener(() => cardPanel.AddCardToDeck(card));
    }

    public void Lock() {
        // locked = true;
        lockedSprite.gameObject.SetActive(true);
    }
    
    public void Unlock() {
        // locked = false;
        lockedSprite.gameObject.SetActive(false);
    }
}
