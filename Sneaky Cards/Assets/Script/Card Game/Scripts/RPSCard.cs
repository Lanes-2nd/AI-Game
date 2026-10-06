using UnityEngine;

public class RPSCard : MonoBehaviour
{
    public CardType cardType;

    public GameManager gameManager;

    public bool isPlayerCard;

    public void SetupCard(CardType type)
    {
        cardType = type;

        Debug.Log("Card created: " + cardType);
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