using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{

    [SerializeField] float cameraMoveSpeed;
    [SerializeField] float scrollSpeed;
    [SerializeField] Vector3 minCoordinates;
    [SerializeField] Vector3 maxCoordinates;
    [SerializeField] Vector3 defaultRotation;


    Vector3 rightDir;
    Vector3 forwardDir;
    private void Start()
    {
        transform.rotation = Quaternion.Euler(defaultRotation);

        rightDir = transform.right;
        rightDir.y = 0;
        rightDir.Normalize();

        forwardDir = transform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = (maxCoordinates + minCoordinates)/ 2;
        Vector3 size = Vector3.Max(maxCoordinates - minCoordinates, minCoordinates - maxCoordinates );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos, size);
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

            if (transform.position.y + scrollMovementDir.y < minCoordinates.y)
            {
                float multiplier = (minCoordinates.y - (transform.position.y + scrollMovementDir.y)) / transform.forward.y;
                scrollMovementDir += transform.forward * multiplier;
            }else if (transform.position.y + scrollMovementDir.y > maxCoordinates.y)
            {
                float multiplier = ((transform.position.y + scrollMovementDir.y) - maxCoordinates.y) / transform.forward.y;
                scrollMovementDir -= transform.forward * multiplier;
            }
        }

        Vector3 nextPos = transform.position + movementDir * Time.deltaTime + scrollMovementDir;

        nextPos = new Vector3(Mathf.Max(minCoordinates.x, Mathf.Min(maxCoordinates.x, nextPos.x)),
                              Mathf.Max(minCoordinates.y, Mathf.Min(maxCoordinates.y, nextPos.y)),
                              Mathf.Max(minCoordinates.z, Mathf.Min(maxCoordinates.z, nextPos.z)));

        transform.position = nextPos;
    }

}
