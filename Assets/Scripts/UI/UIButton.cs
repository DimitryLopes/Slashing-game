using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image border;
    [SerializeField]
    private Image glow;
    [SerializeField]
    private UIAnimationComponent hoverAnimation;
    [SerializeField]
    private UIAnimationComponent clickAnimation;

    public bool IsSelected { get; set; } = false;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        clickAnimation.PlayInAnimations();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!IsSelected)
            hoverAnimation.PlayInAnimations();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!IsSelected)
            hoverAnimation.PlayOutAnimations();
    }
}
