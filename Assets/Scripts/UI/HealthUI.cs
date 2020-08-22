using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
   public Health health;

   public Image healthBar;

   private void Update() {
      if (health)
         healthBar.fillAmount = health.currentHealth / health.maxHealth;
   }
}