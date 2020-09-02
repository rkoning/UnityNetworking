using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
   public PlayerHealth health;

   public Image healthBar;

   private void Update() {
      if (health)
         healthBar.fillAmount = health.currentHealth / health.maxHealth;
   }
}