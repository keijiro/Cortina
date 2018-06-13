using UnityEngine;
using UnityEngine.Playables;

namespace Cortina
{
    public sealed class SceneController : MonoBehaviour
    {
        [System.Serializable]
        struct Activator
        {
            public PlayableDirector Intro;
            public PlayableDirector Outro;
            public KeyCode Key;
        }

        [SerializeField] Activator[] _activators;

        int _mode = -1;

        void Update()
        {
            var input = GetActivatorKey();
            if (input < 0 || input == _mode) return;

            if (_mode >= 0)
            {
                var acOld = _activators[_mode];
                acOld.Intro.Stop();
                acOld.Outro.Play();
            }

            _mode = input;

            var acNew = _activators[_mode];
            acNew.Outro.Stop();
            acNew.Intro.Play();
        }

        int GetActivatorKey()
        {
            for (var i = 0; i < _activators.Length; i++)
                if (Input.GetKeyDown(_activators[i].Key)) return i;
            return -1;
        }
    }
}
