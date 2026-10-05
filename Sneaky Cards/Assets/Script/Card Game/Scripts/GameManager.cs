using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Deck")]
    public List<CardType> deck = new List<CardType>();

    [Header("Hands")]
    public List<CardType> playerHand = new List<CardType>();
    public List<CardType> botHand = new List<CardType>();

    [Header("Bot")]
    public List<CardType> botPlayedCards = new List<CardType>();
    public List<CardType> playerPlayedCards = new List<CardType>();

    [Header("Card Objects")]
    public GameObject cardPrefab;
    public Transform playerCardsParent;
    public Transform botCardsParent;

    void Start()
    {
        StartRPSGame();
    }

    public void StartRPSGame()
    {
        CreateDeck();
        ShuffleDeck();

        playerPlayedCards.Clear();
        botPlayedCards.Clear();

        DealCards();
        CreateCardObjects();

        Debug.Log("RPS Game Started!");
        Debug.Log("Player Cards: " + playerHand.Count);
        Debug.Log("Bot Cards: " + botHand.Count);
        Debug.Log("Cards Remaining: " + deck.Count);
        Debug.Log("Player Played Cards: " + playerPlayedCards.Count);
        Debug.Log("Bot Played Cards: " + botPlayedCards.Count);
    }

    void CreateDeck()
    {
        deck.Clear();

        // 5 Rock
        for (int i = 0; i < 5; i++)
        {
            deck.Add(CardType.Rock);
        }

        // 5 Paper
        for (int i = 0; i < 5; i++)
        {
            deck.Add(CardType.Paper);
        }

        // 5 Scissors
        for (int i = 0; i < 5; i++)
        {
            deck.Add(CardType.Scissors);
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);

            CardType temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DealCards()
    {
        playerHand.Clear();
        botHand.Clear();

        // Give player 5 cards
        for (int i = 0; i < 5; i++)
        {
            playerHand.Add(deck[0]);
            deck.RemoveAt(0);
        }

        // Give bot 5 cards
        for (int i = 0; i < 5; i++)
        {
            botHand.Add(deck[0]);
            deck.RemoveAt(0);
        }
    }

    void CreateCardObjects()
    {
        // Remove old cards
        foreach (Transform child in playerCardsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in botCardsParent)
        {
            Destroy(child.gameObject);
        }

        // Create player cards
        for (int i = 0; i < playerHand.Count; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, playerCardsParent);

            RPSCard card = cardObject.GetComponent<RPSCard>();

            card.SetupCard(playerHand[i]);

            cardObject.transform.localPosition = new Vector3(
                i * 2.0f - 4.0f,
                0,
                0
            );
        }

        // Create bot cards
        for (int i = 0; i < botHand.Count; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, botCardsParent);

            RPSCard card = cardObject.GetComponent<RPSCard>();

            card.SetupCard(botHand[i]);

            cardObject.transform.localPosition = new Vector3(
                i * 2.0f - 4.0f,
                0,
                0
            );
        }
    }

    public void RecordPlayerCard(CardType card)
    {
        playerPlayedCards.Add(card);
    }

    public void RecordBotCard(CardType card)
    {
        botPlayedCards.Add(card);
    }
}