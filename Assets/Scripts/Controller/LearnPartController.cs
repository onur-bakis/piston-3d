using System.Collections.Generic;
using Signals;
using UnityEngine;
using Views;
using Zenject;

namespace Controller
{
    public class LearnPartController
    {
        private SignalBus _signalBus;
        private List<AssembleObjectMainView> _assembleObjectMainViews;
        private AssembleObjectMainView[] _orderedAssembleObjectMainViews;

        private LearnPartSignal _learnPartSignal;
        private int _currentLearnPart;
        private AssembleObjectMainView _assembleObjectMainView;
        
        [Inject]
        public LearnPartController(
            SignalBus signalBus,
            List<AssembleObjectMainView> assembleObjectMainViews)
        {
            _signalBus = signalBus;

            GetLearnAnimationParts(assembleObjectMainViews);
            SubscribeSignals();
        }

        private void GetLearnAnimationParts(List<AssembleObjectMainView> assembleObjectMainViews)
        {
            _assembleObjectMainViews = new List<AssembleObjectMainView>();
            int assembleLearnPart = 0;
            foreach (var assembleObjectMainView in assembleObjectMainViews)
            {
                if (assembleObjectMainView.learnData)
                {
                    assembleLearnPart++;
                    _assembleObjectMainViews.Add(assembleObjectMainView);
                }
            }

            _orderedAssembleObjectMainViews = new AssembleObjectMainView[assembleLearnPart+1];
            foreach (AssembleObjectMainView assembleObjectMainView in _assembleObjectMainViews)
            {
                _orderedAssembleObjectMainViews[assembleObjectMainView.learnOrder] = assembleObjectMainView;
            }

            _learnPartSignal = new LearnPartSignal();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<ResetSignal>(ResetValues);
            _signalBus.Subscribe<LearnNextPieceSignal>(OnLearnNextPieceSignal);
        }

        private void ResetValues()
        {
            _currentLearnPart = 0;
        }
        
        private void OnLearnNextPieceSignal()
        {
            //Cycle through parts send signal to update info at UI and Camera
            _currentLearnPart++;

            _currentLearnPart %= _orderedAssembleObjectMainViews.Length;
            
            _assembleObjectMainView = _orderedAssembleObjectMainViews[_currentLearnPart];

            if (_assembleObjectMainView == null)
            {
                OnLearnNextPieceSignal();
                return;
            }
            _learnPartSignal.name = _assembleObjectMainView.partName;
            _learnPartSignal.position = _assembleObjectMainView.transform.position;
            
            _signalBus.Fire(_learnPartSignal);
        }
    }
}