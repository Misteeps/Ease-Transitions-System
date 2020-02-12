# ETS - Ease Transitions System
ETS is a tool to easily test and apply [ease](https://easings.net/en) transitions to component values in [Unity](https://unity.com/).  
Tween 2D and 3D animations quickly with a simple but powerful system that allows for user flexibility.

ETS allows for time based transitions to properties in components _(Position.X value in Transform)_.  
Values are dynamically transitioned so that values will not snap and instead continue the transition from anywhere.

ETS has a ridiculously low overhead, only containing a single MonoBehaviour Update() to transition all set GameObjects.

<img align="middle" width="100%" src="https://cdn.discordapp.com/attachments/673843892621148160/675758612123222016/ETS_Showcase_4MB.gif">

Of course, the gif showcase above was made in Unity with ETS.

View the [ETS Wiki](https://github.com/Misteeps/Ease-Transitions-System/wiki) for usage and documentation

## Installation
~~Download and install straight from the Asset Store.~~  
_**or**_  
Download [from releases](https://github.com/Misteeps/Ease-Transitions-System/releases/tag/v1.0) and extract the contents into your Unity project's assets folder.

<img align="middle" width="" src="https://cdn.discordapp.com/attachments/673843892621148160/673926333365223464/Extract_Files.png">

More installation and setup instructions found in the [Setup Guide](https://github.com/Misteeps/Ease-Transitions-System/wiki/Setup-Guide).

## Supported Components

<img align="right" width="50%" src="https://cdn.discordapp.com/attachments/673843892621148160/673963895379066892/Editor_Window_Showcase.png">

- Transform
- Sprite Renderer
- Rect Transform
- Image
- Text

Full list of supported components and fields found [here](https://github.com/Misteeps/Ease-Transitions-System/wiki/Supported-Components).

## Editor Window
ETS comes with a custom editor window that can be found under **[Tools/Ease Transitions Editor]**.

<img align="middle" width="40%" src="https://cdn.discordapp.com/attachments/673843892621148160/673926366835507219/Open_Location.png">

The custom editor window allows for:
- Organization of transitions with groups and objects
- Applying transitions quickly with ease _(ba-dum tsss)_
- Visualization and testing of one or more transitions simultaneously in the editor (without using the play button)
- Exporting to code easily by displaying template code

Full [Usage and Documentation](https://github.com/Misteeps/Ease-Transitions-System/wiki/Editor-Window).

> ETS can be used to apply transitions to gameobjects with or without the editor window.

> Warning: Unity has a new UI in 2019.3  
> Editor Window UI is designed for version 2019.3  
> Editor Window UI for versions 2019.2 and older is different and messy, but will still work

## Examples

<img align="left" width="20%" src="https://cdn.discordapp.com/attachments/673843892621148160/676080010741809162/Timed_Diamonds.gif">

> *Left*  
> 4 diamonds, each being transitioned from 30 to 0 in 2 seconds.  
> The timers at the bottom show when each diamond reaches the end.  
> Diamonds closer to the end take less time than diamonds further from the end.  
> Diamonds past the start do not take longer than the set duration.

> *Below*  
> Each ball is transitioned from top to bottom with different ease functions.

<img align="middle" width="70%" src="https://cdn.discordapp.com/attachments/673843892621148160/676096210343297038/Marble_Functions.gif">

***

More examples coming eventually...

## To Do
- [x] Full Documentation in wiki
- [ ] 3D Examples
- [ ] Space Mania Example Project

## Future Considerations
- Seperate UI for 2019.2 and older
- Visualize ease curves
- Custom ease curves
