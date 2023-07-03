using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class camera_moving : MonoBehaviour
{
    Vector3 touch;
    // Start is called before the first frame update
    void Start() {
        Camera.main.orthographicSize = 130;
        Camera.main.transform.localPosition += new Vector3(90, 128, 0);
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
        float deltaSize = Input.mouseScrollDelta.y * Camera.main.orthographicSize / 15;
        if (Camera.main.orthographicSize - deltaSize > 1) {
            Camera.main.orthographicSize -= deltaSize;
        }
        
        //if(Camera.main.orthographicSize < 10) Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 2;
        //else if(Camera.main.orthographicSize < 50) Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 5;
        //else if(Camera.main.orthographicSize < 100) Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 10;
        //else if(Camera.main.orthographicSize < 200) Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 20;
        //else if(Camera.main.orthographicSize < 300) Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 30;
    } 
}
