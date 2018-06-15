Shader "Hidden/Cortina/Slit Line Effect"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _Color("", Color) = (0, 0, 0, 0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Assets/Common/Shaders/SimplexNoise2d.hlsl"

    sampler2D _MainTex;
    half3 _Color;
    half _Threshold;

    half4 Fragment(v2f_img i) : SV_Target
    {
        float t = _Time.y * 1.5;
        float n =
            snoise(float2(i.uv.y * 2, t)) +
            snoise(float2(i.uv.y * 8, t)) / 2;

        half c = abs(n) < _Threshold;

        half4 src = tex2D(_MainTex, i.uv);
        return half4(src.rgb + c, src.a);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment Fragment
            ENDCG
        }
    }
}
