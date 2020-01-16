using System;
using Proto;
using UnityEngine;

namespace Assets.Scripts
{
    public class ActorSystem : MonoBehaviour
    {
        public static readonly RootContext Context = new RootContext();
        private PID _pingerActor;
        private PID _pongerActor;
        private PID _uiManagerActor;

        private void Start()
        {
            var props = Props.FromProducer(() => new UIManagerActor());
            _uiManagerActor = Context.SpawnNamed(props, "UIManagerActor");
            UnityEngine.Debug.Log("PingPong Starting ....");
//            var ret = Context.RequestAsync<object>(actor, new Hello() {Who = "Nicolas"});
            // Create the ping and the pong actor
            _pongerActor = Context.Spawn(Props.FromProducer(() =>
                new PingPongActor(new Messages.Ping(), new Messages.Pong(), _uiManagerActor)));
            _pingerActor = Context.Spawn(Props.FromProducer(() =>
                new PingPongActor(new Messages.Pong(), new Messages.Ping(), _uiManagerActor)));
            
            Context.Request(_pingerActor, new Messages.Pong(), _pongerActor);

        }


        private void OnDestroy()
        {
            _pingerActor.Stop();
            _pongerActor.Stop();
            _uiManagerActor.Stop();
        }
    }
}