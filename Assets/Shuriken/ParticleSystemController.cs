using UnityEngine;
using Klak.Sensel;

namespace Cortina
{
    public class ParticleSystemController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] float _throttle = 100;
        [SerializeField] float _extent = 5;
        [SerializeField] ParticleSystem _template;

        #endregion

        #region Private variables

        const int kMaxContacts = 5;
        Contact[] _contacts = new Contact[kMaxContacts];
        ParticleSystem[] _instances = new ParticleSystem[kMaxContacts];

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _instances[0] = _template;

            for (var i = 1; i < kMaxContacts; i++)
            {
                _instances[i] = Instantiate(_template);
                _instances[i].transform.parent = _template.transform.parent;
            }
        }

        void Update()
        {
            // Update the existing contacts.
            for (var i = 0; i < kMaxContacts; i++)
            {
                // If the contact is alive, try retrieving the latest state.
                if (_contacts[i].IsValid) _contacts[i] = TouchInput.GetContact(_contacts[i].ID);

                // Update the particle system instance.
                if (_contacts[i].IsValid) UpdateInstance(_instances[i], _contacts[i]);
                SwitchEmission(_instances[i], _contacts[i].IsValid);
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
                        // Start using this one. Don't enable the particle system
                        // at this point to avoid particle emission by jump.
                        _contacts[i2] = newEntries[i1];
                        UpdateInstance(_instances[i2], _contacts[i2]);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Private methods

        void UpdateInstance(ParticleSystem instance, Contact contact)
        {
            var input = new Vector2(contact.X, contact.Y);
            var aspect = new Vector2(1, 640.0f / 1920);
            instance.transform.position = (input * 2 - Vector2.one) * _extent * aspect;
        }

        void SwitchEmission(ParticleSystem instance, bool enable)
        {
            var emission = instance.emission;
            //emission.enabled = enable;
            emission.rateOverDistanceMultiplier = _throttle * (enable ? 1 : 0);
        }

        #endregion
    }
}
