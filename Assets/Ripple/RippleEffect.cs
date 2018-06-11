using UnityEngine;

namespace Cortina
{
    sealed class RippleEffect : MonoBehaviour
    {
        #region Public properties

        [SerializeField] float _speed = 1.3f;

        [SerializeField] bool _enableInput = true;

        public bool EnableInput {
            get { return _enableInput; }
            set { _enableInput = value; }
        }

        #endregion

        #region Built-in resources

        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Internal constants and variables

        const int kMaxRipples = 12; // Also defined in .shader

        Material _sheet;

        Vector4[] _ripples = new Vector4[kMaxRipples];
        int _rippleCount;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _sheet = new Material(_shader);

            _ripples = new Vector4[kMaxRipples];
            for (var i = 0; i < _ripples.Length; i++)
                _ripples[i] = new Vector4(0, 0, 0, -1e5f);
        }

        void OnDestroy()
        {
            Destroy(_sheet);
        }

        void Update()
        {
            if (!_enableInput) return;

            var contacts = Klak.Sensel.TouchInput.NewContacts;
            var time = Time.time;

            for (var i = 0; i < contacts.Length; i++)
            {
                var contact = contacts[i];
                _ripples[_rippleCount] = new Vector4(contact.X, contact.Y, contact.Force, time);
                _rippleCount = (_rippleCount + 1) % kMaxRipples;
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _sheet.SetFloat("_Speed", _speed);
            _sheet.SetVectorArray("_Ripples", _ripples);
            Graphics.Blit(source, destination, _sheet, 0);
        }

        #endregion
    }
} 
