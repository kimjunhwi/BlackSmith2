using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class AutoBuilder
{
    private static BuildTarget m_TargetPlatform;
    private static bool m_IsRelease = false;


    // ***** Android KeyStore *****
    private static string keystorePath = "/user.keystore";
    private static string keystorePass = "small0125";
    private static string keyaliasName = "user";
    private static string keyaliasPass = "small0125";

    // ------------------------------


    public static void TestBuild()
    {
        string output ="";
        var args = System.Environment.GetCommandLineArgs();

        for(int nIndex = 0; nIndex < args.Length; nIndex++)
        {
            output += nIndex + "->" + args[nIndex] + "\n";
        }

        Debug.Log(output);
    }

    public static void ArgumentTest()
    {
        string message = "";
        int number = -1;
        var args = System.Environment.GetCommandLineArgs();
        for(int nIndex = 0; nIndex <args.Length; nIndex ++)
        {
            switch(args[nIndex])
            {
                case "-message":
                message = args[nIndex + 1];
                break;
                case "-number":
                number = int.Parse(args[nIndex + 1]);
                break;
                default:
                break;
            }
        }

        Debug.Log("Message is " + message + " Number is "+ number);
    }

    [MenuItem("Build/Build AOS")]
    public static void Build()
    {
        Debug.Log("[ScriptLog] Start Build Android");

        BuildOptions opt = BuildOptions.None;

        opt = BuildOptions.Development;

        SetAndroidKeySetting();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = GetEnableScenes();
        buildPlayerOptions.locationPathName =  "Build(AOS)/Test.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.Development;
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        BuildSummary summary = report.summary;

        if(summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build Successed : " + summary.totalSize + "bytes");
        }

        if(summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed");
        }
    }

    static void SetAndroidKeySetting()
    {
        string keystoreName = System.IO.Directory.GetCurrentDirectory() + keystorePath;

        Debug.Log(keystoreName);

        PlayerSettings.Android.keystoreName = keystoreName;
        PlayerSettings.Android.keystorePass = keystorePass;
        PlayerSettings.Android.keyaliasName = keyaliasName;
        PlayerSettings.Android.keyaliasPass = keyaliasPass;

    }

    static string[] GetEnableScenes()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        List<string> sceneList = new List<string> ();

        for(int index = 0, max = scenes.Length; index < max; ++index)
        {
            if(scenes[index].enabled)
            {
                sceneList.Add(scenes[index].path);
            }
        }

        return sceneList.ToArray();
    }


}
