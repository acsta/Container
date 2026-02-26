using System;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace TaoTie
{

    [Serializable]
    public class MessagerClip : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public string Key;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<MessagerBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Key = Key;
            return playable;
        }
        
    }
}