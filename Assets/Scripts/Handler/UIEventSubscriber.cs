using System;
using Controller;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Handler
{
    public class UIEventSubscriber : MonoBehaviour
    {
        [SerializeField] private UIEventSubscriptionTypes type;
        [SerializeField] private Button button;

        [Inject] [HideInInspector] public UIPanelController _uiPanelController;
        
        [Inject]
        public void Construct(UIPanelController uiPanelController)
        {
            _uiPanelController = uiPanelController;
        }

        public void ButtonClick()
        {
            switch (type)
            {
                case UIEventSubscriptionTypes.OnSimStart:
                {
                    Debug.Log("SeeLevels");
                    _uiPanelController.OnStartSim();
                    break;
                }
                case UIEventSubscriptionTypes.OnLearnNextPiece:
                {
                    _uiPanelController.OnLearnNextPiece();
                    break;
                }
                case UIEventSubscriptionTypes.OnGoAnimationPanel:
                {
                    _uiPanelController.OnGoAnimationPanel();
                    break;
                }
                case UIEventSubscriptionTypes.OnStartSimAnimation:
                {
                    _uiPanelController.OnStartSimAnimation();
                    break;
                }
                case UIEventSubscriptionTypes.OnStartTheTest:
                {
                    _uiPanelController.OnStartTheTest();
                    break;
                }
                case UIEventSubscriptionTypes.OnReturnBackToLearning:
                {
                    _uiPanelController.OnReturnBackToLearning();
                    break;
                }
                case UIEventSubscriptionTypes.OnFinishTestResult:
                {
                    _uiPanelController.OnFinishTestResult();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void OnEnable()
        {
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            button.onClick.AddListener(ButtonClick);
        }

        private void UnSubscribeEvents()
        {
            button.onClick.RemoveListener(ButtonClick);
        }

        public  void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}