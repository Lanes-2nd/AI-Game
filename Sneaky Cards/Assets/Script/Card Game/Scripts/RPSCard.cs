
using UnityEngine;

public class RPSCard : MonoBehaviour
{
    public CardType cardType;

    public GameManager gameManager;

    public bool isPlayerCard;

    [Header("Card Images")]
    [SerializeField] private SpriteRenderer cardRenderer;

    [SerializeField] private Sprite rockImage;
    [SerializeField] private Sprite paperImage;
    [SerializeField] private Sprite scissorsImage;
    [SerializeField] private Sprite cardBackImage;

    public void SetupCard(CardType type, bool showFace)
    {
        cardType = type;

        if (showFace)
        {
            RevealCard();
        }
        else
        {
            cardRenderer.sprite = cardBackImage;
        }

        Debug.Log("Card created: " + cardType);
    }

    public void RevealCard()
    {
        switch (cardType)
        {
            case CardType.Rock:
                cardRenderer.sprite = rockImage;
                break;

            case CardType.Paper:
                cardRenderer.sprite = paperImage;
                break;

            case CardType.Scissors:
                cardRenderer.sprite = scissorsImage;
                break;
        }
    }

    public void PlayCard()
    {
        if (!isPlayerCard)
        {
            return;
        }

        Debug.Log("Player clicked: " + cardType);

        gameManager.PlayPlayerCard(this);
    }
}
