using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField]
    private GameObject pointerPrefab;
    private GameObject playerPointer;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (photonView.IsMine)
        {
            playerPointer = Instantiate(pointerPrefab);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            playerPointer.transform.position = worldPosition;

            photonView.RPC("UpdateCirclePosition", RpcTarget.Others, worldPosition);
        }
    }

    [PunRPC]
    private void UpdateCirclePosition(Vector3 position)
    {
        if (playerPointer == null)
        {
            playerPointer = Instantiate(pointerPrefab);
        }

        playerPointer.transform.position = position;
    }
}
