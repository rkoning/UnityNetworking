using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour {
   public PlayerHealth health;

   public TMP_Text healthText;
   public Image healthBar;

   public TMP_Text armorText;
   public Image armorBar;

   private void Update() {
      if (!health)
         return; 
      string currentHealth = health.currentHealth.ToString("F0");
      string maxHealth = health.maxHealth.ToString("F0");
      healthText.text = $"{currentHealth} / {maxHealth}";
      healthBar.fillAmount = health.currentHealth / health.maxHealth;

      armorText.text = health.armor.ToString("F0");
      armorBar.fillAmount = health.maxArmor > 0 ? health.armor / health.maxArmor : 0f;
   }
}