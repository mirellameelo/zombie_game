using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;


public class read : MonoBehaviour
{

    public Text pont;
    public Text GO;
    //public GameObject red_limit;
    public GameObject green_limit;
    public int pontuation, count;
    public float limit, green, max_limit, tempo;
    UduinoManager manager;

    float[] value = new float[2] { 0, 0 };

    bool decrement = false;

    // Use this for initialization
    void Start()
    {
        pontuation = 0;
        limit = 9f;
        green = 8f;
        tempo = 0f;
        max_limit = 20f;
        manager = UduinoManager.Instance;
        green_limit.transform.position = new Vector3(0, 12, green);
        manager.pinMode(AnalogPin.A0, PinMode.Input);
        //UduinoManager.Instance.pinMode(13, PinMode.Input);
    }

    // Update is called once per frame
    void Update()
    {
        //red_limit.transform.position = new Vector3(0, 0, red);
        pont.text = "Pontuação: \n" + pontuation;
        value[0] = manager.analogRead(AnalogPin.A0); //direita
        //value[0] = 0;
        value[1] = manager.analogRead(AnalogPin.A1); //esquerda

        if (value[0] < 512)
        {
            //this.GetComponent<Animator>().SetFloat("intensity", 512);
            //this.GetComponent<Animator>().SetBool("walk", true);
            //transform.Translate(0f, 0f, (512 - value[0]) / 400 * Time.deltaTime);
            //PERDEU
            if (transform.position.z >= max_limit)
                transform.position = Vector3.zero;
            //fazer força!
            if (transform.position.z >= limit && transform.position.z < max_limit )
            {
                GO.text = "LET'S GO!";
                green_limit.transform.position = new Vector3(0, 0, green);
                decrement = false;
                //yield return new WaitForSeconds(0.1f);
                //tempo++;
                //green_limit.transform.position = new Vector3(0, 0, green);
            }
        }
        else if(value[0] > 512 && decrement == false)
        {
            transform.Translate(0f, 0f, -(value[0] + 512) / 400 * Time.deltaTime);
            if (transform.position.z <= green)
            {
                transform.position = Vector3.zero;
                green = green - 1f;
                decrement = true;
                green_limit.transform.position = new Vector3(0, 12, green);
                GO.text = " ";
            }
            if (transform.position.z <= 0.0)
                transform.position = Vector3.zero;
        }
    }
}

