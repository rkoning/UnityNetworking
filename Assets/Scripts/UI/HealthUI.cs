using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour {

   private PlayerHealth health;

   public PlayerHealth Health {
      get { return health; }
      set {
         health = value;
         health.OnStatusApplied += (Status status) => {
            StatusData data = status.GetData();
            Debug.Log(data.name + " Added");
            var chip = GameObject.Instantiate(statusChipPrefab, statusEffectParent).GetComponent<StatusChip>();
            chip.SetSprite(data.sprite);
            currentStatuses.Add(data.name, chip);
         };

         health.OnStatusRemoved += (Status status) => {
            StatusData data = status.GetData();
            Debug.Log(data.name + " Removed");
            var chip = currentStatuses[data.name];
            Destroy(chip.gameObject);
            currentStatuses.Remove(data.name);            
         };
      }
   }

   public TMP_Text healthText;
   public Image healthBar;

   public TMP_Text armorText;
   public Image armorBar;

   public RectTransform statusEffectParent;

   private Dictionary<string, StatusChip> currentStatuses = new Dictionary<string, StatusChip>();
   public GameObject statusChipPrefab;

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