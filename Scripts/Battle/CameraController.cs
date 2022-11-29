using System;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 _destPos;
        private Camera _camera;
        private float _moveSpeed = 1;
        private float _moveUpdateSpeed = 2f;
        private FloatRange _movableRangeY = new FloatRange(-5f, 5f);

        [EventFunction]
        private void Start()
        {
            _camera = Camera.main;
            Debug.Assert(_camera!=null);
            _destPos = _camera.transform.position;
        }

        [EventFunction]
        private void Update()
        {
            var scroll = Input.mouseScrollDelta;
            _destPos += new Vector3(0, 0, scroll.y * _moveSpeed);
            _destPos.z = _movableRangeY.FixInRange(_destPos.z);

            var delta = (_destPos - _camera.transform.position);
            _camera.transform.position += delta * (Time.deltaTime * _moveUpdateSpeed);
        }
    }
}