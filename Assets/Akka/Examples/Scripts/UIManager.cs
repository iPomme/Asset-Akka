using System;
using System.Threading;
using Akka.Actor;
using Jorand.AkkaUnity;
using UnityEngine;
using System.Threading;
using LaYumba.Functional;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviourActor
    {
        public Text speedInfo;

        private void Start()
        {
            base.Start(this.name);
        }

        public override void OnReceive(object message, IActorRef sender)
        {
            UnityEngine.Debug.Log(Thread.CurrentThread.ManagedThreadId + ": received from '" + sender.ToString() +
                                  "' the message: " + message.ToString());

            switch (message)
            {
                case UIMessages.ButtonMessage button:
                    UnityEngine.Debug.LogFormat("button '{0}' is playing", button.name);
                    UnityThread.executeInUpdate(() => changeLabelButton(button.name));
                    break;
                case UIMessages.SpeedInfo speed:
                    UnityEngine.Debug.LogFormat("speedInfo received with '{0}' as value", speed.speed);
                    UnityThread.executeInUpdate(() => speedInfo.text = $"{speed.speed:n0} Msg/sec");
                    break;
                default:
                    UnityEngine.Debug.LogErrorFormat("Unknown message {0}", message);
                    break;
            }
        }

        private void changeLabelButton(string name)
        {
            Option<Button> button = GameObject.Find(name).GetComponent<Button>();
            button.Match(
                Some: b => b.GetComponentInChildren<Text>().text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                None: () => Debug.LogFormat("The button '{0}' is not found", name));
        }
    }


    public class UIMessages
    {
        public class ButtonMessage
        {
            internal string name;

            public ButtonMessage(string nameOfTheButton)
            {
                this.name = nameOfTheButton;
            }
        }

        public class SpeedInfo
        {
            internal float speed;

            public SpeedInfo(float speed)
            {
                this.speed = speed;
            }
        }
    }
}