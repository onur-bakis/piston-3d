using System.Collections.Generic;
using DG.Tweening;
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
            LearnReferenceAssembleAnimationController learnReferenceAssembleAnimationController,
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
                
                //Set all part main controller at raycast controller
                _raycastClickController.SetDragObjectController(
                    assembleObjectMainController.dragObjectController
                    ,assembleObjectMainView.gameObject);

                _assembleObjectMainControllers.Add(assembleObjectMainController);
                
                //Get all assemble points from assemble parts
                foreach (AssemblePointController assemblePointController in assembleObjectMainController.assemblePointControllers)
                {
                    _assemblePointControllers.Add(assemblePointController);
                }
            }
            
            //Set connections for assemble points and assemble parts
            foreach (var assembleObjectMainController in _assembleObjectMainControllers)
            {
                assembleObjectMainController.SetConnections(_assemblePointControllers);
            }

            SubscribeSignals();
            
            learnReferenceAssembleAnimationController.Init(_assembleObjectMainControllers);
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<ResetSignal>(OnStartTheTestSignal);
            _signalBus.Subscribe<CheckSimFinishSignal>(OnCheckSimFinish);
            _signalBus.Subscribe<StartTheTestSignal>(OnStartTheTestSignal);
        }

        private void OnStartTheTestSignal()
        {
            //Stop all animations and reset values for sim test
            DOTween.KillAll();
            foreach (var assembleObjectMainController in _assembleObjectMainControllers)
            {
                assembleObjectMainController.ResetValues();
            }
        }

        public void OnCheckSimFinish()
        {
            //Check if all parts is assembled if so send finish signal
            foreach (var assembleObjectMainView in _assembleObjectMainViews)
            {
                if(!assembleObjectMainView.assembleAnimationView.assembled)
                    return; 
            }
            
            _signalBus.Fire<SimFinishSignal>();
        }
        
    }
}