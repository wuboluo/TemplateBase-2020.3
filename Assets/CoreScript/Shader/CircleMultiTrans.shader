Shader "eroAlucard/CiecleMultiTrans" {
    Properties {
	_MainTex("Sprite Texture", 2D) = "white" {}
        _Density_1 ("Density_1", Range(0, 10)) = 1
        _Radius_1 ("Radius_1", Range(0.01, 0.5)) = 0.5
        _PosX_1 ("PosX_1", Range(-5, 5)) = 0
        _PosY_1 ("PosY_1", Range(-5, 5)) = 0
        _Density_2 ("Density_2", Range(0, 10)) = 1
        _Radius_2 ("Radius_2", Range(0, 0.5)) = 0.5
        _PosX_2 ("PosX_2", Range(-5, 5)) = 0
        _PosY_2 ("PosY_2", Range(-5, 5)) = 0
        _Oapcity ("Oapcity", Range(0, 1)) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0
            uniform float _Radius_1;
            uniform float _PosX_1;
            uniform float _PosY_1;
            uniform float _PosX_2;
            uniform float _PosY_2;
            uniform float _Radius_2;
            uniform float _Density_1;
            uniform float _Density_2;
            uniform float _Oapcity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_8502 = (float2(_PosX_1,_PosY_1)+i.uv0);
                float node_5095 = (1.0 - (distance(node_8502,float2(0.5,0.5))/_Radius_1));
                float node_1141_if_leA = step(node_5095,0.0);
                float node_1141_if_leB = step(0.0,node_5095);
                float node_8104 = 1.0;
                float node_1460 = (node_5095*_Density_1);
                float node_2570 = (node_1460*node_1460);
                float node_1141 = lerp((node_1141_if_leA*node_8104)+(node_1141_if_leB*(1.0/pow(2.718,node_2570))),node_8104,node_1141_if_leA*node_1141_if_leB);
                float node_2704 = (1.0 - node_1141);
                float2 node_3534 = (float2(_PosX_2,_PosY_2)+i.uv0);
                float node_3818 = (1.0 - (distance(node_3534,float2(0.5,0.5))/_Radius_2));
                float node_21_if_leA = step(node_3818,0.0);
                float node_21_if_leB = step(0.0,node_3818);
                float node_2012 = 1.0;
                float node_8113 = (node_3818*_Density_2);
                float node_21 = lerp((node_21_if_leA*node_2012)+(node_21_if_leB*(1.0/pow(2.718,(node_8113*node_8113)))),node_2012,node_21_if_leA*node_21_if_leB);
                float node_3375 = (1.0 - node_21);
                float node_6008 = (node_2704*node_3375);
                float3 emissive = float3(node_6008,node_6008,node_6008);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,((node_1141*node_21)*_Oapcity));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
