# UnityTutorialSystem
Tutorial plugin for Unity Game project. Main purpose for this project is automation of creation tutorials for your games written in Unity.\
How to use:

1. Import asset into your project

2. Navigate to {your project}/plugins/Artisso/Tutorial System/Prefabs

3. Drop prefab TutorialSystemManager to your scene

4. Choose TutorialSystemManager  prefab inside your scene to open editor

5. Drop default canvas from your scene to [Default canvas] field. Tutorial system will automatically use this canvas to attach dialog window and overlay.

6. You are ready to create you first tutorial

IMPORTANT NOTICE: You can have one TurorialSystemManager instance per scene. Using more then one component per scene will lead to errors. If you are attaching Tutorial prefab directly to game instance, this rule applies to whole game.

Basic options:

1. Auto start - set true if you want to start tutorial automatically, when scene loads.

2. Tutorial number - if auto start is set to true, specify tutorial number that will start automatically

3. Default canvas - main canvas that Tutorial System is attached to. Without specifying this parameters tutorial will not work. 

4. Dialog template - dialog window template, that is used in current tutorial. You can find basic templates inside /plugins/Artisso/Tutorial System/Prefabs/DialogTemplates or you can create your own customized prefab. Check default dialog prefab for required script and fields. 

Additional parameters:

1. Allow to skip - if checked to true, displays close button in tutorial window. This option will allow user to close tutorial in any step.

2. Destroy on end - if checked to true, this option will destroy tutorial instance in current scene when main tutorial specified in [tutorial number] is finished. Please note that this will break any custom references inside your scene, to tutorial manager. For example if  you create buttons or triggers that are supposed to launch specific tutorial.

3. Overlay color. You can specify default color of your overlay when playing tutorial steps. Please note that overlay for 3d object is currently not supported. In this case uncheck [Use overlay] in current tutorial step. See: [Tutorial steps] below.

4. Save method. None means that tutorial progress is not saved. Player preferences will save tutorial progress locally to player preferences. This method is useful when you start tutorial automatically and don't want tutorial to relaunch itself every time game reloads. In this case after tutorial is finished, it will not start again automatically, but you can still trigger tutorial to start manually.

5. Text typing effect. If slider value is set to > 0 this option will enable text typing effect in tutorial dialog window. Please note that typing effect will be automatically disabled if current step [Freeze time] is enabled, to ensure proper text display.

6. Use image, If set to true, this option will enable displaying Images in dialog window.

7. Default image. Set default image that will be displayed in dialog window. This field is mandatory if [use image] is set to true.


