using UnityEngine.UI;
using UnityEngine;
public class DetectionBar : MonoBehaviour
{
    public GuardVision guardVision;
    private Image fillImage;

    private void Start()
    {
        fillImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (guardVision == null)
            return;

        fillImage.fillAmount =
            guardVision.detection / guardVision.detectionThreshold;
    }
}