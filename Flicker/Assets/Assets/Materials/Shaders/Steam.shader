Shader "Flicker/Particles/Steam" {
Properties {
	_DiffColor ("Diffuse Color", Color) = (.2,.2,.2,0)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Tags { "LightMode" = "Vertex" }
	Cull Off
	Lighting On
	//Material { Diffuse [_DiffColor] }
	ColorMaterial AmbientAndDiffuse
	ZWrite Off
	ColorMask RGB
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .001
	Pass { 
		SetTexture [_MainTex] { combine primary * texture }
	}
}
}