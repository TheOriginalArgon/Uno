using System;
using System.Collections.Generic;
using System.Linq;

public static class Intelligence
{
    public static Card[] SelectAllPossibleCards(List<Card> cards)
    {
        return (from c in cards where Engine.IsPlayableCard(c) select c).ToArray();
    }
    public static Card SelectBest(Card[] cards)
    {
        if (Engine.GetNextPlayer().Count() <= 2)
        {
            return (from c in cards orderby c.TrickPriority descending select c).FirstOrDefault();
        }
        return (from c in cards orderby c.TrickPriority ascending select c).FirstOrDefault();
    }

    public static string GetConvenientColor()
    {
        string[] redCards = (from c in Engine.LastPlayer where c.CardColor == "Red" select c.CardColor).ToArray();
        string[] blueCards = (from c in Engine.LastPlayer where c.CardColor == "Blue" select c.CardColor).ToArray();
        string[] greenCards = (from c in Engine.LastPlayer where c.CardColor == "Green" select c.CardColor).ToArray();
        string[] yellowCards = (from c in Engine.LastPlayer where c.CardColor == "Yellow" select c.CardColor).ToArray();

        string[][] allCards =
        {
            redCards,
            blueCards,
            greenCards,
            yellowCards
        };

        string[] chosenColor = (from c in allCards where c.Length > 0 orderby c.Length descending select c).FirstOrDefault();

        return chosenColor[0];
    }
}