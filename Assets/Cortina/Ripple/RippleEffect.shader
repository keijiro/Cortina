Shader "Hidden/Cortina/Ripple"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half4 frag_master(v2f_img i) : SV_Target
    {
        float2 p = i.uv - 0.5;
        p.x *= _MainTex_TexelSize.y * _MainTex_TexelSize.z;
        float l = 1 - cos(saturate(length(p) * 5 - 2) * UNITY_PI * 2);

        p = i.uv - 0.6;
        p.x *= _MainTex_TexelSize.y * _MainTex_TexelSize.z;
        l += 1 - cos(saturate(length(p) * 5 - 1) * UNITY_PI * 2);

        p = i.uv - float2(0.3, 0.55);
        p.x *= _MainTex_TexelSize.y * _MainTex_TexelSize.z;
        l += 1 - cos(saturate(length(p) * 5 - 1.2) * UNITY_PI * 2);

        l += snoise(float3(p * 3, 1)) * 0.8;

        float d = length(half2(ddx_fine(l), ddy_fine(l)));

        d = pow(d * 10, 3);

        return half4((half3)d, 1);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_master
            #pragma target 5.0
            ENDCG
        }
    }
}
