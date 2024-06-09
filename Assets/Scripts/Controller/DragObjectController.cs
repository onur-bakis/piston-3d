using System;
using Managers;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class DragObjectController
    {
        private AssembleAnimationController _assembleAnimationController;

        private GameObject _draggableObject;
        private Vector3 _dragOffset;
        public DragObjectController(
            AssembleAnimationController assembleAnimationController,
            GameObject draggableObject)
        {
            _assembleAnimationController = assembleAnimationController;
            _draggableObject = draggableObject;
        }
        
        public void Drag(Vector3 position)
        {
            _assembleAnimationController.Drag(position);
        }

        public void Drop()
        {
            _assembleAnimationController.Drop();
        }

        public void StartDrag(Vector3 position)
        {
            _assembleAnimationController.StartDrag(position);
        }
    }
}