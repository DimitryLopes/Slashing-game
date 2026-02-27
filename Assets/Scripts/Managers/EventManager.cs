using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;

public class EventManager : MonoBehaviour, IManager
{
    public static EventManager Instance;
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            IsInitialized = true;
            return;
        }
        Destroy(gameObject);
    }

    public static UnityEvent<IScreen> OnScreenAfterHideEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenAfterShowEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenBeforeHideEvent = new UnityEvent<IScreen>();
    public static UnityEvent<IScreen> OnScreenBeforeShowEvent = new UnityEvent<IScreen>();

    public static UnityEvent<int> OnPlayerDamaged = new UnityEvent<int>();
    public static UnityEvent<float> OnScoreUpdated = new UnityEvent<float>();
    public static UnityEvent<Target, HitInfo> OnTargetHit = new UnityEvent<Target, HitInfo>();
    public static UnityEvent<Target> OnTargetMiss = new UnityEvent<Target>();

    //public static UnityEvent OnConnectedToMasterEvent = new UnityEvent();
    public static UnityEvent OnLobbyJoinedEvent = new UnityEvent();
    //Lobby Screen
    public static UnityEvent<List<RoomInfo>> OnRoomListUpdateEvent = new UnityEvent<List<RoomInfo>>();
    //Room Screen
    public static UnityEvent<RoomInfo, Player> OnPlayerJoinedRoomEvent = new UnityEvent<RoomInfo, Player>();
    public static UnityEvent<Player> OnPlayerLeftRoomEvent = new UnityEvent<Player>();
    public static UnityEvent<RoomInfo> OnJoinedRoomEvent = new UnityEvent<RoomInfo>();
    public static UnityEvent<Player> OnPlayerReadyStatusChanged = new UnityEvent<Player>();
    //public static Action<string> OnJoinRoomFailedEvent;
    //In Game
    public static UnityEvent OnSliceAreaMoveTimerEnded = new UnityEvent();
    public static UnityEvent<SliceArea> OnSliceAreaMoved = new UnityEvent<SliceArea>();
}
