Shader "Hidden/Cortina/Blot"
{
    Properties
    {
        _MainTex("", 2D) = "black"
        _InputTex("", 2D) = "black"
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Assets/Common/Shaders/SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    sampler2D _InputTex;
    Buffer<float> _StateBuffer;

    half4 FragmentFeedback(v2f_img input) : SV_Target
    {
        const float aspect = _MainTex_TexelSize.x * _MainTex_TexelSize.w;

        float2 uv_n = input.uv * (4 + 4 * _StateBuffer[1]);
        float t_n = _StateBuffer[0];
        float amp_n = 0.005 + 0.2 * _StateBuffer[1];

        float2 uv = input.uv;
        uv.x += snoise(float3(uv_n, t_n - 11)) * amp_n * aspect;
        uv.y += snoise(float3(t_n + 13, uv_n)) * amp_n;

        half p0 = tex2D(_MainTex, uv).r * 0.96;
        half p1 = saturate(tex2D(_InputTex, input.uv).r * 2);

        return max(p0, p1);
    }

    half4 FragmentBlit(v2f_img input) : SV_Target
    {
        half4 src = tex2D(_MainTex, input.uv);
        half level = tex2D(_InputTex, input.uv).r;
        return half4(src.rgb + level, src.a);
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
