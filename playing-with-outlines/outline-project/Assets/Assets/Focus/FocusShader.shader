Shader "Unlit/FocusShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 8
        _Height ("Height", Float) = 8
        _CenterPoint ("Center Point", Vector) = (0, 0, 0)
        _FocusRadius ("Focus Radius", Float) = 0
        _UnfocusedAlpha ("Unfocused Alpha", Float) = 1
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThick ("Outline Thickness", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Width;
            float _Height;
            float2 _CenterPoint;
            float _FocusRadius;
            float4 _OutlineColor;
            float _OutlineThick;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 pos : TEXCOORD2;
                // float3 normal : TEXCOORD1;
            };

            float4 outline(float4 vertexPos, float outThick) 
            {
                float4x4 scale = float4x4
                (
                    1 + outThick, 0, 0, 0,
                    0, 1 + outThick, 0, 0,
                    0, 0, 1 + outThick, 0,
                    0, 0, 0, 1 + outThick
                );

                return mul(scale, vertexPos);
            }

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                float4 vertexPos = outline(v.vertex, _OutlineThick);
                o.vertex = UnityObjectToClipPos(vertexPos); // local space to clip space
                o.uv = 1 - 2 * v.uv0; // remaping uv coordinates so the center is (0, 0)
                o.pos = mul(unity_ObjectToWorld, float4(vertexPos.xyz, 1)); // local to world space position
                // o.normal = UnityObjectToWorldNormal(v.normals); // to world space normals
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                float2 dist = round((i.pos.xy - _CenterPoint) * float2(_Width, _Height));
                float distMagnitude = length(dist) + cos(_Time.w * 1.5) * clamp(_FocusRadius, 0, 1);
                
                float black = step(_FocusRadius, distMagnitude);

                _OutlineColor.w = 1 - black;
                
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Width;
            float _Height;
            float2 _CenterPoint;
            float _FocusRadius;
            float _UnfocusedAlpha;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 pos : TEXCOORD2;
                // float3 normal : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                o.uv = 1 - 2 * v.uv0; // remaping uv coordinates so the center is (0, 0)
                o.pos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)); // local to world space position
                // o.normal = UnityObjectToWorldNormal(v.normals); // to world space normals
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                float2 dist = round((i.pos.xy - _CenterPoint) * float2(_Width, _Height));
                float distMagnitude = length(dist) + cos(_Time.w * 1.5) * clamp(_FocusRadius, 0, 1);
                
                float black = step(_FocusRadius, distMagnitude);

                col.xyz = min(col.xyz, float3(black, black, black));
                col.w = step(black, _UnfocusedAlpha);
                
                return col;
            }
            ENDCG
        }
    }
}
