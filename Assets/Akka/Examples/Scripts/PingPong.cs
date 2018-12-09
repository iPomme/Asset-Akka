using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System;
using Jorand.AkkaUnity;
using System.Threading;

public class PingPong : MonoBehaviourActor
{
    void Start()
    {
        // Initialize the Actor
        base.Start(this.name);
        UnityEngine.Debug.Log("PingPong Starting ....");

        // Create the ping and the pong actor
        var ponger = system.ActorOf(PingPongActor.Props("ping", "pong"), "ponger");
        var pinger = system.ActorOf(PingPongActor.Props("pong", "ping"), "pinger");

        // Create a scheduler to make sure we can send messages to this MonoBehavior
        system.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2), internalActor,
            "hello", internalActor);

        // Start the game ....
        pinger.Tell("pong", ponger);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnReceive(object message, IActorRef sender)
    {
        UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": received from '"+ sender.ToString()+ "' the message: " + message.ToString());
    }
}


public class PingPongActor : UntypedActor
{
    private readonly string _matcher;
    private readonly string _answer;

    private int count = 0;

    public PingPongActor(string matcher, string answer)
    {
        _matcher = matcher;
        _answer = answer;
        UnityEngine.Debug.LogWarning("Starting " + matcher + " ==>" + answer);
    }

    protected override void OnReceive(object message)
    {
        if (message.ToString() == _matcher)
        {
            Sender.Tell(_answer);
            count += 1;
            if (count % 100000 == 0)
                UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": " + count.ToString("N0") +
                                      ": received " + message.ToString() + " answering " +
                                      _answer);
        }
    }

    public static Props Props(string matcher, string answer)
    {
        return Akka.Actor.Props.Create(() => new PingPongActor(matcher, answer));
    }
}