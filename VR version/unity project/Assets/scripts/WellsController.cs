using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behaviour of the object containing every well
public class WellsController : MonoBehaviour
{
    private int selectedWellID;
    [SerializeField] private Color selectionColor;
    [SerializeField] private GameObject audiobox;

    // Start is called before the first frame update
    void Start()
    {
        selectedWellID = -1;
    }

    // Update is called once per frame, when right mouse button is clicked, everything is deselected
    void Update()
    {
        if(Input.GetMouseButton(1)){
            deselectWell();
        }
        if(OVRInput.Get(OVRInput.RawButton.LIndexTrigger, OVRInput.Controller.LTouch)){
            deselectWell();
        }

    }

    //when a well is selected, deselect the lastly selected one and any selected collection, show the audio's box
    public void selectWell(int wellID){
        //deselects previous well
        if(selectedWellID!=-1 && selectedWellID!= wellID){
            getWellByID(selectedWellID).GetComponent<WellController>().deselect();
        }
        //deselects collection if any is selected
        int selectedCollectionID = GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getSelectedCollectionID();
        if(selectedCollectionID!=-1){
            GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getCollectionByID(selectedCollectionID).gameObject.GetComponent<SphereCollectionBehaviour>().deselect();
        }
        //select new well, clear old audio box and generate new audio spheres, change selected well material
        selectedWellID = wellID;
        getWellByID(selectedWellID).GetComponent<MeshRenderer>().material.color = selectionColor;
        List<AudioClip> audios = getWellByID(wellID).GetComponent<WellController>().getAudios();
        AudioboxController box = audiobox.GetComponent<AudioboxController>();
        box.clearBox();
        for(int i = audios.Count - 1; i>-1; i--){
            box.createAudioSphere(i, audios[i]);
        }
    }


    //deselects selected well if any, clears the audioBox
    public void deselectWell(){
        if(selectedWellID != -1){
            getWellByID(selectedWellID).GetComponent<WellController>().deselect();
            selectedWellID = -1;
            audiobox.GetComponent<AudioboxController>().clearBox();
            return;
        }
    }
        
    //assign new audioclip to the currently selected well 
    public void getNewAudio(AudioClip newAudio){
        if(selectedWellID!=-1){
            getWellByID(selectedWellID).GetComponent<WellController>().getNewAudio(newAudio);
            selectWell(selectedWellID);
        }
    }

    //returns the id of the currently selected well (no selection is -1)
    public int getSelectedWellID(){
        return selectedWellID;
    }

    //returns how many audio clips should a well have
    public int getSelectedWellNumberOfAudios(){
        return getWellByID(selectedWellID).GetComponent<WellController>().getNumberOfAudios();
    }

    //retruns the transform of the specified well
    public Transform getWellByID(int wellID){
        for(int i = 0; i<32; i++){
            if(wellID == this.gameObject.transform.GetChild(i).GetChild(0).GetComponent<WellController>().wellID){
                return this.gameObject.transform.GetChild(i).GetChild(0);
            }
        }
        return null;
    }

    //assign audioclip to specific well at specific position
    public void assignAudioToWell(AudioClip myClip, int wellID, int audioNumber){
        getWellByID(wellID).GetComponent<WellController>().assignAudio(myClip, audioNumber);
    }

}
