using UnityEngine;
using Klak.Sensel;

namespace Cortina
{
    public class SlitLineEffect : MonoBehaviour
    {
        [SerializeField] Color _color = Color.white;

        [SerializeField, HideInInspector] Shader _shader;
        Material _sheet;

        void Start()
        {
            _sheet = new Material(_shader);
        }

        void OnDestroy()
        {
            Destroy(_sheet);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var input = Mathf.Clamp01(TouchInput.Contact.Force * 10);

            _sheet.SetColor("_Color", _color);
            _sheet.SetFloat("_Threshold", input * 0.2f);

            Graphics.Blit(source, destination, _sheet, 0);
        }
    }
} 
