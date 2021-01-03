using UnityEngine.Events;
using DrakesGames.Tweening;

namespace HitTheStuff.Events.MyUnityEvents
{
    [System.Serializable]
    public class TweenSOUnityEvent : UnityEvent<TweenSO>
    {

    }

    [System.Serializable]
    public class TweenSOListUnityEvent : UnityEvent<TweenSO[]>
    {

    }

    [System.Serializable]
    public class TweenSequenceSOUnityEvent : UnityEvent<TweenSequenceSO>
    {

    }
}


