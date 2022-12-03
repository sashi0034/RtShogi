using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Battle.UI
{
    public record ObtainedKomaElementProps(
        EKomaKind Kind, 
        Sprite Icon);
    public class ObtainedKomaElement : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textQty;

        private EKomaKind _kind;
        public EKomaKind Kind => _kind;
        
        private int _numQty = 0;
        public int NumQuantity => _numQty;
        
        public void Init(ObtainedKomaElementProps props)
        {
            _kind = props.Kind;
            iconImage.sprite = props.Icon;
            setQuantity(1);
        }
        private void setQuantity(int qty)
        {
            Debug.Assert(qty>=0);
            _numQty = qty;
            textQty.text = qty.ToString();
        }

        public void IncQuantity()
        {
            setQuantity(_numQty + 1);
        }

        public void DecQuantity()
        {
            setQuantity(_numQty - 1);
        }
        
    }
}