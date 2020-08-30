using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behaviour of single well
public class WellController : MonoBehaviour
{
    private Transform myTransform;
    public float distance;
    public int wellID;
    private static int selectedID;
    private Color originalColor;
    private List<AudioClip> audios;
    private bool clicked = false;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.gameObject.transform;
        distance = 20f;
        originalColor = this.gameObject.GetComponent<MeshRenderer>().material.color;
        //initializes audios list of the correct size according to how many audios are recorded in player preferences
        audios = new List<AudioClip>(PlayerPrefs.GetString("WellAudios_"+wellID, "").Split('/').Length-1);
        //fills audios list
        AudioLoader loader = transform.parent.parent.GetComponent<AudioLoader>();
        loader.getAudiosForWell(wellID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //when clicked, select it
    void OnMouseDown()
    {
        select();
    }

    public void pointerClick(){
        select();
    }

    //change material of the selected well
    void select(){
        this.gameObject.GetComponent<PointerEvents>().changeColor = false;
        this.gameObject.transform.parent.parent.GetComponent<WellsController>().selectWell(wellID);
    }

    //reset the material
    public void deselect(){
        GetComponent<PointerEvents>().changeColor = true;
        this.gameObject.GetComponent<MeshRenderer>().material.color = originalColor;
    }

    //assign new audio as last clip
    public void getNewAudio(AudioClip newAudio){
        audios.Add(newAudio);
    }

    //assign new audio at the specific position
    public void assignAudio(AudioClip myClip, int audioNumber){
        audios.Insert(audioNumber, myClip);
    }

    //retrieve how many audios the well is holding
    public int getNumberOfAudios(){
        return audios.Count;
    }

    //retrieve the audios the well is holding
    public List<AudioClip> getAudios(){
        return audios;
    }

    //deletes audio from player preferences
    public void deleteAudio(int audioToDelete){
        audios.RemoveAt(audioToDelete);
        string stringToSet = "WellAudios_"+wellID;
        List<string> clips = new List<string>(PlayerPrefs.GetString(stringToSet,"").Split('/'));
        clips.RemoveAt(audioToDelete);
        string newValue = string.Join("/", clips);
        PlayerPrefs.SetString(stringToSet, newValue);
        PlayerPrefs.Save();
        select();
    }

}
