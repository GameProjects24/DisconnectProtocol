Shader "Custom/DeathScreenEffect"
{
    Properties
    {
        _EdgeWidth("Edge Width", Range(0,0.5)) = 0.2
        _GlitchIntensity("Glitch Intensity", Range(0,1)) = 0.5
        _GlitchSpeed("Glitch Speed", Range(0,5)) = 1.0
        _GlitchPulseSpeed("Glitch Pulse Speed", Range(0,10)) = 5.0
        _GlitchPulseAmplitude("Glitch Pulse Amplitude", Range(0,1)) = 0.1
        _NoiseTex("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _NoiseTex;
            float _EdgeWidth;
            float _GlitchIntensity;
            float _GlitchSpeed;
            float _GlitchPulseSpeed;
            float _GlitchPulseAmplitude;
            // Удалена строка: float4 _Time; // _Time уже встроенная переменная

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
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                // Вычисляем расстояние от центра по оси X (центр = 0.5)
                float distanceFromCenter = abs(uv.x - 0.5);
                // Маска: эффект усиливается от центра к краям
                float edgeMask = saturate((distanceFromCenter - (0.5 - _EdgeWidth)) / _EdgeWidth);
                
                // Динамическое смещение UV для эффекта глича:
                float2 glitchUV = uv * 10.0 + _Time.y * _GlitchSpeed;
                float noise = tex2D(_NoiseTex, glitchUV).r;
                
                // Пульсирующий порог: синусоидальное колебание добавляет "живости"
                float pulse = sin(_Time.y * _GlitchPulseSpeed) * _GlitchPulseAmplitude;
                float threshold = 1.0 - (_GlitchIntensity + pulse);
                
                // Если шум превышает порог, включаем глич (step возвращает 1, если noise >= threshold)
                float glitch = step(threshold, noise);
                
                // Черный фон, на который накладывается белый глич по краям
                fixed4 col = fixed4(0, 0, 0, 1);
                col.rgb += glitch * edgeMask;
                
                return col;
            }
            ENDCG
        }
    }
}
