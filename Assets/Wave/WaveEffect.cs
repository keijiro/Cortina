using UnityEngine;

namespace Cortina
{
    sealed class WaveEffect : MonoBehaviour
    {
        #region Public properties

        [SerializeField, Range(0, 0.1f)] float _distortion = 0.05f;

        public float Distortion {
            get { return _distortion; }
            set { _distortion = value; }
        }

        [SerializeField, Range(0, 1)] float _feedback = 1;

        public float Feedback {
            get { return _feedback; }
            set { _feedback = value; }
        }

        [SerializeField] bool _hold = true;

        public bool Hold {
            get { return _hold; }
            set { _hold = value; }
        }

        #endregion

        #region Built-in resources

        [SerializeField, HideInInspector] Shader _shader;
        Material _sheet;

        #endregion

        #region Internal objects

        RenderTexture _rt1;
        RenderTexture _rt2;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _sheet = new Material(_shader);
            _rt1 = new RenderTexture(1920, 640, 0, RenderTextureFormat.ARGBHalf);
            _rt2 = new RenderTexture(1920, 640, 0, RenderTextureFormat.ARGBHalf);
        }

        void OnDestroy()
        {
            Destroy(_sheet);
            Destroy(_rt1);
            Destroy(_rt2);
        }

        void Update()
        {
            // Feedback
            _sheet.SetFloat("_Distortion", _distortion);
            _sheet.SetFloat("_Feedback", _feedback);
            Graphics.Blit(_rt1, _rt2, _sheet, 0);

            // Double buffering
            var temp = _rt1;
            _rt1 = _rt2;
            _rt2 = temp;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_hold) Graphics.Blit(source, _rt1, _sheet, 0);
            Graphics.Blit(_rt1, destination, _sheet, 1);
        }

        #endregion
    }
}
