using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Signals;
using UnityEngine;
using UnityEngine.Serialization;
using Views;
using Zenject;


namespace Controller
{
    public class AssembleAnimationController
    {
        
        private List<AssemblePointController> _allAssemblePointControllers;
        private AssemblePointController _ownAssemblePointController;
        private AssembleAnimationView _assembleAnimationView;
        private SignalBus _signalBus;

        private float minDistance = 0.02f;
        private float animationTime = 1f;

        private bool noConnection;

        private GameObject assemblePreview;
        private Vector3 _dragOffset;

        public AssembleAnimationController(AssembleAnimationView assembleAnimationView,SignalBus signalBus)
        {
            _assembleAnimationView = assembleAnimationView;
            _signalBus = signalBus;

            if (_assembleAnimationView.assembleDistance < minDistance)
            {
                _assembleAnimationView.assembleDistance = minDistance;
            }
            else
            {
                minDistance = _assembleAnimationView.assembleDistance;
            }
        }
        
        public void Init(List<AssemblePointController> allAssemblePointControllers)
        {
            _allAssemblePointControllers = allAssemblePointControllers;
            noConnection = true;
            
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

            if (currentDistance < minDistance)
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
            
            
            Debug.Log(currentDistance);
            
            if (currentDistance < minDistance)
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

        public bool CheckCanBeDragged()
        {
            foreach (var assembleAnimationView in _assembleAnimationView.lockedAfterAssemble)
            {
                if (assembleAnimationView.assembled)
                {
                    Debug.Log("Disassemble "+assembleAnimationView.gameObject.name+" first.");
                    return false;
                }
            }
            return true;
        }
        
        private bool CheckCanBeAssembled()
        {
            foreach (var assembleAnimationView in _assembleAnimationView.needsToBeAssembled)
            {
                if (!assembleAnimationView.assembled)
                {
                    Debug.Log("Assemble "+assembleAnimationView.gameObject.name+" first.");
                    return false;
                }
            }
            return true;
        }

        private void AnimationMoveComplete()
        {
            _signalBus.Fire<AssembleMovementAnimationFinished>();
            _signalBus.Fire<CheckSimFinishSignal>();
        }

        public void StartDrag(Vector3 position)
        {
            _dragOffset = position - _assembleAnimationView.transform.position;
        }
    }
}