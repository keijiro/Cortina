using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] float _density = 0;

    void LateUpdate()
    {
        if (_density < 0.0001f)
        {
            RenderSettings.fog = false;
        }
        else
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = _density;
        }
    }
}
