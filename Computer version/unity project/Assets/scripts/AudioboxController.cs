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
        forcedStart();
        
    }

    //used to force the start of the ojbect
    public void forcedStart(){
        spheres = new List<GameObject>();
        originalPosition = transform.localPosition;
    }
    
    
    void Update()
    {
        // make the mouse wheel move the audiobox, so that it does not leave the screen
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && transform.localPosition.x < audiosInBox * 1.5f) // forward
        {
            transform.Translate(new Vector3(0.2f,0,0));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && transform.localPosition.x > -6) // backwards
        {
            transform.Translate(new Vector3(-0.2f,0,0));
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
        spheres.Add(sphere);
        sphere.GetComponent<AudioSource>().clip = audioclip;
        //position the sphere
        sphere.transform.SetParent(Camera.main.transform.GetChild(0));
        float xpos = 3 + (-1.5f*audiosInBox);
        sphere.transform.localPosition = new Vector3(xpos, 0, -0.5f);
        sphere.transform.localRotation = new Quaternion(0,0,0,0);
        //change the text on the sphere
        sphere.transform.GetChild(0).GetComponent<TextMesh>().text = audioNumber.ToString();
        //move audiobox to the right in order to center it
        transform.Translate(new Vector3(1.5f,0,0), Space.Self);
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
