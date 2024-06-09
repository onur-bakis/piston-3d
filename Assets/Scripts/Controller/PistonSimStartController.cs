using System.Collections.Generic;
using Managers;
using Signals;
using UnityEngine;
using Views;
using Zenject;

namespace Controller
{
    public class PistonSimStartController
    {
        private List<AssembleObjectMainView> _assembleObjectMainViews;
        private List<AssembleObjectMainController> _assembleObjectMainControllers;
        private List<AssemblePointController> _assemblePointControllers;

        private RaycastClickController _raycastClickController;
        private SignalBus _signalBus;

        [Inject]
        public PistonSimStartController(
            List<AssembleObjectMainView> assembleObjectMainViews,
            RaycastClickController raycastClickController,
            SignalBus signalBus)
        {
            _assembleObjectMainViews = assembleObjectMainViews;
            _raycastClickController = raycastClickController;
            _signalBus = signalBus;

            _assembleObjectMainControllers = new List<AssembleObjectMainController>();
            _assemblePointControllers = new List<AssemblePointController>();
            
            foreach (var assembleObjectMainView in _assembleObjectMainViews)
            {
                AssembleObjectMainController assembleObjectMainController =
                    new AssembleObjectMainController(assembleObjectMainView,_signalBus);
                
                _raycastClickController.SetDragObjectController(
                    assembleObjectMainController.dragObjectController
                    ,assembleObjectMainView.gameObject);

                _assembleObjectMainControllers.Add(assembleObjectMainController);
                
                foreach (AssemblePointController assemblePointController in assembleObjectMainController.assemblePointControllers)
                {
                    _assemblePointControllers.Add(assemblePointController);
                }
            }

            foreach (var assembleObjectMainController in _assembleObjectMainControllers)
            {
                assembleObjectMainController.SetConnections(_assemblePointControllers);
            }

            SubscribeSignals();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<CheckSimFinishSignal>(OnCheckSimFinish);
        }

        public void OnCheckSimFinish()
        {
            
            foreach (var assembleObjectMainView in _assembleObjectMainViews)
            {
                if(!assembleObjectMainView.assembleAnimationView.assembled)
                    return; 
            }
            
            _signalBus.Fire<SimFinishSignal>();
        }
        
    }
}