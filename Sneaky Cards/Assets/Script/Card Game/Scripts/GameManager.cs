using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Transform playerPlayedCardPosition;
    public Transform botPlayedCardPosition;

    [Header("Round")]
    public int currentRound = 1;
    public bool playerHasPlayed = false;
    public RPSBotAI botAI;

    public int playerWins = 0;
    public int botWins = 0;
    public int drawCount = 0;

    public bool matchOver = false;

    private RPSCard playerPlayedCard;
    private RPSCard botPlayedCard;

    [Header("Camera")]
    public Camera TwoDCamera;

    [Header("UI")]
    public GameObject nextRoundButton;
    public float nextRoundButtonDelay = 2.0f;

    void Start()
    {
        StartRPSGame();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartNextRound();
        }


        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Vector3 worldPosition = TwoDCamera.ScreenToWorldPoint(
                new Vector3(mousePosition.x, mousePosition.y, 10f)
            );

            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (hit != null)
            {
                RPSCard card = hit.GetComponent<RPSCard>();

                if (card != null)
                {
                    card.PlayCard();
                }
            }
        }
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
            card.gameManager = this;
            card.isPlayerCard = true;


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

    void RearrangeCards()
    {
        // Rearrange player cards
        int playerCardCount = playerCardsParent.childCount;

        for (int i = 0; i < playerCardCount; i++)
        {
            float x = (i - (playerCardCount - 1) / 2.0f) * 1.8f;

            playerCardsParent.GetChild(i).localPosition = new Vector3(
                x,
                0,
                0
            );
        }

        // Rearrange bot cards
        int botCardCount = botCardsParent.childCount;

        for (int i = 0; i < botCardCount; i++)
        {
            float x = (i - (botCardCount - 1) / 2.0f) * 1.8f;

            botCardsParent.GetChild(i).localPosition = new Vector3(
                x,
                0,
                0
            );
        }
    }

    public void PlayPlayerCard(RPSCard card)
    {

        // Don't allow cards after the match is over
        if (matchOver)
        {
            return;
        }

        // Allows player to play only once per round
        if (playerHasPlayed)
        {
            return;
        }

        playerHasPlayed = true;

        Debug.Log("Player played: " + card.cardType);

        RecordPlayerCard(card.cardType);

        playerHand.Remove(card.cardType);

        card.transform.position = playerPlayedCardPosition.position;
        card.transform.SetParent(null);

        playerPlayedCard = card;

        RearrangeCards();
        PlayBotCard();

        DetermineRoundWinner(card.cardType, botPlayedCards[botPlayedCards.Count - 1]);
    }

    public void PlayBotCard()
    {
        if (botHand.Count == 0)
        {
            return;
        }

        CardType chosenCard = botAI.ChooseCard();

        int cardIndex = botHand.IndexOf(chosenCard);

        if (cardIndex == -1)
        {
            return;
        }

        // Find the actual card object that matches the chosen card
        RPSCard chosenCardObject = null;

        foreach (Transform child in botCardsParent)
        {
            RPSCard card = child.GetComponent<RPSCard>();

            if (card != null && card.cardType == chosenCard)
            {
                chosenCardObject = card;
                break;
            }
        }

        if (chosenCardObject == null)
        {
            Debug.LogWarning("Could not find the bot's physical card: " + chosenCard);
            return;
        }

        // Remove the card from the bot's hand
        botHand.RemoveAt(cardIndex);

        // Record the card
        RecordBotCard(chosenCard);

        // Move the actual card to the played position
        chosenCardObject.transform.position = botPlayedCardPosition.position;

        // Move it out of the bot hand hierarchy
        chosenCardObject.transform.SetParent(null);

        botPlayedCard = chosenCardObject;

        RearrangeCards();

        Debug.Log("Bot played: " + chosenCard);
    }

    public void StartNextRound()
    {
        if (matchOver)
        {
            return;
        }

        // Remove the previous played cards
        if (playerPlayedCard != null)
        {
            Destroy(playerPlayedCard.gameObject);
            playerPlayedCard = null;
        }

        if (botPlayedCard != null)
        {
            Destroy(botPlayedCard.gameObject);
            botPlayedCard = null;
        }

        currentRound++;

        playerHasPlayed = false;

        Debug.Log("Starting Round " + currentRound);

        nextRoundButton.SetActive(false);
    }

    public void RecordPlayerCard(CardType card)
    {
        playerPlayedCards.Add(card);
    }

    public void RecordBotCard(CardType card)
    {
        botPlayedCards.Add(card);
    }

    public void DetermineRoundWinner(CardType playerCard, CardType botCard)
    {
        // Draw
        if (playerCard == botCard)
        {
            drawCount++;

            Debug.Log("Round Result: Draw!");
            Debug.Log("Score - Player: " + playerWins + " | Bot: " + botWins + " | Draws: " + drawCount);

            CheckMatchWinner();

            StartCoroutine(ShowNextRoundButton());


            return;
        }

        // Player wins
        if (
            (playerCard == CardType.Rock && botCard == CardType.Scissors) ||
            (playerCard == CardType.Paper && botCard == CardType.Rock) ||
            (playerCard == CardType.Scissors && botCard == CardType.Paper)
        )
        {
            playerWins++;
            Debug.Log("Round Result: Player Wins!");
        }
        // Bot wins
        else
        {
            botWins++;
            Debug.Log("Round Result: Bot Wins!");
        }

        Debug.Log("Score - Player: " + playerWins + " | Bot: " + botWins + " | Draws: " + drawCount);

        CheckMatchWinner();

        StartCoroutine(ShowNextRoundButton());
    }

    public void CheckMatchWinner()
    {
        // Someone reaches 3 wins
        if (playerWins >= 3)
        {
            matchOver = true;
            Debug.Log("MATCH OVER! PLAYER WINS THE GAME!");
            return;
        }

        if (botWins >= 3)
        {
            matchOver = true;
            Debug.Log("MATCH OVER! BOT WINS THE GAME!");
            return;
        }

        // All 5 rounds have been completed
        if (playerWins + botWins + drawCount >= 5)
        {
            if (playerWins > botWins)
            {
                matchOver = true;
                Debug.Log("MATCH OVER! PLAYER WINS THE GAME!");
            }
            else if (botWins > playerWins)
            {
                matchOver = true;
                Debug.Log("MATCH OVER! BOT WINS THE GAME!");
            }
            else
            {
                matchOver = true;
                Debug.Log("MATCH OVER! THE MATCH IS A TIE!");
            }
        }
    }

    IEnumerator ShowNextRoundButton()
    {
        yield return new WaitForSeconds(nextRoundButtonDelay);

        if (!matchOver)
        {
            nextRoundButton.SetActive(true);
        }
    }
}