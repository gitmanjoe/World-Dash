Shader "Hidden/SimpleBloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold ("Threshold", Float) = 0.8
        _Intensity ("Intensity", Float) = 1.2
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Name "Prefilter"
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_prefilter
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Threshold;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag_prefilter(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float lum = dot(col.rgb, float3(0.2126, 0.7152, 0.0722));
                if (lum > _Threshold) return col; else return fixed4(0,0,0,0);
            }
            ENDCG
        }

        Pass
        {
            Name "Blur"
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_blur
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float2 _BlurDirection; // set from script

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag_blur(v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy;
                float2 off = _BlurDirection * texel;

                fixed4 sum = tex2D(_MainTex, i.uv) * 0.3966;
                sum += tex2D(_MainTex, i.uv + off) * 0.2154;
                sum += tex2D(_MainTex, i.uv - off) * 0.2154;
                sum += tex2D(_MainTex, i.uv + off * 2.0) * 0.0625;
                sum += tex2D(_MainTex, i.uv - off * 2.0) * 0.0625;
                return sum;
            }
            ENDCG
        }

        Pass
        {
            Name "Composite"
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_composite
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BloomTex;
            float _Intensity;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag_composite(v2f i) : SV_Target
            {
                fixed4 src = tex2D(_MainTex, i.uv);
                fixed4 bloom = tex2D(_BloomTex, i.uv) * _Intensity;
                fixed4 result = src + bloom;
                result.rgb = result.rgb / (result.rgb + 1); // simple tonemap
                return fixed4(result.rgb, src.a);
            }
            ENDCG
        }
    }
}
