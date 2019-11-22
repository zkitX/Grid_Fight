using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
/*#if UNITY_SWITCH
using nn.fs;
#endif*/

namespace PlaytraGamesLtd
{
	public class Utils
	{
		public static void Serialize<T>(T obj, string path)
		{
			var serializer = new XmlSerializer(obj.GetType());
			using (var writer = XmlWriter.Create(path))
			{
				serializer.Serialize(writer, obj);
			}
		}


       

		public static T Deserialize<T>(string file) where T : class
		{
			var serializer = new XmlSerializer(typeof(T));
			using (StreamReader reader = new StreamReader(file))
			{
				return serializer.Deserialize(reader) as T;
			}
		}


     /*  public static void SerializeAndSaveOnFileSwitch<T>(T obj, string fileName)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(memStream, new System.Text.ASCIIEncoding()))
                {
                    // Write your XML data here

                    writer.Flush();

                    nn.fs.FileHandle fHandle = new nn.fs.FileHandle();

                    // This assumes the file and directory already exist
                    nn.Result result = nn.fs.File.Open(ref fHandle, "/" + fileName + ".xml", nn.fs.OpenFileMode.Write);
                    if (!result.IsSuccess())
                    {
                        // Do any required error handling
                        return;
                    }

                    // Set the file to the correct size for the data
                    result = nn.fs.File.SetSize(fHandle, memStream.Length);
                    if (!result.IsSuccess())
                    {
                        // Do any required error handling
                        nn.fs.File.Close(fHandle);
                        return;
                    }

                    // Write out the data, close the file, and commit the changes
                    nn.fs.File.Write(fHandle, 0, memStream.GetBuffer(), memStream.Length, nn.fs.WriteOption.Flush);
                    nn.fs.File.Close(fHandle);
                    nn.fs.FileSystem.Commit("save");
                }

            }
        }

        public static T DeserializeFileSwitch<T>(string fileName) where T : class
        {
            nn.fs.FileHandle fHandle = new nn.fs.FileHandle();
            XmlReader xmlReader;

            var openResult = nn.fs.File.Open(
                ref fHandle, Application.streamingAssetsPath + "//" + fileName + ".xml", nn.fs.OpenFileMode.Read);
            if (openResult.IsSuccess())
            {
                long fileSize = 0;
                nn.fs.File.GetSize(ref fileSize, fHandle);
                byte[] buffer = new byte[1];
                var readResult = nn.fs.File.Read(fHandle, 0, buffer, fileSize);
                if (readResult.IsSuccess())
                {
                    xmlReader = XmlReader.Create(System.Text.Encoding.Default.GetString(buffer));
                    return DeserializeFromString<T>(xmlReader.Value);
                }
            }

            return DeserializeFromString<T>("");
        }*/



        public static string SerializeToString<T>(T obj)
		{
			var serializer = new XmlSerializer(obj.GetType());
			using (StringWriter textWriter = new StringWriter())
			{
				serializer.Serialize(textWriter, obj);
				return textWriter.ToString();
			}
		}

		public static T DeserializeFromString<T>(string Value) where T : class
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var reader = new System.IO.StringReader(Value))
			{
				return serializer.Deserialize(reader) as T;
			}
		}

		public static T DeserializeStreamingAssetsCSV<T>(string path) where T : new()
		{
			List<string> stringList = new List<string>();
			StreamReader inp_stm = new StreamReader(path);
            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                stringList.Add(inp_ln);
            }
            inp_stm.Close();

			return DeserializerCSVtoLevelStorageClass<T>(stringList);
		}

		public static T DeserializeResourcesCSV<T>(string path) where T : new()
        {

			TextAsset PrnFile = Resources.Load(path) as TextAsset;
            List<string> stringList = new List<string>();
			stringList.AddRange(PrnFile.text.Split('\n').ToList());
			stringList.Remove(stringList.Last());
			return DeserializerCSVtoLevelStorageClass<T>(stringList);
        }

		public static T DeserializerCSVtoLevelStorageClass<T>(List<string> stringList) where T : new()
		{
			T res = new T();
            for (int i = 1; i < stringList.Count; i++)
            {
               
            }
			return res;
		}

        public static void WriteLog(string Log, Color c)
        {
            string path;
            Log = Environment.NewLine + Log;
#if UNITY_EDITOR
            path = System.IO.Path.Combine(Application.streamingAssetsPath, "Log.txt");
            try
            {
                using (TextWriter outputFile = new StreamWriter(path, true))
                {
                    //GameManagerScript
                    if (c == Color.red)
                    {
                        outputFile.WriteLine("<color=#ff0000ff>" + Log + "</color>");
                    }
                    //CharacterBase
                    if (c == Color.green)
                    {
                        outputFile.WriteLine("<color=#00ff00ff>" + Log + "</color>");
                    }
                    //AI
                    if (c == Color.blue)
                    {
                        outputFile.WriteLine("<color=#00ffffff>" + Log + "</color>");
                    }
                    //UI/UserInput
                    if (c == Color.yellow)
                    {
                        outputFile.WriteLine("<color=#ffff00ff>" + Log + "</color>");
                    }
                    //Info
                    if (c == Color.black)
                    {
                        outputFile.WriteLine(Log);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error  1 " + ex.Message + ex.Source);
            }

#elif UNITY_ANDROID && !UNITY_EDITOR
        path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        Debug.Log("Enter");
        try
        {
            using (TextWriter outputFile = new StreamWriter(path, true))
            {
                //GameManagerScript
                if (c == Color.red)
                {
                    outputFile.WriteLine("<color=#ff0000ff>" + Log + "</color>");
                }
                //CharacterBase
                if (c == Color.green)
                {
                    outputFile.WriteLine("<color=#00ff00ff>" + Log + "</color>");
                }
                //AI
                if (c == Color.blue)
                {
                    outputFile.WriteLine("<color=#00ffffff>" + Log + "</color>");
                }
                //UI/UserInput
                if (c == Color.yellow)
                {
                    outputFile.WriteLine("<color=#ffff00ff>" + Log + "</color>");
                }
                //Info
                if (c == Color.black)
                {
                    outputFile.WriteLine(Log);
                }
            }
        }
        catch(System.Exception ex)
        {
        Debug.Log("Error  1 " + ex.Message + ex.Source);
        }
#elif UNITY_IOS && !UNITY_EDITOR

        path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        try
        {
            
        Debug.Log("FileStream");
            //GameManagerScript
            if (c == Color.red)
            {
                File.AppendAllText(path, "<color=#ff0000ff>" + Log + "</color>");
            }
            //CharacterBase
            if (c == Color.green)
            {
                File.AppendAllText(path,"<color=#00ff00ff>" + Log + "</color>");
            }
            //AI
            if (c == Color.blue)
            {
                File.AppendAllText(path,"<color=#00ffffff>" + Log + "</color>");
            }
            //UI/UserInput
            if (c == Color.yellow)
            {
                File.AppendAllText(path,"<color=#ffff00ff>" + Log + "</color>");
            }

            //Info
            if (c == Color.black)
            {
                File.AppendAllText(path,Log);
            }
           
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error  0 " + ex.Message + ex.Source);
        }


#endif


        }

        public static void ClearLog()
        {
            string path;
#if UNITY_EDITOR
            path = Application.dataPath + "Log.txt";
#elif UNITY_ANDROID && !UNITY_EDITOR
path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        try
        {
            using (StreamWriter outputFile = new StreamWriter(path, true))
            {
                outputFile.WriteLine("");
            }
        }
        catch(System.Exception ex)
        {
        Debug.Log("Error  0 " + ex.Message + ex.Source);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        path = System.IO.Path.Combine(Application.persistentDataPath, "Log.txt");
        try
        {
            byte[] info = new UTF8Encoding(true).GetBytes("");
            Debug.Log("Clean 1 ");
            File.WriteAllBytes(path, info);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error  0 " + ex.Message + ex.Source);
        }
#endif


        }
    }
}


