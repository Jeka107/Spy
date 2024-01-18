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
        TapCardAgain = "הקש שוב על הקלף והעבר לשחקן הבא",
        Spy = "אתה המרגל !"+ "\n\n" + "גלה את מילת המפתח מבלי שיחשפו אותך" + "\n\n" + TapCardAgain,
        TimerOn = "התחילו לשאול שאלות! \n\n גלו מי המרגל לפני שהזמן נגמר!";
}

