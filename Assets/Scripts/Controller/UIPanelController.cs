using DG.Tweening;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Controller
{
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _learnPartText;
        [SerializeField] private TextMeshProUGUI _assembleLockedText;
        [SerializeField] private Image _assebleLockedTextBG; 
        public GameObject[] panels;

        private SignalBus _signalBus;

        private Color _assebleLockedTextBGColor;
        private Color _assembleLockedTextColor;
        
        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            
            _signalBus.Subscribe<SimFinishSignal>(TestFinished);
            _signalBus.Subscribe<LearnPartSignal>(ChangeLearnPartText);
            _signalBus.Subscribe<PartNeedPriorityMessage>(OnPartNeedPriorityMessage);

            _assembleLockedTextColor = _assembleLockedText.color;
            _assebleLockedTextBGColor = _assebleLockedTextBG.color;
            _assembleLockedTextColor.a = 0f;
            _assebleLockedTextBGColor.a = 0f;
        }


        public void OnStartSim()
        {
            panels[0].SetActive(false);
            panels[1].SetActive(true);
            
            _signalBus.Fire<LearnNextPieceSignal>();
        }

        public void OnLearnNextPiece()
        {
            _signalBus.Fire<LearnNextPieceSignal>();
        }

        public void OnGoAnimationPanel()
        {
            panels[1].SetActive(false);
            panels[2].SetActive(true);
            
            _signalBus.Fire<StartSimAnimationSignal>();
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

        public void ChangeLearnPartText(LearnPartSignal learnPartSignal)
        {
            _learnPartText.text = learnPartSignal.name;
        }
        
        private void OnPartNeedPriorityMessage(PartNeedPriorityMessage partNeedPriorityMessage)
        {
            //Show warning if warning message is different and no warning is showing
            if(_assembleLockedText.text == partNeedPriorityMessage.assemblyMessage && _assebleLockedTextBG.color != _assebleLockedTextBGColor)
                return;
            
            _assembleLockedText.text = partNeedPriorityMessage.assemblyMessage;
            
            _assembleLockedText.color = _assembleLockedTextColor;
            _assebleLockedTextBG.color = _assebleLockedTextBGColor;
            
            _assembleLockedText.DOKill();
            _assebleLockedTextBG.DOKill();
            
            //Warning instruction inOut animation
            _assembleLockedText.DOFade(1f, 1f).SetLoops(2, LoopType.Yoyo);
            _assebleLockedTextBG.DOFade(1f, 1f).SetLoops(2, LoopType.Yoyo);
        }
    }
}