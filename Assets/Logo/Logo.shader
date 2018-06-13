Shader "Logo"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _RedChannelColor("Red Channel", Color) = (1, 0, 0, 0)
        [Gamma] _Opacity("Opacity", Range(0, 1)) = 1
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    half4 _RedChannelColor;
    half _Opacity;

    void Vertex(
        float4 position : POSITION,
        out float4 sv_position : SV_Position,
        inout float2 uv : TEXCOORD0
    )
    {
        sv_position = UnityObjectToClipPos(position);
    }

    half4 Fragment(
        float4 sv_position : SV_Position,
        float2 uv : TEXCOORD0
    ) : SV_Target
    {
        half4 src = tex2D(_MainTex, uv);
        return (half4(0, src.gba) + src.r * _RedChannelColor) * _Opacity;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend One One
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
