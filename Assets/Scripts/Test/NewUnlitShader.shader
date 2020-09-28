/********************************************************************
 FileName: DepthDecal.shader
 Description: 根据深度的贴花效果
 history: 17:12:2018 by puppet_master
 https://blog.csdn.net/puppet_master
*********************************************************************/
Shader "Decal/DepthDecal"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

		SubShader
	{
		Tags {"Queue" = "Transparent+100"}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float3 ray : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D_float _CameraDepthTexture;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.ray = UnityObjectToViewPos(v.vertex) * float3(-1,-1,1);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//深度重建视空间坐标
				float2 screenuv = i.screenPos.xy / i.screenPos.w;
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenuv);
				float viewDepth = Linear01Depth(depth) * _ProjectionParams.z;
				float3 viewPos = i.ray * viewDepth / i.ray.z;
				//转化到世界空间坐标
				float4 worldPos = mul(unity_CameraToWorld, float4(viewPos, 1.0));
				//转化为物体空间坐标
				float3 objectPos = mul(unity_WorldToObject, worldPos);
				//剔除掉在立方体外面的内容
				clip(float3(0.5, 0.5, 0.5) - abs(objectPos));
				//使用物体空间坐标的xz坐标作为采样uv
				float2 uv = objectPos.xz + 0.5;
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}

	}
}