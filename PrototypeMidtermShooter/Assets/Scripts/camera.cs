using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    [SerializeField] int senseHori;
    [SerializeField] int senseVert;

    [SerializeField] int locVertMax;
    [SerializeField] int locVertMin;

    [SerializeField] bool invert;
    float vexRotaion = 0;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        //getting the input
        float MouseX = Input.GetAxis("Mouse X") * senseHori * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * senseVert * Time.deltaTime;

        //inverting the look
        if (invert)
        {
            vexRotaion += MouseY;
        }
        else
        {
            vexRotaion -= MouseY;
        }


        //clamp the angle the camera can rotate to
        vexRotaion = Mathf.Clamp(vexRotaion, locVertMin, locVertMax);

        //rotate the camera on the x axis
        transform.localRotation = Quaternion.Euler(vexRotaion, 0, 0);

        //rotate the transform
        transform.parent.Rotate(Vector3.up * MouseX);
    }
}
