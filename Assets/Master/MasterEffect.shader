Shader "Hidden/Cortina/Master"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _Color("", Color) = (0, 0, 0, 0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3d.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half2 _Fade; // fade_width/2, 2/fade_width
    half4 _Margin;

    half3 _Color;
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

    half3 DynamicPalette(half p)
    {
        float3 x = p * float3(7.73, 9.31, 8.11);
        x += _Time.y * float3(2.31, 3.78, 2.54);
        return sin(x) * 0.5 + 0.5;
    }

    half Mask(float2 uv)
    {
        const half     aspect = _MainTex_TexelSize.x * _MainTex_TexelSize.w;
        const half inv_aspect = _MainTex_TexelSize.y * _MainTex_TexelSize.z;

        uv += (uv.xy < 0.5) ? -_Margin.xy : _Margin.zw;

        half2 pos = abs(uv - 0.5);
        pos.x = (pos.x + 0.5 * (aspect - 1)) * inv_aspect;
        pos = saturate(pos - 0.5 + _Fade.x);

        half l = saturate(1 - _Fade.y * length(pos));
        return GammaToLinearSpace(smoothstep(0, 1, l));
    }

    half4 FragmentColor(v2f_img i) : SV_Target
    {
        half4 src = tex2D(_MainTex, i.uv);
        return half4(_Color.rgb * src.rgb * Mask(i.uv), src.a);
    }

    half4 FragmentGradient(v2f_img i) : SV_Target
    {
        half src = tex2D(_MainTex, i.uv).r;
        half mask = smoothstep(0.05, 0.2, src) * Mask(i.uv);
        return half4(CosineGradient(src) * mask, src);
    }

    half4 FragmentDynamic(v2f_img i) : SV_Target
    {
        half src = tex2D(_MainTex, i.uv).r;
        half mask = smoothstep(0.05, 0.2, src) * Mask(i.uv);
        return half4(DynamicPalette(src) * mask, src);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentColor
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentGradient
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentDynamic
            ENDCG
        }
    }
}
