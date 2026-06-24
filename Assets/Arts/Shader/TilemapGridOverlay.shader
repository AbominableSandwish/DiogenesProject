Shader "Custom/TilemapGridOverlay"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _GridColor ("Grid Color", Color) = (0,0,0,1)
        _CellSize ("Cell Size", Float) = 1.0
        _LineThickness ("Line Thickness", Range(0.001, 0.2)) = 0.03
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "UniversalMaterialType"="Unlit"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "SpriteUnlit"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float3 worldPos    : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _GridColor;
                float _CellSize;
                float _LineThickness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);

                OUT.positionHCS = posInputs.positionCS;
                OUT.worldPos = posInputs.positionWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                // Position dans la grille monde
                float2 gridUV = IN.worldPos.xy / _CellSize;

                // Position locale dans la cellule : 0..1
                float2 local = frac(gridUV);

                // Distance au bord le plus proche
                float2 edgeDist = min(local, 1.0 - local);

                // Anti-aliasing
                float2 fw = fwidth(gridUV);

                // Ligne visible près des bords
                float lineX = 1.0 - smoothstep(_LineThickness, _LineThickness + fw.x, edgeDist.x);
                float lineY = 1.0 - smoothstep(_LineThickness, _LineThickness + fw.y, edgeDist.y);

                float gridMask = max(lineX, lineY);

                // On mélange seulement sur les bordures
                half4 finalColor = lerp(baseColor, _GridColor, saturate(gridMask) * _GridColor.a);

                return finalColor;
            }
            ENDHLSL
        }
    }
}