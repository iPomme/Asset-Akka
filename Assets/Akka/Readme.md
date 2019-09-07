# AKKA integration

## Motivation
Using often Akka when coding in Scala, I got used to this paradigm and would like to use it when coding in C#.
I added as well the [LaYumba.Functional](https://github.com/la-yumba/functional-csharp-code) library to use missing feature in c# 4.x.

## GameObject as Actor

To compose the monoBehavior with an Actor, perform the following
 - extends `MonoBehaviorActor` instead of `MonoBehavior`.
 - in the Start() method add `base.Start(this.name);`.
 
 When `MonoBehaviorActor` is extended, it will add code for 
 - Get/create the actor system
 - Register your Gameobject actor
 > The registered name is composed with your scene name and your Gameobject name seperated with a hyphen. For eg, if the scene is named 'PingPongScene' and the gameObject is named 'UIManager' then the path of the actor is '**/user/PingPongScene-UIManager**'

The actor is accessible from the class using the `internalActor` property.
The system actor is accessible from the class using the `system` property.

## Get an actor reference

When you want to get an actor reference in your class, you can add the following properties:

```c#
[Header("Akka setup")]
public string UIManagerPath;
 
private Option<IActorRef> uiManager;
```

in the Start() method you can try to get a reference with:

```c#
uiManager = getActorRef(UIManagerPath, system)
```

But as the instance creation of the GameObject is done by Unity engine, there is no guarantee that the actor is created before the current class.
So when you want to send a message to the actor use the following:

```c#
 uiManager = uiManager.OrElse(getActorRef(UIManagerPath, system));
 uiManager.ForEach(actor => actor.Tell(new UIMessages.ButtonClicked(this.name), internalActor));
```