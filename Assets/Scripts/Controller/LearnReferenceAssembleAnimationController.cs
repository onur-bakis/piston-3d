using System.Collections.Generic;
using DG.Tweening;
using Signals;
using Views;
using Zenject;

namespace Controller
{
    public class LearnReferenceAssembleAnimationController
    {
        private SignalBus _signalBus;

        private List<AssembleObjectMainController> _assembleObjectMainControllers;
        
        [Inject]
        public LearnReferenceAssembleAnimationController(SignalBus signalBus)
        {
            _signalBus = signalBus;

            SubscribeSignals();
        }
        
        public void Init(List<AssembleObjectMainController> assembleObjectMainControllers)
        {
            _assembleObjectMainControllers = assembleObjectMainControllers;
            
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<StartSimAnimationSignal>(OnStartSimAnimationSignal);
        }

        private void OnStartSimAnimationSignal()
        {
            //Stop all previous animations
            DOTween.KillAll();
            foreach (var assembleObjectMainController in _assembleObjectMainControllers)
            {
                //Reset position and values of reset values
                assembleObjectMainController.ResetValues();
                //Start assemble animation with parts
                assembleObjectMainController.assembleAnimationController.AnimateWithDelay();
            }
        }

    }
}