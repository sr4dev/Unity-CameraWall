using UnityEngine;

namespace PerspectiveCameraWall.Sample1
{
    
    [RequireComponent(typeof(CharacterController))]
    public class SampleCameraTestPlayer : MonoBehaviour
    {
        [Range(0.1f, 10f)]
        public float speed = 2.0f;

        [Range(0.1f, 40f)]
        public float gravity = 20.0f;

        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            SampleCameraManager.Instance.target = gameObject;
        }

        private void FixedUpdate()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 moveDir = new Vector3(0, -gravity * Time.fixedDeltaTime, 0);
            Vector3 camAngles = SampleCameraManager.Instance.mainCamera.transform.eulerAngles;
            Quaternion newQuaternion = Quaternion.Euler(new Vector3(0, camAngles.y, camAngles.z));
            Vector3 camDir = newQuaternion * input.normalized;

            if (_controller.isGrounded)
            {
                var mag = input.sqrMagnitude;

                if (mag < 0.3f * 0.3f)
                {
                    moveDir += camDir * (speed * Time.fixedDeltaTime * 0.3f);
                }
                else
                {
                    moveDir += camDir * (speed * Time.fixedDeltaTime);
                }
            }

            if (moveDir != Vector3.zero)
                transform.LookAt(transform.position + new Vector3(camDir.x, 0, camDir.z));

            _controller.Move(moveDir);
        }
    }
}