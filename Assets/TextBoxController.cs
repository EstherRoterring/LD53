using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TextBoxController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Image image;
    private string text;
    
    public Transform rechtsOben;
    public Transform linksUnten;

    public Animator animator;
    public GameObject holdSpaceWrapper;

    //set Methode umkomplette Box richtig zu konfigurieren
    public void setTextBox(string text, Sprite imageSprite, int position){
        //text
        textMeshPro.text = text;
        //Bild
        if (image != null) {
            image.sprite = imageSprite;
        }
        //Sichbarkeit
        Visible(true);

        if(position==2){
            Position2();
        }
        else {
            Position1();
        }
    }

    public void SetTextBoxAutoPos(string text, Sprite imageSprite)
    {
        int position = 1;
        if (OfficeController.INSTANCE.player.transform.localPosition.y < 0)
        {
            position = 2;
        }
        OfficeController.INSTANCE.textbox.setTextBox(text, imageSprite, position);
    }

    void Start(){}

    void Update(){}

    //wird einmal am Anfang ausgefuehrt
    void Awake()
    {
        //Text
            textMeshPro.text = text;
        //Position
            Position1();
            Visible(false);
    }
    
    //aendert den Text
    public void SetText(string text) {
        this.text = text;
        textMeshPro.text = text;
    }
    
    public void Position1()
    {
        //unten links -3,-3
        transform.position = linksUnten.position;
    }

    public void Position2()
    {
        //oben rechts 3,3
        transform.position = rechtsOben.position;
    }

    public void Visible(bool visible){
        gameObject.SetActive(visible);
    }

}
