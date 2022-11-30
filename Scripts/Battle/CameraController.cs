#nullable enable
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 0.1f;
        
        private Vector3 _destPos;
        private Camera mainCamera => Camera.main;
        private float _moveSpeed = 1;
        private float _moveUpdateSpeed = 2f;
        private FloatRange _movableRangeY = new FloatRange(-8f, 8f);
        private Vector3? _mousePosOnRightClicked = null;
        private Vector3 _rotationOnRightClicked = Vector3.zero;
        private const int mouseRightId = 1;

        [EventFunction]
        private void Start()
        {
            Debug.Assert(mainCamera!=null);
            _destPos = mainCamera.transform.position;
        }

        [EventFunction]
        private void Update()
        {
            updateRot(Time.deltaTime);
            updatePos(Time.deltaTime);
        }

        private void updateRot(float deltaTime)
        {
            // 右クリックを押したときの座標を覚える
            checkUpdateMousePosOnRightClicked();

            // 右クリックを押していないときはnullになっている
            if (_mousePosOnRightClicked == null) return;

            var currPos = Input.mousePosition;
            var deltaY = currPos.y - _mousePosOnRightClicked.Value.y;

            mainCamera.transform.rotation = Quaternion.Euler(
                _rotationOnRightClicked - rotationSpeed * new Vector3(deltaY, 0, 0));

            // 離したら再びnullを代入
            if (Input.GetMouseButtonUp(mouseRightId)) _mousePosOnRightClicked = null;
        }

        private void checkUpdateMousePosOnRightClicked()
        {
            bool isJustClickRight = Input.GetMouseButtonDown(mouseRightId);
            if (!isJustClickRight) return;
            _mousePosOnRightClicked = Input.mousePosition;
            _rotationOnRightClicked = mainCamera.transform.rotation.eulerAngles;
        }

        private void updatePos(float deltaTime)
        {
            var scroll = Input.mouseScrollDelta;
            _destPos += new Vector3(0, 0, scroll.y * _moveSpeed);
            _destPos.z = _movableRangeY.FixInRange(_destPos.z);

            var delta = (_destPos - mainCamera.transform.position);
            mainCamera.transform.position += delta * (deltaTime * _moveUpdateSpeed);
        }
    }
}