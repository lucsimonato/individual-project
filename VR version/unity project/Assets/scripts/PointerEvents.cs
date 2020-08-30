using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//class used to define events triggered by the pointer
public class PointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color enterColor = Color.white;
    [SerializeField] private Color downColor = Color.white;
    [SerializeField] private UnityEvent OnClick = new UnityEvent();

    private float waitTime = 0.5f; 
    private MeshRenderer meshRenderer = null;
    public bool changeColor;

    private bool clicked;
    private bool isCounting = false;
    private float isCountingFor;
    [SerializeField] private bool allowDelete;
    private float elapsedTime = 0;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    //controls the behavior for audio annotation deletions
    void Update()
    {
        if(isCounting){
            elapsedTime += Time.deltaTime;
            if(elapsedTime>=isCountingFor){
                elapsedTime = 0;
                isCounting = false;
                gameObject.GetComponent<AudioSphereController>().deleteAudio();
            }
        }
    }

    //change color on pointer enter
    public void OnPointerEnter(PointerEventData eventData)
    {   
        if(changeColor)
            meshRenderer.material.color = enterColor;
    }

    //reset color on pointer exit
    public void OnPointerExit(PointerEventData eventData)
    {
        if(changeColor)
            meshRenderer.material.color = normalColor;
        if(isCounting){
            isCounting = false;
            elapsedTime = 0;
        }
    }

    //changes color on pointer pressed and start counting for audio deletion
    public void OnPointerDown(PointerEventData eventData)
    {
        if(changeColor)
            meshRenderer.material.color = downColor;
        if(allowDelete){
            isCountingFor = 3;
            isCounting = true;
        }
    }

    //on pointer peleased, stop counting and assign the correct color
    public void OnPointerUp(PointerEventData eventData)
    {
        if(changeColor && meshRenderer.material.color == downColor)
            meshRenderer.material.color = enterColor;
        if(isCounting){
            isCounting = false;
            elapsedTime = 0;
        }
        
    }

    //on pointer click invoke the desired onclick function
    public void OnPointerClick(PointerEventData eventData)
    {
        if(clicked)
            return;
        clicked = true;
        OnClick.Invoke();
        StartCoroutine(Wait());
    }

    //used to wait for predefined time to correctly control the colors changing behavior
    IEnumerator Wait(){
        yield return new WaitForSeconds(waitTime);
        clicked = false;
    }

    //force click release and resets color
    public void resetClicked(){
        clicked = false;
        meshRenderer.material.color = normalColor;
    }
}