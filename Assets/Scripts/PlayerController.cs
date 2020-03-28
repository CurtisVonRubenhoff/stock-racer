using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  public bool isControlled = true;
  private Camera mainCamera;

  public float maxSize;
  public float minSize = 0f;
  public float pushSpeed = 1000f;
  public float scaleRate = 0.1f;
  public float maxSpeed = 1000f;
  // Start is called before the first frame update
  void Start () {
    mainCamera = Camera.main;
  }

  // Update is called once per frame
  void FixedUpdate () {
    if (isControlled) {
      var pos = mainCamera.ScreenToWorldPoint (Input.mousePosition, 0);
      var maxPosX = mainCamera.ScreenToWorldPoint (new Vector3 (Screen.width * .3f, 0, 0), 0).x;

      if (pos.x < maxPosX) {
        transform.position = pos;
      } else {
        transform.position = new Vector3 (maxPosX, pos.y, 0);
      }

      if (Input.GetMouseButton (0)) {
        isControlled = false;
      }
    } else {
      var currScale = transform.localScale;
      var currRotation = transform.rotation;
      var currSpeed = (currScale.x / 8f) * pushSpeed;
      var rigid2D = GetComponent<Rigidbody2D> ();
      if (Input.GetMouseButton (0)) {
        transform.localScale = new Vector3 (currScale.x + scaleRate, currScale.y + scaleRate, 1);
      }
      if (Input.GetMouseButton (1)) {
        transform.localScale = new Vector3 (currScale.x - scaleRate, currScale.y - scaleRate, 1);
      }
      if (Input.GetKey (KeyCode.A)) {
        if (rigid2D.angularVelocity < maxSpeed) {
          rigid2D.AddTorque (currSpeed);
        }
      }
      if (Input.GetKey (KeyCode.D)) {
        if (rigid2D.angularVelocity > -maxSpeed) {
          rigid2D.AddTorque (-currSpeed);
        }
      }

      if (!Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) {
        var spinningGravityAmount = (rigid2D.angularVelocity > 0) ? -currSpeed : ((rigid2D.angularVelocity < 0) ? currSpeed : 0);
        rigid2D.AddTorque (spinningGravityAmount);
      }
    }
  }
}
