using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class camera_moving : MonoBehaviour
{
    Vector3 touch;
    // Start is called before the first frame update
    void Start() {
    } 
    // Update is called once per frame
    void Update() { 
        if(Input.GetMouseButtonDown(0)) { 
            touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } 
        if (Input.GetMouseButton(0)) {
            Vector3 direction = touch - Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            Camera.main.transform.position += direction; 
        } 
    } 
}
