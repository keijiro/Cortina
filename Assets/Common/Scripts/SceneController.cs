using UnityEngine;
using UnityEngine.Playables;

namespace Cortina
{
    public sealed class SceneController : MonoBehaviour
    {
        [System.Serializable]
        public struct Activator
        {
            public PlayableDirector Intro;
            public PlayableDirector Outro;
            public KeyCode Key;
        }

        [SerializeField] Activator[] _effectActivators;
        [SerializeField] Activator[] _optionActivators;

        int _currentEffect = -1;
        int _currentOption = -1;

        void Update()
        {
            _currentEffect = ProcessInputWithActivators(_effectActivators, _currentEffect);
            _currentOption = ProcessInputWithActivators(_optionActivators, _currentOption);
        }

        int ProcessInputWithActivators(Activator[] activators, int current)
        {
            var input = GetActivatorKey(activators);
            if (input < 0 || input == current) return current;

            if (current >= 0)
            {
                var prev = activators[current];
                if (prev.Intro != null) prev.Intro.Stop();
                if (prev.Outro != null) prev.Outro.Play();
            }

            var next = activators[input];
            if (next.Outro != null) next.Outro.Stop();
            if (next.Intro != null) next.Intro.Play();

            return input;
        }

        int GetActivatorKey(Activator[] activators)
        {
            for (var i = 0; i < activators.Length; i++)
                if (Input.GetKeyDown(activators[i].Key)) return i;
            return -1;
        }
    }
}
