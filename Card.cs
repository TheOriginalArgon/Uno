using System;

public class Card
{
    public string? CardColor { get; private set; }
    public string? Special { get; private set; }
    public int Number { get; private set; } = -1;

    public int TrickPriority { get; private set; }

    public bool IsActive { get; set; }

    public bool IsWild()
    {
        if (CardColor == "Wild")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DisplayInfo(bool indexed)
    {
        Console.ForegroundColor = GetCardFontColor();
        if (!IsWild() || (IsWild() && indexed))
        {
            Console.WriteLine((indexed ? "> " : "") + CardColor + " " + (Number > -1 ? Number.ToString() : Special));
        }
        else if (IsWild() && !indexed)
        {
            Console.Write((indexed ? "> " : "") + CardColor + " " + (Number > -1 ? Number.ToString() : Special));
        }
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void DisplayDebugInfo()
    {
        Console.ForegroundColor = GetCardFontColor();
        Console.Write("> " + CardColor + " " + (Number > -1 ? Number.ToString() : Special) + "; ");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void DisableEffect()
    {
        IsActive = false;
    }

    public void SetWildColor(string col)
    {
        CardColor = col;
    }

    public Card(string col, int num, string? spec = null, int tPrior = -1)
    {
        CardColor = col;
        Number = num;
        Special = spec;
        TrickPriority = tPrior;
        if (spec != null)
        {
            IsActive = true;
        }
        else
        {
            IsActive = false;
        }
    }

    public ConsoleColor GetCardFontColor()
    {
        switch (CardColor)
        {
            case "Red":
                return ConsoleColor.Red;
            case "Green":
                return ConsoleColor.Green;
            case "Blue":
                return ConsoleColor.Blue;
            case "Yellow":
                return ConsoleColor.Yellow;
            case "Wild":
                return ConsoleColor.Gray;
        }
        return ConsoleColor.White;
    }
}