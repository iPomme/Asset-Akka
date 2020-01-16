using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using UnityEngine;
using Proto;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public Text speedInfo;

        private void changeLabelButton(string name)
        {
            Option<Button> button = GameObject.Find(name).GetComponent<Button>();
            button.Match(
                Some: b => b.GetComponentInChildren<Text>().text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                None: () => Debug.LogFormat("The button '{0}' is not found", name));
        }
    }

    public class UIManagerActor : IActor{
        public Task ReceiveAsync(IContext context)
        {
//            Debug.Log(Thread.CurrentThread.ManagedThreadId + ": received from '" + context.Sender.ToString() +
//                                  "' the message: " + context.Message.ToString());
            var message = context.Message;
            switch (message)
            {
                case UIMessages.ButtonMessage button:
                    UnityEngine.Debug.LogFormat("button '{0}' is playing", button.name);
//                    UnityThread.executeInUpdate(() => changeLabelButton(button.name));
                    break;
                case UIMessages.SpeedInfo speed:
                    UnityEngine.Debug.LogFormat("speedInfo received with '{0}' as value", speed.speed);
//                    UnityThread.executeInUpdate(() => speedInfo.text = $"{speed.speed:n0} Msg/sec");
                    break;
                default:
//                    UnityEngine.Debug.LogErrorFormat("Unknown message {0}", message);
                    break;
            }
            return Actor.Done;
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

            public override string ToString()
            {
                return $"SpeedInfo({speed})";
            }
        }
    }
}