using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class used to move the model
public class LayersMovementScript : MonoBehaviour
{
    public bool isActive;
    private Transform myTransform;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool[] visibleLayers;
    private bool isWellVisible;
    // Start is called before the first frame update
    private void Start()
    {
        isActive = true;
        myTransform = gameObject.transform;
        originalPosition = myTransform.position;
        originalRotation = myTransform.rotation;
        originalScale = myTransform.localScale;
        visibleLayers = new bool[6];
        isWellVisible = true;
        for (int i = 0;i<6;i++){
            visibleLayers[i] = true;
        }
    }

    // Update is called once per frame
    //if r is pressed reset the model to original position
    //if in model control mode, check for model movement
    //always check for layers selection to hide/show layers and wells
    private void Update()
    {
        if(isActive){
            checkForLayersMovement();
        }
        checkForLayersSelection();
        if(Input.GetKeyDown(KeyCode.R)){
            resetModel();
        }
    }


    //used to hide/show layers of the model and wells and make them unclickable
    private void checkForLayersSelection()
    {
        //if 0 is pressed show/hide all layers
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(visibleLayers[0]){
                visibleLayers[0] = false;
                for(int i = 0;i<5;i++){
                    visibleLayers[i+1] = false;
                    myTransform.GetChild(i).GetChild(0).GetComponent<Renderer>().enabled = false;
                    myTransform.GetChild(i).GetChild(1).GetComponent<Renderer>().enabled = false;
                    myTransform.GetChild(i).GetChild(0).GetComponent<MeshCollider>().enabled = false;
                    myTransform.GetChild(i).GetChild(1).GetComponent<MeshCollider>().enabled = false;


                }
            }else{
                visibleLayers[0] = true;
                for(int i = 0;i<5;i++){
                    visibleLayers[i+1] = true;
                    myTransform.GetChild(i).GetChild(0).GetComponent<Renderer>().enabled = true;
                    myTransform.GetChild(i).GetChild(1).GetComponent<Renderer>().enabled = true;
                    myTransform.GetChild(i).GetChild(0).GetComponent<MeshCollider>().enabled = true;
                    myTransform.GetChild(i).GetChild(1).GetComponent<MeshCollider>().enabled = true;

                }
            }
        }

        //if 1-2-3-4-5 is pressed hide/show the corresponding layer
        for(int i = 1;i<6;i++){
            if (Input.GetKeyDown(i.ToString()))
            {
                if(visibleLayers[i]){
                    visibleLayers[i] = false;
                    myTransform.GetChild(i-1).GetChild(0).GetComponent<Renderer>().enabled = false;
                    myTransform.GetChild(i-1).GetChild(1).GetComponent<Renderer>().enabled = false;
                    myTransform.GetChild(i-1).GetChild(0).GetComponent<MeshCollider>().enabled = false;
                    myTransform.GetChild(i-1).GetChild(1).GetComponent<MeshCollider>().enabled = false;
                }else{
                    visibleLayers[i] = true;
                    myTransform.GetChild(i-1).GetChild(0).GetComponent<Renderer>().enabled = true;
                    myTransform.GetChild(i-1).GetChild(1).GetComponent<Renderer>().enabled = true;
                    myTransform.GetChild(i-1).GetChild(0).GetComponent<MeshCollider>().enabled = true;
                    myTransform.GetChild(i-1).GetChild(1).GetComponent<MeshCollider>().enabled = true;
                }
            }
        }

        //if x is pressed hide/show the wells
        if(Input.GetKeyDown(KeyCode.X)){
            if(isWellVisible){
                isWellVisible = false;
                for(int i=0;i<32;i++){
                    myTransform.GetChild(6).GetChild(i).GetChild(0).GetComponent<Renderer>().enabled = false;
                    myTransform.GetChild(6).GetChild(i).GetChild(0).GetComponent<MeshCollider>().enabled = false;

                }
            }else{
                isWellVisible = true;
                for(int i=0;i<32;i++){
                    myTransform.GetChild(6).GetChild(i).GetChild(0).GetComponent<Renderer>().enabled = true;
                    myTransform.GetChild(6).GetChild(i).GetChild(0).GetComponent<MeshCollider>().enabled = true;
                }
            }
        }
    }

    //check for keyboard inputs for controlling the model's transform
    private void checkForLayersMovement()
    {
        //make the model bigger/smaller E/Q
        if (Input.GetKey(KeyCode.E))
        {
            var x = myTransform.localScale.x;
            x = x*1.01f;
            myTransform.localScale = new Vector3(x,x,x);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            var x = myTransform.localScale.x;
            x = x*0.99f;
            myTransform.localScale = new Vector3(x,x,x);
        }
        //rotate the model vertically A/D
        if (Input.GetKey(KeyCode.A))
        {
            myTransform.Rotate(new Vector3(0,0,1), Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            myTransform.Rotate(new Vector3(0,0,-1), Space.Self);
        }
        //rotate the model horizontally W/S
        if (Input.GetKey(KeyCode.W))
        {
            myTransform.Rotate(new Vector3(1,0,0), Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            myTransform.Rotate(new Vector3(-1,0,0), Space.World);
        }
    }

    //reset model to original position and display all layers
    public void resetModel(){
        
        myTransform.position = originalPosition;
        myTransform.rotation = originalRotation;
        myTransform.localScale = originalScale;

        visibleLayers[0] = true;
        for(int i = 0;i<5;i++){
            visibleLayers[i+1] = true;
            myTransform.GetChild(i).GetChild(0).GetComponent<Renderer>().enabled = true;
            myTransform.GetChild(i).GetChild(1).GetComponent<Renderer>().enabled = true;
        }
    }
}
