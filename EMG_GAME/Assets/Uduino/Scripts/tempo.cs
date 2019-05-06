using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;
using System.Threading;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class tempo : MonoBehaviour
{

    //public Text pont;
    //public Text GO;
    int pontuation;
    public int id;

    float warning, second_warning, limit;
    UduinoManager manager;
    float coT = 0.0f, countdown;

    public TextMesh text3d;
    public AudioSource audiosource;
    public AudioClip audio1;
    public AudioClip audio2;
    float[] value = new float[2] { 0, 0 };

    bool decrement = false;
    bool cond = true;
    bool next_challenge = false;

    private bool canDie = false, Ver = true;
    private bool threadCreated = false;
    //adicionar id
    string path = @"C:\Users\Mirella\Desktop\EMG_GAME\ID.txt";


    // Use this for initialization
    void Start()
    {
        pontuation = 0;
        warning = 2f;
        limit = 2f; //SEGUNDOS
        second_warning = 15f;
        manager = UduinoManager.Instance;
        manager.pinMode(AnalogPin.A0, PinMode.Input);
        audiosource = GetComponent<AudioSource>();
    }

    IEnumerator waittodie()
    {
        yield return new WaitUntil(() => this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("fallingback"));
        yield return new WaitUntil(() => this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("idle"));
        cond = true;
    }
    // Update is called once per frame
    void Update()
    {
        //restart - click space
        if (Input.GetKeyDown("space")){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        value[0] = manager.analogRead(AnalogPin.A0); //direita
        value[1] = manager.analogRead(AnalogPin.A1); //esquerda
        countdown = Mathf.Abs(limit - coT);

        if (value[0] == 0) //músculo relaxado
        {
            if (limit != 5)
            {
                this.GetComponent<Animator>().SetBool("walk", true);
                if (!audiosource.isPlaying)
                {
                    audiosource.volume = 0.25f;
                    audiosource.clip = audio2;
                    audiosource.Play();
                }
                //fazer força!
                if (transform.position.z >= warning && transform.position.z < (second_warning - 0.1))
                {
                    text3d.text = ("Hold on! " + countdown);
                    decrement = false;
                }
                else if (transform.position.z > second_warning && transform.position.z > second_warning)
                {
                    this.GetComponent<Animator>().SetTrigger("attack");
                    text3d.text = ("GAME OVER");
                }
            }
        }
        if (value[0] > 0 && decrement == false && cond == true) //músculo contraído
        {
            if (next_challenge == true)
            {
                string createText = value[0] + ";\n";
                File.AppendAllText(path, createText);
                next_challenge = false;
            }
            else
            {
                string createText = value[0] + ",\n";
                File.AppendAllText(path, createText);
            }
            coT = coT + Time.deltaTime;
            text3d.text = ("Hold on! " + countdown);

            if (coT > limit)
            //cumpriu o objetivo
            {
                this.GetComponent<Animator>().SetBool("walk", false);
                this.GetComponent<Animator>().SetTrigger("die");
                //audiosource.clip = audio1;
                audiosource.Stop();
                text3d.text = (" ");
                coT = 0.0f;
                next_challenge = true;
                limit = limit + 1.0f;
                
                if (limit == 5) //ganhou
                {
                    text3d.text = ("You won!");
                    this.GetComponent<Animator>().SetTrigger("exit");
                    StartCoroutine(waittodie());           
                }
                StartCoroutine(waittodie());
                cond = false;
                decrement = true;
            }
        }
    }
    //    Thread.Sleep(100);
}