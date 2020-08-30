using UnityEngine;
using System;

//class used to represent a virtual pointer in the scene
public class Pointer : MonoBehaviour
{
    public float defaultLength = 3.0f;
    private LineRenderer lineRenderer = null;
    private void Awake(){
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update(){
        UpdateLength();
    }

    //updates the length of the poointer
    private void UpdateLength(){
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, CalculateEnd());
    }

    //calculates end point of the pointer
    private Vector3 CalculateEnd(){
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(defaultLength);

        if(hit.collider){
            endPosition = hit.point;
        }

        return endPosition;
    }

    //creates a forward raycast spawning from the player's hand
    private RaycastHit CreateForwardRaycast(){
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

    //returns the default end of the pointer if no collider was hit
    private Vector3 DefaultEnd(float length){
        return transform.position + (transform.forward * length);
    }

    //returns hit point in a particular format, used for place mode
    public Tuple<bool,RaycastHit> GetHitForPlaceMode(){
        RaycastHit hit;
        bool outcome;
        Ray ray = new Ray(transform.position, transform.forward);
        outcome = Physics.Raycast(ray, out hit, defaultLength);
        return new Tuple<bool, RaycastHit>(outcome, hit);
    }
}
