using System.Collections.Generic;
using Controller;
using UnityEngine;

namespace Views
{
    public class AssembleObjectMainView : MonoBehaviour
    {
        //Main view that holds reference for whole assemble part and connection points
        public AssembleAnimationView assembleAnimationView;
        public List<AssemblePointView> assemblePointViews;

        public bool learnData;
        public int learnOrder;
        public string partName;
    }
}