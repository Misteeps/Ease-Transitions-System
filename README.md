# ETS - Ease Transitions System
ETS is a tool to easily test and apply [ease](https://easings.net/en) transitions to component values in [Unity](https://unity.com/).  
Tween 2D and 3D animations quickly with a simple but powerful system that allows for user flexibility.  

ETS allows for time based transitions to individual values in a component _(Position.X value in Transform)_.  
Values are dynamically transitioned so that values will not snap and instead continue the transition wherever it is currently.  

~~Download from the Asset Store~~  
View the [ETS Wiki](https://github.com/Misteeps/Ease-Transitions-System/wiki) for usage and documentation  

## Installation
Extract files and folders into a Unity project's asset folder  

## Supported Components
- Transform
- Rect Transform
- Sprite Renderer
- Image
- Text

## Editor Window
ETS comes with a custom editor window that can be found under **[Tools/Ease Transitions Editor]**.  
ETS can be used to apply transitions to your gameobjects with or without the editor window.  
The custom editor allows for:  
- Organization of transitions with groups and objects
- Applying transitions with ease _(ba-dum tsss)_
- Visualization and testing of one or more transitions simultaneously without using the play button
- Simple exports to code for more user control

> Warning: Unity has a new UI in 2019.3  
> Editor Window UI is designed for version 2019.3  
> Editor Window UI for versions 2019.2 and older is different and messy, but will still work  

## Quick Start (Will be replaced with in-depth documentation soon)
- Add EaseTransitions.cs anywhere to the scene
- Open the Ease Transitions Editor from "Tools/Ease Transitions Editor"
- Create asset file "Ease Transitions Data" and store in Editor Folder

>- Insert asset file into editor window
>- Create Groups
>- Create Elements and insert/search gameobjects
>- Select transition options and add components to transition
>- Copy templated code and paste into game code

## To Do
- Full Documentation in wiki:  
    - [x] Home
    - [ ] Setup Guide
    - [ ] How ETS Works
    - [ ] Supported Components
    - [ ] Editor Window

## Future Considerations
- Seperate UI for 2019.2 and older
- Visualize ease curves
- Custom ease curves
