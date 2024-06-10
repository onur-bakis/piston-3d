using System.Collections.Generic;
using DG.Tweening;
using Signals;
using UnityEngine;
using Views;
using Zenject;


namespace Controller
{
    public class AssembleAnimationController
    {
        
        private List<AssemblePointController> _allAssemblePointControllers;
        private AssembleObjectMainController _assembleObjectMainController;
        private AssemblePointController _ownAssemblePointController;
        private AssembleAnimationView _assembleAnimationView;
        private SignalBus _signalBus;

        private float minAssembleDistance = 0.02f;
        private float animationTime = 1f;

        private bool noConnection;

        private PartNeedPriorityMessage _partNeedPriorityMessage;

        private GameObject assemblePreview;
        private Vector3 _dragOffset;

        public AssembleAnimationController(AssembleAnimationView assembleAnimationView,AssembleObjectMainController assembleObjectMainController,SignalBus signalBus)
        {
            _assembleAnimationView = assembleAnimationView;
            _assembleObjectMainController = assembleObjectMainController;
            _signalBus = signalBus;
            _partNeedPriorityMessage = new PartNeedPriorityMessage();

            
            //Get minDistance if available otherwise use default
            if (_assembleAnimationView.assembleDistance < minAssembleDistance)
            {
                _assembleAnimationView.assembleDistance = minAssembleDistance;
            }
            else
            {
                minAssembleDistance = _assembleAnimationView.assembleDistance;
            }
        }
        
        public void Init(List<AssemblePointController> allAssemblePointControllers)
        {
            _allAssemblePointControllers = allAssemblePointControllers;
            noConnection = true;
            
            //Crosscheck assemble points and assemble object parts to see if there is any connection 
            foreach (var assemblePointController in _allAssemblePointControllers)
            {
                if (assemblePointController._assemblePointView.assembleObjectMainView.assembleAnimationView == _assembleAnimationView)
                {
                    _ownAssemblePointController = assemblePointController;
                    noConnection = false;
                }
            }

            if (noConnection)
            {
                _assembleAnimationView.assembled = true;
            }
        }

        public void StartDrag(Vector3 position)
        {
            //Get drag point and object pivot offset
            _dragOffset = position - _assembleAnimationView.transform.position;
        }
        
        public void Drag(Vector3 position)
        {
            if(noConnection)
                return;

            if (!CheckCanBeDragged())
            {
                return;
            }
            
            _assembleAnimationView.transform.position = position-_dragOffset;
            
            float currentDistance = Vector3.Distance(_ownAssemblePointController.GetTransform().position,_assembleAnimationView.transform.position);

            //If it is less then assemble distance show faded part
            if (currentDistance < minAssembleDistance)
            {
                if (!CheckCanBeAssembled())
                {
                    return;
                }
                
                if(assemblePreview == null)
                    assemblePreview = GameObject.Instantiate(_assembleAnimationView.assemblePreview);

                if (!assemblePreview.activeSelf)
                {
                    
                    assemblePreview.SetActive(true);
                }
                
                assemblePreview.transform.position = _ownAssemblePointController.GetTransform().position;
            }
            else
            {
                if(assemblePreview == null)
                    return;
                
                if (assemblePreview.activeSelf)
                {
                    assemblePreview.SetActive(false);
                }
            }
        }


        public void Drop()
        {
            if (noConnection)
            {
                AnimationMoveComplete();
                return;
            }

            if (!CheckCanBeAssembled())
            {
                
                AnimationMoveComplete();
                return;
            }
            
            if(assemblePreview != null)
                assemblePreview.SetActive(false);
            
            float currentDistance = Vector3.Distance(_ownAssemblePointController.GetTransform().position,_assembleAnimationView.transform.position);
            
            //If close when mouse up assemble part
            if (currentDistance < minAssembleDistance)
            {
                _assembleAnimationView.transform.parent = _ownAssemblePointController.GetTransform().parent.transform;
                _assembleAnimationView.assembled = true;
                
                if (_ownAssemblePointController._assemblePointView.hasAnimation)
                {
                    GameObject[] animationPoints = _ownAssemblePointController._assemblePointView.animationData;

                    Sequence animation = DOTween.Sequence();
                    float animationSliceTime = animationTime/ (animationPoints.Length+1); 
                    foreach (var animationPoint in animationPoints)
                    {
                        animation.Append(_assembleAnimationView.transform
                            .DOMove(animationPoint.transform.position, 
                                animationSliceTime));
                    }

                    animation.Append(_assembleAnimationView.transform
                        .DOMove(_ownAssemblePointController.GetTransform().position, animationSliceTime));

                    if (_ownAssemblePointController._assemblePointView.hasRotation)
                    {
                        animation.Join(_assembleAnimationView.transform
                            .DORotate(_ownAssemblePointController.GetTransform().eulerAngles-Vector3.forward*180f,
                                animationSliceTime / 5f).SetLoops(5,LoopType.Incremental));
                    }

                    animation.Play().OnComplete(AnimationMoveComplete);
                }
                else
                {
                    _assembleAnimationView.transform.
                        DOMove(_ownAssemblePointController.GetTransform().position,animationTime)
                        .OnComplete(AnimationMoveComplete);
                }
            }
            else
            {
                _assembleAnimationView.transform.parent = null;
                _assembleAnimationView.assembled = false;
                AnimationMoveComplete();
            }
        }

        private bool CheckCanBeDragged()
        {
            //Check if there is any other part blocking disassembly
            foreach (var assembleAnimationView in _assembleAnimationView.lockedAfterAssemble)
            {
                if (assembleAnimationView.assembled)
                {
                    _partNeedPriorityMessage.assemblyMessage =
                        "Once "+assembleAnimationView.assembleObjectMainView.partName+" ayir.";
                    _signalBus.Fire(_partNeedPriorityMessage);
                    return false;
                }
            }
            return true;
        }
        
        private bool CheckCanBeAssembled()
        {
            //Check if there is any part need to be assembled before this part
            foreach (var assembleAnimationView in _assembleAnimationView.needsToBeAssembled)
            {
                if (!assembleAnimationView.assembled)
                {
                    _partNeedPriorityMessage.assemblyMessage =
                        "Once " + assembleAnimationView.assembleObjectMainView.partName + " yerlestir.";
                    _signalBus.Fire(_partNeedPriorityMessage);
                    return false;
                }
            }
            return true;
        }

        private void AnimationMoveComplete()
        {
            //Assemble animation is finished can take input again
            _signalBus.Fire<AssembleMovementAnimationFinished>();
            //Check if all parts are assembled
            _signalBus.Fire<CheckSimFinishSignal>();
        }


        public void AnimateWithDelay()
        {
            if (noConnection)
            {
                return;
            }
            //Start animation for learn assemble animation 
            float animationDelay = 1.1f * _assembleAnimationView.tutorialAnimationNumber;
            Animate(animationDelay);
        }

        private void Animate(float animationDelay)
        {
            _assembleAnimationView.transform.parent = _ownAssemblePointController.GetTransform().parent.transform;
                
            if (_ownAssemblePointController._assemblePointView.hasAnimation)
            {
                GameObject[] animationPoints = _ownAssemblePointController._assemblePointView.animationData;

                Sequence animation = DOTween.Sequence();
                float animationSliceTime = animationTime/ (animationPoints.Length+1); 
                foreach (var animationPoint in animationPoints)
                {
                    animation.Append(_assembleAnimationView.transform
                        .DOMove(animationPoint.transform.position, 
                            animationSliceTime));
                }

                animation.Append(_assembleAnimationView.transform
                    .DOMove(_ownAssemblePointController.GetTransform().position, animationSliceTime));

                if (_ownAssemblePointController._assemblePointView.hasRotation)
                {
                    animation.Join(_assembleAnimationView.transform
                        .DORotate(_ownAssemblePointController.GetTransform().eulerAngles-Vector3.forward*180f,
                            animationSliceTime / 5f).SetLoops(5,LoopType.Incremental));
                }

                animation.Play().SetDelay(animationDelay);
            }
            else
            {
                _assembleAnimationView.transform.
                    DOMove(_ownAssemblePointController.GetTransform().position,animationTime)
                    .SetDelay(animationDelay);
            }
        }
    }
}