using System.Collections.Generic;
using UnityEngine;

public class RPSBotAI : MonoBehaviour
{
    public GameManager gameManager;

    [Range(0f, 1f)]
    public float smartChance = 0.7f;

    public CardType ChooseCard()
    {
        List<CardType> availableCards = gameManager.botHand;

        if (availableCards.Count == 0)
        {
            Debug.LogWarning("No cards for Enemy");
            return CardType.Rock;
        }

        // Sometimes make a random choice
        if (Random.value > smartChance)
        {
            return availableCards[Random.Range(0, availableCards.Count)];
        }

        return MakeSmartChoice(availableCards);
    }

    CardType MakeSmartChoice(List<CardType> availableCards)
    {
        CardType mostCommonPlayerCard = GetMostCommonPlayerCard();

        CardType counter = GetCounter(mostCommonPlayerCard);

        // If the bot has the counter, use it
        if (availableCards.Contains(counter))
        {
            return counter;
        }

        // Otherwise choose randomly from its available cards
        return availableCards[Random.Range(0, availableCards.Count)];
    }

    CardType GetMostCommonPlayerCard()
    {
        int rockCount = 0;
        int paperCount = 0;
        int scissorsCount = 0;

        foreach (CardType card in gameManager.playerPlayedCards)
        {
            switch (card)
            {
                case CardType.Rock:
                    rockCount++;
                    break;

                case CardType.Paper:
                    paperCount++;
                    break;

                case CardType.Scissors:
                    scissorsCount++;
                    break;
            }
        }

        // If the player hasn't played anything yet,
        // choose randomly.
        if (gameManager.playerPlayedCards.Count == 0)
        {
            return (CardType)Random.Range(0, 3);
        }

        if (rockCount >= paperCount && rockCount >= scissorsCount)
        {
            return CardType.Rock;
        }

        if (paperCount >= rockCount && paperCount >= scissorsCount)
        {
            return CardType.Paper;
        }

        return CardType.Scissors;
    }

    CardType GetCounter(CardType card)
    {
        switch (card)
        {
            case CardType.Rock:
                return CardType.Paper;

            case CardType.Paper:
                return CardType.Scissors;

            case CardType.Scissors:
                return CardType.Rock;
        }

        return CardType.Rock;
    }
}