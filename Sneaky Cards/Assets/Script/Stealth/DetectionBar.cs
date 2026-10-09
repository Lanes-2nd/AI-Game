using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DetectionBar : MonoBehaviour
{
    public GuardVision guardVision;

    private Image fillImage;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }
    //Fill the detection bar base on detection level
    private void Update()
    {
        if (guardVision == null || fillImage == null)
            return;

        if (guardVision.detectionThreshold <= 0f)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(
            guardVision.detection /
            guardVision.detectionThreshold
        );
    }
}