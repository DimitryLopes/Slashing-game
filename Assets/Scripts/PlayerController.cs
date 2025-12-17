using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField]
    private GameObject bladePrefab;

    private GameObject playerBlade;

    private Camera mainCamera;
    private int playerId => photonView.OwnerActorNr;
    private readonly Dictionary<Collider2D, HitInfo> activeStrikes = new();

    void Start()
    {
        mainCamera = Camera.main;
        if (photonView.IsMine)
        {
            playerBlade = Instantiate(bladePrefab, transform);
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

            transform.position = worldPosition;

            photonView.RPC(nameof(UpdateCirclePosition), RpcTarget.Others, worldPosition);
        }
    }

    private Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.CompareTag(Constants.Tags.TARGET_TAG))
        {
            activeStrikes.Add(collider2D, new HitInfo(GetMousePosition(), (byte)playerId));
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(Constants.Tags.TARGET_TAG))
        {
            Target target = collision.gameObject.GetComponent<Target>();
            
            if(target.IsCutted)
            {
                if (activeStrikes.ContainsKey(collision))
                {
                    activeStrikes.Remove(collision);
                    return;
                }
            }

            HitInfo hitInfo = activeStrikes[collision];
            hitInfo.ExitPoint = GetMousePosition();
            target.Hit(hitInfo);

            activeStrikes.Remove(collision);
        }
    }

    [PunRPC]
    private void UpdateCirclePosition(Vector3 position)
    {
        if (playerBlade == null)
        {
            playerBlade = Instantiate(bladePrefab, transform);
        }

        playerBlade.transform.position = position;
    }
}
