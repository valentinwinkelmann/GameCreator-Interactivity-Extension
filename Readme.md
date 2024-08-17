# GameCreator2 Interactivity Extension
This extension adds a new way to make interactive objects. It was designed to make it easiert to implement vehicles and extend existing third party assets like NWH Vehicle Physics or Edy's Vehicle Physics. But it can be used for any other kind of interactive object like Chairs, Beds, Workbenches, Chests and so on.

## 🚨 Important
This Extension is a work in progress and purely experimental. It is currently not recommended in production.

## 📚 Table of Contents
- [Installation](#-installation)
- [Requirements](#-requirements)
- [InteractiveMonoBehaviour](#-interactivemonobehaviour)
  - [Methods](#-methods)
  - [Properties](#-properties)
  - [IK Events](#-ik-events)
- [AOT Safety](#-aot-safety)
- [Bug, Suggestions and Contributions](#-bug-suggestions-and-contributions)
- [License](#-license)

## 💾 Installation
To install this extension, you can easily add it to your project via the Unity Package Manager. Just Go to Window -> Package Manager and click on the "+" Button in the top left corner. Select "Add package from git URL" and paste the following URL:
```
https://github.com/valentinwinkelmann/GameCreator-Interactivity-Extension
```
After that, the package will be downloaded and added to your project.

## 📦 Requirements
This extension requires the following assets to be installed in your project:
- [GameCreator2](https://assetstore.unity.com/packages/tools/game-toolkits/game-creator-2-203069)
- [Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290)

## ❓ Why not just use the GameCreator2 Visual Scripting?
I totally recommend to use the GameCreator2 Visual Scripting for most of the things. But when it comes to more complex interactions, like entering a vehicle, it can get very complex and hard to maintain (at least for me). I often find myself in the situation, where i think some complex task would be much easier and cleaner to implement in code. For me that are things like vehicles and workbenches.

## ⚡ InteractiveMonoBehaviour
![InteractiveMonoBehaviour](/.GithubDocumentation~/Inspector_01.png)

The InteractiveMonoBehaviour is meant to be used as a base class for interactive objects. It hooks into the GameCreator2 Interaction System and let the player Interact with it. While it's custom inspector has 4 different Instruction fields, it is meant to be extended by code. The InteractiveMonoBehaviour also still shows your custom properties in the Custom Inspector, but will organize them in a foldout.
A simple and working example for NWH Vehicle Physics can be this:
```csharp
public class InteractiveVehicle : InteractiveMonoBehaviour
{
    [SerializeField] private VehicleController _vehicleController;

    public OnInteract(){
        _vehicleController.Inputs.SetAutoInput(true);
        await InteractiveUtility.WaitForInput(KeyCode.E);
        InteractionStop();
    }
    public void OnBeforeStopInteract(){
        _vehicleController.Inputs.SetAutoInput(false);
    }
}
```
*You don't need more than this, to make a vehicle interactive.*

The Custom Inspector of the InteractiveMonoBehaviour will let you assign a Marker and a custom Character State. The Character will move to the Marker and if reached, the character will enter the State. If the State has a Entry Animation, it will be played and after that entry animation the OnInteract() method will be called. This way it is very easy to let a character first walk to a car door, enter the car and then overtaking the controlls. The same procedure will be done when the character stops the interaction.
### 🔌 Methods
The InteractiveMonoBehaviour has 4 different states, which we can hook into with our code.
- **OnBeforeInteract(Character character)** - Called when the character has reached the Marker.
- **OnInteract(Character character)** - Called when the character has finished the Entry Animation.
- **OnBeforeStopInteract(Character character)** - Called as soon as the character starts playing the Exit Animation.
- **OnStopInteract(Character character)** - Called when the character has finished the Exit Animation.
- **OnFail(CancelReason reason)** - Called when the character can't reach the Marker or the Interactive Object is blocked by another character.

All of these methods are called from the InteractiveMonoBehaviour by refflection. You don't need to override them, just make sure they are public. Also all of these methods are provided as instruction list in the Custom Inspector of the InteractiveMonoBehaviour, so you can easily assign visual scripting actions to them. The Instructions are blocling, so you can add a Wait instruction into it to make the character wait in the interaction.

### 🎛️ Properties
The InteractiveMonoBehaviour provides some propertis which are aviable in the Custom Inspector and can be overridden from code too:
- **bool CharacterBusy** - If true, the character will be set to busy and can't interact with other objects.
- **bool CharacterControllable** - If true, the character will be set to uncontrollable and can't be moved by the player.
- **bool CharacterMount** - If true, the character will be mounted to the Interactive Object and will be moved with it.
- **GameObject MountObject** - If set, the character will be mounted to this object instead of the Interactive Object.
- **Location CharacterLocation** - If set, the character will move to this location instead of the Interactive Object.
- **State CharacterState** - If set, the character will enter this state.
- **Transform[] CharacterIKPoints** - Points which will be used for the Final IK Integration.

Every property can be set in the Custom Inspector or be overridden from code. If overriden from code, the Custom Inspectors value will be ignored. You can override it like this:
```csharp
public class InteractiveVehicle : InteractiveMonoBehaviour
{
    protected override bool CharacterBusy => false;
}
```

### 🩻 IK Events
The InteractiveMonoBehaviour provides a way to make the character grab or release IK Points from the **CharacterIKPoints** array. To do this, you have to add AnimationEvents to your Animation Clips.
Just add an AnimationEvent with the name "IK" and give it a string parameter with the name of the IK Point followed by _LeftHand or _RightHand. You can also use _LeftFoor, _RightFoot or _Body. The Events work like a Toggle, so if you call it for the first time on that limb, the character will grab the IK Point. If you call it again, the character will release the IK Point. After the Interaction is stopped, all IK Points will automatically be released.
You can of course also override the IK temprarily with a Custom Instruction called **Set Final IK Weight**. This way you can make the charachter release a Hand from the steering wheel to aim with a gun and after aim you call the **Set Final IK Weight** again to make the character grab the steering wheel again.


## 🛡️ AOT Safety
The InteractiveMonoBehaviour was tested on WebGL to test if it is AOT safe. I couldn't test it on iOS but it should work there too. You are forced to make all callback methods public, so Code Stripping should not remove them. Also the Refflected methods are cached after the first call, so there should be no performance impact.

## 🦄 Bug, Suggestions and Contributions
If you find a bug or have a suggestion, feel free to open an issue. If you want to contribute, you can also open a pull request with your changes. I'm mainly a 3D Artist and not a full time programmer, so i'm sure there is a lot of room for improvement and not everything follows the best practices. I'm happy for every contribution.

## 📜 License
This Extension is licensed under the MIT License. You can use it for your commercial projects as you want. If you like it, you can give me a credit, but you don't have to. You are not allowed to sell this extension or any part of it as your own.