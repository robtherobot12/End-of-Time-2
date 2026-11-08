Shader "UI/IrisWipe"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Center ("Center (pixels)", Vector) = (0,0,0,0)
        _Radius ("Radius (pixels)", Float) = 2000
        _Softness ("Edge softness (pixels)", Float) = 8
        _RimColor ("Rim color", Color) = (1,0.8,0.4,1)
        _RimWidth ("Rim width (pixels)", Float) = 6
        _Wobble ("Wobble (pixels)", Float) = 6
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f { float4 pos : SV_POSITION; float4 screen : TEXCOORD0; };

            fixed4 _Color, _RimColor;
            float4 _Center;
            float _Radius, _Softness, _RimWidth, _Wobble;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screen = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 px = (i.screen.xy / i.screen.w) * _ScreenParams.xy;
                float2 d = px - _Center.xy;
                float dist = length(d);

                // wobbly edge: radius varies with angle + time
                float ang = atan2(d.y, d.x);
                float r = _Radius + sin(ang * 6 + _Time.y * 4) * _Wobble;

                float black = smoothstep(r, r + _Softness, dist);              // 0 inside hole, 1 outside
                float rim = 1 - smoothstep(0, _RimWidth, abs(dist - r));       // glowing ring at the edge
                rim *= saturate(_Radius / 20);                                  // fade rim when nearly closed

                fixed4 col = lerp(fixed4(0,0,0,0), _Color, black);
                col = lerp(col, _RimColor, rim * _RimColor.a);
                return col;
            }
            ENDCG
        }
    }
}
