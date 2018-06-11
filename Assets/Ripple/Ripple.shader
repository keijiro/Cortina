Shader "Hidden/Cortina/Ripple"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Assets/Common/Shaders/SimplexNoise3D.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half _Speed;

    static const uint kMaxRipples = 12; // Also defined in .cs
    float4 _Ripples[kMaxRipples];

    half2 FixAspect(half2 uv)
    {
        uv.x *= _MainTex_TexelSize.y * _MainTex_TexelSize.z;
        return uv;
    }

    half4 Fragment(v2f_img input) : SV_Target
    {
        const half2 uv = input.uv;
        const half aspect = _MainTex_TexelSize.y * _MainTex_TexelSize.z;

        half level = 0;

        for (uint i = 0; i < kMaxRipples; i++)
        {
            float4 ripple = _Ripples[i];

            half2 pos = FixAspect(input.uv - ripple.xy);
            half force = saturate(0.02 + ripple.z * 2);

            float time = _Time.y - ripple.w;
            float phase = length(pos) - time * _Speed;
            
            float amp = saturate(1 + phase * 0.2 / force);
            amp *= force * (phase < 0);

            level += (1 - cos(phase * 4 / force)) * amp;
        }

        half nf = snoise(float3(FixAspect(uv) * 1.8, _Time.y * 2));
        level += level * nf * 0.8;

        half4 src = tex2D(_MainTex, input.uv);
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
            #pragma fragment Fragment
            ENDCG
        }
    }
}
