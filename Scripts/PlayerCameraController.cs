using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine; 
//Sam Armstrong
public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private Vector2 maxFollowOffset = new Vector2(-20f, 20f);
    [SerializeField] private Vector2 maxFollowOffsetNew = new Vector2(-20f, 20f);
    [SerializeField] private Vector2 cameraVelocity = new Vector2(4f, 0.25f);
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    public static bool aliveC = true;

    public static float cameraTurnSpeedPOV = .2f;
    public static float cameraTurnSpeedFPV = 1f;

    public static float cameraVertPOV = 1;
    public static float cameraVertFPV = .25f;

    //controlling variables for how camera moves, set here for 3rd person
    public static float cameraTurnSpeed = .2f;
    public static float cameraVerticalSpeed = 1;
    //use this to plug into the new camera distance  
    public static Vector3 thirdPersonDistance = new Vector3(0, 0, -7);

    public static Vector3 firstPersonDistance = new Vector3(0, 10, -0.1f);


    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }
    private static CinemachineTransposer transposer;

    public override void OnStartAuthority()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        virtualCamera.gameObject.SetActive(true);

        enabled = true;

        Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        
    }
    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();
    public static void SetAliveC(bool tf)
    {
        aliveC = tf;
    }

    public static void ThirdPersonCameraDistance()
    {
        transposer.m_FollowOffset = thirdPersonDistance;
    }
    public static void FirstPersonCameraDistance()
    {
        transposer.m_FollowOffset = firstPersonDistance;
    }

    public static void SetCameraFirstperson(bool fp)
    {
        if(fp)
        {
            FirstPersonCameraDistance();
            cameraTurnSpeed = cameraTurnSpeedFPV;
            cameraVerticalSpeed = cameraVertFPV;
        }
        if(!fp)
        {
            ThirdPersonCameraDistance();
            cameraTurnSpeed = cameraTurnSpeedPOV;
            cameraVerticalSpeed = cameraVertPOV;
        }
    }

    private void Look(Vector2 lookAxis)
    {
        float deltaTime = Time.deltaTime;


        transposer.m_FollowOffset.y = Mathf.Clamp(transposer.m_FollowOffset.y - (lookAxis.y * cameraVelocity.y * cameraVerticalSpeed * deltaTime), maxFollowOffsetNew.x, maxFollowOffsetNew.y);
        //transposer.m_FollowOffset.y = Mathf.Clamp(transposer.m_FollowOffset.y - (lookAxis.y * cameraVelocity.y * cameraVerticalSpeed * deltaTime), -30f, 30f);
        //if (aliveC)
        //{
        playerTransform.Rotate(0f, lookAxis.x * cameraVelocity.x * cameraTurnSpeed * deltaTime, 0f);
        //}

    }
}
 