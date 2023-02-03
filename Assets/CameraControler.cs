using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{

    [SerializeField] float cameraMoveSpeed;
    [SerializeField] float scrollSpeed;
    [SerializeField] float defaultCameraHeight;
    [SerializeField] float minCameraHeight;
    [SerializeField] Vector3 defaultRotation;


    Vector3 rightDir;
    Vector3 forwardDir;
    private void Start()
    {
        transform.rotation = Quaternion.Euler(defaultRotation);
        transform.position = new Vector3(transform.position.x, defaultCameraHeight, transform.position.z);

        rightDir = transform.right;
        rightDir.y = 0;
        rightDir.Normalize();

        forwardDir = transform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();
    }

    private void Update()
    {
        Vector3 movementDir = Vector3.zero;
        Vector3 scrollMovementDir = Vector3.zero;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            movementDir += Input.GetAxis("Horizontal") * rightDir * cameraMoveSpeed;
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0)
        {
            movementDir += Input.GetAxis("Vertical") * forwardDir * cameraMoveSpeed;
        }
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0)
        {


            scrollMovementDir += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * transform.forward * Time.deltaTime;

            if (transform.position.y + scrollMovementDir.y < minCameraHeight)
            {
                float multiplier = (minCameraHeight - (transform.position.y + scrollMovementDir.y)) / transform.forward.y;
                scrollMovementDir += transform.forward * multiplier;
            }else if (transform.position.y + scrollMovementDir.y > defaultCameraHeight)
            {
                float multiplier = ((transform.position.y + scrollMovementDir.y) - defaultCameraHeight) / transform.forward.y;
                scrollMovementDir -= transform.forward * multiplier;
            }
        }

        /* float mag = movementDir.magnitude;
         if (mag > 1)
         {
             movementDir = movementDir / mag;
         }*/
        
        transform.position += movementDir * Time.deltaTime + scrollMovementDir;
    }

}
