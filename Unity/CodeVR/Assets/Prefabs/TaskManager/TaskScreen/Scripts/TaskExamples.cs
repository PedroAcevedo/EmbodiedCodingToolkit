using System.Collections.Generic;

public static class TaskExamples
{
    private static readonly Dictionary<string, string> _examples =
        new Dictionary<string, string>()
        {
            {
                "caesar_cipher",
                "Input: \"HELLO\"\n" +
                "Shift: 3\n" +
                "Output: \"KHOOR\"\n\n" +
                "Each letter is shifted 3 positions forward in the alphabet. " +
                "For example, H becomes K, E becomes H, and L becomes O."
            },
        };

    public static string GetExample(string taskId)
    {
        if (_examples.TryGetValue(taskId, out string example))
            return example;

        return "";
    }
}