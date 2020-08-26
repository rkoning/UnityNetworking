using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ForCardInDeck : SpellEffect {
   public Card searchCard;
   public float duration;
   
   public UnityEvent eachCardEvent;

   public override void Cast() {
      var found = spell.owner.deck.cards.FindAll(c => c == searchCard);
      StartCoroutine(InvokeOverTime(duration, found));
   }

   private IEnumerator InvokeOverTime(float duration, List<Card> cards) {
      float delta = duration / (float) cards.Count;      // time between each card
      float now = Time.fixedTime;
      float end = now + duration;

      int currentIndex = 0;
      float last = 0f;
      while (now < end || currentIndex < cards.Count) {
         now = Time.fixedTime;
         if (now > last + delta) {                       // if the current time is less than the time between cards, invoke the event
            eachCardEvent.Invoke();
            last = now;
            currentIndex++;
         }
         yield return null;                              // yields until next frame
      }
   }
}