using System.Collections.Generic;
using Views;
using Zenject;

namespace Controller
{
    public class AssembleObjectMainController
    {
        public AssembleAnimationController assembleAnimationController;
        public DragObjectController dragObjectController;
        public List<AssemblePointController> assemblePointControllers;
        
        public AssembleObjectMainController(AssembleObjectMainView assembleObjectMainView,SignalBus signalBus)
        {
            assembleAnimationController = new AssembleAnimationController(assembleObjectMainView.assembleAnimationView,signalBus);

            assemblePointControllers = new List<AssemblePointController>();
            foreach (var assemblePointView in assembleObjectMainView.assemblePointViews)
            {
                AssemblePointController assemblePointController = new AssemblePointController(assemblePointView,assembleAnimationController);
                assemblePointControllers.Add(assemblePointController);
            }
            
            dragObjectController = new DragObjectController(
                assembleAnimationController,
                assembleObjectMainView.gameObject);
        }

        public void SetConnections(List<AssemblePointController> allAssemblePointControllers)
        {
            assembleAnimationController.Init(allAssemblePointControllers);
        }
    }
}