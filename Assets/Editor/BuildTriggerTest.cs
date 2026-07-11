using System.IO;
using UnityEditor;

public static class BuildTriggerTest
{
    [InitializeOnLoadMethod]
    private static void TriggerOnLoad()
    {
        File.WriteAllText("C:\\Users\\vipgo\\Dev\\PBL2\\startup_trigger.txt", "loaded");
    }
}
