﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SceneGenerator
{
    public class DataManager : MonoBehaviour
    {
        private int nr = 0;
        public StringRuntimeSet textures;

        public IList<string> models;

        public ModelRuntimeSet modelData;

        public StringVariable texPath;

        public StringVariable modPath;

        public StringRuntimeSet categories;

        public void LoadModelData()
        {
            if (File.Exists( modPath.Value + "/modelData.txt"))
            {
                string modJson = File.ReadAllText( modPath.Value + "/modelData.txt");

                models = JsonConvert.DeserializeObject<List<string>>(modJson);
                try
                {
                    foreach (String s in models)
                    {
                        modelData.Add(JsonConvert.DeserializeObject<ModelData>(s));
                    }

                    models.Clear();
                }
                catch (Exception)
                {
                    models = new List<string>();
                }

            }
            else WriteJsonToFile("",  modPath.Value + "/modelData.txt");

        }

        public void LoadTextureData()
        {
            foreach (string file in Directory.GetFiles(texPath.Value))
            {
                string fileName = file.Split('\\')[file.Split('\\').Length - 1];
                textures.Add(texPath.Value + "/" + fileName);
            }
        }

        public void SaveModelData()
        {
            foreach (ModelData d in  modelData.Items)
            {
                 models.Add(JsonConvert.SerializeObject(d, Formatting.Indented));
            }

            string modJson = JsonConvert.SerializeObject( models, Formatting.Indented);

            models.Clear();

            WriteJsonToFile(modJson,  modPath.Value + "/modelData.txt");
        }

        public void WriteJsonToFile(string json, string filepath)
        {
            StreamWriter file;

            try
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }

                file = File.CreateText(filepath);
                file.Close();
                File.WriteAllText(filepath, json);
                //file.Close();
            }
            catch (Exception e)
            {
                if (!Directory.Exists(modPath.Value))
                {
                    Directory.CreateDirectory(modPath.Value);
                    WriteJsonToFile(json, filepath);
                }
                Debug.Log(e.Message);
            }
        }

        public void MoveFile(string sourcePath, string directory)
        {
            //TODO: Support for Unix-Systems (check OS and do adaptive split) 

            string fileName = sourcePath.Split('\\')[sourcePath.Split('\\').Length - 1];

            if (!File.Exists(directory + "/" + fileName))
            {
                File.Move(sourcePath, directory + "/" + fileName);
                if (directory.Equals( texPath.Value))
                {
                     textures.Add(directory + "/" + fileName);
                }
            }
        }

        public void Init()
        {
            texPath.Value = Application.streamingAssetsPath + "/textures";
            modPath.Value = Application.streamingAssetsPath + "/models";

            if (!Directory.Exists(modPath.Value)) Directory.CreateDirectory(modPath.Value);
            if (!Directory.Exists(texPath.Value)) Directory.CreateDirectory(texPath.Value);

            LoadTextureData();
            LoadModelData();
            LoadCategories();

        }

        public void LoadCategories()
        {
            foreach(string s in Directory.GetDirectories(modPath.Value))
            {
                categories.Add(s.Split('\\')[s.Split('\\').Length - 1]);
            }
        }

        public void CreateCategoryDir(string newCat)
        {
            if (!Directory.Exists(modPath.Value + "/" + newCat)) Directory.CreateDirectory(modPath.Value + "/" + newCat);
        }

        private void Start()
        {
            models = new List<string>();
            Init();
        }

        public void WriteToCSV(string directory, string content, int diff, string format, int dataPoints, int modelCount, bool shadows)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string path = directory + "/SceneData_Diff_" + nr.ToString() + "_" + diff.ToString() + "_" + format + "_" + dataPoints + "_" + modelCount + "_" + shadows.ToString() + ".csv";
            File.WriteAllText(path, content);
            nr++;
        }

    }
}
