using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System;
using System.Diagnostics;
using Jorand.AkkaUnity;
using System.Threading;
using Assets.Scripts;
using LaYumba.Functional;
using Unity.Profiling;
using static LaYumba.Functional.F;
using Debug = Akka.Event.Debug;

public class PingPong : MonoBehaviourActor
{
    [Header("Akka settings")] 
    public string UIManagerPath;

    private Option<IActorRef> uiManager;
    
    void Start()
    {
        // Initialize the Actor
        base.Start(this.name);
        uiManager = getActorRef(UIManagerPath, system);
        UnityEngine.Debug.Log("PingPong Starting ....");

        // Create the ping and the pong actor
        var ponger = system.ActorOf(PingPongActor.Props(new Messages.Ping(), new Messages.Pong(), UIManagerPath), "ponger");
        var pinger = system.ActorOf(PingPongActor.Props(new Messages.Pong(), new Messages.Ping(), UIManagerPath), "pinger");

        // Create a scheduler to make sure we can send messages to this MonoBehavior
        system.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), internalActor,
            "2 sec Scheduler triggered", internalActor);

        // Start the game ....
        pinger.Tell(new Messages.Pong(), ponger);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnReceive(object message, IActorRef sender)
    {
        UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": received from '"+ sender.ToString()+ "' the message: " + message.ToString());
        uiManager = uiManager.OrElse(getActorRef(UIManagerPath, system));
        uiManager.ForEach(actor => actor.Tell(new UIMessages.ButtonMessage("LabelBtn")));
    }
    
    public class Messages
    {
        public interface IPingPong
        {
            
        }
        public class Ping : IPingPong
        {
            internal string name;

            public Ping()
            {
                this.name = "Ping";
            }
        }
        
        public class Pong : IPingPong
        {
            internal string name;

            public Pong()
            {
                this.name = "Pong";
            }
        }
    }
}


public class PingPongActor : UntypedActor
{
    static ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("AKKAUnity.OnReceive");
    
    private readonly PingPong.Messages.IPingPong _matcher;
    private readonly PingPong.Messages.IPingPong _answer;
    private readonly string _uiManagerPath;

    private int count = 0;
    private Option<IActorRef> uiManager;

    private Stopwatch timeMessages = Stopwatch.StartNew();

    public PingPongActor(PingPong.Messages.IPingPong matcher, PingPong.Messages.IPingPong answer, string uiManagerPath)
    {
        _matcher = matcher;
        _answer = answer;
        _uiManagerPath = uiManagerPath;
        UnityEngine.Debug.LogWarning("Starting " + matcher + " ==>" + answer);
    }

    protected override void OnReceive(object message)
    {
        s_PreparePerfMarker.Begin();
        
        if (message.GetType() == _matcher.GetType())
        {
            Sender.Tell(_answer);
            count += 1;
            if (count % 100000 == 0)
            {
                UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": " + count.ToString("N0") +
                                      ": received " + message.ToString() + " answering " +
                                      _answer);

                uiManager = uiManager.OrElse(MonoBehaviourActor.getActorRef(_uiManagerPath, AkkaSystem.Instance.GetActorSystem()));
                int time = timeMessages.Elapsed.Milliseconds;
                timeMessages = Stopwatch.StartNew();
                UIMessages.SpeedInfo msg = new UIMessages.SpeedInfo(100000 / time * 1000);
                UnityEngine.Debug.LogFormat("about to send {0} to the UIManager {1}",msg, uiManager);
                uiManager.ForEach(a => a.Tell(msg, Self));
            }
        }
        
        s_PreparePerfMarker.End();
    }

    public static Props Props(PingPong.Messages.IPingPong matcher, PingPong.Messages.IPingPong answer, string uiManagerPath)
    {
        return Akka.Actor.Props.Create(() => new PingPongActor(matcher, answer, uiManagerPath));
    }
}