using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KartRace
{
    public class KartController : MonoBehaviour
    {
        [Header("Spawn Point Manager")]
        [SerializeField] private SpawnPointManager spawnPointManager;
        [Header("References")]
        [SerializeField] private Transform kartModel;
        [SerializeField] private Transform kartNormal;
        [SerializeField] private Rigidbody sphere;
        public Rigidbody Sphere => sphere;

        [Header("Parameters")]
        [SerializeField] private float acceleration = 30f;
        [SerializeField] private float steering = 30f;
        [SerializeField] private float gravity = 10f;

        [Header("Movement Settings")]
        float _speed, _currentSpeed;
        float _rotate, _currentRotate;

        [Header("Environment")]
        [SerializeField] private LayerMask layerMask;

        [Header("Model Parts")]
        [SerializeField] private Transform leftFrontWheels;
        [SerializeField] private Transform rightFrontWheels;
        [SerializeField] private Transform leftBackWheels;
        [SerializeField] private Transform rightBackWheels;
        [SerializeField] private Transform steeringWheel;
        
        [Header("Visual Settings")]
        [SerializeField] private float maxWheelTurnAngle = 30f;
        [SerializeField] private float maxSteeringWheelAngle = 45f;

        private void Awake()
        {
            // spawnPointManager = FindAnyObjectByType<SpawnPointManager>();   
        }
        private void Update()
        {
            // Sphere Position
            SpherePosition(sphere.transform.position);    

            // Acceleration and Steering
            float v = Input.GetAxis("Vertical"); 
            float h = Input.GetAxis("Horizontal");

            // Move and Steer
            MoveHandler(v, h);

            // Visual Steering
            UpdateSteeringVisuals();

        }            

        private void FixedUpdate()
        {
            // Forward Acceleration
            sphere.AddForce(-kartModel.transform.right * _currentSpeed, ForceMode.Acceleration);

            // Gravity
            sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

            // Steering
            // transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + _currentRotate, 0), Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                transform.rotation * Quaternion.Euler(0, _currentRotate, 0),
                Time.deltaTime * 5f
            );

            Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out RaycastHit hitOn, 1.1f,layerMask);
            Physics.Raycast(transform.position + (transform.up * .1f)   , Vector3.down, out RaycastHit hitNear, 2.0f, layerMask);

            //Normal Rotation
            kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
            kartNormal.Rotate(0, transform.eulerAngles.y, 0);

        }

        public void MoveHandler(float v, float h)
        {
            if (v != 0) Move(Mathf.Sign(v) == 1 ? 1 : -1, Mathf.Abs(v));
            if (h != 0) Steer(Mathf.Sign(h) == 1 ? 1 : -1, Mathf.Abs(h));
            _currentSpeed = Mathf.SmoothStep(_currentSpeed, _speed, Time.deltaTime * 12f); 
            _speed = 0f;
            _currentRotate = Mathf.Lerp(_currentRotate, _rotate, Time.deltaTime * 4f);
            _rotate = 0f;
        }

        public void Respawn()
        {
            if (spawnPointManager == null)
                return;
            Transform spawn = spawnPointManager.SelectRandomSpawnpoint();
            sphere.velocity = Vector3.zero;
            sphere.angularVelocity = Vector3.zero;

            sphere.position = spawn.position;
            sphere.rotation = spawn.rotation;
            transform.position = spawn.position - new Vector3(0, 1f, 0);
            transform.rotation = spawn.rotation;
            sphere.rotation = spawn.rotation;

            transform.localRotation = Quaternion.Euler(0, 270, 0);

        }

        private void SpherePosition(Vector3 pos)
        {
            transform.position = pos - new Vector3(0, 1f, 0);
        }

        public void Steer(int direction, float amount)
        {
            _rotate = (steering * direction) * amount;
        }

        public void Move(int direction, float amount)
        {
            _speed = (acceleration * direction) * amount;
        }
        private void UpdateSteeringVisuals()
        {
            float wheelAngle = Input.GetAxis("Horizontal") * maxWheelTurnAngle;
            float steerAngle = Input.GetAxis("Horizontal") * maxSteeringWheelAngle;
            // Wheel Rotation
            leftFrontWheels.localEulerAngles = new Vector3(0, wheelAngle, leftFrontWheels.localEulerAngles.z);
            rightFrontWheels.localEulerAngles = new Vector3(0, wheelAngle, rightFrontWheels.localEulerAngles.z);
            // Wheel Spin
            // Calculate the spin amount based on the sphere's velocity and direction
            float direction = Vector3.Dot(sphere.velocity.normalized, -kartModel.right) >= 0 ? 1f : -1f;
            float spinAmount = direction * sphere.velocity.magnitude * 360f * Time.deltaTime;
            leftFrontWheels.localEulerAngles += new Vector3(0, 0, spinAmount / 2);
            rightFrontWheels.localEulerAngles += new Vector3(0, 0, spinAmount / 2);
            leftBackWheels.localEulerAngles += new Vector3(0, 0, spinAmount / 2);
            rightBackWheels.localEulerAngles += new Vector3(0, 0, spinAmount / 2);

            // Steering Wheel Rotation
            steeringWheel.localEulerAngles = new Vector3(-25, 90, steerAngle);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
            Gizmos.DrawLine(transform.position + transform.up, transform.position + (transform.forward * 2));
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 400, 400), $"Speed: {_currentSpeed:F2}");
        }

    }
}
