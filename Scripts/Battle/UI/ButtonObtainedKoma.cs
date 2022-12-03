using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Battle.UI
{
    public class ButtonObtainedKoma : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textQty;

        
        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }
        public void SetQuantity(int qty)
        {
            textQty.text = qty.ToString();
        }
        
    }
}