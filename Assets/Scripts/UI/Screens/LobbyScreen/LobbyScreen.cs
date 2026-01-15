using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : UIScreen<LobbyScreenController>
{
    private const int ROOMS_PER_PAGE = 5;

    [SerializeField] 
    private Button createRoomButton;
    [SerializeField]
    private Button refreshButton;
    [SerializeField]
    private LobbyScreenPageButton pageButtonPrefab;
    [SerializeField]
    private RoomView roomViewPrefab;
    [SerializeField] 
    private Transform roomListContainer;
    [SerializeField]
    private Transform pageButtonContainer;
    [SerializeField]
    private GameObject lobbyContainer;

    private List<RoomInfo> availableRooms = new();
    private readonly List<RoomView> roomViewPool = new();
    private readonly List<LobbyScreenPageButton> pageButtonPool = new();

    private int currentPage = 0;

    private void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        refreshButton.onClick.AddListener(OnRefreshButtonClicked);
    }

    protected override void OnBeforeShow()
    {
        EventManager.OnRoomListUpdateEvent += OnRoomListUpdate;
        UpdateRoomListUI();
        UpdatePageButtons();
    }

    protected override void OnBeforeHide()
    {
        EventManager.OnRoomListUpdateEvent -= OnRoomListUpdate;
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;
        currentPage = 0;
        UpdateRoomListUI();
        UpdatePageButtons();
    }

    private void UpdateRoomListUI()
    {
        DeactivateAllRoomViews();

        int startIdx = currentPage * ROOMS_PER_PAGE;
        int endIdx = Mathf.Min(startIdx + ROOMS_PER_PAGE, availableRooms.Count);

        for (int i = startIdx, poolIdx = 0; i < endIdx; i++, poolIdx++)
        {
            RoomView view;
            if (poolIdx < roomViewPool.Count)
            {
                view = roomViewPool[poolIdx];
            }
            else
            {
                view = Instantiate(roomViewPrefab, roomListContainer);
                roomViewPool.Add(view);
            }

            var room = availableRooms[i];
            int ping = PhotonNetwork.GetPing(); // ou use um método customizado se necessário

            view.Initialize($"{room.Name}", ping, () => Controller.OnJoinClicked?.Invoke(room.Name));
            view.Activate();
        }
    }

    private void OnRefreshButtonClicked()
    {
        //Controller.OnRefreshButtonClicked?.Invoke();
    }

    private void OnCreateRoomButtonClicked()
    {
        Controller.OnCreateRoomButtonClicked?.Invoke();
    }

    private void UpdatePageButtons()
    {
        DeactivateAllPageButtons();

        int pageCount = Mathf.CeilToInt(availableRooms.Count / (float)ROOMS_PER_PAGE);

        for (int i = 0; i < pageCount; i++)
        {
            LobbyScreenPageButton btn;
            if (i < pageButtonPool.Count)
            {
                btn = pageButtonPool[i];
            }
            else
            {
                btn = Instantiate(pageButtonPrefab, pageButtonContainer);
                pageButtonPool.Add(btn);
            }

            int pageIndex = i;
            btn.Initialize((pageIndex + 1).ToString(), () =>
            {
                currentPage = pageIndex;
                UpdateRoomListUI();
                HighlightCurrentPageButton();
            });
            btn.gameObject.SetActive(true);
        }
        HighlightCurrentPageButton();
    }

    private void HighlightCurrentPageButton()
    {
        for (int i = 0; i < pageButtonPool.Count; i++)
        {
            var indicator = pageButtonPool[i].transform.Find("SelectionIndicator");
            if (indicator != null)
                indicator.gameObject.SetActive(i == currentPage);
        }
    }

    #region Pooling

    private void DeactivateAllPageButtons()
    {
        foreach(var button in pageButtonPool)
        {
            button.Deactivate();
        }
    }

    private void DeactivateAllRoomViews()
    {
        foreach (var view in roomViewPool)
        {
            view.Deactivate();
        }
    }

    private LobbyScreenPageButton GetAvailablePageButton()
    {
        foreach (var button in pageButtonPool)
        {
            if (!button.IsActive)
                return button;
        }

        var newButton = Instantiate(pageButtonPrefab, pageButtonContainer);
        pageButtonPool.Add(newButton);
        return newButton;
    }

    private RoomView GetAvailableRoomView()
    {
        foreach (var view in roomViewPool)
        {
            if (!view.IsActive)
                return view;
        }
        var newView = Instantiate(roomViewPrefab, roomListContainer);
        roomViewPool.Add(newView);
        return newView;
    }
    #endregion
}
