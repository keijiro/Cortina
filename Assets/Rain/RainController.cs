using UnityEngine;
using Klak.Sensel;

namespace Cortina
{
    sealed class RainController : MonoBehaviour
    {
        #region Private variables

        RainRenderer _renderer;

        int _originalCount;
        float _originalLength;

        Contact _contact1;
        Contact _contact2;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _renderer = GetComponent<RainRenderer>();

            // Retrieve the component original settings.
            _originalCount = _renderer.LineCount;
            _originalLength = _renderer.Length;
        }

        void Update()
        {
            // Update the first contact point.
            if (_contact1.IsValid)
                _contact1 = TouchInput.GetContact(_contact1.ID);
            else
                _contact1 = TouchInput.GetContactExclude(_contact2.ID);

            // Update the second contact point.
            if (_contact2.IsValid)
                _contact2 = TouchInput.GetContact(_contact2.ID);
            else
                _contact2 = TouchInput.GetContactExclude(_contact1.ID);

            // Dual touch mode?
            var dual = _contact1.IsValid && _contact2.IsValid;

            // Calculate the centroid of the input points.
            var input = Vector2.Lerp(
                new Vector2(_contact1.X, _contact1.Y),
                new Vector2(_contact2.X, _contact2.Y),
                dual ? 0.5f : (_contact1.IsValid ? 0 : 1)
            );

            // Calculate the angle of the input points.
            var angle = dual ? Mathf.Atan2(_contact2.Y - _contact1.Y, _contact2.X - _contact1.X) : 0;

            // The total sum of the input force.
            var force = (_contact1.Force + _contact2.Force) * (dual ? 1 : 2);

            // Apply the input as Euler angles.
            transform.rotation = Quaternion.Euler(new Vector3(
                90 - input.y * 180, input.x * 180 - 90, angle * Mathf.Rad2Deg
            ));

            // Apply the input to the renderer properties.
            _renderer.LineCount = (int)(_originalCount * Mathf.Clamp01(force * 10));
            _renderer.Length = _originalLength * (1 + 1 * Mathf.Clamp01(force * 10 - 2));
        }

        #endregion
    }
}
