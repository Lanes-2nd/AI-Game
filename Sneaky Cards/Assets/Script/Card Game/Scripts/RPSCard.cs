using UnityEngine;

public class RPSCard : MonoBehaviour
{
    public CardType cardType;

    public void SetupCard(CardType type)
    {
        cardType = type;

        Debug.Log("Card created: " + cardType);
    }
}