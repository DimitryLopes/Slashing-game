using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    public static UnityEvent<IScreen> OnScreenAfterHideEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenAfterShowEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenBeforeHideEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenBeforeShowEvent = new UnityEvent<IScreen>();
    
    public static UnityEvent<int> OnPlayerDamaged = new UnityEvent<int>();
    public static UnityEvent<Target, HitInfo> OnTargetHit = new UnityEvent<Target, HitInfo>();
    public static UnityEvent<Target> OnTargetMiss = new UnityEvent<Target>();

    public static Action OnConnectedToMasterEvent;
    public static Action<List<RoomInfo>> OnRoomListUpdateEvent;
    public static Action<RoomInfo, Player> OnPlayerJoinedRoomEvent;
    public static Action<RoomInfo> OnJoinedRoomEvent;
    //public static Action<string> OnJoinRoomFailedEvent;
}
