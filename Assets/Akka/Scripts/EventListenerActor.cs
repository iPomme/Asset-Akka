using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Akka.Actor;
using System.Linq;
using Akka.Util.Internal;
using System.Text;
using System;

namespace Jorand.AkkaUnity
{
    public class EventListenerActor : UntypedActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        private Dictionary<string, List<IActorRef>> listeners = new Dictionary<string, List<IActorRef>>();
        private bool debugFieldReady;

        protected override void OnReceive(object message)
        {
            if (message is Register)
            {
                Register register = message as Register;
                List<IActorRef> list;

                if (!listeners.TryGetValue(register.evt, out list))
                {
                    list = new List<IActorRef>();
                    listeners.Add(register.evt, list);
                }

                list.Add(register.actorRef);
                if (register.evt == "DebugMessage")
                {
                    debugFieldReady = true;
                    Self.Tell(new DebugMessage("Debugger filed ready !!!!!!!!!!!!!!!!!!!!!!"));// TODO use Becom or FSM to manage initialisation
                }
            }
            else if (message is PrintListeners)
            {
                var toPrint = listeners
                    .Select(kvp => kvp.Key + ": " + string.Join(", ", kvp.Value.Select(a => a.ToString())))
                    .Aggregate(
                        new StringBuilder(),
                        (current, next) => current.Append(current.Length == 0 ? "" : "\n").Append(next))
                    .ToString();
                Debug.Log(Self.ToString() + ": The list of the listeners are: \n" + toPrint);
            }
            else if (message is DebugMessage)
            {
                if (debugFieldReady)
                {
                    Stash.UnstashAll();
                    Debug.Log("Unstashing all the messages:");
                    List<IActorRef> list;
                    if (listeners.TryGetValue(message.GetType().Name, out list))
                    {
                        list.ForEach(actorRef => actorRef.Forward(message));
                    }
                }
                else
                {
                    Debug.Log("Stashing message " + message.ToString());
                    Stash.Stash();
                }
            }
            else
            {
                UnityEngine.Debug.Log("Got message to transmit: " + message.ToString());
                List<IActorRef> list;
                if (listeners.TryGetValue(message.GetType().Name, out list))
                {
                    list.ForEach(actorRef => actorRef.Forward(message));
                }
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<EventListenerActor>();
        }
    }
}