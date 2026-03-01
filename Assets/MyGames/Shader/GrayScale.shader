Shader "Custom/GrayScale"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        AlphaToMask On

        Tags
        {
            "Queue" = "AlphaTest"
            "RenderType" = "AlphaTest"
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
                float2 uv : TEXCOORD;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD;
            };

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

           fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);

                color *= 0.333;
                fixed gray = color.r + color.g + color.b;

                return fixed4(gray, gray, gray, tex2D(_MainTex, i.uv).a);
            }
            ENDCG
        }
    }
}