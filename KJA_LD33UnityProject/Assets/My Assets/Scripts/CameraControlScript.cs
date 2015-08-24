using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraControlScript : MonoBehaviour {

    Transform playerPos;
    public float cameraSpeed = 5;
    public float zoomSpeed = 5;
    public bool LimitMovement;
    public Transform TLLimitPoint;
    public Transform BRLimitPoint;
    Vector2 TLborder;
    Vector2 BRborder;

	// Use this for initialization
	void Start () {
        TLborder = new Vector2(TLLimitPoint.position.x, TLLimitPoint.position.y);
        BRborder = new Vector2(BRLimitPoint.position.x, BRLimitPoint.position.y);
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
	}

    void Update()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta > 0) { Camera.main.orthographicSize -= scrollDelta * zoomSpeed; }
        if (scrollDelta < 0) { Camera.main.orthographicSize -= scrollDelta * zoomSpeed; }
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 10, 60);

        Vector3 tempV3 = transform.position;

        if (Input.mousePosition.x <= 0)
        {
            tempV3.x -= cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width)
        {
            tempV3.x += cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= 0)
        {
            tempV3.y -= cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y >= Screen.height)
        {
            tempV3.y += cameraSpeed * Time.deltaTime;
        }
        if (Input.GetButton("CameraReset"))
        {
           tempV3 = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);
        }
        if (LimitMovement)
        {
            tempV3.x = Mathf.Clamp(tempV3.x, TLborder.x, BRborder.x);
            tempV3.y = Mathf.Clamp(tempV3.y, BRborder.y, TLborder.y);
        }
        transform.position = tempV3;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(TLLimitPoint.position, new Vector2(TLLimitPoint.position.x, BRLimitPoint.position.y));
        Gizmos.DrawLine(TLLimitPoint.position, new Vector2(BRLimitPoint.position.x, TLLimitPoint.position.y));
        Gizmos.DrawLine(BRLimitPoint.position, new Vector2(BRLimitPoint.position.x, TLLimitPoint.position.y));
        Gizmos.DrawLine(BRLimitPoint.position, new Vector2(TLLimitPoint.position.x, BRLimitPoint.position.y));
    }
}
