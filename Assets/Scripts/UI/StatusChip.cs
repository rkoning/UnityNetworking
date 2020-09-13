using UnityEngine.UI;
using UnityEngine;

public class StatusChip : MonoBehaviour
{
    public Image image;

    public void SetSprite(Sprite s) {
        image.sprite = s;
    }
}
