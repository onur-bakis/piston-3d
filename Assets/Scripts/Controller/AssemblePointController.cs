using System;
using UnityEngine;
using UnityEngine.Serialization;
using Views;

namespace Controller
{
    public class AssemblePointController 
    {
        private AssembleAnimationController _assembleAnimationController;
        public AssemblePointView _assemblePointView;
        
        public AssemblePointController(AssemblePointView assemblePointView,AssembleAnimationController assembleAnimationController)
        {
            _assemblePointView = assemblePointView;
            _assembleAnimationController = assembleAnimationController;
        }

        public AssembleAnimationController GetAnimationController()
        {
            return _assembleAnimationController;
        }

        public Transform GetTransform()
        {
            return _assemblePointView.transform;
        }
    }
}