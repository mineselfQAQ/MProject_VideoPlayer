using UnityEngine;
using System.Collections;
using System.IO;
using System;
public class write : MonoBehaviour
{

    // Use this for initialization
    string a = "2027/12/15 20:00:00";
    void Start()
    {
        CreatFile(Application.dataPath, "First2.Txt", a);
    }

    // Update is called once per frame
    void CreatFile(string path, string name, string info)
    {
        StreamWriter sw;

        FileInfo t = new FileInfo("C://ProgramData" + "//" + name);
        if (!t.Exists)
        {
            sw = t.CreateText();
            sw.Write(info);

            sw.Close();

            sw.Dispose();
        }


    }

    void Update()
    {

    }
}
