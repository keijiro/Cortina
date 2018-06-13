using UnityEngine;
using Klak.Sensel;

namespace Cortina
{
    public class SwarmController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField, Range(0, 1)] float _throttle = 1;
        [SerializeField] float _lineWidth = 0.1f;
        [SerializeField] float _extent = 5;
        [SerializeField] Kvant.SwarmMV[] _targets;

        #endregion

        #region Private variables

        Contact[] _contacts;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _contacts = new Contact[_targets.Length];
        }

        void Update()
        {
            // Update the existing contacts.
            for (var i = 0; i < _contacts.Length; i++)
            {
                // If the contact is alive, try retrieving the latest state.
                if (_contacts[i].IsValid) _contacts[i] = TouchInput.GetContact(_contacts[i].ID);

                // Update the target swarm.
                UpdateTarget(_targets[i], _contacts[i]);
            }

            // Add new entries to the contact array.
            var newEntries = TouchInput.NewContacts;
            for (var i1 = 0; i1 < newEntries.Length; i1++)
            {
                // Find an unsed contact.
                for (var i2 = 0; i2 < _contacts.Length; i2++)
                {
                    if (!_contacts[i2].IsValid)
                    {
                        // Start using this one.
                        _contacts[i2] = newEntries[i1];
                        UpdateTarget(_targets[i2], _contacts[i2]);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Private methods

        void UpdateTarget(Kvant.SwarmMV target, Contact contact)
        {
            if (contact.IsValid)
            {
                var input = new Vector2(contact.X, contact.Y);
                var aspect = new Vector2(1, 640.0f / 1920);
                target.attractorPosition = (input * 2 - Vector2.one) * _extent * aspect;

                if (target.throttle == 0) target.Restart();
            }

            var delta = (contact.IsValid ? 1 : -1) * Time.deltaTime;
            var throttle = Mathf.Clamp01(target.throttle + delta) * _throttle;

            target.throttle = throttle;
            target.lineWidth = _lineWidth * throttle;
        }

        #endregion
    }
}
