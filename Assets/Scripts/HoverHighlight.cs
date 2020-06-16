using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;


public class HoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public int myId;
    private Outline mOutline;
    private bool mouse_over = false;
    private void Start()
    {
        mOutline = GetComponent<Outline>();
        mOutline.enabled = false;
    }


    [DllImport("__Internal")]
    private static extern void Hover(int side);

    void Update()
    {
        mOutline.enabled = mouse_over;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
        Hover(myId);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }



}
