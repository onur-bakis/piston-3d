using Signals;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class UIPanelController : MonoBehaviour
    {
        public GameObject[] panels;

        private SignalBus _signalBus;
        
        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            
            _signalBus.Subscribe<SimFinishSignal>(TestFinished);
        }
        
        public void OnStartSim()
        {
            panels[0].SetActive(false);
            panels[1].SetActive(true);
        }

        public void OnLearnNextPiece()
        {
            _signalBus.Fire<LearnNextPieceSignal>();
        }

        public void OnGoAnimationPanel()
        {
            panels[1].SetActive(false);
            panels[2].SetActive(true);
        }
        
        public void OnStartSimAnimation()
        {
            _signalBus.Fire<StartSimAnimationSignal>();
        }

        public void OnStartTheTest()
        {
            panels[2].SetActive(false);
            panels[3].SetActive(true);
            
            
            _signalBus.Fire<StartTheTestSignal>();
        }

        public void OnReturnBackToLearning()
        {
            panels[3].SetActive(false);
            panels[0].SetActive(true);
            
            _signalBus.Fire<ResetSignal>();
        }

        public void OnFinishTestResult()
        {
            panels[4].SetActive(false);
            panels[0].SetActive(true);
            
            
            _signalBus.Fire<ResetSignal>();
        }

        public void TestFinished()
        {
            panels[3].SetActive(false);
            panels[4].SetActive(true);
        }
    }
}