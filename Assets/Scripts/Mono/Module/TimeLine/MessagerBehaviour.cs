using System;
using UnityEngine;
using UnityEngine.Playables;

namespace TaoTie
{
    [Serializable]
    public class MessagerBehaviour: PlayableBehaviour
    {
        public string Key;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Messager.Instance?.Broadcast(0, MessageId.ClipStartPlay, Key, playable.GetTime(),
                playable.GetDuration());
            base.OnBehaviourPlay(playable, info);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Messager.Instance?.Broadcast(0, MessageId.ClipProcess, Key, playable.GetTime(),
                playable.GetDuration());
            base.ProcessFrame(playable, info, playerData);
        }
    }

}