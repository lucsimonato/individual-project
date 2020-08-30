using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behaviour attached to the main camera
public class MyCameraController : MonoBehaviour
{
    public bool isActive;
    public bool isInPlaceMode;
    private Transform myTransform;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    public GameObject prefabSphere;
    private AudioClip clipToEnter;
    private int audiosInScene;
    private int collectionsInScene;
    // Start is called before the first frame update
    void Start()
    {
        isInPlaceMode =false;
        isActive = false;
        myTransform = gameObject.transform;
        originalPosition = myTransform.position;
        originalRotation = myTransform.rotation;
        originalScale = myTransform.localScale;
        //gets how many audios are present in the scene
        audiosInScene = PlayerPrefs.GetInt("AudiosInScene",0);
        collectionsInScene = PlayerPrefs.GetInt("CollectionsInScene",0);
    }

    // Update is called once per frame
    void Update()
    {
        //check if camera is moving if is in camera mode
        if(isActive){
            checkForCameraMovement();
        }
        //reset camera position when r is pressed
        if(Input.GetKeyDown(KeyCode.R)){
            resetCameraPosition();
        }
        //deletes all audios in scene, and also those attachedto wells
        if(Input.GetKeyDown(KeyCode.H)){
            PlayerPrefs.DeleteAll();
        }

        //Placemode is used after recording an audio withaut having selected a well or collection before
        //after the audio has finished recording the user enters placemode where he can either click on 
        //an already existing collection to add the clip to that collection or click somewhere else to create a new
        //collection and assign the audio to that collection
        //onclick either generates a new collection or select the clicked one, assign new audio, update player prefs and show audiobox again
        if(Input.GetMouseButtonDown(0) && isInPlaceMode){
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject sphere = null;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.tag != "SphereCollection"){
                    sphere = Instantiate(prefabSphere, hit.point, Quaternion.identity);
                    sphere.transform.SetParent(GameObject.Find("CollectionsContainer").transform);
                    sphere.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    sphere.GetComponent<SphereCollectionBehaviour>().setID(collectionsInScene);
                    //refactor coordinates according to how the model was scaled and rotated
                    Transform modelTransform = sphere.transform.parent.parent;
                    Vector3 tempscale = modelTransform.localScale;
                    Quaternion temprotation = modelTransform.rotation;
                    modelTransform.rotation = Quaternion.identity;
                    modelTransform.localScale = new Vector3(1, 1, 1);
                    Vector3 realPosition = sphere.transform.position;
                    modelTransform.localScale = tempscale;
                    modelTransform.rotation = temprotation;
                    //save sphere coordinates in player prefs
                    PlayerPrefs.SetString("SphereCollectionPosition_"+collectionsInScene, realPosition.x+","+realPosition.y+","+realPosition.z);
                    collectionsInScene++;
                    PlayerPrefs.SetInt("CollectionsInScene", collectionsInScene);
                }else{
                    sphere = hit.collider.gameObject;
                }
                if(sphere!=null){
                    isInPlaceMode = false;
                    string audioName = "SceneAudio_"+audiosInScene;
                    SavWav.Save(audioName, clipToEnter);
                    sphere.GetComponent<SphereCollectionBehaviour>().assignAudio(clipToEnter);
                    //sphere.GetComponent<SphereCollectionBehaviour>().select();
                    //update player prefs to have SphereCollectionAudios_<ID> to hold a record of all the audios ID it should contain
                    string stringToSet = "SphereCollectionAudios_"+sphere.GetComponent<SphereCollectionBehaviour>().getID();
                    PlayerPrefs.SetString(stringToSet, PlayerPrefs.GetString(stringToSet,"")+audiosInScene+"/");
                    audiosInScene++;
                    PlayerPrefs.SetInt("AudiosInScene", audiosInScene);
                    PlayerPrefs.Save();
                    transform.GetChild(0).gameObject.SetActive(true);
                }
            }  
        }
    }

    //check for keyboard inputs for controlling the camera's transform
    private void checkForCameraMovement()
    {
        
        //camera move right/left
        if (Input.GetKey(KeyCode.D))
        {
            myTransform.Translate(Vector3.right* 0.5f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            myTransform.Translate(Vector3.left* 0.5f);
        }

        //camera move forward/backwards
        if (Input.GetKey(KeyCode.W))
        {
            myTransform.Translate(Vector3.forward* 0.5f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            myTransform.Translate(Vector3.back* 0.5f);
        }

        //camera move up/down
        if (Input.GetKey(KeyCode.E))
        {
            myTransform.Translate(Vector3.up* 0.5f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            myTransform.Translate(Vector3.down* 0.5f);
        }

        //rotate camera left and right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            myTransform.Rotate(new Vector3(0,0.5f,0), Space.Self);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            myTransform.Rotate(new Vector3(0,-0.5f,0), Space.Self);
        }

        //rotate camera un and down
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myTransform.Rotate(new Vector3(-0.5f,0,0), Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            myTransform.Rotate(new Vector3(0.5f,0,0), Space.Self);
        }

    }

    //moves camera back to the original position
    public void resetCameraPosition(){
        myTransform.position = originalPosition;
        myTransform.rotation = originalRotation;
        myTransform.localScale = originalScale;
    }

    //enters the mode in wich users can pick where to assign the new audio clip
    public void enterPlaceMode(AudioClip clip){
        clipToEnter = clip;
        isInPlaceMode = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    //create an audiocollection with the specified id at the specified position and returns it
    public GameObject assignCollectionToScene(int id, Vector3 spherePosition){
        GameObject sphere = Instantiate(prefabSphere, spherePosition, Quaternion.identity);
        sphere.transform.SetParent(GameObject.Find("CollectionsContainer").transform);
        sphere.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        sphere.GetComponent<SphereCollectionBehaviour>().setID(id);
        return sphere;
    }

    //returns how many audioclips are present in the scene (not attached to wells)
    public int getAudiosInScene(){
        return audiosInScene;
    }
    
    //increments the number of audios in the scene
    public void incrementAudiosInScene(){
        audiosInScene++;
    }
}
