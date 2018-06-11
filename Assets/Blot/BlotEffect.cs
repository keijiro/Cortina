using UnityEngine;

namespace Cortina
{
    sealed class BlotEffect : MonoBehaviour
    {
        #region Public properties

        [SerializeField] bool _enableInput = true;

        public bool EnableInput {
            get { return _enableInput; }
            set { _enableInput = value; }
        }

        #endregion

        #region Built-in resources

        [SerializeField, HideInInspector] Shader _shader;
        [SerializeField, HideInInspector] ComputeShader _compute;

        #endregion

        #region Internal objects

        Klak.Sensel.ForceMap _forceMap;
        Material _sheet;
        ComputeBuffer _state;
        RenderTexture _rt1;
        RenderTexture _rt2;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _forceMap = new Klak.Sensel.ForceMap();
            _sheet = new Material(_shader);
            _state = new ComputeBuffer(2, sizeof(float));
            _rt1 = new RenderTexture(1920, 640, 0, RenderTextureFormat.RHalf);
            _rt2 = new RenderTexture(1920, 640, 0, RenderTextureFormat.RHalf);
        }

        void OnDestroy()
        {
            if (_forceMap != null) _forceMap.Dispose();
            Destroy(_sheet);
            if (_state != null) _state.Dispose();
            Destroy(_rt1);
            Destroy(_rt2);
        }

        void Update()
        {
            if (_forceMap != null) _forceMap.Update();

            // State update
            var kernel = _enableInput ? 0 : 1;
            _compute.SetFloat("DeltaTime", Time.deltaTime);
            _compute.SetTexture(kernel, "TotalTexture", _forceMap.TotalInputTexture);
            _compute.SetBuffer(kernel, "StateBuffer", _state);
            _compute.Dispatch(kernel, 1, 1, 1);

            // Feedback
            _sheet.SetTexture("_InputTex", _enableInput ? _forceMap.FilteredInputTexture : null);
            _sheet.SetBuffer("_StateBuffer", _state);
            Graphics.Blit(_rt1, _rt2, _sheet, 0);

            // Double buffering
            var temp = _rt1;
            _rt1 = _rt2;
            _rt2 = temp;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _sheet.SetTexture("_InputTex", _rt2);
            Graphics.Blit(source, destination, _sheet, 1);
        }

        #endregion
    }
}
