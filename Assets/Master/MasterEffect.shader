Shader "Hidden/Cortina/Master"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _Margin("", Vector) = (0.1, 0.1, 0.1, 0.1)
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3d.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half4 _Margin;

    half3 _CoeffsA;
    half3 _CoeffsB;
    half3 _CoeffsC;
    half3 _CoeffsD;

    half3 CosineGradient(float p)
    {
        half3 rgb = saturate(_CoeffsA + _CoeffsB * cos(_CoeffsC * p + _CoeffsD));
        rgb = GammaToLinearSpace(rgb);
        return rgb;
    }

    half4 frag_master(v2f_img i) : SV_Target
    {
        half4 c = tex2D(_MainTex, i.uv);
        float2 p = i.uv - 0.5;
        p.x *= _MainTex_TexelSize.y * _MainTex_TexelSize.z;
        p *= 1.2;
//        c.rgb = CosineGradient(snoise(float3(p, 1))) * smoothstep(0, 0.001, Luminance(c.rgb));
        half4 pos = float4(i.uv.xy, 1 - i.uv.xy);
        half4 amp = smoothstep(0, _Margin, pos);
        c.rgb *= GammaToLinearSpace(min(min(min(amp.x, amp.y), amp.z), amp.w));
        return c;
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
            ENDCG
        }
    }
}
