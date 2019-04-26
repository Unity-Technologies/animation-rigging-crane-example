using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CraneInputs : MonoBehaviour
{
    public Transform ikRotation;
    public Transform iKHandle;
    public Transform clampPivot; 
    public bool clamp;
    float smoothTime = 0.3f;
    float liftVelocity = 0.0f;
    float clampVelocity = 0.0f;
    public GameObject containerCon;
    public Transform IK_RC;
    public Transform IK_LC;
    public Transform RCLift;
    public Transform LCLift;
    void Update()
    {
        //base rotation
        float turn = Input.GetAxis ("Rotation");
        var ikRot = Quaternion.Euler(0,turn,0);
        ikRotation.transform.rotation *= ikRot;

        //IK forward distance
        float fwd = Input.GetAxis ("FwdBck");
        Vector3 fwdVector = iKHandle.transform.rotation * Vector3.forward * (fwd);
        iKHandle.transform.position += fwdVector;

        //IK up/down
        float vert = Input.GetAxis ("UpDown");
        Vector3 upVector = iKHandle.transform.rotation * Vector3.up * (vert);
        iKHandle.transform.position += upVector;

        //clamp pivot
        float pivot = Input.GetAxis ("Pivot");
        var tip = Quaternion.Euler(0,pivot,0);
        clampPivot.transform.rotation *= tip;

        //input limits
        float zLimit = iKHandle.transform.localPosition.z;
        float yLimit = iKHandle.transform.localPosition.y;
        zLimit = Mathf.Clamp(zLimit, 12.0f, 38.0f);
        Vector3 zVector = new Vector3 (0,0,zLimit);
        yLimit = Mathf.Clamp(yLimit, -8.0f, 12.0f);
        Vector3 yVector = new Vector3 (0,yLimit,0);

        Vector3 ikVector = yVector + zVector;
        iKHandle.transform.localPosition = ikVector;

        
        // CLAMPS
        if (Input.GetButtonDown("Drop"))
		{
            clamp = !clamp;
		}
        
        //Container drop
        var cPConstraint = containerCon.GetComponent<MultiParentConstraint>();
        var cPconstraintData = cPConstraint.data;
        cPConstraint.weight = clamp ? 1.0f : 0.0f;
        var containerRBc = containerCon.GetComponent<Rigidbody>();
        containerRBc.isKinematic = clamp ? true : false;

        //Right Lift
        float xLR = RCLift.transform.localPosition.x;
        float yLR = clamp ? 0 : 2;
        float liftPosition = Mathf.SmoothDamp(RCLift.transform.localPosition.y, yLR, ref liftVelocity, smoothTime);
        float zLR = RCLift.transform.localPosition.z;
        var rightLift = new Vector3(xLR,liftPosition,zLR);
        RCLift.transform.localPosition = rightLift;

        //Left Lift
        float xLL = RCLift.transform.localPosition.x;
        var leftLift = new Vector3(xLL,-liftPosition,zLR);
        LCLift.transform.localPosition = leftLift;

        //Right Clamp IK 
        float xCR = clamp ? -4.75f : -3.8f;
        float clampPosition = Mathf.SmoothDamp(IK_RC.transform.localPosition.x, xCR, ref clampVelocity, smoothTime);
        float yCR = IK_RC.transform.localPosition.y;
        float zCR = IK_RC.transform.localPosition.z;
        var rightClamp = new Vector3(clampPosition,yCR,zCR);
        IK_RC.transform.localPosition = rightClamp;

        //Left Clamp IK 
        float yCL = IK_LC.transform.localPosition.y;
        var leftClamp = new Vector3(clampPosition,yCL,zCR);
        IK_LC.transform.localPosition = leftClamp;
    }
}
