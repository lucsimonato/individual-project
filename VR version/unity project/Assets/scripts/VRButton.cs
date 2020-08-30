using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//class used to handle behavior of buttons in the welcome scene
public class VRButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    public string sceneToLoad;
    public float pressLength;
    public bool pressed;

    Vector3 startPos;
    Rigidbody rb;

    //remember the start position
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    //only allows the button to move within the desireb range, if distance moved is greater then set the position to the max distance
    //if the button is moving in the opposite direction, set the position to the original position
    void Update()
    {
        float distance = Mathf.Abs(transform.position.z - startPos.z);
        if (distance >= pressLength)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z - pressLength);
            if (!pressed)
            {
                pressed = true;
                // load next scene
                if(sceneToLoad!="null")
                    SceneManager.LoadScene(sceneToLoad);
            }
        } else
        {
            // If we aren't all the way down, reset our press
            pressed = false;
        }
        if (transform.position.z > startPos.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z);
        }
    }
}