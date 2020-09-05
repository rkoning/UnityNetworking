using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Charge : SpellEffect {
   public float maxChargeTime;
   public float minChargeTime;
   public float chargeIncrementTime;
   private float currentCharge;
   private float nextChargeIncrement;

   public UnityEvent onChargeIncrement;
   public UnityEvent onChargeStart;
   public UnityEvent onChargeCanceled;

   private bool chargeStarted = false;

   public override void Cast() {
      if (!chargeStarted) {
         chargeStarted = true;
         onChargeStart.Invoke();
         nextChargeIncrement = chargeIncrementTime;
      }
   }

   public override void Hold() {
      if (chargeStarted) {
         if (currentCharge <= maxChargeTime) {
            currentCharge += Time.deltaTime;
         } else {
            base.Cast();
            onChargeCanceled.Invoke();
            currentCharge = 0f;
            chargeStarted = false;
            return;
         }

         if (currentCharge > nextChargeIncrement) {
            nextChargeIncrement += chargeIncrementTime;
            onChargeIncrement.Invoke();
         }
      }
   }

   public override void Release() {
      if (chargeStarted && currentCharge >= minChargeTime) {
         base.Cast();
      }
      onChargeCanceled.Invoke();
      currentCharge = 0f;
      chargeStarted = false;
   }

   public override bool Done() {
      return !chargeStarted;
   }

   public override void CleanUp() {
      onChargeCanceled.Invoke();
   }
}