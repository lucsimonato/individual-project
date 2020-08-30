using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class used to request microphone permissions at the beginning of the experience
public class RequestMicPermission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Microphone.Start(Microphone.devices[0],false,1,44100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
