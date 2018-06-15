Shader "Hidden/Cortina/Slide Line Effect"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _Color("", Color) = (0, 0, 0, 0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "Assets/Common/Shaders/Common.hlsl"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half3 _Color;
    half _Width;
    half2 _Speed;

    static const uint kMaxLines = 12; // Also defined in .cs
    float _LineParams[kMaxLines];

    half Line(float2 uv, float x, float w)
    {
        float t = _Time.y;
        half c = 0;

        w *= _Width;

        for (uint i = 0; i < kMaxLines; i++)
        {
            half spd = lerp(_Speed.x, _Speed.y, Random(i));
            half l = spd * (t - _LineParams[i]);
            c += 1 - step(w, abs(x - l));
        }

        half4 src = tex2D(_MainTex, uv);
        return half4(src.rgb + c, src.a);
    }

    half4 FragmentRight(v2f_img i) : SV_Target
    {
        return Line(i.uv, i.uv.x, _MainTex_TexelSize.x);
    }

    half4 FragmentLeft(v2f_img i) : SV_Target
    {
        return Line(i.uv, 1 - i.uv.x, _MainTex_TexelSize.x);
    }

    half4 FragmentUp(v2f_img i) : SV_Target
    {
        return Line(i.uv, i.uv.y, _MainTex_TexelSize.y);
    }

    half4 FragmentDown(v2f_img i) : SV_Target
    {
        return Line(i.uv, 1 - i.uv.y, _MainTex_TexelSize.y);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentRight
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentLeft
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentUp
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentDown
            ENDCG
        }
    }
}
