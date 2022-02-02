using LowNet.ClientPackets;
using UnityEngine;

namespace LowNet.Unity3D
{
    /// <summary>
    /// LowNet Example Networkplayer Controller
    /// </summary>
    public class NetworkPlayercontroller : MonoBehaviour
    {
        /// <summary>
        /// Player Instance
        /// </summary>
        public static NetworkPlayercontroller Instance;
        #region Components
        private Animator animator;
        private bool HasAnimation = false;
        private CharacterController controller;
        internal NetworkPlayer player = null;
        /// <summary>
        /// PlayerCamera
        /// </summary>
        [Header("Player Camera"), Tooltip("Will Destroy Automatic if not Own view")]
        public GameObject PlayerCamera;
        #endregion Components

        #region Controller Settings
        /// <summary>
        /// Walk Speed
        /// </summary>
        [Range(0, 10), Header("Player Walk Speed")]
        public float speed = 6.0f;
        /// <summary>
        /// Jump Power
        /// </summary>
        [Range(0, 10), Header("Player Jump Speed")]
        public float jumpSpeed = 8.0f;
        /// <summary>
        /// Extra Gravity
        /// </summary>
        [Range(0, 10), Header("Not Ground extra Gravity")]
        public float gravity = 20.0f;
        /// <summary>
        /// Mouse Senibility
        /// </summary>
        [Range(0, 10), Header("Controll Senibility")]
        public float sensitivityX, sensitivityY = 3.4f;
        /// <summary>
        /// Can Player controll the Object
        /// </summary>
        public bool CanControll = false;
        /// <summary>
        /// Is this my Player or not
        /// </summary>
        public bool IsMyPlayer = false;
        #endregion

        /// <summary>
        /// Network Sync Rate == Position Differenz
        /// </summary>
        [Range(0, 10f)]
        public float SyncUpdate = 0.45f;
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private byte[] AnimatorSync = { 0, 0, 0, 0 };

        void Start()
        {
            bool state = TryGetComponent<NetworkPlayer>(out player);
            bool cc = TryGetComponent<CharacterController>(out controller);
            HasAnimation = TryGetComponent<Animator>(out animator);
            if (state.Equals(false))
            {
                Client.Log($"No NetworkPlayer Found on this Gameobjeckt: {gameObject.name}, Need's to Use this Component!", Enums.LogType.LogError);
                Destroy(this);
            }
            if (cc.Equals(false))
            {
                Client.Log($"No CharakterController Found on this Gameobjeckt: {gameObject.name}, Need's to Use this Component!", Enums.LogType.LogError);
                Destroy(this);
            }
            if (cc.Equals(false))
            {
                Client.Log($"No Animator Found on this Gameobjeckt: {gameObject.name}, Object will no Have Synced Animation!", Enums.LogType.LogWarning);
            }
            if (player.IsMyView.Equals(false))
                Destroy(PlayerCamera.gameObject);
            else
            {
                IsMyPlayer = true;
                CanControll = true;
            }
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }

        void Update()
        {
            if (IsMyPlayer && CanControll)
            {
                PlayerInput();
                Sync();
            }
        }

        void PlayerInput()
        {
            if (controller.isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                }
            }

            if (Input.GetAxis("Vertical") != 0 || Input.GetKey(KeyCode.W))
            {
                animator.SetInteger("walk", 1);
                AnimatorSync[0] = 1;
            }
            else
            {
                animator.SetInteger("walk", 0);
                AnimatorSync[0] = 0;
            }

            if (Input.GetAxis("Vertical") != 0 && Input.GetAxis("Vertical") <= -0.1 || Input.GetKey(KeyCode.S))
            {
                animator.SetInteger("walk", 0);
                animator.SetInteger("walkback", 2);
                AnimatorSync[1] = 1;
            }
            else
            {
                animator.SetInteger("walkback", 0);
                AnimatorSync[1] = 0;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetInteger("running", 1);
                AnimatorSync[2] = 1;
            }
            else
            {
                animator.SetInteger("running", 0);
                AnimatorSync[2] = 0;
            }

            #region Gravity 
            this.gameObject.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            #endregion

            CameraMovment();
        }

        private void CameraMovment()
        {
            float rotationX = PlayerCamera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            float rotationY = PlayerCamera.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = UnwrapAngle(Mathf.Clamp(WrapAngle(rotationY), -90.0f, 90.0f));
            PlayerCamera.transform.transform.localEulerAngles = new Vector3(rotationY, 0, 0);
        }

        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private static float UnwrapAngle(float angle)
        {
            if (angle >= 0)
                return angle;

            angle = -angle % 360;

            return 360 - angle;
        }

        void Sync()
        {
            if (Vector3.Distance(transform.position, lastPosition) >= SyncUpdate || Quaternion.Angle(transform.rotation, lastRotation) >= SyncUpdate)
            {
                Client.Log("Network Sync Update", Enums.LogType.LogDebug);
                lastPosition = transform.position;
                lastRotation = transform.rotation;
                LOWNET_PLAYER_SYNC.Sendpacket(transform.position, transform.rotation, AnimatorSync);
            }
        }

        public void SetSync(byte[] sync)
        {
            if(sync[0]== 1)
            {
                animator.SetInteger("walk", 1);
            }
            else
            {
                animator.SetInteger("walk", 0);
            }
            if (sync[1] == 1)
            {
                animator.SetInteger("walkback", 2);
            }
            else
            {
                animator.SetInteger("walkback", 0);
            }
            if (sync[2] == 1)
            {
                animator.SetInteger("running", 1);
            }
            else
            {
                animator.SetInteger("running", 0);
            }
        }
    }

    /// <summary>
    /// Player Functions
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Get my Playerinfos
        /// </summary>
        public static NetworkPlayer GetPlayer => NetworkPlayercontroller.Instance.player;
        /// <summary>
        /// Get MyPlayer Controller
        /// </summary>
        public static NetworkPlayercontroller GetController => NetworkPlayercontroller.Instance;
        /// <summary>
        /// Lock my Player
        /// </summary>
        public static bool LockPlayer => NetworkPlayercontroller.Instance.CanControll = false;
        /// <summary>
        /// Unlock my Player
        /// </summary>
        public static bool UnLockPlayer => NetworkPlayercontroller.Instance.CanControll = true;
        /// <summary>
        /// Get my Player Lock State
        /// </summary>
        public static bool GetLockstate => NetworkPlayercontroller.Instance.CanControll;
    }
}