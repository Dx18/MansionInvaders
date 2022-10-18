// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SpriteLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	_LightingTex ("Lighting texture", 2D) = "white" {}
    }
    SubShader
    {
	Pass {
	    CGPROGRAM
	    #pragma vertex vert
	    #pragma fragment frag

	    struct vertex_shader_input {
		float4 vertex : POSITION;
		float2 uv_main : TEXCOORD0;
		float2 uv_lighting : TEXCOORD1;
	    };

	    struct fragment_shader_input {
		float4 vertex : SV_POSITION;
		float2 uv_main : TEXCOORD0;
		float2 uv_lighting : TEXCOORD1;
	    };

	    sampler2D _MainTex;
	    sampler2D _LightingTex;

	    fragment_shader_input vert(vertex_shader_input input) {
		fragment_shader_input output;

		output.vertex = UnityObjectToClipPos(input.vertex);
		output.uv_main = input.uv_main;
		output.uv_lighting = input.uv_lighting;

		return output;
	    }

	    float4 frag(fragment_shader_input input) : SV_Target {
		float4 color_main = tex2D(_MainTex, input.uv_main);
		float4 color_lighting = tex2D(_LightingTex, input.uv_lighting);
		float4 result = color_main + color_lighting - 0.5;
		return result;
	    }

	    ENDCG
	}
    }
}
