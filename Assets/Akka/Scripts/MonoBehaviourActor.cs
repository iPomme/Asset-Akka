using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using Akka.Actor;
using Akka;

namespace Jorand.AkkaUnity
{
    public abstract class MonoBehaviourActor : MonoBehaviour
    {
        protected ActorSystem system;

        public abstract void OnReceive(object message, IActorRef sender);

        protected IActorRef internalActor;


        public MonoBehaviourActor()
        {
            Debug.Log("MonoBehaviourActor Constructor of " + this.GetType().Name + " ...");
        }


        protected void Start()
        {
            Debug.Log("MonoBehaviourActor Starting ....");
            system = SystemActor.Instance.GetActorSystem();
            internalActor = system.ActorOf(InternalActor.Props(this.GetType().Name, this), this.GetType().Name);
        }

        protected void OnDisable()
        {
            Debug.Log("MonoBehaviourActor OnDisable ....");
            system.Terminate();
        }
    }

    public class InternalActor : UntypedActor
    {
        private string _name;
        private MonoBehaviourActor _unity;

        public InternalActor(string name, MonoBehaviourActor unity)
        {
            UnityEngine.Debug.Log("Starting Actor " + name);
            _name = name;
            _unity = unity;
        }

        protected override void OnReceive(object message)
        {
            _unity.OnReceive(message, Sender);
        }

        public static Props Props(string name, MonoBehaviourActor unity)
        {
            return Akka.Actor.Props.Create(() => new InternalActor(name, unity));
        }
    }
}