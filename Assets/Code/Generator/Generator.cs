﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SceneGenerator
{
    public class Generator : MonoBehaviour
    {
        public StringRuntimeSet categories;

        public ModelRuntimeSet modelData;

        public StringRuntimeSet textures;

        public DataManager dataManager;

        public ModelLoader modelLoader;

        public Tracer tracer;

        public StringRuntimeSet chosenCategories;

        public IList<ModelData> models;

        public IList<GameObject> loadedModels;

        public Vector3 basePos;

        public int datasetSize;

        public bool randomDiff;

        public IntVariable diff;

        public IntVariable noise;

        public Text modelCount;

        public Text meshCount;

        public Text difficulty;

        public Text triangleCount;

        public int usedDiff;

        private bool random_color = false;

        public void GenerateDataSet()
        {
            StartCoroutine(SetCoroutine());
        }

        IEnumerator SetCoroutine()
        {
            random_color = true;
            for (int i = 0; i < datasetSize; i++)
            {
                GenerateScene();
                yield return new WaitForSecondsRealtime(18);
                tracer.render_shadows = true;
                tracer.TraceScene();
                yield return new WaitForSecondsRealtime(4);
                tracer.ExportDataToCSV();
                yield return new WaitForSecondsRealtime(4);
                tracer.render_shadows = false;
                tracer.TraceScene();
                yield return new WaitForSecondsRealtime(4);
                tracer.ExportDataToCSV();
                yield return new WaitForSecondsRealtime(4);
            }
            random_color = false;
        }

        public void GenerateScene()
        {
            DestroyAllLoadedModels();

            Light l = GameObject.Find("SceneLight").GetComponent<Light>();
            l.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            Dictionary<string, int> counter = new Dictionary<string, int>();

            foreach(string s in chosenCategories.Items)
            {
                counter.Add(s, 0);
            }

            float remainingDifficulty = GetDifficulty();

            float realdiff = remainingDifficulty / 2f;
            GameObject dummy;
            loadedModels = new List<GameObject>();

            int shelfcounter = 0;
            int chaircounter = 0;
            int tablecounter = 0;


            foreach (ModelData m in modelData.Items)
            {
                foreach(string s in chosenCategories.Items)
                {
                    if (m.category.Equals(s) /*&& m.difficulty <= remainingDifficulty*/)
                    {
                        models.Add(m);
                    }
                }
            }
            while(remainingDifficulty >= 0)
            {
                ModelData model = models[Random.Range(0, models.Count)];

                string cat = model.category;

                if (cat.Equals("Bookshelf_4"))
                {
                    if(shelfcounter < (realdiff/3))
                    {
                        shelfcounter++;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (cat.Equals("Chair_5"))
                {
                    if (chaircounter < (realdiff / 3))
                    {
                        chaircounter++;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (cat.Equals("Desk_6"))
                {
                    if (tablecounter < (realdiff / 3))
                    {
                        tablecounter++;
                    }
                    else
                    {
                        continue;
                    }
                }

                float size = model.scale.x;

                Vector3 randomPosition = new Vector3(Random.Range(-2.5f + size, 2.5f - size), Random.Range(-0.5f, 0.5f), Random.Range(-2.5f + size, 2.5f - size));

                if (modelLoader.LoadObjectWithMaterials(model, out dummy, basePos))
                {
                    //remainingDifficulty -= model.difficulty;
                    remainingDifficulty -= model.scale.x;
                    counter[model.category]++;
                    foreach(Transform child in dummy.transform.GetChild(0))
                    {
                        child.GetComponent<Label>().counter = counter[model.category];
                    }
                    loadedModels.Add(dummy);
                }
                else
                {
                    throw new System.Exception("Are the right paths set in modelData.txt");
                }
            }
            
            StartCoroutine(ApplyGravity());

            UpdateSceneInfo(loadedModels);
        }

        public void ToggleCategory(Toggle toggle)
        {
            if (toggle.isOn)
            {
                chosenCategories.Add(toggle.GetComponentInChildren<Text>().text);
            }
            else
            {
                chosenCategories.Remove(toggle.GetComponentInChildren<Text>().text);
            }
        }

        public void DestroyAllLoadedModels()
        {
            if(loadedModels != null)
            {
                foreach (GameObject g in loadedModels)
                {
                    Destroy(g);
                }
            }

        }

        public void SetDatasetSize(InputField input)
        {
            int size = int.Parse(input.text);
            if (size < 1) size = 1;
            input.text = size.ToString();

            datasetSize = size;
        }

        public void SetDifficulty(Slider slider)
        {
            diff.Value = (int)slider.value;

            GameObject.Find("DiffTextSet").GetComponent<Text>().text = diff.Value.ToString();
        }

        public void SetNoise(Slider slider)
        {
            noise.Value = (int)slider.value;
        }

        public void SetResolution(InputField input)
        {
            int res = int.Parse(input.text);
            //if (res < 50) res = 50;
            tracer.resolution = res;
            input.text = res.ToString();
        }

        public void ToggleRandDiff(Toggle toggle)
        {
            randomDiff = toggle.isOn;
        }

        public void ToggleShadows(Toggle toggle)
        {
            tracer.render_shadows = toggle.isOn;
        }

        public float GetDifficulty()
        {
            float difficulty = randomDiff ? Random.Range(1, 10.49f) : diff.Value;
            usedDiff = (int)difficulty;

            tracer.perspectives = 6 - (int)(usedDiff/3);
            //difficulty = (5 * difficulty) / (1 + 0.15f*(6 - tracer.perspectives));
            return usedDiff*2f;
        }

        IEnumerator ApplyGravity()
        {
            foreach (GameObject obj in loadedModels)
            {
                foreach (Transform t in obj.transform.GetChild(0))
                {
                    Destroy(t.GetComponent<MeshCollider>());
                    t.gameObject.AddComponent<BoxCollider>();
                }
                obj.AddComponent<Rigidbody>().useGravity = true;
            }

            yield return new WaitForSecondsRealtime(8);


            foreach (GameObject obj in loadedModels)
            {
                bool gravity = obj.GetComponent<Rigidbody>().useGravity;
                obj.GetComponent<Rigidbody>().useGravity = !gravity;
                Destroy(obj.GetComponent<Rigidbody>());

                foreach (Transform t in obj.transform.GetChild(0))
                {
                    Destroy(t.gameObject.GetComponent<BoxCollider>());
                }
            }

            yield return new WaitForSecondsRealtime(4);

            foreach (GameObject obj in loadedModels)
            {
                foreach (Transform t in obj.transform.GetChild(0))
                {
                    t.gameObject.AddComponent<MeshCollider>();
                }
            }
            Debug.Log("Gravity applied");
        }

        public void UpdateSceneInfo(IList<GameObject> loadedModels)
        {
            modelCount.text = loadedModels.Count.ToString();

            int meshes = 0;
            int triangles = 0;

            foreach(GameObject g in loadedModels)
            {
                meshes += g.transform.GetChild(0).childCount;

                foreach(Transform child in g.transform.GetChild(0))
                {
                    triangles += child.GetComponent<MeshFilter>().mesh.triangles.Length/3;
                }
            }

            triangleCount.text = triangles.ToString();

            meshCount.text = meshes.ToString();

            difficulty.text = usedDiff.ToString();

        }


        // Start is called before the first frame update
        void Start()
        {
            models = new List<ModelData>();
        }

    }

}