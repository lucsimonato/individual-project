using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

//class used to call async functions to load audioclips and generate audio collections
public  class AudioLoader: MonoBehaviour
{

    [SerializeField] private GameObject collectionContainer;
    //on start retrieve audioclips attached to the scene
    void Start(){
        generateAudioCollections();
    }

    //used to retrieve audioclips assigned to wells, this method can only be used if this script is a component of the object containing all the wells
    public void getAudiosForWell(int wellID){

        string tempString = PlayerPrefs.GetString("WellAudios_"+wellID,"");
        List<string> audios = new List<string>(tempString.Split('/'));
        audios.RemoveAt(audios.Count-1);
        string basepath = "file://"+Application.persistentDataPath;
        int i = 0;
        foreach(string audioID in audios){
            string finalpath = basepath+"\\WellAudio_"+audioID+".wav";
            StartCoroutine(GetWellAudioClip(finalpath, wellID, i));
            i++;
        }
    }

    //starts an asynchronous web request to load local files of audio clips and on completion assign it to a well
     IEnumerator GetWellAudioClip(string path, int wellID, int audioNumber){
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return www.SendWebRequest();
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            if(myClip){
                GameObject.Find("wells").GetComponent<WellsController>().assignAudioToWell(myClip, wellID, audioNumber);
            }
        }
    }

    //queries player prefs to generate audio collection spheres in the correct positions and then populates them
    public void generateAudioCollections(){
        int collections = PlayerPrefs.GetInt("CollectionsInScene", 0);
        for(int i = 0;i<collections;i++){
            string[] audioPosition = PlayerPrefs.GetString("SphereCollectionPosition_" + i, "0,0,0").Split(',');
            if(audioPosition[0] != "deleted"){
                Vector3 newSpherePosition = new Vector3(float.Parse(audioPosition[0]),float.Parse(audioPosition[1]),float.Parse(audioPosition[2]));
                GameObject sphere = collectionContainer.GetComponent<CollectionsContainerBehaviour>().assignCollectionToScene(i, newSpherePosition);
                getCollectionAudios(sphere);
            }
        }
    }

    //populates the given audiocollection by quering playerprefs for audios contained in the sphere with the given ID 
    //and starting an async function to load the audioclips
    private void getCollectionAudios(GameObject sphere){
        string tempString = PlayerPrefs.GetString("SphereCollectionAudios_"+sphere.GetComponent<SphereCollectionBehaviour>().getID(),"");
        List<string> audios = new List<string>(tempString.Split('/'));
        audios.RemoveAt(audios.Count-1);

        string basepath = "file://"+Application.persistentDataPath;
        int i = 0;
        foreach(string audioID in audios){
            string finalpath = basepath+"\\SceneAudio_"+audioID+".wav";
            StartCoroutine(GetSceneAudioClip(finalpath, sphere, i));
            i++;
        }

    }

    //async function that retrieves an audioclip and assigns it to a sphere
    IEnumerator GetSceneAudioClip(string path, GameObject sphere, int audioNumber){
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return www.SendWebRequest();
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            if(myClip){
                sphere.GetComponent<SphereCollectionBehaviour>().assignAudio(myClip, audioNumber);
            }
        }

    }

}