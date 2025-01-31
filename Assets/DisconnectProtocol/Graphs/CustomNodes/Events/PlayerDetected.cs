using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/PlayerDetected")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "PlayerDetected", message: "Agent has detected Player", category: "DisconnectProtocol/Events", id: "f3cbf92855d55b0337e3c8d921904335")]
public partial class PlayerDetected : EventChannelBase
{
    public delegate void PlayerDetectedEventHandler();
    public event PlayerDetectedEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        PlayerDetectedEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as PlayerDetectedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as PlayerDetectedEventHandler;
    }
}

