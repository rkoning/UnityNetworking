using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public bool canCast;
    public Image thumbnail;
    public TMP_Text manaCostText;

    public void SetCard(Card card, bool canCast) {
        this.thumbnail.sprite = card.sprite;
        this.manaCostText.text = card.manaCost.ToString();
        this.canCast = canCast;
    }
}
