Shader "Custom/CurvedX"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Curvature ("Curvature Amount", Float) = 0.001
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float _Curvature;
            float4 _BaseColor;

            float3 unity_CameraWorldPos;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                float xDist = worldPos.x - unity_CameraWorldPos.x;
                float yOffset = pow(xDist, 2) * _Curvature;

                worldPos.y -= yOffset;

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}
