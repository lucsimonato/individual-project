using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//behaviour of a audiocollection
public class SphereCollectionBehaviour : MonoBehaviour
{
    private Transform myTransform;
    private float distance;
    private Material originalMaterial;
    private List<AudioClip> audios;
    private int ID;
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = this.gameObject.GetComponent<MeshRenderer>().material;
        myTransform = this.gameObject.transform;
        distance = 15f;
    }

    //assign clip to this collection at specific position
    public void assignAudio(AudioClip myClip, int audioNumber){
        audios.Insert(audioNumber, myClip);
    }
    //assign clip to this collection as last clip
    public void assignAudio(AudioClip myClip){
        audios.Add(myClip);
    }

    //sets the ID and initializes audios list of the correct size according to how many audios are recorded in player preferences
    public void setID(int id){
        ID = id;
        string[] audiosNumber = PlayerPrefs.GetString("SphereCollectionAudios_"+ID,"").Split('/');
        audios = new List<AudioClip>(audiosNumber.Length-1);
    }

    //retrieves the ID
    public int getID(){
        return ID;
    }

    //reset the material
    public void deselect(){
        this.gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
    }

    //retrieve audios in collection
    public List<AudioClip> getAudios(){
        return audios;
    }

    //retrieve number of audios in collection
    public int getNumberOfAudios(){
        return audios.Count;
    }

    //when clicked, select it
    void OnMouseDown()
    {
        select();   
    }


    //change material of the selected collection
    public void select(){
        this.gameObject.transform.parent.GetComponent<CollectionsContainerBehaviour>().selectCollection(ID);
    }

    //deletes audio from player preferences, if no audios are left in the collection, removes the collection
    public void deleteAudio(int audioToDelete){
        audios.RemoveAt(audioToDelete);
        string stringToSet = "SphereCollectionAudios_"+ID;
        List<string> clips = new List<string>(PlayerPrefs.GetString(stringToSet,"").Split('/'));
        clips.RemoveAt(audioToDelete);
        string newValue = string.Join("/", clips);
        //if no audios are in the collection, delete collection
        if(newValue == ""){
            PlayerPrefs.SetString("SphereCollectionPosition_" + ID, "deleted");
            PlayerPrefs.Save();
            transform.parent.GetComponent<CollectionsContainerBehaviour>().deselectCollection();
            gameObject.SetActive(false);
        }
        PlayerPrefs.SetString(stringToSet, newValue);
        PlayerPrefs.Save();
        select();
    }


}
