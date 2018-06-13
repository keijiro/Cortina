using UnityEngine;
using UnityEngine.Playables;

namespace Cortina
{
    class SceneController : MonoBehaviour
    {
        [System.Serializable]
        struct Activator
        {
            public PlayableDirector Intro;
            public PlayableDirector Outro;
            public KeyCode Key;
        }

        [SerializeField] Activator[] _activators;

        bool[] _flags;

        void Start()
        {
            _flags = new bool[_activators.Length];
        }

        void Update()
        {
            for (var i = 0; i < _activators.Length; i++)
            {
                var ac = _activators[i];
                if (Input.GetKeyDown(ac.Key))
                {
                    _flags[i] = !_flags[i];
                    (_flags[i] ? ac.Outro : ac.Intro).Stop();
                    (_flags[i] ? ac.Intro : ac.Outro).Play();
                }
            }
        }
    }
}
