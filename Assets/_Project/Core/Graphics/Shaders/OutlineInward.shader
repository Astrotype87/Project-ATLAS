Shader "Sprite/OutlineInward"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Size (pixels)", Range(1,10)) = 2.0
        _AlphaCutoff ("Alpha cutoff", Range(0,1)) = 0.1
        _Selected ("Selected (0/1)", Float) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _AlphaCutoff;
            float _Selected;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            // Safe sampler: returns 0 alpha if UV is outside [0,1]
            inline float safeSampleAlpha(sampler2D tex, float2 uv)
            {
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return 0.0;
                return tex2D(tex, uv).a;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 center = tex2D(_MainTex, uv) * i.color;
                float centerA = center.a;

                // If not selected → draw normal sprite only
                if (_Selected < 0.5)
                    return center;

                int outlineSize = (int)_OutlineWidth;
                bool isEdge = false;

                if (centerA > _AlphaCutoff)
                {
                    [loop]
                    for (int x = -outlineSize; x <= outlineSize; x++)
                    {
                        [loop]
                        for (int y = -outlineSize; y <= outlineSize; y++)
                        {
                            if (x == 0 && y == 0) continue;

                            float2 offset = float2(x * _MainTex_TexelSize.x, y * _MainTex_TexelSize.y);
                            float neighborA = safeSampleAlpha(_MainTex, uv + offset);

                            if (neighborA < _AlphaCutoff)
                            {
                                isEdge = true;
                                break;
                            }
                        }
                        if (isEdge) break;
                    }
                }

                if (isEdge)
                {
                    // Blend outline over the original sprite pixel
                    float4 outc = _OutlineColor;
                    outc.a *= centerA; // scale outline alpha by sprite alpha

                    // Premultiplied alpha blend: result = sprite + outline * (1 - spriteAlpha)
                    float4 result;
                    result.rgb = lerp(center.rgb, outc.rgb, outc.a);
                    result.a = max(centerA, outc.a); // keep sprite visible
                    return result;
                }

                return center;
            }

            ENDCG
        }
    }
}
