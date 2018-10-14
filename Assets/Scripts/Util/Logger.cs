using System.Diagnostics;

public static class Logger
{
    public static readonly int SPACES_PER_TAB = 4;

    public static void LogInfo(string message)
    {
        UnityEngine.Debug.Log(FormatLog("Info", message));
    }

    public static void LogWarning(string message)
    {
        UnityEngine.Debug.LogWarning(FormatLog("Warning", message));
    }

    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(FormatLog("Error", message));
    }

    public static void LogException(string message)
    {
        UnityEngine.Debug.LogError(FormatLog("Excepetion", message));
    }

    private static string FormatLog(string logType, string message)
    {
        string oneTab = Util.Tabs(4, 1);
        string formattedMessage = logType + ":";
        formattedMessage += "\n{";
        formattedMessage += "\n" + oneTab + "Message:";
        formattedMessage += "\n" + oneTab + "{";
        formattedMessage += "\n" + oneTab + oneTab + Util.Indent(message, SPACES_PER_TAB, 2);
        formattedMessage += "\n" + oneTab + "}";
        formattedMessage += "\n" + oneTab + "Stack Trace:";
        formattedMessage += "\n" + oneTab + "{";
        formattedMessage += "\n" + oneTab + oneTab + Util.Indent((new StackTrace(2, true)).ToString(), SPACES_PER_TAB, 2);
        formattedMessage += "\n" + oneTab + "}";
        formattedMessage += "\n}\n";
        return formattedMessage;
    }
}