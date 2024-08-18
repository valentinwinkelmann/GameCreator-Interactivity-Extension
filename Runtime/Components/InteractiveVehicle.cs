using GameCreator.Runtime.Characters;
using UnityEngine;

namespace vwgamedev.GameCreator{
    public class InteractiveVehicle : InteractiveMonoBehaviour
    {
        public string ThisIsAField = "This is a field";

        [Range(0, 1)]
        public float RangeSlider = 0.5f;

        public Transform[] TestArray = new Transform[3];
        public Vector3 TestVector = new Vector3(1, 2, 3);
        public GameObject TestGameObject = null;

        // // Optional Overrideble Properties
        // [SerializeField] private Marker _marker; protected override Marker CharacterMarker => _marker;
        // [SerializeField] private State _state; protected override State CharacterState => _state;

        // protected override bool CharacterMount => true;
        // [SerializeField] private bool busy = false;protected override bool CharacterBusy => busy;
        // [SerializeField] private bool controlable = false; protected override bool CharacterControllable => controlable;
        // [SerializeField] private Transform[] ikTransforms; protected override Transform[] CharacterIKPoints => ikTransforms;
        public async void OnInteract(Character character)
        {
            await InteractiveUtility.WaitForInput(KeyCode.E);
            InteractionStop();
        }
        public void OnStop(Character character)
        {
        }
        public void OnFail(CancelReason reason)
        {
        }
    }

}
