using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class tipdraw : MonoBehaviour {
    public GameObject myo = null;
    private int z = 4;
    private int s = 14;
    private Vector3 OldVector;
    private Vector3 changeVector;
    private Vector3 currVector;
    private Vector3 tempVector;
    private Quaternion _antiYaw = Quaternion.identity;
    private float _referenceRoll = 0.0f;
    private Pose _lastPose = Pose.Unknown;
    public Material waveInMaterial;
    public Material waveOutMaterial;
    public Material doubleTapMaterial;
    public bool draw = false;
    public Color nColor;
	// Use this for initialization
	void Start () {
        OldVector = new Vector3(myo.transform.forward.x, myo.transform.forward.y, z);
        currVector = new Vector3(0, 0, z);
        transform.position = new Vector3(0, 0, z);
        GetComponent<TrailRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo>();
        bool updateReference = false;
        changeVector = new Vector3(0 - (myo.transform.forward.x) * s, myo.transform.forward.y * s, z);
        tempVector = changeVector - OldVector;
        currVector = currVector + tempVector;
        //transform.position = currVector;
        OldVector = changeVector;

        if (thalmicMyo.pose != _lastPose)
        {
            _lastPose = thalmicMyo.pose;

            if (thalmicMyo.pose == Pose.FingersSpread)
            {
                updateReference = true;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }
            else if (thalmicMyo.pose == Pose.DoubleTap)
            {
                if (draw == true)
                {
                    z = 4;
                    
                    Vector3 fuck = transform.position;
                    fuck.z = z;
                    transform.position = fuck;
                }
                else
                {
                    z = 5;
                    Vector3 fuck = transform.position;
                    fuck.z = z;
                    transform.position = fuck;
                    GetComponent<TrailRenderer>().enabled = true;
                }
                draw = !draw;
            }
        }

	}
    float rollFromZero(Vector3 zeroRoll, Vector3 forward, Vector3 up)
    {

        float cosine = Vector3.Dot(up, zeroRoll);

        Vector3 cp = Vector3.Cross(up, zeroRoll);
        float directionCosine = Vector3.Dot(forward, cp);
        float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

        return sign * Mathf.Rad2Deg * Mathf.Acos(cosine);
    }

    Vector3 computeZeroRollVector(Vector3 forward)
    {
        Vector3 antigravity = Vector3.up;
        Vector3 m = Vector3.Cross(myo.transform.forward, antigravity);
        Vector3 roll = Vector3.Cross(m, myo.transform.forward);

        return roll.normalized;
    }

    float normalizeAngle(float angle)
    {
        if (angle > 180.0f)
        {
            return angle - 360.0f;
        }
        if (angle < -180.0f)
        {
            return angle + 360.0f;
        }
        return angle;
    }

    void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard)
        {
            myo.Unlock(UnlockType.Timed);
        }

        myo.NotifyUserAction();
    }
}
