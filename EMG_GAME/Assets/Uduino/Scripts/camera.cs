using UnityEngine;

public class camera : MonoBehaviour
{
    private tempo meutempo;

    void Start()
    {
        GameObject zombie = GameObject.Find("Zombie");
        meutempo = zombie.GetComponent<tempo>();
        bool ata = meutempo.VR;
        if (!ata)
        transform.Translate(-30, 0 , 0);
    }
}
