using UnityEngine;
using Klak.Chromatics;

namespace Cortina
{
    [ExecuteInEditMode]
    public class MasterEffect : MonoBehaviour
    {
        #region Editable attributes

        enum Mode { Color, Gradient, Dynamic }

        [Space]
        [SerializeField] Mode _mode;
        [SerializeField] Color _color;
        [SerializeField] CosineGradient _gradient;
        [Space]
        [SerializeField, Range(0.001f, 1)] float _fadeWidth = 0.1f;
        [SerializeField, Range(0, 1)] float _leftMargin;
        [SerializeField, Range(0, 1)] float _rightMargin;
        [SerializeField, Range(0, 1)] float _topMargin;
        [SerializeField, Range(0, 1)] float _bottomMargin;

        #endregion

        #region Editable attributes

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

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Lazy initialization
            if (_sheet == null)
            {
                _sheet = new Material(_shader);
                _sheet.hideFlags = HideFlags.DontSave;
            }

            // Update trimming parameters.
            var vfade = new Vector2(_fadeWidth / 2, 2 / _fadeWidth);
            _sheet.SetVector("_Fade", vfade);

            var vmargin = new Vector4(_leftMargin, _bottomMargin, _rightMargin, _topMargin);
            _sheet.SetVector("_Margin", vmargin / 2);

            if (_mode == Mode.Color)
            {
                _sheet.SetColor("_Color", _color);
                Graphics.Blit(source, destination, _sheet, 0);
            }
            else if (_mode == Mode.Gradient)
            {
                _sheet.SetVector("_CoeffsA", _gradient.coeffsA);
                _sheet.SetVector("_CoeffsB", _gradient.coeffsB);
                _sheet.SetVector("_CoeffsC", _gradient.coeffsC2);
                _sheet.SetVector("_CoeffsD", _gradient.coeffsD2);
                Graphics.Blit(source, destination, _sheet, 1);
            }
            else // _mode == Mode.Dynamic
            {
                Graphics.Blit(source, destination, _sheet, 2);
            }
        }

        #endregion
    }
} 
