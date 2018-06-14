using UnityEngine;
using Klak.Sensel;

namespace Cortina
{
    public class LightController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] float _intensity = 1;
        [SerializeField] float _extent = 5;
        [SerializeField] Light[] _targets;

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

                // Update the target spray.
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

        void UpdateTarget(Light target, Contact contact)
        {
            var input = new Vector2(contact.X, contact.Y);
            var aspect = new Vector2(1, 640.0f / 1920);
            target.transform.localPosition = (input * 2 - Vector2.one) * _extent * aspect;
            target.intensity = Mathf.Clamp01(contact.Force * 10) * _intensity;
        }

        #endregion
    }
}
