using UnityEngine;

namespace ProjectATLAS.Simulation.Sim06_AircraftTesting
{
    public class MeteorTracker : MonoBehaviour
    {
        public Transform target;
        public float speed = 5f;
        public float trackingStrength = 2f;

        void Update()
        {
            if (target == null) return;

            // Smooth follow horizontally (focus sa player)
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, target.position.x, Time.deltaTime * trackingStrength);
            pos.y -= speed * Time.deltaTime; // pababa ang movement
            transform.position = pos;
        }
    }
}
