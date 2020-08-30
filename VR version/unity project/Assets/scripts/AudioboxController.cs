using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//class containing the behaviour of the audiobox, child of the main camera the audiobox contains 
//the object to create new audios as well as audio clips of the selected item, all represented as spheres
public class AudioboxController : MonoBehaviour
{
    //public bool isMouseOver;
    public GameObject prefabSphere;
    private int audiosInBox;
    private List<GameObject> spheres;
    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        //isMouseOver = false;
        forcedStart();
        audiosInBox = 0;
        
    }

    //used to force the start of the ojbect
    public void forcedStart(){
        spheres = new List<GameObject>();
        originalPosition = transform.localPosition;
    }
    
    
    void Update()
    {
        // make the mouse right thimbstick move the audiobox, so that it does not leave the screen
        if (OVRInput.Get(OVRInput.RawButton.RThumbstickRight, OVRInput.Controller.RTouch) && transform.localPosition.x < audiosInBox * 0.15f) // forward
        {
            transform.Translate(new Vector3(0.01f,0,0));
        }
        else if (OVRInput.Get(OVRInput.RawButton.RThumbstickLeft, OVRInput.Controller.RTouch) && transform.localPosition.x > -0.5) // backwards
        {
            transform.Translate(new Vector3(-0.01f,0,0));
        }

    }

    //creates a sphere containing an audioclip inside the audiobox
    public void createAudioSphere(int audioNumber, AudioClip audioclip){
        if(spheres==null){
            forcedStart();
        }
        //create sphere and set audio
        audiosInBox++;
        GameObject sphere = Instantiate(prefabSphere, new Vector3(0, 0, 0), Quaternion.identity);
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        spheres.Add(sphere);
        sphere.GetComponent<AudioSource>().clip = audioclip;
        //position the sphere
        sphere.transform.SetParent(gameObject.transform);
        float xpos = 2 + (-1.5f*audiosInBox);
        sphere.transform.localPosition = new Vector3(xpos, 0, -0.5f);
        sphere.transform.localRotation = new Quaternion(0,0,0,0);
        //change the text on the sphere
        sphere.transform.GetChild(0).GetComponent<TextMesh>().text = audioNumber.ToString();
        //move audiobox to the right in order to center it
        transform.Translate(new Vector3(0.15f,0,0), Space.Self);
    }

    //removes all the spheres from the audiobox
    public void clearBox(){
        if(spheres!=null){
            foreach(GameObject sphere in spheres){
                Destroy(sphere);
            };
        }
        transform.localPosition = originalPosition;
        audiosInBox = 0;
    }

    //used to remove an audio from a location
    public void deleteAudioFromSelected(int audioToDelete){
        int wellID = GameObject.Find("wells").GetComponent<WellsController>().getSelectedWellID();
        if(wellID!=-1){
            removeAudioFromWell(audioToDelete, wellID);


        }else{
            int collectionID = GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getSelectedCollectionID();
            if(collectionID!=-1){
                removeAudioFromCollection(audioToDelete, collectionID);
            }
        }
    }

    //used to remova an audio from a Collection
    void removeAudioFromCollection(int audioToDelete, int collectionID){
        GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getCollectionByID(collectionID).GetComponent<SphereCollectionBehaviour>().deleteAudio(audioToDelete);
    }

    //used to remove an audio from a Well
    void removeAudioFromWell(int audioToDelete, int wellID){
        GameObject.Find("wells").GetComponent<WellsController>().getWellByID(wellID).GetComponent<WellController>().deleteAudio(audioToDelete);
    }

}
