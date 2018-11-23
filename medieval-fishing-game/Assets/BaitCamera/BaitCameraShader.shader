Shader "Hidden/BaitCameraShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FadeValue ("Fade Value", Range (0, 1)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _FadeValue;
			sampler2D BaitCameraTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				float v1 = smoothstep(0, 0.5, _FadeValue);
				float v2 = smoothstep(0.5, 1, _FadeValue);
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(BaitCameraTexture, i.uv - fixed2(0.15, 0));
				float w = smoothstep(1 - v1 * 0.6, 1 - v2 *0.4, i.uv.x);
				fixed4 finalColor = lerp(col, col2, w);
				return finalColor;
			}
			ENDCG
		}
	}
}
