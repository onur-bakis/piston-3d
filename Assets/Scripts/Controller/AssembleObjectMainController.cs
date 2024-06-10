using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Views;
using Zenject;

namespace Controller
{
    public class AssembleObjectMainController
    {
        public AssembleAnimationController assembleAnimationController;
        public DragObjectController dragObjectController;
        public List<AssemblePointController> assemblePointControllers;

        private AssembleObjectMainView _assembleObjectMainView;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Transform _transformParent;
        private bool _assembled;
        public string partName;
        
        public AssembleObjectMainController(AssembleObjectMainView assembleObjectMainView,SignalBus signalBus)
        {
            _assembleObjectMainView = assembleObjectMainView;
            partName = assembleObjectMainView.partName;
            
            assembleAnimationController = new AssembleAnimationController(assembleObjectMainView.assembleAnimationView,this,signalBus);
            
            //Get all assemble points from view
            assemblePointControllers = new List<AssemblePointController>();
            foreach (var assemblePointView in assembleObjectMainView.assemblePointViews)
            {
                AssemblePointController assemblePointController = new AssemblePointController(assemblePointView,assembleAnimationController);
                assemblePointControllers.Add(assemblePointController);
            }
            
            dragObjectController = new DragObjectController(
                assembleAnimationController,
                assembleObjectMainView.gameObject);
        }

        public void SetConnections(List<AssemblePointController> allAssemblePointControllers)
        {
            assembleAnimationController.Init(allAssemblePointControllers);

            
            _startPosition = _assembleObjectMainView.transform.position;
            _startRotation = _assembleObjectMainView.transform.rotation;
            _transformParent = _assembleObjectMainView.transform.parent;
            _assembled = _assembleObjectMainView.assembleAnimationView.assembled;
        }

        public void ResetValues()
        {
            _assembleObjectMainView.transform.DOKill();
            _assembleObjectMainView.transform.position = _startPosition;
            _assembleObjectMainView.transform.rotation = _startRotation;
            _assembleObjectMainView.transform.parent = _transformParent;
            _assembleObjectMainView.assembleAnimationView.assembled = _assembled;
        }
    }
}