#nullable enable

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RtShogi.Scripts.Battle.UI
{
    public record ObtainedKomaElementProps(
        EKomaKind Kind, 
        Sprite Icon,
        ETeam OwnerTeam);
    public class ObtainedKomaElement : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textQty;
        [SerializeField] private Image foregroundShadow;
        [SerializeField] private Image cursorIconPrefab;

        private EKomaKind _kind;
        public EKomaKind Kind => _kind;
        
        private int _numQty = 0;
        public int NumQuantity => _numQty;

        private Tween _tweenAnim;

        private BoolFlag _hasBeginDrag = new BoolFlag();
        public IBoolFlagTaker HasBeginDrag => _hasBeginDrag;
        
        private BoolFlag _hasEndDrag = new BoolFlag();
        public IBoolFlagTaker HasEndDrag => _hasEndDrag;

        [EventFunction]
        private void Awake()
        {
            _tweenAnim = Util.GetCompletedTween();
        }

        public void Init(ObtainedKomaElementProps props)
        {
            _kind = props.Kind;
            iconImage.sprite = props.Icon;
            setQuantity(1);
            if (props.OwnerTeam==ETeam.Enemy) initAsEnemy();
        }

        private void initAsEnemy()
        {
            iconImage.transform.Rotate(new Vector3(0, 0, 180));
            Util.DestroyComponent(GetComponent<EventTrigger>());
        }

        public Image CreateIconCursorIcon(Transform parent)
        {
            var icon = Instantiate(cursorIconPrefab, parent);
            icon.sprite = iconImage.sprite;
            return icon;
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
        

        [EventFunction]
        public void OnBeginDrag()
        {
            _tweenAnim.Kill();
            _tweenAnim = transform.DOScale(Vector3.one * 0.9f, 0.3f).SetEase(Ease.InOutBack);
            _hasBeginDrag.UpFlag();
            _hasEndDrag.Clear();
        }
        
        [EventFunction]
        public void OnEndDrag()
        {
            if (gameObject == null) return;
            _tweenAnim.Kill();
            _tweenAnim = transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutBack);
            _hasEndDrag.UpFlag();
        }

        [EventFunction]
        public void OnPointerEnter()
        {
            animShakeLittle().Forget();
        }
        
        [EventFunction]
        public void OnPointerDown()
        {
            foregroundShadow.gameObject.SetActive(true);
        }

        [EventFunction]
        public void OnPointerUp()
        {
            foregroundShadow.gameObject.SetActive(false);
        }

        private async UniTask animShakeLittle()
        {
            if (_tweenAnim.active) return;
            await transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.InSine);
            if (_tweenAnim.active) return;
            await transform.DOScale(Vector3.one * 1.0f, 0.1f).SetEase(Ease.OutSine);
        }

        
    }
}