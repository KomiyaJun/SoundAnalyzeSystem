Shader "Unlit/DissolveShader_Manual"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        // 時間の代わりに、0(表示)～1(消失) の値を直接指定する
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _OutlineThickness("OutlineThickness", float) = 0.05
        _OutlineStrength("OutlineStrength", float) = 1.0
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
            float _OutlineThickness;
            float _DissolveAmount; // ここで数値を受け取る

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col   = tex2D(_MainTex, i.uv);
                fixed4 noise = tex2D(_NoiseTex, i.uv);

                // ノイズの明るさが DissolveAmount より低い場所を消す
                float clipVal = noise.r - _DissolveAmount;

                // 消失処理 (step関数で 0 か 1 にする)
                // noise < amount の場合、描画しない(透明にする)
                if (clipVal < 0)
                {
                    // アルファを0にする、または discard で描画をスキップ
                    col.a = 0;
                    // discard; // 完全に消す場合はこちらでもOK
                }
                else
                {
                    // アウトライン処理
                    // 消える寸前の部分（clipValがThickness未満の場所）を発光させる
                    if (clipVal < _OutlineThickness)
                    {
                        col.rgb += _OutlineColor * _OutlineStrength;
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}