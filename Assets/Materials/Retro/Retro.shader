Shader "Gnoming/Retro"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _MainTexRGB ("Base (RGB)", 2D) = "white" {}	
        _MainShadowMap ("ShadowMap", 2D) = "white" {}	

        _LitTex ("Light Hatch", 2D) = "white" {}
        _MedTex ("Medium Hatch", 2D) = "white" {}
        _HvyTex ("Heavy Hatch", 2D) = "white" {}
        _Repeat ("Repeat Tile", float) = 4

        _OutlineWidth("OutlineWidth", float) = 1.0

        [Toggle(_retro_toggle)] _retro_toggle ("_retro_toggle", Float) = 0.0
        [Toggle(_comic_toggle)] _comic_toggle ("_comic_toggle", Float) = 0.0
        [Toggle(_draw_toggle)] _draw_toggle ("_draw_toggle", Float) = 0.0

        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300
 
        Pass {
            CGPROGRAM
			#pragma vertex vert_img;
			#pragma fragment frag;            
            
            #pragma multi_compile _retro_toggle _comic_toggle _draw_toggle _
            
			#include "UnityCG.cginc" 
 
			uniform sampler2D _MainTexRGB;  
            uniform sampler2D _MainShadowMap;

			uniform float4 _Color; 
            fixed _Repeat;
       
            #if defined(_retro_toggle)
			    float4 frag(v2f_img i) : COLOR {
                    float4 c = tex2D(_MainTexRGB, i.uv);
                
                    float lum = c.r*.31 + c.g*.3 + c.b*.45;
				    float4 output_color = float4(0.5,0.5,0.5,1);

                    //twarz
                    if(lum < 0.8) {
    					output_color = float4(0.7,0.5,0.6,1);
				    }

                    //bok dolnych kamieni - rzulty
                    if(lum < 0.7) {
    					output_color = float4(0.7,0.5,0.1,1);
	    			}

                    //drugi 
                    if(lum < 0.65) {
    					output_color = float4(0.3,0.3,0,1);
				    }

                    if(lum < 0.62) {
    					output_color = float4(1,0.5,0.5,1);
				    }

                    if(lum < 0.5) {
				    	output_color = float4(0.4,0.4,1,1);
				    }

                    if(lum < 0.4) {
    					output_color = float4(1,0.4,0.4,1);
				    }
                
                    //gora lisci i dol gory
                    if(lum < 0.3) {
    					output_color = float4(0.8,0.5,0.4,1);
				    }

                    //dol lisci
                    if(lum < 0.2) {
    					output_color = float4(0,1,0,1);
	    			}

                    if(lum < 0.1) {
    					output_color = float4(1,1,1,1);
				    }

				    float x = i.pos.x;
				    float y = i.pos.y;
				    float block_size = 7;
				    float rad = (block_size) / 2;
				
				    float cx = (x - (x % block_size)) + rad;
				    float cy = (y - (y % block_size)) + rad;
				    float dx = x - cx;
				    float dy = y - cy;
				    float r = sqrt((dx*dx) + (dy*dy));
				    float cr = (block_size * lum) / 1.2f;

                    if(r > cr) {					
    					output_color = float4(0.01, 0.05, 0.01,1) ; // black
	    			}
                    
                    output_color.rgb = output_color.rgb*_Color.rgb;
		    		return output_color * (0.3/c);   
			}

            #else
            #if defined(_comic_toggle)

                float4 frag(v2f_img i) : COLOR {
                    float4 c = tex2D(_MainTexRGB, i.uv);
                
                
                    float lum = c.r*.31 + c.g*.3 + c.b*.45;
				    float4 output_color = float4(0.9,0.9,0.9,1);

				    float x = i.pos.x;
				    float y = i.pos.y;
				    float block_size = 3;
				    float rad = (block_size) / 2;
				
				    float cx = (x - (x % block_size)) + rad;
				    float cy = (y - (y % block_size)) + rad;

			
				    float dx = x - cx;
				    float dy = y - cy;
				    float r = sqrt((dx*dx) + (dy*dy));
				    float cr = (block_size * lum);
				
                    float r1 = sqrt((dx*dx) + (dy*dy) + 2);
				    float cr1 = (block_size * lum);

                    if(r1-1.5 > cr*2.5) {					
    					output_color = float4(0.9, 0.9, 0.9,1); 
	    			}

                    if(r+1 > cr) {					
    					output_color = float4(0.5, 0.5, 0.5,1);
	    			}

                    if(r1-1 > cr*2) {					
    					output_color = float4(0.05, 0.05, 0.05,1);
	    			}

                    if(r > cr1+3) {					
    					output_color = float4(0.5, 0.5, 0.5,1);
	    			}

                    output_color = output_color * c;
                    
                    output_color.rgb = output_color.rgb*_Color.rgb;


		    		return output_color;  
                }
            #else
            #if defined(_draw_toggle)
            
                sampler2D _MainTex;
                sampler2D _LitTex;
                sampler2D _MedTex;
                sampler2D _HvyTex;
                
                half4 frag(v2f_img i) : COLOR 
                {
                    float4 texColor = tex2D(_MainTexRGB, i.uv);
                    texColor = (texColor.r + texColor.g + texColor.b)/3;
                    
                    half4 cLit = tex2D(_LitTex, i.uv * _Repeat);
                    half4 cMed = tex2D(_MedTex, i.uv * _Repeat);
                    half4 cHvy = tex2D(_HvyTex, i.uv * _Repeat);
                    half4 c;
                      
                    c.rgb = lerp(cHvy, cMed, 0.5f);
                    c.rgb = lerp(c.rgb, cLit, 0.5f);
                    
                    c.rgb = c.rgb + texColor; 


                    c.rgb = c.rgb*_Color.rgb;
                    return c;
                }

            #else

                float4 frag(v2f_img i) : COLOR {
                    float4 c = tex2D(_MainTexRGB, i.uv);
                    c.rgb = c.rgb*_Color.rgb;
                    return c/1.75f;
                }
            #endif
            #endif
            #endif
			ENDCG
        }
        

        Pass
        {
            Cull Front
            Blend [_SrcBlend] [_DstBlend]
            
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

            half _OutlineWidth;
            half4 _outlineColor; 
             
           float4 vert
		   (
                float4 position : POSITION,
                float3 normal : NORMAL) : SV_POSITION {

                float4 clipPosition = UnityObjectToClipPos(position);
                float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, normal));

                float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * clipPosition.w *6;
                clipPosition.xy += offset;

                return clipPosition;
                }

            half4 frag() : SV_TARGET {
                return _outlineColor;
            }

            ENDCG
        }
    }
    FallBack "VertexLit"
}