using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behaviour of spheres containing audios in the audiobox
public class AudioSphereController : MonoBehaviour
{

    Coroutine playingCoroutine;
    private int audionumber;
    // Start is called before the first frame update
    void Start()
    {
        audionumber = int.Parse(gameObject.transform.GetChild(0).GetComponent<TextMesh>().text);
    }

    void OnMouseOver () {
        if(Input.GetMouseButtonDown(1)){
            deleteAudio();
        }
    }

    //notifies the audiobox to delete this audio
    public void deleteAudio(){
        gameObject.transform.parent.GetComponent<AudioboxController>().deleteAudioFromSelected(audionumber);
    }

    //when clicked play audio or stop it, also change text
    void OnMouseDown()
    {   
        if(!GetComponent<AudioSource>().isPlaying){
            GetComponent<AudioSource>().Play();
            gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = "~";
            playingCoroutine = StartCoroutine(StartMethod(GetComponent<AudioSource>().clip.length));

        }else{
            StopCoroutine(playingCoroutine);
            gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = audionumber.ToString();
            GetComponent<AudioSource>().Stop();
        }
    }

    //on poointer click, play or stop playing audio annotation, also change text
    public void OnPointerClick(){
        if(!GetComponent<AudioSource>().isPlaying){
            GetComponent<AudioSource>().Play();
            gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = "~";
            playingCoroutine = StartCoroutine(StartMethod(GetComponent<AudioSource>().clip.length));

        }else{
            StopCoroutine(playingCoroutine);
            gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = audionumber.ToString();
            GetComponent<AudioSource>().Stop();
        }
    }
 
    //stops the audio after the clip has finished playing
    private IEnumerator StartMethod(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        GetComponent<AudioSource>().Stop();
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = audionumber.ToString();
    }
}
