using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    //script used to make an object rotation's follow another target object's rotation but only on the Y axis
    public Transform objectToFollow;
    [SerializeField] private Vector3 positionOffset;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame, updates rotation accordingly
    void Update()
    {
        transform.eulerAngles = new Vector3(0, objectToFollow.eulerAngles.y, 0);
    }
}
