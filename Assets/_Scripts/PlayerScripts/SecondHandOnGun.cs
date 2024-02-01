using System;
using UnityEngine;
using System.Collections;
using Mirror;
using RootMotion.FinalIK;
 
public class SecondHandOnGun : NetworkBehaviour {
 
    // References to the IK components
    public AimIK aim;
    public FullBodyBipedIK ik;
    public LookAtIK look;
 
    // Just quick shortcuts to the hand effectors for better readability
    private IKEffector leftHand { get { return ik.solver.leftHandEffector; }}
    private IKEffector rightHand { get { return ik.solver.rightHandEffector; }}
 
    private Quaternion leftHandRotationRelative;
 
    void Start() {
        // Disabling (and initiating) the IK components
        aim.enabled = false;
        ik.enabled = false;
        look.enabled = false;
 
        ik.solver.OnPostUpdate += OnPostFBBIK; // Add to the OnPostUpdate delegate of the FBBIK solver
    }

    void LateUpdate() {


        Vector3 toLeftHandRelative = rightHand.bone.InverseTransformPoint(leftHand.bone  .position);
 
        leftHandRotationRelative = Quaternion.Inverse(rightHand.bone.rotation) * leftHand.bone.rotation;
 
        aim.solver.IKPosition = look.solver.IKPosition;
 
        aim.solver.Update();
 
        leftHand.position = rightHand.bone.TransformPoint(toLeftHandRelative);
        leftHand.positionWeight = 1f;
 
        // Making sure the right hand won't budge during solving
        rightHand.position = rightHand.bone.position;
        rightHand.positionWeight = 1f;
        ik.solver.GetLimbMapping(FullBodyBipedChain.RightArm).maintainRotationWeight = 1f;
        
        ik.solver.Update();
        
        look.solver.Update();
    }
 
    // Rotate the left hand after FBBIK has finished, called by the FBBIK solver
    private void OnPostFBBIK() {
        leftHand.bone.rotation = rightHand.bone.rotation * leftHandRotationRelative;
    }
 
}