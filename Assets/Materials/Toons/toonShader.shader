Shader "Gnoming/toonShader"
{
    Properties
    {
        _Albedo("Albedo", Color) = (1,1,1,1)
        _Shades("Shades", Range(1,20)) = 3
        _InkColor("InkColor", Color) = (0,0,0,0)
        _InkSize("InkSize", float) = 1.0
        _Texture ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _InkColor;
            float _InkSize;

            v2f vert (appdata v)
            {
                v2f o;
                //Translate the vertex along the normal vector
                // Increased size of model -> outline
                o.vertex = UnityObjectToClipPos(v.vertex + (_InkSize * v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _InkColor;
            }
            ENDCG
        }


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worldNormal : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Albedo;
            float _Shades;
            sampler2D _Texture;
            float4 _Texture_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _Texture);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //Calc cosine between normal and light
                //world space light direction -> _WorldSpaceLightPos0
                //world space normal -> i.worldNormal

                float cosineAngle = dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                cosineAngle = max(cosineAngle, 0.2);

                cosineAngle = floor(cosineAngle * _Shades) / 15 ;
                fixed4 col = tex2D(_Texture, i.uv);
               
                return col * (cosineAngle);
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}
