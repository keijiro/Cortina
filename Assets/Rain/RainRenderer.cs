using UnityEngine;

namespace Cortina
{
    [ExecuteInEditMode]
    public sealed class RainRenderer : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] int _lineCount = 1000;

        [SerializeField] float _speed = 10;
        [SerializeField, Range(0, 1)] float _speedRandomness = 0.5f;

        [SerializeField] float _length = 1;
        [SerializeField, Range(0, 1)] float _lengthRandomness = 0.5f;

        [SerializeField] Vector3 _extent = Vector3.one * 10;
        [SerializeField, ColorUsage(false)] Color _color = Color.white;

        void OnValidate()
        {
            _lineCount = Mathf.Max(_lineCount, 0);
            _speed = Mathf.Max(_speed, 0);
            _length = Mathf.Max(_length, 0);
            _extent = Vector3.Max(_extent, Vector3.zero);
        }

        #endregion

        #region Public properties

        public int LineCount {
            get { return _lineCount; }
            set { _lineCount = Mathf.Max(value, 0); }
        }

        public float Length {
            get { return _length; }
            set { _length = Mathf.Max(value, 0); }
        }

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] Shader _shader;
        Material _sheet;

        #endregion

        #region MonoBehaviour implementation

        void OnDestroy()
        {
            if (_sheet != null)
            {
                if (Application.isPlaying)
                    Destroy(_sheet);
                else
                    DestroyImmediate(_sheet);
            }
        }

        void Update()
        {
            // Lazy initialization
            if (_sheet == null)
            {
                _sheet = new Material(_shader);
                _sheet.hideFlags = HideFlags.DontSave;
            }

            // Normalized speed parameter vector (min, max)
            var nspeed = new Vector2(1 - _speedRandomness, 1);
            nspeed *= _speed / (_extent.z * 2);

            // Line length parameter vector (min, max)
            var length = new Vector2(1 - _lengthRandomness, 1) * _length;

            // Local time. Pause while edit mode.
            var time = Application.isPlaying ? Time.time + 10 : 10;

            // Update the property sheet.
            _sheet.SetVector("_NSpeed", nspeed);
            _sheet.SetVector("_Length", length);
            _sheet.SetVector("_Extent", _extent);
            _sheet.SetColor("_Color", _color);
            _sheet.SetFloat("_LocalTime", time);
            _sheet.SetMatrix("_ObjectMatrix", transform.localToWorldMatrix);
        }

        void OnRenderObject()
        {
            if (_sheet == null) return;

            // Check the camera condition.
            var camera = Camera.current;
            if ((camera.cullingMask & (1 << gameObject.layer)) == 0) return;
            if (camera.name == "Preview Scene Camera") return;

            // Pure procedural draw
            _sheet.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Lines, _lineCount * 2, 1);
        }

        void OnDrawGizmos()
        {
            // Show the extent with a yellow box.
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, _extent * 2);
        }

        #endregion
    }
}
