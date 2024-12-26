using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class read : MonoBehaviour
{

    //System.DateTime FirstDate = new System.DateTime(2023,3,20);
    //System.DateTime EndDate = new System.DateTime(2023,4,20);

    //System.DateTime tempDay = new System.DateTime(2000,1,1);
    int compareValue;
    int compareValue01;
    int compareValue02;
    int compareValue03;
    int compareValue05;
    public GameObject zzh;
    public GameObject hei;

    float Openzzh = 1;

    // Use this for initialization
    void Start()
    {

        ArrayList arrayList = LoadFile(Application.streamingAssetsPath, "ans.txt");
        int password = int.Parse(arrayList[0].ToString());

        Openzzh = password;

        ArrayList info = LoadFile("C://ProgramData", "First2.txt");
        string ans = info[0].ToString();
        DateTime item = DateTime.Parse(ans);
        compareValue01 = item.CompareTo(System.DateTime.Now);
        if (compareValue01 > 0)
        {
            Debug.Log("有效期范围内");
            zzh.active = true;
            hei.active = true;
        }
        else
        {
            Debug.Log("程序过期");
            if (Openzzh != 6666)
            {
                zzh.active = false;
                hei.active = false;
                //shiyong.active = false;
            }
        }
        #region Test
        //foreach(string str in info)
        //{
        //	string[] strs =str.Split(',');
        //	int aa = strs.Length;
        //	Debug.Log(aa);
        //	for(int i=0;i<(aa-1);i++)
        //	{
        //		compareValue = tempDay.CompareTo(System.DateTime.Parse(strs[i]));
        //		if(compareValue>0)
        //		{
        //			tempDay = tempDay;
        //			Debug.Log(tempDay);
        //		}
        //		else
        //		{
        //			tempDay = System.DateTime.Parse(strs[i]);
        //			Debug.Log("空的");
        //		}
        //	}
        //	compareValue01 = FirstDate.CompareTo (System.DateTime.Now);
        //	compareValue02 = EndDate.CompareTo (System.DateTime.Now);
        //	compareValue03 = EndDate.CompareTo (tempDay);
        //	////compareValue05 = tempDay.CompareTo (System.DateTime.Now);
        //	////if ((compareValue01<0)&&(compareValue02>0)&&(compareValue05<0)&&(compareValue03>0)) 
        //	if ((compareValue01 < 0) && (compareValue02 > 0) && (compareValue03 > 0))
        //	{
        //		Debug.Log("有效期范围内");
        //		zzh.active = false;
        //		hei.active = false;
        //		//shiyong.active = true;
        //	}
        //	else
        //	{
        //		Debug.Log("程序过期");
        //		if (Openzzh != 6666)
        //		{
        //			zzh.active = true;
        //			hei.active = true;
        //			//shiyong.active = false;
        //		}
        //	}
        //}	
        #endregion
    }

    ArrayList LoadFile(string path, string name)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception ex)
        {
            return null;
        }
        string line;
        ArrayList arrlist = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            arrlist.Add(line);
        }
        sr.Close();
        sr.Dispose();
        return arrlist;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
