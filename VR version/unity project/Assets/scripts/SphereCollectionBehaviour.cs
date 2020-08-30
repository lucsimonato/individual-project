using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//behaviour of an audiocollection
public class SphereCollectionBehaviour : MonoBehaviour
{
    private Transform myTransform;
    private float distance;
    [SerializeField] private Color originalColor;
    private List<AudioClip> audios;
    private int ID;
    private GameObject model;
    // Start is called before the first frame update
    //ignore collisions with the model
    void Start()
    {
        myTransform = this.gameObject.transform;
        distance = 15f;
        model = GameObject.Find("ModelContainer");
        Collider[] colliders = model.GetComponentsInChildren<Collider>();
			foreach (Collider c in colliders)
			{
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), c, true);
            }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        this.gameObject.GetComponent<PointerEvents>().changeColor = true;
        this.gameObject.GetComponent<MeshRenderer>().material.color = originalColor;
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

    //select collection on pointerclick
    public void OnPointerClick()
    {
        select();   
    }


    //change material of the selected collection
    public void select(){
        this.gameObject.GetComponent<PointerEvents>().changeColor = false;
        this.gameObject.transform.parent.GetComponent<CollectionsContainerBehaviour>().selectCollection(ID);
    }

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
        }else{
            select();
        }
        PlayerPrefs.SetString(stringToSet, newValue);
        PlayerPrefs.Save();
        
    }


}
