using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//used to control the behavior of buttons used to hide or show model's layers
public class LayerSelectionButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonEvent : UnityEvent { }
    public GameObject layer;
    public float pressLength;
    public bool pressed;
    public bool pressable;
    private bool isLayerVisible;
    public GameObject table;
    public Collider[] collidersToIgnore = null;
    public GameObject[] collidersToIgnoreChildren = null;
    [SerializeField] GameObject cross;

    Vector3 startPos;
    Rigidbody rb;
    public bool debug;

    //sets the button to ignore the unwanted collisions
    void Start()
    {
        startPos = transform.localPosition;
        pressable = true;
        rb = GetComponent<Rigidbody>();
        for(int i = 0;i<collidersToIgnore.Length;i++){
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collidersToIgnore[i],true);
        }
        for(int i = 0;i<collidersToIgnoreChildren.Length;i++){
            Collider[] colliders = collidersToIgnoreChildren[i].GetComponentsInChildren<Collider>();
			foreach (Collider c in colliders)
			{
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), c,true);
            }
        }
        isLayerVisible = true;
    }

    //only allows the button to move within the desireb range, if distance moved is greater then set the position to the max distance
    //if the button is moving in the opposite direction, set the position to the original position
    void Update()
    {
        float distance = Mathf.Abs(transform.localPosition.z - startPos.z);
        if (debug) Debug.Log(distance);
        if (distance >= pressLength)
        {
            
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, startPos.z - pressLength);
            if (!pressed && pressable)
            {
                pressed = true;
                pressable = false;
                // hide/show layer
                if (isLayerVisible){
                    layer.SetActive(false);
                    cross.SetActive(true);
                    isLayerVisible = false;
                }else{
                    layer.SetActive(true);
                    cross.SetActive(false);
                    isLayerVisible = true;
                }
            }
        } else
        {
            // If we aren't all the way down, reset our press
            pressed = false;
        }
        if(distance < pressLength/2){
            pressable = true;
        }

        //prevent backwards movement
        if (transform.localPosition.z > startPos.z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, startPos.z);
        }
        if (debug) Debug.Log(distance);
    }
}