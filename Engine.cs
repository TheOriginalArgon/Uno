using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class Engine
{
    static Random rnd = new Random();
    private static bool IsDebugging { get; set; } = false;
    static bool GameOver { get; set; } = false;

    public static Card? TopCard { get; private set; }

    static TurnRotation Rotation { get; set; } = TurnRotation.ClockWise;
    public static List<Card>? LastPlayer { get; set; }

    // Lists of cards containing the cards that each player holds
    public static List<Card> PlayerCards { get; set; } = new List<Card>();
    public static List<Card> ComCards_C1 { get; set; } = new List<Card>();
    public static List<Card> ComCards_C2 { get; set; } = new List<Card>();
    public static List<Card> ComCards_C3 { get; set; } = new List<Card>();

    // Game functions
    public static void StartNewGame(bool debug = false)
    {
        IsDebugging = debug;
        DealCards(7, 4);
        UpdateTurn();
    }

    public static void UpdateTurn()
    {
        if (!GameOver)
        {
            if (LastPlayer == PlayerCards)
            {
                Thread.Sleep(2300);

                // Chack for special cards.
                if (TopCard.IsActive)
                {
                    switch (TopCard?.Special)
                    {
                        case "+2":
                            PlayerCards.Add(Deck.DrawCard());
                            PlayerCards.Add(Deck.DrawCard());
                            Console.WriteLine("You drew two cards.");
                            TopCard.DisableEffect();
                            break;
                        case "+4":
                            PlayerCards.Add(Deck.DrawCard());
                            PlayerCards.Add(Deck.DrawCard());
                            PlayerCards.Add(Deck.DrawCard());
                            PlayerCards.Add(Deck.DrawCard());
                            Console.WriteLine("You drew four cards.");
                            TopCard.DisableEffect();
                            break;
                        case "skip":
                            Console.WriteLine("You were skipped.");
                            TopCard.DisableEffect();
                            break;
                    }
                }
                else
                {

                    // Show the amount of cards each player has.
                    Console.WriteLine("=========================================");
                    Console.Write("Alex has " + ComCards_C1.Count + " cards" + (ComCards_C1.Count == 1 ? " UNO!" : "."));
                    // Debug peek.
                    if (IsDebugging)
                    {
                        foreach (Card c1Card in ComCards_C1)
                        {
                            c1Card.DisplayDebugInfo();
                        }
                    }
                    Console.WriteLine();

                    Console.Write("Diana has " + ComCards_C2.Count + " cards" + (ComCards_C2.Count == 1 ? " UNO!" : "."));
                    // Debug peek.
                    if (IsDebugging)
                    {
                        foreach (Card c1Card in ComCards_C2)
                        {
                            c1Card.DisplayDebugInfo();
                        }
                    }
                    Console.WriteLine();

                    Console.Write("Steve has " + ComCards_C3.Count + " cards" + (ComCards_C3.Count == 1 ? " UNO!" : "."));
                    // Debug peek.
                    if (IsDebugging)
                    {
                        foreach (Card c1Card in ComCards_C3)
                        {
                            c1Card.DisplayDebugInfo();
                        }
                    }
                    Console.WriteLine();

                    // Show the cards the player can play.
                    Console.WriteLine("Your cards: ");
                    for (int i = 0; i < PlayerCards.Count(); i++)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write((i + 1) + " ");
                        PlayerCards[i].DisplayInfo(true);
                    }

                    // Show the card at the table.
                    Console.Write("The card on the table is: ");
                    TopCard?.DisplayInfo(false);
                    Console.WriteLine("Choose a card to play (1-" + PlayerCards.Count() + "), or type 0 to draw a card.");

                    // Process player play.
                    int cardIndex = Convert.ToInt32(Console.ReadLine());
                    if (cardIndex == 0)
                    {
                        // The palyer draws a card.
                        PlayerCards.Add(Deck.DrawCard());
                        LastPlayer = PlayerCards;

                        // Show the cards the player can play.
                        Console.WriteLine("Your cards: ");
                        for (int i = 0; i < PlayerCards.Count(); i++)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write((i + 1) + " ");
                            PlayerCards[i].DisplayInfo(true);
                        }
                        // Show the card at the table.
                        Console.Write("The card on the table is: ");
                        TopCard?.DisplayInfo(false);
                        Console.WriteLine("Choose a card to play (1-" + PlayerCards.Count() + "), or type 0 to pass.");

                        int secondPlayIndex = Convert.ToInt32(Console.ReadLine());
                        if (secondPlayIndex != 0)
                        {
                            Card cardToPlay = PlayerCards[secondPlayIndex - 1];
                            if (IsPlayableCard(cardToPlay))
                            {
                                PlayerCards.Remove(cardToPlay);
                                LastPlayer = PlayerCards;
                                TopCard = cardToPlay;
                                Console.SetCursorPosition(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top - 1);
                                Console.Write("You played ");
                                cardToPlay.DisplayInfo(false);
                                if (TopCard.IsWild())
                                {
                                    Console.WriteLine();
                                    PickColor(true);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Illegal move, draw two...");
                                LastPlayer = PlayerCards;
                                PlayerCards.Add(Deck.DrawCard());
                                PlayerCards.Add(Deck.DrawCard());
                            }
                        }
                    }
                    else
                    {
                        Card cardToPlay = PlayerCards[cardIndex - 1];
                        if (IsPlayableCard(cardToPlay))
                        {
                            PlayerCards.Remove(cardToPlay);
                            LastPlayer = PlayerCards;
                            TopCard = cardToPlay;
                            Console.SetCursorPosition(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top - 1);
                            Console.Write("You played ");
                            cardToPlay.DisplayInfo(false);
                            if (TopCard.IsWild())
                            {
                                Console.WriteLine();
                                PickColor(true);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Illegal move, draw two...");
                            LastPlayer = PlayerCards;
                            PlayerCards.Add(Deck.DrawCard());
                            PlayerCards.Add(Deck.DrawCard());
                        }
                    }

                    // Check if player wins with this move.
                    if (PlayerCards.Count <= 0)
                    {
                        Console.WriteLine("You win!");
                        return;
                    }
                }

                // Update the rotation of turns.
                UpdateRotation();
                LastPlayer = GetNextPlayer();
            }
            else
            {
                if (Deck.DiscardPile.Count() > 1)
                {
                    MakeAIPlay(LastPlayer);
                }
                LastPlayer = GetNextPlayer();
            }
            UpdateTurn();
        }
    }

    private static string GetPlayerName(int ply)
    {
        switch (ply)
        {
            case 1:
                return "Alex";
            case 2:
                return "Diana";
            case 3:
                return "Steve";
        }
        return "Player";
    }

    private static void PlayerWins(int ply)
    {
        Console.WriteLine(GetPlayerName(ply) + " wins!");
        GameOver = true;
    }

    // Pick a color.
    private static void PickColor(bool player)
    {
        if (player)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Pick a color: ");
            Console.Write("1 > ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Red");
            Console.Write("2 > ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Blue");
            Console.Write("3 > ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Green");
            Console.Write("4 > ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Yellow");

            int colorIndex = Convert.ToInt32(Console.ReadLine());
            switch (colorIndex)
            {
                case 1:
                    TopCard?.SetWildColor("Red");
                    break;
                case 2:
                    TopCard?.SetWildColor("Blue");
                    break;
                case 3:
                    TopCard?.SetWildColor("Green");
                    break;
                case 4:
                    TopCard?.SetWildColor("Yellow");
                    break;
            }
        }
        else
        {
            TopCard?.SetWildColor(Intelligence.GetConvenientColor());
        }
        Console.ForegroundColor = TopCard.GetCardFontColor();
        Console.Write(" + Color set to " + TopCard?.CardColor + " +");
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static void UpdateRotation()
    {
        if (TopCard?.Special == "reverse" && TopCard.IsActive)
        {
            Console.Write(" + Rotation canged +");
            Console.WriteLine();
            if (Rotation == TurnRotation.ClockWise)
            {
                Rotation = TurnRotation.CounterClockwise;
            }
            else
            {
                Rotation = TurnRotation.ClockWise;
            }
            TopCard.DisableEffect();
        }
    }

    public static List<Card> GetNextPlayer()
    {
        if (Rotation == TurnRotation.ClockWise)
        {
            if (LastPlayer == PlayerCards)
            {
                return ComCards_C1;
            }
            if (LastPlayer == ComCards_C1)
            {
                return ComCards_C2;
            }
            if (LastPlayer == ComCards_C2)
            {
                return ComCards_C3;
            }
            if (LastPlayer == ComCards_C3)
            {
                return PlayerCards;
            }
        }
        if (Rotation == TurnRotation.CounterClockwise)
        {
            if (LastPlayer == PlayerCards)
            {
                return ComCards_C3;
            }
            if (LastPlayer == ComCards_C1)
            {
                return PlayerCards;
            }
            if (LastPlayer == ComCards_C2)
            {
                return ComCards_C1;
            }
            if (LastPlayer == ComCards_C3)
            {
                return ComCards_C2;
            }
        }
        return PlayerCards;
    }

    public static void MakeAIPlay(List<Card> cardList)
    {
        if (!GameOver)
        {
            // Register player names.
            int playerNumber = 0;
            if (cardList == ComCards_C1)
            {
                playerNumber = 1;
            }
            if (cardList == ComCards_C2)
            {
                playerNumber = 2;
            }
            if (cardList == ComCards_C3)
            {
                playerNumber = 3;
            }

            // Initialize turn.
            Thread.Sleep(2300);
            if (TopCard.IsActive)
            {
                switch (TopCard?.Special)
                {
                    case "+2":
                        cardList.Add(Deck.DrawCard());
                        cardList.Add(Deck.DrawCard());
                        Console.WriteLine(GetPlayerName(playerNumber) + " drew two cards.");
                        TopCard.DisableEffect();
                        return;
                    case "+4":
                        cardList.Add(Deck.DrawCard());
                        cardList.Add(Deck.DrawCard());
                        cardList.Add(Deck.DrawCard());
                        cardList.Add(Deck.DrawCard());
                        Console.WriteLine(GetPlayerName(playerNumber) + " drew four cards.");
                        TopCard.DisableEffect();
                        return;
                    case "skip":
                        Console.WriteLine(GetPlayerName(playerNumber) + " was skipped.");
                        TopCard.DisableEffect();
                        return;
                }
            }

            // Play the best card.
            Card[] possiblePlays = Intelligence.SelectAllPossibleCards(cardList);
            if (possiblePlays.Length > 0)
            {
                Card finalPlay = Intelligence.SelectBest(possiblePlays);
                TopCard = finalPlay;
                cardList.Remove(finalPlay);
                Console.Write(GetPlayerName(playerNumber) + " played ");
                finalPlay.DisplayInfo(false);
                if (cardList.Count() <= 0)
                {
                    PlayerWins(playerNumber);
                    return;
                }
                UpdateRotation();
                if (TopCard.IsWild())
                {
                    Console.WriteLine();
                    PickColor(false);
                }
                return;
            }

            // Draw a card.
            Console.WriteLine(GetPlayerName(playerNumber) + " drew a card.");
            Card drewCard = Deck.DrawCard();
            cardList.Add(drewCard);

            // Play the drew card if possible.
            if (IsPlayableCard(drewCard))
            {
                Thread.Sleep(500);
                TopCard = drewCard;
                cardList.Remove(drewCard);
                Console.Write(GetPlayerName(playerNumber) + " played ");
                drewCard.DisplayInfo(false);
                if (cardList.Count() <= 0)
                {
                    PlayerWins(playerNumber);
                    return;
                }
                UpdateRotation();
                if (TopCard.IsWild())
                {
                    PickColor(false);
                }
                return;
            }
        }
    }

    static string GetRandomColor()
    {
        int i = rnd.Next(0, 3);
        switch (i)
        {
            case 0:
                return "Red";
            case 1:
                return "Blue";
            case 2:
                return "Green";
            case 3:
                return "Yellow";
        }
        return "Null";
    }

    public static bool IsPlayableCard(Card card)
    {
        if (TopCard?.CardColor == "Wild" || card.CardColor == "Wild")
        {
            return true;
        }
        if (card.CardColor == TopCard?.CardColor ||
            card.Number == TopCard?.Number)
        {
            return true;
        }
        if ((card.CardColor == TopCard?.CardColor ||
            card.Special == TopCard?.Special) && TopCard?.Special != null)
        {
            return true;
        }
        return false;
    }

    public static void DealCards(int amount, int plyAmount)
    {
        for (int i = 0; i < plyAmount; i++)
        {
            for (int j = 0; j < amount; j++)
            {
                Card newCard = Deck.DrawCard();
                switch (i)
                {
                    case 0:
                        PlayerCards.Add(newCard);
                        break;
                    case 1:
                        ComCards_C1.Add(newCard);
                        break;
                    case 2:
                        ComCards_C2.Add(newCard);
                        break;
                    case 3:
                        ComCards_C3.Add(newCard);
                        break;
                }
            }
        }
        TopCard = Deck.DrawCard();
        LastPlayer = ComCards_C3;
        UpdateRotation();
        LastPlayer = GetNextPlayer();
    }
}

public enum TurnRotation
{
    ClockWise,
    CounterClockwise
}