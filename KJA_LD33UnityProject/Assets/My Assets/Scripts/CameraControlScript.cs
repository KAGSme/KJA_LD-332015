using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraControlScript : MonoBehaviour {

    Transform playerPos;
    public float cameraSpeed = 5;
    public float zoomSpeed = 5;

	// Use this for initialization
	void Start () {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
	}

    void Update()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta > 0) { Camera.main.orthographicSize -= scrollDelta * zoomSpeed; }
        if (scrollDelta < 0) { Camera.main.orthographicSize -= scrollDelta * zoomSpeed; }
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 10, 80);

        if (Input.mousePosition.x <= 0)
        {
            transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.x >= Screen.width)
        {
            transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }

        if (Input.mousePosition.y <= 0)
        {
            transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y >= Screen.height)
        {
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
        }

        if (Input.GetButton("CameraReset"))
        {
           transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
        }
    }
}
