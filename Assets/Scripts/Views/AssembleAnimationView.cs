using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Views
{
    public class AssembleAnimationView : MonoBehaviour
    {
        //Holds info for animation and assembly order
        public AssembleObjectMainView assembleObjectMainView;
        public int tutorialAnimationNumber;
        public GameObject assemblePreview;
        public bool assembled;
        public float assembleDistance;

        public List<AssembleAnimationView> lockedAfterAssemble;
        public List<AssembleAnimationView> needsToBeAssembled;
    }
}