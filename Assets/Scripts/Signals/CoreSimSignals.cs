using UnityEngine;

namespace Signals
{
    public class CheckSimFinishSignal {}
    public class SimFinishSignal{}

    public class LearnPartSignal
    {
        public Vector3 position;
        public string name;
    }
    
    //Called when a part need assembly of another part
    //Or when disassembly of a part needs another disassembly
    public class PartNeedPriorityMessage
    {
        public string assemblyMessage;
    }
}