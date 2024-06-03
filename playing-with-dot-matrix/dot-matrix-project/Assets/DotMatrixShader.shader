Shader "Unlit/DotMatrixShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CircularDots ("Circular Dots", Float) = 0
        _Brightness ("Brightness", Range(0.1, 5.0)) = 2
        _Contrast ("Contrast", Range(0.1, 5.0)) = 2
        _Luminance ("Luminance", Vector) = (0.3086, 0.6094, 0.0820)
        _CellSpace ("Cell space", Range(0.1, 100.0)) = 8
        _CellContentInterpolator ("Cell content interpolator", Range(0.1, 100.0)) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.141592 

            sampler2D _MainTex;
            //float4 _MainTex_TexelSize;
            float _CircularDots;
            float3 _Luminance;
            float _Brightness;
            float _Contrast;
            float _CellSpace;
            float _CellContentInterpolator;

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normals : NORMAL;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 pos : TEXCOORD2;
                //float3 normal : TEXCOORD1;
                //float3 viewNormal : TEXCOORD3;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv0;
                o.pos = mul(unity_ObjectToWorld, v.vertex).xyz; // local to world space position
                //o.normal = UnityObjectToWorldNormal(v.normals); // to world space normals

                //float3 viewNorm = mul((float3x3)UNITY_MATRIX_V, o.normal);
 
                // get view space position of vertex
                //float3 viewPos = UnityObjectToViewPos(v.vertex);
                //float3 viewDir = normalize(viewPos);
 
                //o.viewNormal = viewNorm + 0.5;
           
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float gray = dot( tex2D(_MainTex, i.uv).xyz, _Luminance ) * _Contrast - 1;
                gray *= _Brightness;

                if ( _CircularDots > 0)
                {
                    float2 p = ( floor(i.uv * _ScreenParams.xy / _CellSpace) + 0.5 ) * _CellSpace;
                    float r = length( i.uv * _ScreenParams.xy - p) / _CellContentInterpolator;

                    // the next two lines make the dots move with the camera. its cool but its nauseating.
                    //float2 p = ( floor(i.vertex / _CellSpace) + 0.5 ) * _CellSpace;
                    //float r = length( i.vertex - p) / _CellContentInterpolator;
                    
                    return float4(gray, gray, gray, 1) * clamp(1 - r * r, 0, 1);
                }

                const float sinPer = PI / _CellSpace;
                const float frac = _CellContentInterpolator / _CellSpace;

                float2 stripes = abs( sin(sinPer * i.uv * _ScreenParams.xy) ) - frac; // vertical and horizontal lines to make the grid
                
                // same nauseating thing.
                //float2 stripes = abs( sin(sinPer * i.vertex) ) - frac;
                
                return float4(gray, gray, gray, 1) * stripes.x * stripes.y; 
            }
            ENDCG
        }
    }
}