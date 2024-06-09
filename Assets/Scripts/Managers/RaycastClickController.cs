using System.Collections.Generic;
using System.Linq;
using Controller;
using Signals;
using UnityEngine;
using Zenject;

namespace Managers
{
    public class RaycastClickController
    {
        private SignalBus _signalBus;
        private float _dragDistance;
        private Camera _camera;
        private DragObjectController _dragObjectController;
        private Dictionary<string, DragObjectController> _draggableObjects;
        private int dragObjectCount;
        private Vector3 _cacheVector3;
        private bool startDrag;
        private bool animationOnGoing;
        
        [Inject]
        public RaycastClickController(SignalBus signalBus,CameraController cameraController)
        {
            _signalBus = signalBus;
            _camera = cameraController.camera;
            _draggableObjects = new Dictionary<string, DragObjectController>();

            SubscribeSignals();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<MouseLeftClickedSignal>(OnMouseLeftClickedSignal);
            _signalBus.Subscribe<MouseLeftDragSignal>(OnMouseLeftDragSignal);
            _signalBus.Subscribe<MouseLeftButtonUpSignal>(OnMouseLeftButtonUpSignal);
            _signalBus.Subscribe<AssembleMovementAnimationFinished>(OnAssembleMovementAnimationFinished);
        }

        private void OnAssembleMovementAnimationFinished()
        {
            animationOnGoing = false;
        }

        private void OnMouseLeftClickedSignal(MouseLeftClickedSignal mouseLeftClickedSignal)
        {
            if (animationOnGoing)
            {
                return;
            }
            
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(mouseLeftClickedSignal.inputDataParams.inputPoint);

            if (Physics.Raycast(ray, out hit))
            {
                if (_draggableObjects.ContainsKey(hit.transform.gameObject.name))
                {
                    _dragObjectController = _draggableObjects[hit.transform.gameObject.name];
                    _dragDistance = Vector3.Distance(hit.point , _camera.transform.position);
                    
                    
                    _cacheVector3 = mouseLeftClickedSignal.inputDataParams.inputPoint;
                    _cacheVector3.z = _dragDistance;
                    Vector3 startPosition = _camera.ScreenToWorldPoint(_cacheVector3);
                    _dragObjectController.StartDrag(startPosition);
                    startDrag = true;
                }
            }
        }

        private void OnMouseLeftDragSignal(MouseLeftDragSignal mouseLeftDragSignal)
        {
            if(!startDrag)
                return;
            
            _cacheVector3 = mouseLeftDragSignal.inputDataParams.inputPoint;
            _cacheVector3.z = _dragDistance;

            //_cacheVector3.z = 0.5f;//Vector3.Distance(_dragObjectController.gameObject.transform.position , _camera.transform.position);
            
            
            Vector3 movePosition = _camera.ScreenToWorldPoint(_cacheVector3);
            _dragObjectController.Drag(movePosition);
        }
        
        private void OnMouseLeftButtonUpSignal()
        {
            if(!startDrag)
                return;
            
            startDrag = false;
            animationOnGoing = true;
            _dragObjectController.Drop();
        }

        public void SetDragObjectController(DragObjectController dragObjectController, GameObject draggableObject)
        {
            dragObjectCount++;
            draggableObject.name = dragObjectCount.ToString();
            
            _draggableObjects.Add(dragObjectCount.ToString(),dragObjectController);
        }
    }
}