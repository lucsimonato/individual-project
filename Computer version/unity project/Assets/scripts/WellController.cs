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
    private Material originalMaterial;
    private List<AudioClip> audios;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.gameObject.transform;
        distance = 20f;
        originalMaterial = this.gameObject.GetComponent<MeshRenderer>().material;
        //initializes audios list of the correct size according to how many audios are recorded in player preferences
        audios = new List<AudioClip>(PlayerPrefs.GetString("WellAudios_"+wellID, "").Split('/').Length-1);
        //fills audios list
        AudioLoader loader = transform.parent.parent.GetComponent<AudioLoader>();
        loader.getAudiosForWell(wellID);
    }

    //when clicked, select it
    void OnMouseDown()
    {
        select();
    }

    //change material of the selected well
    void select(){
        //Camera.main.transform.position = Vector3.MoveTowards(myTransform.parent.position, new Vector3(10f, -100f, -50f), distance);
        //Camera.main.transform.LookAt(myTransform.parent);
        this.gameObject.transform.parent.parent.GetComponent<WellsController>().selectWell(wellID);
    }

    //reset the material
    public void deselect(){
        this.gameObject.GetComponent<MeshRenderer>().material = originalMaterial;
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

    //deletes the audio from the well, updates player preferences
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
