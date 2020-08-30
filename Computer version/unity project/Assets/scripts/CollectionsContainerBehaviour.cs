using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class describing the behaviour of a parent object containing all of the spheres placed in the scene containing audio clips
public class CollectionsContainerBehaviour : MonoBehaviour
{
    private int selectedCollectionID;
    // Start is called before the first frame update
    void Start()
    {
        selectedCollectionID = -1;
    }

    // Update is called once per frame
    //deselects selected audio collection or right mouse click
    void Update()
    {
        if(Input.GetMouseButton(1)){
            deselectCollection();
        }
    }

    //when a collection is selected, deselect the lastly selected one and any selected well, show the audio's box
    public void selectCollection(int collectionID){
        //deselects previous collection
        if(selectedCollectionID!=-1){
            getCollectionByID(selectedCollectionID).GetComponent<SphereCollectionBehaviour>().deselect();
        }
        //deselects well if any is selected
        int selectedWellID = GameObject.Find("wells").GetComponent<WellsController>().getSelectedWellID();
        if(selectedWellID!=-1){
            GameObject.Find("wells").GetComponent<WellsController>().getWellByID(selectedWellID).gameObject.GetComponent<WellController>().deselect();
        }


        //select new collection, clear old audio box and generate new audio spheres, change selected collection material
        Camera.main.transform.GetChild(0).gameObject.SetActive(true);
        selectedCollectionID = collectionID;
        getCollectionByID(selectedCollectionID).GetComponent<MeshRenderer>().material = Resources.Load<Material>("red material");
        List<AudioClip> audios = getCollectionByID(collectionID).GetComponent<SphereCollectionBehaviour>().getAudios();
        AudioboxController box = Camera.main.transform.GetChild(0).GetComponent<AudioboxController>();
        box.clearBox();
        for(int i = audios.Count - 1; i>-1; i--){
            box.createAudioSphere(i, audios[i]);
        }
    }

    //deselects selected collection if any, clears the audioBox
    public void deselectCollection(){
        if(selectedCollectionID != -1){
            getCollectionByID(selectedCollectionID).GetComponent<SphereCollectionBehaviour>().deselect();
            selectedCollectionID = -1;
            Camera.main.transform.GetChild(0).GetComponent<AudioboxController>().clearBox();
            return;
        }
    }

    //retruns the transform of the specified collection
    public Transform getCollectionByID(int collectionID){
        for(int i = 0; i<transform.childCount; i++){
            if(collectionID == this.gameObject.transform.GetChild(i).GetComponent<SphereCollectionBehaviour>().getID()){
                return this.gameObject.transform.GetChild(i);
            }
        }
        return null;
    }

    //returns the id of the currently selected collection (no selection is -1)
    public int getSelectedCollectionID(){
        return selectedCollectionID;
    }

    //returns how many audio clips should a collection have
    public int getSelectedCollectionNumberOfAudios(){
        return getCollectionByID(selectedCollectionID).GetComponent<SphereCollectionBehaviour>().getNumberOfAudios();
    }
}
