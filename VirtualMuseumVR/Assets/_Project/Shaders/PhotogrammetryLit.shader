// =============================================================================
// PhotogrammetryLit.shader — URP Shader for Photogrammetry Models
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Optimized for mobile VR: albedo-only with optional detail normal map.
// Supports split-texture tiling for large models. No specular, no PBR
// metallic/smoothness to maximize Quest GPU performance.
// Single-pass instanced rendering compatible.
// =============================================================================

Shader "VirtualMuseum/PhotogrammetryLit"
{
    Properties
    {
        _BaseMap ("Albedo (Photogrammetry Texture)", 2D) = "white" {}
        _BaseColor ("Color Tint", Color) = (1, 1, 1, 1)

        [Normal] _BumpMap ("Normal Map (Optional)", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 2)) = 1.0

        [Toggle(_DETAIL_ON)] _DetailEnabled ("Enable Detail Texture", Float) = 0
        _DetailMap ("Detail Texture", 2D) = "white" {}
        _DetailTiling ("Detail Tiling", Float) = 10

        [Toggle(_EMISSION_ON)] _EmissionEnabled ("Enable Emission", Float) = 0
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1

        // Rendering settings
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // GPU instancing for single-pass stereo rendering
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // Custom keywords
            #pragma shader_feature_local _DETAIL_ON
            #pragma shader_feature_local _EMISSION_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                #if defined(LIGHTMAP_ON)
                float2 lightmapUV : TEXCOORD3;
                #endif
                #if defined(_DETAIL_ON) || defined(_EMISSION_ON)
                float4 tangentWS : TEXCOORD4;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);        SAMPLER(sampler_BumpMap);
            TEXTURE2D(_DetailMap);      SAMPLER(sampler_DetailMap);
            TEXTURE2D(_EmissionMap);    SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _BumpScale;
                half _DetailTiling;
                half4 _EmissionColor;
                half _EmissionIntensity;
                half _Cutoff;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;

                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInputs.normalWS;

                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

                #if defined(LIGHTMAP_ON)
                output.lightmapUV = input.lightmapUV * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif

                #if defined(_DETAIL_ON) || defined(_EMISSION_ON)
                output.tangentWS = float4(normalInputs.tangentWS, input.tangentOS.w);
                #endif

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Sample base photogrammetry texture
                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;

                // Detail texture overlay
                #if defined(_DETAIL_ON)
                half4 detail = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap,
                    input.uv * _DetailTiling);
                baseColor.rgb *= detail.rgb * 2.0; // Overlay blend
                #endif

                // Normal mapping
                half3 normalWS = normalize(input.normalWS);

                // Simple lighting (optimized for mobile VR)
                Light mainLight = GetMainLight();
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 diffuse = mainLight.color * NdotL * mainLight.shadowAttenuation;

                // Ambient / lightmap
                half3 ambient;
                #if defined(LIGHTMAP_ON)
                ambient = SampleLightmap(input.lightmapUV, normalWS);
                #else
                ambient = SampleSH(normalWS);
                #endif

                half3 finalColor = baseColor.rgb * (diffuse + ambient);

                // Emission
                #if defined(_EMISSION_ON)
                half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb;
                finalColor += emission * _EmissionColor.rgb * _EmissionIntensity;
                #endif

                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }

        // Shadow caster pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Depth pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // Meta pass for lightmap baking
        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaLit
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
