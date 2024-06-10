using System;
using UnityEngine;
using UnityEngine.Serialization;
using Views;

namespace Controller
{
    public class AssemblePointController 
    {
        public AssemblePointView _assemblePointView;
        
        public AssemblePointController(AssemblePointView assemblePointView,AssembleAnimationController assembleAnimationController)
        {
            _assemblePointView = assemblePointView;
        }
        
        public Transform GetTransform()
        {
            return _assemblePointView.transform;
        }
    }
}