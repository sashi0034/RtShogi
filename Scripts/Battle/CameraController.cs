#nullable enable
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    [Serializable]
    public struct PositionAndRotation
    {
        [SerializeField] private Vector3 pos;
        [SerializeField] private Vector3 rot;
        public Vector3 Pos => pos;
        public Vector3 Rot=> rot;
    }
    
    public class CameraController : MonoBehaviour
    {
        private Vector3 _destPos;
        private Camera mainCamera => Camera.main;
        private readonly float movingSpeed = 1;
        private readonly float movingUpdateSpeed = 2f;
        private readonly float rotationSpeed = 0.1f;
        private readonly float elevationSpeed = 0.5f;
        private FloatRange _movableRangeElevation = new FloatRange(2.5f, 12.5f);
        private FloatRange _movableRangeZ = new FloatRange(-8f, 8f);
        private Vector3? _mousePosOnRightClicked = null;
        private Vector3 _cameraRotOnRightClicked = Vector3.zero;
        private const int mouseRightId = 1;
        private const float animDurationQuick = 0.3f;

        [SerializeField] private PositionAndRotation cameraPropsAbove;
        [SerializeField] private PositionAndRotation cameraPropsDiagonal;

        [EventFunction]
        private void Start()
        {
            Debug.Assert(mainCamera!=null);
            _destPos = mainCamera.transform.position;
        }

        [EventFunction]
        private void Update()
        {
            updateRotAndElevation(Time.deltaTime);
            updatePos(Time.deltaTime);
        }

        private void updateRotAndElevation(float deltaTime)
        {
            // 右クリックを押したときの座標を覚える
            checkUpdateMousePosOnRightClicked();

            // 右クリックを押していないときはnullになっている
            if (_mousePosOnRightClicked == null) return;

            var currPos = Input.mousePosition;
            var deltaY = currPos.y - _mousePosOnRightClicked.Value.y;

            // カメラ向きを変更
            mainCamera.transform.rotation = Quaternion.Euler(
                _cameraRotOnRightClicked - rotationSpeed * new Vector3(deltaY, 0, 0));

            // 離したら再びnullを代入
            if (Input.GetMouseButtonUp(mouseRightId)) _mousePosOnRightClicked = null;
        }

        private void checkUpdateMousePosOnRightClicked()
        {
            bool isJustClickRight = Input.GetMouseButtonDown(mouseRightId);
            if (!isJustClickRight) return;
            _mousePosOnRightClicked = Input.mousePosition;
            _cameraRotOnRightClicked = mainCamera.transform.rotation.eulerAngles;
        }

        private void updatePos(float deltaTime)
        {
            var scroll = Input.mouseScrollDelta;

            // 右クリック中かどうかで分岐
            if (Input.GetMouseButton(mouseRightId))
            {
                // カメラ高さを変更
                _destPos.y = _movableRangeElevation.FixInRange(
                    _destPos.y - scroll.y * elevationSpeed);
            }
            else
            {
                // z方向の位置を変更
                _destPos += new Vector3(0, 0, scroll.y * movingSpeed);
                _destPos.z = _movableRangeZ.FixInRange(_destPos.z);
            }

            var delta = (_destPos - mainCamera.transform.position);
            mainCamera.transform.position += delta * (deltaTime * movingUpdateSpeed);
        }

        [Button]
        public void OnPushAbove()
        {
            _destPos = cameraPropsAbove.Pos;
            mainCamera.transform.DORotate(cameraPropsAbove.Rot, animDurationQuick);
        }
        
        [Button]
        public void OnPushDiagonal()
        {
            _destPos = cameraPropsDiagonal.Pos;
            mainCamera.transform.DORotate(cameraPropsDiagonal.Rot, animDurationQuick);
        }

    }
}