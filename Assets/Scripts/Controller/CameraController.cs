using Cinemachine;
using Signals;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        public Camera camera;
        public SignalBus _signalBus;
        
        [SerializeField] private CinemachineFreeLook _cameraStart;
        [SerializeField] private CinemachineFreeLook _cameraFreeLookLearnParts;
        [SerializeField] private CinemachineFreeLook _cameraFreeLookLearnAnimation;
        


        private CinemachineVirtualCameraBase _cinemachineVirtualCameraBase;
        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _cinemachineVirtualCameraBase = _cameraStart;
            SubscribeSignal();
        }

        private void SubscribeSignal()
        {
            _signalBus.Subscribe<ResetSignal>(OnResetSignal);
            _signalBus.Subscribe<LearnPartSignal>(MoveForPart);
            _signalBus.Subscribe<StartSimAnimationSignal>(OnStartSimAnimationSignal);
            
        }

        private void OnResetSignal()
        {
            //Reset camera to start state
            _cinemachineVirtualCameraBase.Priority = 0;
            _cinemachineVirtualCameraBase = _cameraStart;
            _cameraStart.Priority = 1;
        }

        private void OnStartSimAnimationSignal()
        {
            //Move camera for animation
            _cinemachineVirtualCameraBase.Priority = 0;
            _cinemachineVirtualCameraBase = _cameraFreeLookLearnAnimation;
            _cameraFreeLookLearnAnimation.Priority = 1;
        }

        private void MoveForPart(LearnPartSignal learnPartSignal)
        {
            //Move camera to showcase parts 
            _cinemachineVirtualCameraBase.Priority = 0;
            _cinemachineVirtualCameraBase = _cameraFreeLookLearnParts;
            
            _cameraFreeLookLearnParts.Priority = 3;
            Vector3 position = learnPartSignal.position-Vector3.forward;
            position.z = -0.4f;
            _cameraFreeLookLearnParts.transform.position = position;
        }
    }
}