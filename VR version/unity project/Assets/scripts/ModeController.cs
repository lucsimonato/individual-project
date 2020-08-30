using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


//controls navigation between modes
//0 is layer mode, where user can hie or show layers
//1 is model mode, where user can move and scale the model
//2 is pointer mode, where user can use a pointer to select locations and annotate the data
//this class also contains evaluation tests

public class ModeController : MonoBehaviour
{
    private int fpscounter = 0;
    private float fpstimer = 0;
    private bool isfpsCounting = false;
    private int numOfModes = 3;
    private int mode;
    public GameObject layersControlTable;
    private bool pressed;
    public Collider colliderToDisable;
    public GameObject RHand;
    public GameObject LHand;
    public GameObject pointer;
    [SerializeField] GameObject audioBox;
    [SerializeField] GameObject prefabSphere;

    private bool isInPlaceMode;

    private AudioClip clipToEnter;
    private int collectionsInScene;
    private int audiosInScene;
    private Vector3 modelStartLocalPos;
    private Vector3 modelStartLocalScale;
    private Quaternion modelStartRotation;
    [SerializeField] GameObject player;
    private bool isCounting = false;
    private float timeElapsed = 0;
    [SerializeField] private String sceneToLoad;


    void Start()
    {
        mode = 0;
        pressed = false;
        isInPlaceMode = false;
        setModelMovingEnabled(false);
        audiosInScene = PlayerPrefs.GetInt("AudiosInScene",0);
        collectionsInScene = PlayerPrefs.GetInt("CollectionsInScene",0);
        modelStartLocalPos = transform.parent.localPosition;
        modelStartLocalScale = transform.parent.localScale;
        modelStartRotation = transform.parent.rotation;
    }

    // Update is called once per frame and checks for inputs
    void Update()
    {
        //clears all the player preferences and annotations
        if(Input.GetKeyDown(KeyCode.H)){
            PlayerPrefs.DeleteAll();
        }

        //changes mode and prevents multiple clicks
        if(OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch)){
            if(!pressed){
                pressed = true;
                changeMode();
            }
        }else{
            pressed = false;
        }
        //if in placemode, place audio on trigger pull
        if(isInPlaceMode && OVRInput.Get(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch)){
            placeAudio();
        }

        //if left trigger is pulled for 5 seconds, launches welcome scene
        if(mode == 2 && OVRInput.Get(OVRInput.RawButton.LIndexTrigger, OVRInput.Controller.LTouch)){
            isCounting = true;
            timeElapsed += Time.deltaTime;
            if(timeElapsed > 5){
                resetAll();
            }
        }else{
            isCounting = false;
            timeElapsed = 0;
        }

        if(isfpsCounting){
            Debug.Log("frame number "+fpscounter+" : time elasped "+ fpstimer);
            fpscounter++;
            fpstimer += Time.deltaTime;
        }
        
    }


    // used to change mode, goes to the following mode and sets the correct UI
    // this method also contains evaluation tests
    private void changeMode(){
        if(isInPlaceMode)
            return;
        mode++;
        if(mode == numOfModes) mode = 0;

        switch (mode)
        {
            case 0:
                setModelMovingEnabled(false);
                setTableEnabled(true);
                setPointerEnabled(false);
                setAudioBoxEnabled(false);
                isfpsCounting = false;
                fpscounter = 0;
                fpstimer = 0;
                break;
            case 1:
                setModelMovingEnabled(true);
                setTableEnabled(false);
                setPointerEnabled(false);
                setAudioBoxEnabled(false);
                break;
            case 2:
                setModelMovingEnabled(false);
                setTableEnabled(false);
                setPointerEnabled(true);
                setAudioBoxEnabled(true);
                //uncomment next line for debug 
                //isfpsCounting = true;
                break;
            default :
                return;
            
        }
    }

    //activates or deactivates model movement and scaling
    private void setModelMovingEnabled(bool enabled){
        colliderToDisable.enabled = enabled;
        RHand.GetComponent<OVRGrabber>().clearGrabCandidates();
        LHand.GetComponent<OVRGrabber>().clearGrabCandidates();
        RHand.GetComponent<CustomHandGrabber>().clearGrabCandidates();
        LHand.GetComponent<CustomHandGrabber>().clearGrabCandidates();
        RHand.GetComponent<CustomHandGrabber>().GrabVolumeEnable(true);
        LHand.GetComponent<CustomHandGrabber>().GrabVolumeEnable(true);
    }

    //enables or disables layer selection table
    private void setTableEnabled(bool enabled){
        layersControlTable.SetActive(enabled);
    }

    //enables or disables pointer, also deactivates or activates player's hand
    private void setPointerEnabled(bool enabled){
        pointer.SetActive(enabled);
        RHand.SetActive(!enabled);
    }

    //enables or disables audiobox
    private void setAudioBoxEnabled(bool enabled){
        audioBox.SetActive(enabled);
    }

    //enters place mode, next pointerclick will place audio in  scene
    public void enterPlaceMode(AudioClip clip){
        clipToEnter = clip;
        isInPlaceMode = true;
        setAudioBoxEnabled(false);
    }

    //exits placemode
    public void quitPlaceMode(){
        isInPlaceMode = false;
        setAudioBoxEnabled(true);
        //resets the click possibility
        audioBox.transform.GetChild(0).GetComponent<PointerEvents>().resetClicked();
    }

    //used to place a new annotation in the scene, 
    //if a collection wasn't hit, generates a new collection, saves its position in player prefs, adds audio to that collection
    //if a collection was hit, place the audio in the clicked collection
    private void placeAudio(){
        Tuple<bool, RaycastHit> outcome = pointer.GetComponent<Pointer>().GetHitForPlaceMode();
        if(!outcome.Item1)
            return;
        GameObject sphere = null;
        RaycastHit hit = outcome.Item2;
        if(hit.collider.gameObject.tag != "SphereCollection"){
                    sphere = Instantiate(prefabSphere, hit.point, Quaternion.identity);
                    sphere.transform.SetParent(GameObject.Find("CollectionsContainer").transform);
                    sphere.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    sphere.GetComponent<SphereCollectionBehaviour>().setID(collectionsInScene);
                    //refactor coordinates according to how the model was scaled and rotated
                    Transform modelTransform = transform.parent;
                    Vector3 tempscale = modelTransform.localScale;
                    Quaternion temprotation = modelTransform.rotation;
                    Vector3 tempPos = modelTransform.localPosition;
                    modelTransform.rotation = modelStartRotation;
                    modelTransform.localScale = modelStartLocalScale;
                    modelTransform.localPosition = modelStartLocalPos;
                    Vector3 realPosition = sphere.transform.localPosition;
                    modelTransform.localScale = tempscale;
                    modelTransform.rotation = temprotation;
                    modelTransform.localPosition = tempPos;
                    //save sphere coordinates in player prefs
                    PlayerPrefs.SetString("SphereCollectionPosition_"+collectionsInScene, realPosition.x+","+realPosition.y+","+realPosition.z);
                    collectionsInScene++;
                    PlayerPrefs.SetInt("CollectionsInScene", collectionsInScene);
                }else{
                    sphere = hit.collider.gameObject;
                }
                if(sphere!=null){
                    string audioName = "SceneAudio_"+audiosInScene;
                    SavWav.Save(audioName, clipToEnter);
                    sphere.GetComponent<SphereCollectionBehaviour>().assignAudio(clipToEnter);
                    sphere.GetComponent<SphereCollectionBehaviour>().select();
                    //update player prefs to have SphereCollectionAudios_<ID> to hold a record of all the audios ID it should contain
                    string stringToSet = "SphereCollectionAudios_"+sphere.GetComponent<SphereCollectionBehaviour>().getID();
                    PlayerPrefs.SetString(stringToSet, PlayerPrefs.GetString(stringToSet,"")+audiosInScene+"/");
                    audiosInScene++;
                    PlayerPrefs.SetInt("AudiosInScene", audiosInScene);
                    PlayerPrefs.Save();
                    quitPlaceMode();
                }
        
    }

    //increments the number of audios in the scene
    public void incrementAudiosInScene(){
        audiosInScene++;
    }

    //returns how many audioclips are present in the scene (not attached to wells)
    public int getAudiosInScene(){
        return audiosInScene;
    }

    //launches the welcome scene
    private void resetAll(){
        if(sceneToLoad!="null")
                    SceneManager.LoadScene(sceneToLoad);
    }
}
