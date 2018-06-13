Shader "Hidden/Cortina/Wave"
{
    Properties
    {
        _MainTex("", 2D) = "black"
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Assets/Common/Shaders/SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half _Distortion;
    half _Feedback;

    half4 FragmentFeedback(v2f_img input) : SV_Target
    {
        const float aspect = _MainTex_TexelSize.x * _MainTex_TexelSize.w;
        float3 np = float3(input.uv * float2(aspect, 1) * 5, _Time.y * 0.75);
        float2 nf = snoise_grad(np).xy * _Distortion;
        float2 delta = cross(float3(nf, 0), float3(0, 0, 1));
        float4 src = tex2D(_MainTex, input.uv + delta * float2(aspect, 1));
        return src * _Feedback;
    }

    half4 FragmentBlit(v2f_img input) : SV_Target
    {
        return tex2D(_MainTex, input.uv);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentFeedback
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentBlit
            ENDCG
        }
    }
}
