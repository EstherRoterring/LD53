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


    private Vector3 rechtsOben = new Vector3(3, 3, -3);
    private Vector3 linksUnten = new Vector3(-3, -3, -3);

    //set Methode umkomplette Box richtig zu konfigurieren
    public void setTextBox(string text, Sprite imageSprite, int position){
        //text
        this.text = text;
        textMeshPro.text = text;
        //Bild
        if (image != null) {
            image.sprite = imageSprite;
        }
        //sichbarkeit
        Visible(true);

        if(position==2){
            Position2();
        }
        else {
            Position1();
        }
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
    public void SetText(string text){
        this.text = text;
        textMeshPro.text = text;
    }
    
    void Position1()
    {
        //unten links -3,-3
        transform.localPosition = rechtsOben;
    }

    void Position2()
    {
        //oben rechts 3,3
        transform.localPosition = linksUnten;
    }

    void Visible(bool visible){
        gameObject.SetActive(visible);
    }

}
