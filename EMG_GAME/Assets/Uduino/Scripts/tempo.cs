using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;
using System.Threading;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEditor;

public class tempo : MonoBehaviour
{

    //public Text pont;
    //public Text GO;

    public TextMesh textsemvr;
    int pontuation;
    public int id;
    public int health;
    

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
    public float start;
    public bool VR;

    private bool canDie = false, Ver = true;
    private bool threadCreated = false;
    string path_VR, path;

    void Start()
    {
        //if (!VR)
            //SceneManager.LoadScene(2);
        //GameObject fullscene = GameObject.Find("scene");
        pontuation = 0;
        warning = 2f;
        limit = start; //SEGUNDOS
        second_warning = 15f;
        manager = UduinoManager.Instance;
        manager.pinMode(AnalogPin.A0, PinMode.Input);
        audiosource = GetComponent<AudioSource>();
        path = string.Format(@"C:\Users\Mirella\Desktop\zombie_game\EMG_GAME\tests\{0}.txt", id.ToString());
        path_VR = string.Format(@"C:\Users\Mirella\Desktop\zombie_game\EMG_GAME\tests\{0}_VR.txt", id.ToString());
        health = (int)(limit*1000);
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
        if (VR)
        {
            gameCOMvr();
        }
        else
        {
            audiosource.Stop();
            gameSEMvr();
        }
    }


    void gameCOMvr()
    {
        //restart - click space
        if (Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        value[0] = manager.analogRead(AnalogPin.A0); //direita
        value[1] = manager.analogRead(AnalogPin.A1); //esquerda
        countdown = Mathf.Abs(limit - coT);

        if (value[0] == 0 && cond == true) //músculo relaxado
        {
            if (limit != start + 3)
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
                    //text3d.text = ("Hold on! " + countdown);
                    health = (int)(countdown * 1000);
                    decrement = false;
                }
                else if (transform.position.z > second_warning && transform.position.z > second_warning)
                {
                    this.GetComponent<Animator>().SetTrigger("attack");
                    text3d.text = ("MAIS FORÇA!");
                }
            }
        }
        if (value[0] > 0 && decrement == false && cond == true) //músculo contraído
        {
            if (next_challenge == true)
            {
                string createText = value[0] + "\n";
                File.AppendAllText(path_VR, createText);
                next_challenge = false;
            }
            else
            {
                string createText = value[0] + ",";
                File.AppendAllText(path_VR, createText);
            }
            coT = coT + Time.deltaTime;
            health = (int)(countdown*1000);
            //text3d.text = ("Hold on! " + countdown);

            if (coT > limit)
            //cumpriu o objetivo
            {
                this.GetComponent<Animator>().SetBool("walk", false);
                this.GetComponent<Animator>().SetTrigger("die");
                audiosource.clip = audio1;
                audiosource.Stop();
                coT = 0.0f;
                next_challenge = true;
                limit = limit + 1.0f;
                //mudar cor p branco
                text3d.text = ("Relax");
                StartCoroutine(waittodie());
                cond = false;
                decrement = true;
                if (limit == start + 3) //ganhou
                {
                    //mudar cor p verde
                    text3d.text = ("You won!");
                    this.GetComponent<Animator>().SetTrigger("exit");
                }
            }
        }
    }

    void gameSEMvr()
    {
        value[0] = manager.analogRead(AnalogPin.A0); //direita
        value[1] = manager.analogRead(AnalogPin.A1); //esquerda
        countdown = Mathf.Abs(limit - coT);

        if (value[0] == 0 && cond == true) //músculo relaxado
        {
            if (limit != start + 3)
            {
                this.GetComponent<Animator>().SetBool("walk", true);
                //fazer força!
                if (transform.position.z >= warning && transform.position.z < (second_warning + 3))
                {
                    text3d.text = ("Hold on! " + countdown);
                    decrement = false;
                }
            }
        }
        if (value[0] > 0 && decrement == false && cond == true) //músculo contraído
        {
            if (next_challenge == true)
            {
                string createText = value[0] + "\n";
                File.AppendAllText(path, createText);
                next_challenge = false;
            }
            else
            {
                string createText = value[0] + ",";
                File.AppendAllText(path, createText);
            }
            coT = coT + Time.deltaTime;
            text3d.text = ("Hold on! " + countdown);

            if (coT > limit)
            //cumpriu o objetivo
            {
                this.GetComponent<Animator>().SetBool("walk", false);
                this.GetComponent<Animator>().SetTrigger("die");
                audiosource.clip = audio1;
                audiosource.Stop();
                coT = 0.0f;
                next_challenge = true;
                limit = limit + 1.0f;
                text3d.text = ("Relax");
                StartCoroutine(waittodie());
                cond = false;
                decrement = true;
                if (limit == start + 3) //ganhou
                {
                    text3d.text = ("");
                    this.GetComponent<Animator>().SetTrigger("exit");
                }
            }
        }
    }
}
//    Thread.Sleep(100);