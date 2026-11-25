
using UnityEngine;
using UnityEngine.UI;

public class MenuHover : MonoBehaviour
{
    [SerializeField] private Image menuBackground;

    [SerializeField] private Sprite normalImage;
    [SerializeField] private Sprite playLitImage;
    [SerializeField] private Sprite quitLitImage;

    public void HoverPlay()
    {
        menuBackground.sprite = playLitImage;
    }

    public void HoverQuit()
    {
        menuBackground.sprite = quitLitImage;
    }

    public void ResetMenu()
    {
        menuBackground.sprite = normalImage;
    }
}
