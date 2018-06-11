using UnityEngine;
using Klak.Chromatics;

namespace Cortina
{
    [ExecuteInEditMode]
    public class MasterEffect : MonoBehaviour
    {
        [SerializeField, Range(0, 0.5f)] float _leftMargin;
        [SerializeField, Range(0, 0.5f)] float _rightMargin;
        [SerializeField, Range(0, 0.5f)] float _topMargin;
        [SerializeField, Range(0, 0.5f)] float _bottomMargin;

        [SerializeField] CosineGradient _gradient;

        [SerializeField, HideInInspector] Shader _shader;

        Material _material;

        void OnDestroy()
        {
            if (_material != null)
            {
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetVector("_Margin", new Vector4(
                _leftMargin, _bottomMargin, _rightMargin, _topMargin
            ));

            if (_gradient != null)
            {
                _material.SetVector("_CoeffsA", _gradient.coeffsA);
                _material.SetVector("_CoeffsB", _gradient.coeffsB);
                _material.SetVector("_CoeffsC", _gradient.coeffsC2);
                _material.SetVector("_CoeffsD", _gradient.coeffsD2);
            }

            Graphics.Blit(source, destination, _material, 0);
        }
    }
} 
