# Point Cloud Ray Tracer

This repository contains code to generate synthetic point cloud scenes where the lighting conditions are taken into account. In a nutshell, meshes are randomly placed in the room, and the colour of points is determined by ray tracing method. Furthermore, shadows are also contained in the synthetic point cloud by calculating shadow rays.  

<img src="./Figures/GeneratedPointCloud.jpg" width="800"/>

## Requirements

Intergrate the following plugins:

* [TriLib](https://assetstore.unity.com/packages/tools/modeling/trilib-2-model-loading-package-157548)
* [SimpleFileBrowser](https://assetstore.unity.com/packages/tools/gui/runtime-file-browser-113006)
* [QuickOutline](https://github.com/chrisnolet/QuickOutline)
* [JsonDotNet](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html)

## Quickstart

<img src="./Figures/GeneratedScene.jpg" width="800"/>

1) Open the project in Unity. 
2) Open the *MenuScene.unity* in the *./Assets/Scenes/Menu* directory
3) Start the scene and click on *Scene Generator*
4) Click on *Generate single scene*
5) Add some object categories on the left
6) Click on *Generate*
7) Wait some seconds for the console message *Gravity Applied*
8) Click on *Trace Scene*
9) Click on *Save Scene* - the scene will be stored in the *./StreamingAssetsPath/data* folder as .csv file

## Create Dataset

<img src="./Figures/ImportScene.jpg" width="800"/>

The data should be stored in the StreamingAssets path. Here you create 2 Folders called *models* *textures*. Place all you textures in the *textures* folder. Create a folder foreach category (class) in the *models* folder. For instance, you could create a directory *StreamingAssetsPath/models/Chair* for the chair class. All chair models should be located in the chair folder. In Unity, open and start the scene *./Assets/Scenes/AssetImport/ImportScene.unity*. Click on *Current Folder* and navigate into a class folder located in the *models* folder. Choose the appropriate category in the dropdown menu. Apply a texture by clicking on one of the textures in the right context menu. Choose the appropriate size (S, M, L) for your model. Click on save model and continue with the next model. The textured models will be stored in a *StreamingAssetsPath/models/modelData.txt* file.

To create a point cloud scenes dataset with the meshes, open the menu scene such as in step 2) in the quickstart section. After that, click on *Scene Generator* followed by *Generate Dataset*. The interface is the same as the *Generate single scene* interface. Specify a dataset size and click on *Generate* if the other settings are okay.

## Citation

Coming Soon