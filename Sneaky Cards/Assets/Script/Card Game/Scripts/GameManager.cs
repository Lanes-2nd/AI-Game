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

    void Start()
    {
        StartRPSGame();
    }

    public void StartRPSGame()
    {
        CreateDeck();
        ShuffleDeck();
        DealCards();

        Debug.Log("RPS Game Started!");
        Debug.Log("Player Cards: " + playerHand.Count);
        Debug.Log("Bot Cards: " + botHand.Count);
        Debug.Log("Cards Remaining: " + deck.Count);
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

    public void RecordPlayerCard(CardType card)
    {
        playerPlayedCards.Add(card);
    }

    public void RecordBotCard(CardType card)
    {
        botPlayedCards.Add(card);
    }
}