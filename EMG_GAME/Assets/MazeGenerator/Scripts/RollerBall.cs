using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Uduino;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class RollerBall : MonoBehaviour {

    UduinoManager manager;
    float[] value = new float[2] { 0, 0 };

    public GameObject ViewCamera = null;
	public AudioClip JumpSound = null;
	public AudioClip HitSound = null;
	public AudioClip CoinSound = null;

	private Rigidbody mRigidBody = null;
	private AudioSource mAudioSource = null;
	private bool mFloorTouched = false;

	void Start () {
		mRigidBody = GetComponent<Rigidbody> ();
		mAudioSource = GetComponent<AudioSource> ();
        manager = UduinoManager.Instance;
        manager.pinMode(AnalogPin.A0, PinMode.Input);
    }

	void FixedUpdate () {
		if (mRigidBody != null) {

            value[0] = manager.analogRead(AnalogPin.A0); //direita
            value[1] = manager.analogRead(AnalogPin.A1); //esquerda
            if ((value[0]) < 480)
                //mRigidBody.AddTorque(Vector3.left * (512 - value[0])/400);,
                //mRigidBody.AddTorque(Vector3.left);
                transform.Translate(0f, 0f, (512 - value[0]) / 400 * Time.deltaTime);
            else if (value[0] > 530)
                //mRigidBody.AddTorque(Vector3.right * -(-value[0] + 512) /400);
                //mRigidBody.AddTorque(Vector3.right);
                transform.Translate(0f, 0f, (-value[0] + 512) / 400 * Time.deltaTime);
           // if (Input.GetButtonDown("Jump")) {
			//	if(mAudioSource != null && JumpSound != null){
			//		mAudioSource.PlayOneShot(JumpSound);
			//	}
			//	mRigidBody.AddForce(Vector3.up*200);
			//}
		}
		if (ViewCamera != null) {
			Vector3 direction = (Vector3.up*2+Vector3.back)*2;
			RaycastHit hit;
			Debug.DrawLine(transform.position,transform.position+direction,Color.red);
			if(Physics.Linecast(transform.position,transform.position+direction,out hit)){
				ViewCamera.transform.position = hit.point;
			}else{
				ViewCamera.transform.position = transform.position+direction;
			}
			ViewCamera.transform.LookAt(transform.position);
		}
	}

	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = true;
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		} else {
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		}
	}

	void OnCollisionExit(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = false;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals ("Coin")) {
			if(mAudioSource != null && CoinSound != null){
				mAudioSource.PlayOneShot(CoinSound);
			}
			Destroy(other.gameObject);
		}
	}
}
