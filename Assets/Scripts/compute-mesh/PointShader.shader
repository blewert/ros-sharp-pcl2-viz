Shader "Custom/PointCloudShader"
{
	Properties
	{
		_PointSize("PointSize", Float) = 1
	}

		SubShader
	{
		Pass
		{
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct VertexInput
			{
				float4 v : POSITION;
				float4 color: COLOR;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float psize : PSIZE;
			};

			uniform float _PointSize;

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.v);
				o.psize = _PointSize;
				o.col = v.color;

				return o;
			}

			float4 frag(VertexOutput o) : COLOR
			{
				//Very dark? ok discard this pixel:
				if ((o.col.r + o.col.g + o.col.b) < 0.01)
					discard;

				return o.col;
			}

			ENDCG
		}
	}
}