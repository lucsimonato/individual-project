using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//class used to control the behaviour of the canvas (UI)
public class CanvasController : MonoBehaviour
{
    private bool isModelControlSelected;
    private bool isControlsShown;

    // Start is called before the first frame update, sets up the original colors in the canvas
    void Start()
    {
        isModelControlSelected = true;
        isControlsShown = false;
        this.gameObject.transform.GetChild(1).GetComponent<Text>().color = new Color(0.5f,0.5f,0.5f);
        this.gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color(1,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        //if space is pressed switch control from model to camera and vice versa, chando the highlighted text
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isModelControlSelected){
                isModelControlSelected = false;
                Camera.main.GetComponent<MyCameraController>().isActive = true;
                GameObject.Find("AllLayersWithBackSide").GetComponent<LayersMovementScript>().isActive = false;
                float transparency = this.gameObject.transform.GetChild(0).GetComponent<Text>().color.a;
                Color newColor = new Color(0.5f,0.5f,0.5f);
                newColor.a = transparency;
                this.gameObject.transform.GetChild(0).GetComponent<Text>().color = newColor;
                newColor = new Color(1,1,1);
                newColor.a = transparency;
                this.gameObject.transform.GetChild(1).GetComponent<Text>().color = newColor;

            }else{
                isModelControlSelected = true;
                Camera.main.GetComponent<MyCameraController>().isActive = false;
                GameObject.Find("AllLayersWithBackSide").GetComponent<LayersMovementScript>().isActive = true;
                float transparency = this.gameObject.transform.GetChild(0).GetComponent<Text>().color.a;
                Color newColor = new Color(0.5f,0.5f,0.5f);
                newColor.a = transparency;
                this.gameObject.transform.GetChild(1).GetComponent<Text>().color = newColor;
                newColor = new Color(1,1,1);
                newColor.a = transparency;
                this.gameObject.transform.GetChild(0).GetComponent<Text>().color = newColor;
            }
        }
        //if c is pressed shows the controls and hides everything else, or vice versa
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(isControlsShown){
                
                isControlsShown = false;

                Color newColor = this.gameObject.transform.GetChild(0).GetComponent<Text>().color;
                newColor.a = 1;
                this.gameObject.transform.GetChild(0).GetComponent<Text>().color = newColor;
                
                newColor = this.gameObject.transform.GetChild(1).GetComponent<Text>().color;
                newColor.a = 1;
                this.gameObject.transform.GetChild(1).GetComponent<Text>().color = newColor;

                newColor = this.gameObject.transform.GetChild(2).GetComponent<Text>().color;
                newColor.a = 1;
                this.gameObject.transform.GetChild(2).GetComponent<Text>().color = newColor;

                newColor = this.gameObject.transform.GetChild(3).GetComponent<Text>().color;
                newColor.a = 0;
                this.gameObject.transform.GetChild(3).GetComponent<Text>().color = newColor;
            }else{

                isControlsShown = true;

                Color newColor = this.gameObject.transform.GetChild(0).GetComponent<Text>().color;
                newColor.a = 0;
                this.gameObject.transform.GetChild(0).GetComponent<Text>().color = newColor;
                
                newColor = this.gameObject.transform.GetChild(1).GetComponent<Text>().color;
                newColor.a = 0;
                this.gameObject.transform.GetChild(1).GetComponent<Text>().color = newColor;

                newColor = this.gameObject.transform.GetChild(2).GetComponent<Text>().color;
                newColor.a = 0;
                this.gameObject.transform.GetChild(2).GetComponent<Text>().color = newColor;

                newColor = this.gameObject.transform.GetChild(3).GetComponent<Text>().color;
                newColor.a = 1;
                this.gameObject.transform.GetChild(3).GetComponent<Text>().color = newColor;
            }
        }
    }
}
