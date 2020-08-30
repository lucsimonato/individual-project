using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

//class used to automatically rotate model
public class modelRotation : MonoBehaviour
{   
    public Ease moveEase;
    public Ease rotateEase;
 
    private Vector3 initialLocalPosition;
    private Vector3 initialLocalRotation;
    public float returnSpeed;
    public float rotationSpeed;
    public bool m_isGrabbed;
    // Start is called before the first frame update
    //remember initial position
    private void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    // move the model at costand predefined speed indipendently from frame rate
    //check is model was grabbed or released 
    void Update()
    {   
        bool wasGrabbed = m_isGrabbed;
        m_isGrabbed = gameObject.GetComponent<OVRGrabbable>().isGrabbed;
        if(!m_isGrabbed){
            transform.Rotate(new Vector3(0,rotationSpeed,0) * Time.deltaTime, Space.World);
        }
        if(wasGrabbed && !m_isGrabbed){
            isReleased();
        }
        if(!wasGrabbed && m_isGrabbed){
            isGrabbed();
        }
    }

    //on model release, start movement back to the original position
    public void isReleased()
    {
        transform.DOLocalMove(initialLocalPosition, returnSpeed).SetSpeedBased().SetEase(moveEase);
        transform.DOLocalRotate(initialLocalRotation, rotationSpeed * 2).SetSpeedBased().SetEase(rotateEase);
    }
 
    //on model grab,stop movement towards original position
    public void isGrabbed()
    {
        transform.DOKill();
    }
}