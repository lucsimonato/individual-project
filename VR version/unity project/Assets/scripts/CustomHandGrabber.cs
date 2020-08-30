using System.Collections.Generic;
using UnityEngine;

/// Allows pinching and scaling of objects with the CustomGrabbable component on them.
[RequireComponent(typeof(Rigidbody))]
public class CustomHandGrabber : MonoBehaviour
{
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    bool alreadyUpdated = false;
    [SerializeField]
    protected bool m_parentHeldObject = false;

    [SerializeField]
    protected bool m_moveHandPosition = false;

    [SerializeField]
    protected Transform m_gripTransform = null;
    [SerializeField]
    protected Collider[] m_grabVolumes = null;

    [SerializeField]
    protected OVRInput.Controller m_controller;

    [SerializeField]
    protected Transform m_parentTransform;

    [SerializeField]
    protected GameObject m_player;

    public GameObject m_secondHand;

	protected bool m_grabVolumeEnabled = true;
    protected Vector3 m_lastPos;
    protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
    protected float m_prevIndexFlex;
    protected bool m_prevPinchThumb;
    protected float m_handsDistanceAtStart;
    protected float m_grabbableScaleAtStart;

    protected bool m_isPinching;
    protected bool m_isZooming;
	protected CustomGrabbable m_grabbedObj = null;
    protected CustomGrabbable m_pinchedObj = null;
    protected Vector3 m_grabbedObjectPosOff;
    protected Quaternion m_grabbedObjectRotOff;
	protected Dictionary<CustomGrabbable, int> m_grabCandidates = new Dictionary<CustomGrabbable, int>();
	protected bool m_operatingWithoutOVRCameraRig = true;

    public CustomGrabbable grabbedObject
    {
        get { return m_grabbedObj; }
    }

	public void ForceRelease(CustomGrabbable grabbable)
    {
        bool canRelease = (
            (m_grabbedObj != null) &&
            (m_grabbedObj == grabbable)
        );
        if (canRelease)
        {
            PinchEnd();
        }
    }

    protected virtual void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        if(!m_moveHandPosition)
        {
		    // If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.
		    OVRCameraRig rig = transform.GetComponentInParent<OVRCameraRig>();
		    if (rig != null)
		    {
			    rig.UpdatedAnchors += (r) => {OnUpdatedAnchors();};
			    m_operatingWithoutOVRCameraRig = false;
		    }
        }
    }

    protected virtual void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if(m_parentTransform == null)
        {
			m_parentTransform = gameObject.transform;
        }
    }

    virtual public void Update()
    {
        alreadyUpdated = false;
    }

    virtual public void FixedUpdate()
	{
		if (m_operatingWithoutOVRCameraRig)
        {
		    OnUpdatedAnchors();
        }
	}

    //method called on hands movement
    void OnUpdatedAnchors()
    {
        // Don't want to MovePosition multiple times in a frame, as it causes high judder in conjunction
        // with the hand position prediction in the runtime.
        if (alreadyUpdated) return;
        alreadyUpdated = true;

        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
        Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;

        if (m_moveHandPosition)
        {
            GetComponent<Rigidbody>().MovePosition(destPos);
            GetComponent<Rigidbody>().MoveRotation(destRot);
        }

        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        float prevPinchIndex = m_prevIndexFlex;
        bool prevPinchThumb = m_prevPinchThumb;
		// Update values from inputs
		m_prevIndexFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
        m_prevPinchThumb = OVRInput.Get(OVRInput.Button.One, m_controller);

		CheckForPinch(prevPinchIndex, prevPinchThumb);

        if(m_isZooming){
            CheckForZoom();
        }
    }

    void OnDestroy()
    {
        if (m_grabbedObj != null)
        {
            PinchEnd();
        }
    }

    //on entering collision with another collider, add that collider to candidates
    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
		CustomGrabbable grabbable = otherCollider.GetComponent<CustomGrabbable>() ?? otherCollider.GetComponentInParent<CustomGrabbable>();
        if (grabbable == null) return;

        // Add the grabbable
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;
    }

    //on exiting collision with another collider, remove that collider from candidates if present
    void OnTriggerExit(Collider otherCollider)
    {
		CustomGrabbable grabbable = otherCollider.GetComponent<CustomGrabbable>() ?? otherCollider.GetComponentInParent<CustomGrabbable>();
        if (grabbable == null) return;

        // Remove the grabbable
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found)
        {
            return;
        }

        if (refCount > 1)
        {
            m_grabCandidates[grabbable] = refCount - 1;
        }
        else
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    //checks if controller has begun pinching or has stopped pinching
    protected void CheckForPinch(float prevIndexFlex, bool prevThumbPress){
        if (((m_prevIndexFlex >= grabBegin) && (prevIndexFlex < grabBegin) && (m_prevPinchThumb)) || ((m_prevIndexFlex >= grabBegin) && (m_prevPinchThumb) && (!prevThumbPress)))
        {
            PinchBegin();
        }
        else if ((((m_prevIndexFlex <= grabEnd) && (prevIndexFlex > grabEnd)) || ((!m_prevPinchThumb) && (prevThumbPress))) && (m_isPinching))
        {
            PinchEnd();
        }
    }

    //scales the pinched object according to hands' distance
    protected void CheckForZoom(){
        float currentHandsDistance = Vector3.Distance(transform.position, m_secondHand.transform.position);
        float newScale = currentHandsDistance * m_grabbableScaleAtStart/m_handsDistanceAtStart;
        m_pinchedObj.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    //retrieves the correct candidate for pinch and starts pinching said object, if that object already has another hand pinching it
    protected virtual void PinchBegin(){

        float closestMagSq = float.MaxValue;
		CustomGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
		foreach (CustomGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canPinch = !(grabbable.pinches == 2);
            if (!canPinch)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }


        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            m_isPinching = true;
            closestGrabbable.pinches++;
            m_pinchedObj = closestGrabbable;
            if (m_pinchedObj.pinches == 2)
            {
                m_isZooming = true;
                m_pinchedObj.gameObject.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
                m_handsDistanceAtStart = Vector3.Distance(transform.position, m_secondHand.transform.position);
                m_grabbableScaleAtStart = m_pinchedObj.transform.localScale.x;
            }

        }


    }

    //stops pinching the currently pinched object
    protected void PinchEnd()
    {
        m_isZooming = false;
        m_isPinching = false;
        m_pinchedObj.pinches--;
        m_pinchedObj.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        m_pinchedObj = null;
        GrabVolumeEnable(true);
    }

    //enables or disables the hands' grab volumes, also clears the candidates list if disabling the grab volumes
    public virtual void GrabVolumeEnable(bool enabled)
    {
        if (m_grabVolumeEnabled == enabled)
        {
            return;
        }

        m_grabVolumeEnabled = enabled;
        for (int i = 0; i < m_grabVolumes.Length; ++i)
        {
            Collider grabVolume = m_grabVolumes[i];
            grabVolume.enabled = m_grabVolumeEnabled;
        }

        if (!m_grabVolumeEnabled)
        {
            m_grabCandidates.Clear();
        }
    }

    //clears grab candidates list
    public void clearGrabCandidates(){
        m_grabCandidates.Clear();
    }
}

