using UnityEngine;

namespace FishEvolution.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _target;
        [SerializeField] private SmoothFollow _smoothFollow = new SmoothFollow();
        [SerializeField] private CameraZoom _cameraZoom = new CameraZoom();

        public Transform Target => _target;
        public float OrthographicSize => _camera != null ? _camera.orthographicSize : 0f;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            _camera.orthographic = true;
            _smoothFollow.Initialize(transform, _target);
            _cameraZoom.Initialize(_camera, _target);
        }

        private void Reset()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (_target == null || _camera == null)
            {
                return;
            }

            _smoothFollow.Follow(transform, _target);
            _cameraZoom.Zoom(_camera, _target);
        }

        private void OnDisable()
        {
            _smoothFollow.Stop();
            _cameraZoom.Stop();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _smoothFollow.Initialize(transform, _target);
            _cameraZoom.Initialize(_camera, _target);
        }
    }
}
