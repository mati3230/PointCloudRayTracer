using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SceneGenerator
{
	public class TexGridController : MonoBehaviour
	{
		public StringRuntimeSet textures;

		public GameObject imagePrefab;

		void Start()
		{
			PopulateTexGrid();
			//Pop();
		}

		void PopulateTexGrid()
		{
			GameObject newObj;

			string[] files = Directory.GetFiles(Application.streamingAssetsPath + "/textures");

			foreach (string file in files)
			{
				string s = file;
				if (s.EndsWith(".meta")) continue;
				newObj = (GameObject)Instantiate(imagePrefab, transform);

				byte[] bytes = System.IO.File.ReadAllBytes(s);
				Texture2D texture = new Texture2D(1, 1);
				texture.LoadImage(bytes);

				newObj.GetComponent<RawImage>().texture = texture;
				newObj.name = s.Split('/')[s.Split('/').Length - 1];
			}

			Destroy(imagePrefab);
		}

	}

}
