using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckCard : MonoBehaviour {
   public TMP_Text nameText;
   public TMP_Text manaText;

   public Button button;
   private Card card;
   
   public void SetCard(Card c, CardPanel cardPanel) {
      this.card = c;
      nameText.text = c.name;
      manaText.text = c.manaCost.ToString();

      button.onClick.AddListener(() => cardPanel.RemoveCardFromDeck(this));
   }

   public Card GetCard() {
      return card;
   }
}