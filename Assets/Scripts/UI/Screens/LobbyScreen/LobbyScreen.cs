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
        base.OnBeforeShow();
        EventManager.OnRoomListUpdateEvent += OnRoomListUpdate;
        UpdateRoomListUI();
        UpdatePageButtons();
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
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
            RoomView view = GetAvailableRoomView();

            var room = availableRooms[i];
            int ping = PhotonNetwork.GetPing();

            view.Initialize(room.GetRoomName(), ping, Controller.OnJoinClicked);
            view.Activate();
        }
    }

    private void UpdatePageButtons()
    {
        DeactivateAllPageButtons();

        int pageCount = Mathf.CeilToInt(availableRooms.Count / (float)ROOMS_PER_PAGE);

        for (int i = 0; i < pageCount; i++)
        {
            LobbyScreenPageButton button = GetAvailablePageButton();

            button.Initialize(i, ChangePage);
            button.gameObject.SetActive(true);
        }
        HighlightCurrentPageButton();
    }

    private void ChangePage(int index)
    {
        currentPage = index;
        UpdateRoomListUI();
        HighlightCurrentPageButton();
    }

    private void OnRefreshButtonClicked()
    {
        Controller.OnRefreshButtonClicked?.Invoke();
    }

    private void OnCreateRoomButtonClicked()
    {
        Controller.OnCreateRoomButtonClicked?.Invoke();
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
