Shader "Unlit/DissolveShader" 
{
    Properties
    {
        _StartTime("StartTime", Float) = 0.0
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _Speed("Speed", float) = 0.0
        _OutlineThickness("OutlineThickness", float) = 0.0
        _OutlineStrength("OutlineStrength", float) = 0.0
        _OutlineColor("OutLine", Color) = (1,1,1,1)
     }
     
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            float _OutlineStrength;
            float _Speed;
            float _OutlineThickness;
            float _StartTime;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float currentTime = _Time.y;
                float passedTime  = currentTime - _StartTime;
                fixed4 col   = tex2D(_MainTex, i.uv);
                fixed4 noise = tex2D(_NoiseTex, i.uv);
                col.a   *= step(passedTime/80 * _Speed, noise);
                col.rgb += step(noise,(passedTime/80+_OutlineThickness) * _Speed) 
                         * _OutlineColor *_OutlineStrength;
                return col;
            }
            ENDCG
        }
    }
} 
