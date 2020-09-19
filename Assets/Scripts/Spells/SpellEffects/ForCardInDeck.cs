using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ForCardInDeck : SpellEffect {
   public Card searchCard;
   public float duration;
   public int multiplier = 1;

   public UnityEvent eachCardEvent;

   public override void Cast() {
      var found = ((Avatar) spell.owner).deck.cards.FindAll(c => c == searchCard);
      StartCoroutine(InvokeOverTime(duration, found.Count * multiplier));
   }

   private IEnumerator InvokeOverTime(float duration, int count) {
      float delta = duration / (float) count;      // time between each card
      float now = Time.fixedTime;
      float end = now + duration;

      int currentIndex = 0;
      float last = 0f;
      while (now < end || currentIndex < count) {
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