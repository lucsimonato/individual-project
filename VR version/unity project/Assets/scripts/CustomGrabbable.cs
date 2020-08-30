﻿using System;
using UnityEngine;

public class CustomGrabbable : MonoBehaviour
{
    [SerializeField]
    protected bool m_allowOffhandGrab = true;
    [SerializeField]
    protected bool m_snapPosition = false;
    [SerializeField]
    protected bool m_snapOrientation = false;
    [SerializeField]
    protected Transform m_snapOffset;
    [SerializeField]
    protected Collider[] m_grabPoints = null;

    protected bool m_grabbedKinematic = false;
    protected Collider m_grabbedCollider = null;
    protected CustomHandGrabber m_grabbedBy = null;
    public float pinches;

	/// If true, the object can currently be grabbed.
    public bool allowOffhandGrab
    {
        get { return m_allowOffhandGrab; }
    }

	/// If true, the object is currently grabbed.
    public bool isGrabbed
    {
        get { return m_grabbedBy != null; }
    }

	/// If true, the object's position will snap to match snapOffset when grabbed.
    public bool snapPosition
    {
        get { return m_snapPosition; }
    }

	/// If true, the object's orientation will snap to match snapOffset when grabbed.
    public bool snapOrientation
    {
        get { return m_snapOrientation; }
    }

	/// An offset relative to the CustomHandGrabber where this object can snap when grabbed.
    public Transform snapOffset
    {
        get { return m_snapOffset; }
    }

	/// Returns the CustomHandGrabber currently grabbing this object.
    public CustomHandGrabber grabbedBy
    {
        get { return m_grabbedBy; }
    }

	/// The transform at which this object was grabbed.
    public Transform grabbedTransform
    {
        get { return m_grabbedCollider.transform; }
    }

	/// The Rigidbody of the collider that was used to grab this object.
    public Rigidbody grabbedRigidbody
    {
        get { return m_grabbedCollider.attachedRigidbody; }
    }

	/// The contact point(s) where the object was grabbed.
    public Collider[] grabPoints
    {
        get { return m_grabPoints; }
    }

	/// Notifies the object that it has been grabbed.
	virtual public void GrabBegin(CustomHandGrabber hand, Collider grabPoint)
    {
        m_grabbedBy = hand;
        m_grabbedCollider = grabPoint;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

	/// Notifies the object that it has been released.
	virtual public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = m_grabbedKinematic;
        //rb.velocity = linearVelocity;
        //rb.angularVelocity = angularVelocity;
        m_grabbedBy = null;
        m_grabbedCollider = null;
    }

    void Awake()
    {
        if (m_grabPoints.Length == 0)
        {
            // Get the collider from the grabbable
            Collider collider = this.GetComponent<Collider>();
            if (collider == null)
            {
				throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
            }

            // Create a default grab point
            m_grabPoints = new Collider[1] { collider };
        }
    }

    protected virtual void Start()
    {
        m_grabbedKinematic = GetComponent<Rigidbody>().isKinematic;
    }

    void OnDestroy()
    {
        if (m_grabbedBy != null)
        {
            // Notify the hand to release destroyed grabbables
            m_grabbedBy.ForceRelease(this);
        }
    }
}
