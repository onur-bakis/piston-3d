using System;
using UnityEngine;

namespace Views
{
    public class AssemblePointView : MonoBehaviour
    {
        //Connection points that part can connect
        public bool hasAnimation;
        public bool hasRotation;
        public GameObject[] animationData;
        public AssembleObjectMainView assembleObjectMainView;
    }
}