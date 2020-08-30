using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behaviour of spheres containing audios in the audiobox
public class AudioSphereController : MonoBehaviour
{

    Coroutine playingCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //on right mouse click, delete the audio from the current location
    void OnMouseOver () {
        if(Input.GetMouseButtonDown(1)){
            int audioToDelete = int.Parse(gameObject.transform.GetChild(0).GetComponent<TextMesh>().text);
            gameObject.transform.parent.GetComponent<AudioboxController>().deleteAudioFromSelected(audioToDelete);
        }
    }

    //when clicked play audio or stop it
    void OnMouseDown()
    {   
        if(!GetComponent<AudioSource>().isPlaying){
            GetComponent<AudioSource>().Play();
            playingCoroutine = StartCoroutine(StartMethod(GetComponent<AudioSource>().clip.length));

        }else{
            StopCoroutine(playingCoroutine);
            GetComponent<AudioSource>().Stop();
        }
    }
 
    //stops the audio after the clip has finished playing
    private IEnumerator StartMethod(float clipLength)
    {
        //Debug.Log("waiting for "+clipLength);
        yield return new WaitForSeconds(clipLength);
        GetComponent<AudioSource>().Stop();
        //Debug.Log("stopped the source");
    }
}
