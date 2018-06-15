using UnityEngine;
using Klak.Sensel;

namespace Cortina
{
    public class SlideLineEffect : MonoBehaviour
    {
        enum Direction { Right, Left, Up, Down }

        [SerializeField] Color _color = Color.white;
        [SerializeField] float _width = 1;

        [SerializeField] Direction _direction = Direction.Left;
        [SerializeField] float _speed = 1;
        [SerializeField, Range(0, 1)] float _speedRandomness = 0.4f;

        [SerializeField, HideInInspector] Shader _shader;
        Material _sheet;

        const int kMaxLines = 12; // Also defined in .shader
        float[] _lineParams;
        int _lineCount;

        void Start()
        {
            _sheet = new Material(_shader);

            _lineParams = new float[kMaxLines];
            for (var i = 0; i < kMaxLines; i++) _lineParams[i] = -1e5f;
        }

        void OnDestroy()
        {
            Destroy(_sheet);
        }

        void OnValidate()
        {
            _width = Mathf.Max(0, _width);
            _speed = Mathf.Max(0, _speed);
        }

        void Update()
        {
            var contacts = Klak.Sensel.TouchInput.NewContacts;
            var time = Time.time;

            for (var i = 0; i < contacts.Length; i++)
            {
                _lineParams[_lineCount] = time;
                _lineCount = (_lineCount + 1) % kMaxLines;
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _sheet.SetColor("_Color", _color);
            _sheet.SetFloat("_Width", _width);
            _sheet.SetVector("_Speed", new Vector2(1 - _speedRandomness, 1) * _speed);
            _sheet.SetFloatArray("_LineParams", _lineParams);
            Graphics.Blit(source, destination, _sheet, (int)_direction);
        }
    }
} 
