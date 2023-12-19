public enum ScroolView
{
    NumberOfPlayers,
    NumberOfSpyes,
    Time
}
public enum PackType
{
    Default,
    Custom
}
public static class EnumTitle
{
    public const string
        Players = "שחקנים",
        Spyes = "מרגלים",
        Subjects = "נושא",
        Time = "שעון עצר";
}
public static class EnumExplanation
{
    public const string
        TapCard = "הקש על הקלף",
        TapCardAgain = "הקש שוב על הקלף ותעביר לבן אדם הבא",
        Spy = "אתה המרגל, תגלה את מילת המפתח מבלי שיעלו עליך",
        TimerOn = "גלו מי המרגל לפני שהזמן נגמר";
}

