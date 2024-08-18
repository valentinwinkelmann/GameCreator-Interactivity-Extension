using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
namespace vwgamedev.GameCreator
{

    [System.Serializable]
    public class InteractiveStateMarker{
        public State state;
        public Marker marker;
        public RunConditionsList runConditionsList = new RunConditionsList();
    }
}