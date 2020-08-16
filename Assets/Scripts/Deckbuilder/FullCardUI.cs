using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FullCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text loreText;
    [SerializeField] private TMP_Text manaCostText;
    [SerializeField] private Image art;
    [SerializeField] private Image lockedSprite;

    private bool locked;

    public void SetCard(Card card) {
        nameText.text = card.name;
        descriptionText.text = card.description;
        manaCostText.text = card.manaCost.ToString();
        loreText.text = card.lore.ToString();
        art.sprite = card.sprite;
    }

    public void Lock() {
        locked = true;
        lockedSprite.gameObject.SetActive(true);
    }
    
    public void Unlock() {
        locked = false;
        lockedSprite.gameObject.SetActive(false);
    }
}
