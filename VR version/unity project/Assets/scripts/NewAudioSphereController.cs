using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//behaviour of the sphere used to create new audio clips
public class NewAudioSphereController : MonoBehaviour
{
    private int lastSample;
    private bool wasRecordingStopped;
    [SerializeField] GameObject model;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //when is clicked either begin or stop recording
    void OnMouseDown()
    {   
        
        if(!Microphone.IsRecording(Microphone.devices[0])){
            transform.GetChild(0).GetComponent<TextMesh>().text = "~";
            wasRecordingStopped = false;
            StartCoroutine(recordNewAudio());
        }else{
            wasRecordingStopped = true;
            lastSample = Microphone.GetPosition(Microphone.devices[0]);
            Microphone.End(Microphone.devices[0]);
        }
    }

    //when pointer is clicked, begin or stop recording
    public void OnPonterClicked(){
        if(!Microphone.IsRecording(Microphone.devices[0])){
            transform.GetChild(0).GetComponent<TextMesh>().text = "~";
            wasRecordingStopped = false;
            StartCoroutine(recordNewAudio());
        }else{
            wasRecordingStopped = true;
            lastSample = Microphone.GetPosition(Microphone.devices[0]);
            Microphone.End(Microphone.devices[0]);
        }
    }

    //coroutine used to record audioclip, automatically stops recording after 1 minute
    IEnumerator recordNewAudio(){
        //formats the newly recorded audio
        AudioClip newAudio;
        AudioClip tempAudio = Microphone.Start(Microphone.devices[0],false,60,44100);
        while(Microphone.IsRecording(Microphone.devices[0])){
            yield return new WaitForSeconds(0.1f);
        }
        //if recording was manually stopped, crop the clip to make it the correct lenght
        if(wasRecordingStopped){
            float[] samples = new float[tempAudio.samples];
            tempAudio.GetData(samples, 0);
            float[] ClipSamples = new float[lastSample];
            Array.Copy(samples, ClipSamples, ClipSamples.Length - 1);
            newAudio = AudioClip.Create("playRecordClip", ClipSamples.Length, 1, 44100, false);
            newAudio.SetData(ClipSamples, 0);
        }else{
            newAudio = tempAudio;
        }
        //according to what was selected when audio was being recorder, either assign audio to well, to collection, or to nothing(enter placemode)
        //saves the audio with specific name and updates playerprefs
        int wellID = GameObject.Find("wells").GetComponent<WellsController>().getSelectedWellID();
        if(wellID!=-1){
            int wellsAudios = PlayerPrefs.GetInt("WellsAudios", 0);
            string audioName = "WellAudio_"+wellsAudios;
            SavWav.Save(audioName, newAudio);
            GameObject.Find("wells").GetComponent<WellsController>().getNewAudio(newAudio);
            //update player prefs to have WellAudios_<ID> to hold a record of all the audios ID it should contain
            string stringToSet = "WellAudios_"+wellID;
            PlayerPrefs.SetString(stringToSet, PlayerPrefs.GetString(stringToSet,"")+wellsAudios+"/");
            PlayerPrefs.SetInt("WellsAudios", wellsAudios+1);
            PlayerPrefs.Save();
        }else{
            int collectionID = GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getSelectedCollectionID();
            if(collectionID!=-1){
                int audionumber = GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getSelectedCollectionNumberOfAudios();
                int audiosInScene = model.GetComponent<ModeController>().getAudiosInScene();
                string audioName = "SceneAudio_"+audiosInScene;
                SavWav.Save(audioName, newAudio);
                GameObject sphere = GameObject.Find("CollectionsContainer").GetComponent<CollectionsContainerBehaviour>().getCollectionByID(collectionID).gameObject;
                sphere.GetComponent<SphereCollectionBehaviour>().assignAudio(newAudio);
                sphere.GetComponent<SphereCollectionBehaviour>().select();
                //update player prefs to have SphereCollectionAudios_<ID> to hold a record of all the audios ID it should contain
                string stringToSet = "SphereCollectionAudios_"+collectionID;
                PlayerPrefs.SetString(stringToSet, PlayerPrefs.GetString(stringToSet,"")+audiosInScene+"/");
                model.GetComponent<ModeController>().incrementAudiosInScene();
                PlayerPrefs.SetInt("AudiosInScene", audiosInScene+1);
                PlayerPrefs.Save();
            }else{
                model.GetComponent<ModeController>().enterPlaceMode(newAudio);
            }
        }
        transform.GetChild(0).GetComponent<TextMesh>().text = "+";
    }
}
