Shader "Custom/GlitchEffect" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}

        // Размер сетки
        _CellsX ("Grid Columns", Range(1,1000)) = 20
        _CellsY ("Grid Rows", Range(1,1000)) = 10

        // Минимальная/максимальная длина полосы (в ячейках)
        _MinStripeLength ("Min Stripe Length (cells)", Range(1,500)) = 2
        _MaxStripeLength ("Max Stripe Length (cells)", Range(1,500)) = 10

        // Минимальный/максимальный зазор между полосами (в ячейках)
        _MinGap ("Min Gap (cells)", Range(0,500)) = 1
        _MaxGap ("Max Gap (cells)", Range(0,500)) = 5

        // Скорость смены паттерна
        _Speed ("Stripe Switching Speed", Float) = 5.0

        // Насколько «широкая» центральная мёртвая зона (0 = нет, 1 = вся ширина)
        _DeadZone ("Dead Zone Radius", Range(0,1)) = 0.3

        // Масштаб исходной текстуры внутри ячейки
        _TexScale ("Texture Scale", Range(0,1)) = 0.8

        // Три цвета для полос
        _Color1 ("Stripe Color 1", Color) = (1,0,0,1)
        _Color2 ("Stripe Color 2", Color) = (0,1,0,1)
        _Color3 ("Stripe Color 3", Color) = (0,0,1,1)

        // Вероятности появления каждого цвета
        _Color1Prob ("Probability of Color 1", Range(0,1)) = 0.33
        _Color2Prob ("Probability of Color 2", Range(0,1)) = 0.33
        _Color3Prob ("Probability of Color 3", Range(0,1)) = 0.34
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _CellsX;
            float _CellsY;
            float _MinStripeLength;
            float _MaxStripeLength;
            float _MinGap;
            float _MaxGap;
            float _Speed;
            float _DeadZone;
            float _TexScale;

            float4 _Color1;
            float4 _Color2;
            float4 _Color3;

            float _Color1Prob;
            float _Color2Prob;
            float _Color3Prob;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float rand(float2 co) {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;

                // Получаем индексы ячейки (целые координаты) и локальные UV внутри ячейки
                float2 grid = float2(_CellsX, _CellsY);
                float2 cellPos = floor(uv * grid);  // (x,y) индексы ячеек
                float2 cellUV  = frac (uv * grid);  // локальные координаты в [0..1)
                float rowIndex = cellPos.y;         // номер строки
                float cellX    = cellPos.x;         // номер ячейки по X

                // Используем время для смены паттерна
                float tseed = floor(_Time.y * _Speed);

                // Случайный сдвиг, чтобы «начало» паттерна могло уехать за левый край
                float rowOffset = rand(float2(rowIndex, tseed)) * (_MaxGap + _MaxStripeLength);
                float pos = -rowOffset;

                bool inStripe = false;

                // Максимум сегментов, чтобы не уйти в бесконечный цикл
                int maxSegments = 50;

                // Выбранный цвет полосы (0,1,2)
                int stripeColorIndex = 0;

                for (int seg = 0; seg < maxSegments; seg++) {
                    float fseg = seg;

                    // --- Генерируем зазор
                    float gapRand = rand(float2(rowIndex, fseg + tseed));
                    float gapLength = lerp(_MinGap, _MaxGap, gapRand);
                    pos += gapLength;

                    // Если зазор уже «обогнал» текущую ячейку, значит ячейка попала в зазор
                    if (pos > cellX + 0.001) {
                        break;
                    }

                    // --- Генерируем полосу
                    float stripeRand = rand(float2(rowIndex, fseg + 100.0 + tseed));
                    float stripeLength = lerp(_MinStripeLength, _MaxStripeLength, stripeRand);

                    // Определяем центр сегмента (примерно) по X
                    float centerXOfSegment = (pos + stripeLength * 0.5) / _CellsX; // 0..1
                    float distCenterSeg = abs(centerXOfSegment - 0.5) * 2.0;       // 0..1 (0 в центре, 1 у краёв)

                    // Если _DeadZone >= 1, то полосы вообще не появляются (p=0).
                    // Иначе рассчитываем вероятность появления полосы (p).
                    float p = 0.0;
                    if (_DeadZone < 1.0) {
                        p = saturate((distCenterSeg - _DeadZone) / (1.0 - _DeadZone));
                    }
                    // p=0 означает, что если мы ближе к центру, чем нужно,
                    // полоса не появится вообще.

                    // Подбрасываем случай для "разрешить/не разрешить" этот сегмент
                    float rAllow = rand(float2(rowIndex, fseg + 999.0 + tseed));
                    bool allowStripe = (rAllow < p);

                    if (allowStripe) {
                        // Проверяем, попадает ли текущая ячейка (cellX) в эту полосу
                        if (cellX < pos + stripeLength) {
                            inStripe = true;

                            // --- Выбираем цвет из трёх по заданным вероятностям
                            float sumProb = _Color1Prob + _Color2Prob + _Color3Prob;
                            // Генерируем случайное число в диапазоне [0..sumProb]
                            float rColor = rand(float2(rowIndex, fseg + 200.0 + tseed)) * sumProb;

                            if (rColor < _Color1Prob) {
                                stripeColorIndex = 0;
                            }
                            else if (rColor < _Color1Prob + _Color2Prob) {
                                stripeColorIndex = 1;
                            }
                            else {
                                stripeColorIndex = 2;
                            }

                            break;
                        }
                    }

                    // Если не попали в полосу, двигаемся дальше
                    pos += stripeLength;
                    if (pos >= _CellsX) {
                        break;
                    }
                }

                // Если ячейка не в полосе – делаем её прозрачной
                if (!inStripe) {
                    return float4(0,0,0,0);
                }

                // Если в полосе – берём цвет текстуры, масштабируем и накладываем цвет
                float2 scaledUV = (cellUV - 0.5) * _TexScale + 0.5;
                fixed4 texColor = tex2D(_MainTex, scaledUV);

                float4 stripeColor;
                if (stripeColorIndex == 0)
                    stripeColor = _Color1;
                else if (stripeColorIndex == 1)
                    stripeColor = _Color2;
                else
                    stripeColor = _Color3;

                fixed4 finalColor = texColor * stripeColor;
                // Если хотите всегда полностью непрозрачные полосы, можно принудительно:
                // finalColor.a = 1.0;
                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Transparent/Diffuse"
}
