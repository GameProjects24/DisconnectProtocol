using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AgentStateChanged")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AgentStateChanged", message: "Agent changed state to [state]", category: "DisconnectProtocol/Events", id: "ac57f5e048724a8c4992f87979bf90d9")]
public partial class AgentStateChanged : EventChannelBase
{
    public delegate void AgentStateChangedEventHandler(AgentState state);
    public event AgentStateChangedEventHandler Event; 

    public void SendEventMessage(AgentState state)
    {
        Event?.Invoke(state);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<AgentState> stateBlackboardVariable = messageData[0] as BlackboardVariable<AgentState>;
        var state = stateBlackboardVariable != null ? stateBlackboardVariable.Value : default(AgentState);

        Event?.Invoke(state);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AgentStateChangedEventHandler del = (state) =>
        {
            BlackboardVariable<AgentState> var0 = vars[0] as BlackboardVariable<AgentState>;
            if(var0 != null)
                var0.Value = state;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AgentStateChangedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AgentStateChangedEventHandler;
    }
}

