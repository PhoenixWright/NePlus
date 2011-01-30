//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- Copyright 2011 Christopher Harris --------------------- http://krypton.codeplex.com/ ----- //
// ----------------------------------------------------------------- mailto:xixonia@gmail.com ----- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- PARAMETERS ------------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

float4x4 Matrix;

texture Texture0;
float2 LightPosition;

float2 TexelBias;

float4 AmbientColor;

float Bluriness = 1;
float BlurFactorU = 0;
float BlurFactorV = 0;

//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- SAMPLERS --------------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

sampler2D tex0 = sampler_state
{
	Texture = <Texture0>;
	AddressU = Clamp;
	AddressV = Clamp;
};

//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- STRUCTURES ------------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

// Primarily used for shadow hulls
struct VertexPositionNormal
{
	float4 Position	: POSITION0;
	float4 Normal	: NORMAL0;
};

// Primarily used for lights
struct VertexPositionColorTexture
{
	float4 Position	: POSITION0;
	float4 Color	: COLOR0;
	float2 TexCoord	: TEXCOORD0;
};

// Primarily used for post-processing
struct VertexPositionTexture
{
	float4 Position	: POSITION0;
	float2 TexCoord : TEXCOORD0;
};

//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- VERTEX SHADERS --------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

float4 VS_Shadows_PointLight(VertexPositionNormal input) : POSITION0
{
    float2 Direction = normalize(LightPosition.xy - input.Position.xy);
    
    if(dot(input.Normal.xy, Direction) < 0)
    {
		// Stretch backfacing vertices
		input.Position.xy -= Direction * 1000000;
    }

	return mul(input.Position, Matrix);
};

VertexPositionTexture VS_NoTransform(VertexPositionTexture input)
{
	return input;
};

VertexPositionTexture VS_Transform_VPT(VertexPositionTexture input)
{
	VertexPositionTexture output;

	output.Position = mul(input.Position, Matrix);
	output.TexCoord = input.TexCoord;

	return output;
}

VertexPositionColorTexture VS_Transform_VPCT(VertexPositionColorTexture input)
{
	VertexPositionColorTexture output;

	output.Position = mul(input.Position, Matrix);
	output.Color = input.Color;
	output.TexCoord = input.TexCoord;

	return output;
};

//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- PIXEL SHADERS ---------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

float4 PS_Black() : COLOR0
{
	return float4(0, 0, 0, 1);
};

float4 PS_White() : COLOR0
{
	return float4(1, 1, 1, 1);
};

float4 PS_Blur_Horizontal_HighQuality(in float2 texCoord : TEXCOORD0) : COLOR0
{
	float blurFactor = Bluriness * BlurFactorU / 4.0f;

	// // Was used to opt-out early, and prevent texture from bluring where it was totally black.
	// float center = tex2D(tex0, float2(texCoord.x, texCoord.y) + TexelBias);
	// if(!all(center)) { return center; }
	
	return
	tex2D(tex0, float2(texCoord.x - blurFactor * 4,	texCoord.y)	+ TexelBias) * 0.05f +
	tex2D(tex0, float2(texCoord.x - blurFactor * 3,	texCoord.y)	+ TexelBias) * 0.09f +
	tex2D(tex0, float2(texCoord.x - blurFactor * 2,	texCoord.y)	+ TexelBias) * 0.12f +
	tex2D(tex0, float2(texCoord.x - blurFactor,		texCoord.y)	+ TexelBias) * 0.15f +
	tex2D(tex0, float2(texCoord.x,					texCoord.y)	+ TexelBias) * 0.18f +
	tex2D(tex0, float2(texCoord.x + blurFactor,		texCoord.y)	+ TexelBias) * 0.15f +
	tex2D(tex0, float2(texCoord.x + blurFactor * 2,	texCoord.y)	+ TexelBias) * 0.12f +
	tex2D(tex0, float2(texCoord.x + blurFactor * 3,	texCoord.y)	+ TexelBias) * 0.09f +
	tex2D(tex0, float2(texCoord.x + blurFactor * 4,	texCoord.y)	+ TexelBias) * 0.05f;
};

float4 PS_Blur_Vertical_HighQuality(in float2 texCoord : TEXCOORD0) : COLOR0
{
	float blurFactor = Bluriness * BlurFactorV / 4.0f;

	// // Was used to opt-out early, and prevent texture from bluring where it was totally black.
	// float center = tex2D(tex0, float2(texCoord.x, texCoord.y) + TexelBias);
	// if(!all(center)) { return center + AmbientColor; }

	return
	tex2D(tex0, float2(texCoord.x, texCoord.y - blurFactor * 4)	+ TexelBias) * 0.05f +
	tex2D(tex0, float2(texCoord.x, texCoord.y - blurFactor * 3)	+ TexelBias) * 0.09f +
	tex2D(tex0, float2(texCoord.x, texCoord.y - blurFactor * 2)	+ TexelBias) * 0.12f +
	tex2D(tex0, float2(texCoord.x, texCoord.y - blurFactor)		+ TexelBias) * 0.15f +
	tex2D(tex0, float2(texCoord.x, texCoord.y)					+ TexelBias) * 0.18f +
	tex2D(tex0, float2(texCoord.x, texCoord.y + blurFactor)		+ TexelBias) * 0.15f +
	tex2D(tex0, float2(texCoord.x, texCoord.y + blurFactor * 2)	+ TexelBias) * 0.12f +
	tex2D(tex0, float2(texCoord.x, texCoord.y + blurFactor * 3)	+ TexelBias) * 0.09f +
	tex2D(tex0, float2(texCoord.x, texCoord.y + blurFactor * 4)	+ TexelBias) * 0.05f;
};

float4 PS_PointLight(in float2 texCoord : TEXCOORD0, in float4 color : COLOR0) : COLOR0
{
	// Add PointLight logic here.
	return tex2D(tex0, texCoord) * color;
}

float4 PS_ScreenCopy(in float2 texCoord : TEXCOORD0) : COLOR0
{
	return tex2D(tex0, texCoord + TexelBias);
};

//////////////////////////////////////////////////////////////////////////////////////////////////////
// ------------------------------------------------------------------------------------------------ //
// ----- TECHNIQUES ------------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------ //
//////////////////////////////////////////////////////////////////////////////////////////////////////

technique Shadows_PointLight
{
	// Draws shadows streched from a single point
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Shadows_PointLight();
		PixelShader = compile ps_2_0 PS_Black();
	}
};

/* ----- Not Yet Implemented ----- //
technique Shadows_DirectionalLight
{
	// Draws shadows streched from a single point
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Shadows_DirectionalLight();
		PixelShader = compile ps_2_0 PS_Black();
	}
};

technique Blur_LowQuality
{
	// Blurs the shadows according to bluriness and blur factors
	pass Horizontal
	{
		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_Blur_Horizontal_LowQuality();
	}

	pass Vertical
	{
		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_Blur_Vertical_LowQuality();
	}
};

technique Blur_MediumQuality
{
	// Blurs the shadows according to bluriness and blur factors
	pass Horizontal
	{
		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_Blur_Horizontal_MediumQuality();
	}

	pass Vertical
	{
		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_Blur_Vertical_MediumQuality();
	}
};
// ----- Not Yet Implemented ----- */

technique Blur_HeightQuality
{
	// Blurs the shadows according to bluriness and blur factors
	pass Horizontal
	{
		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_Blur_Horizontal_HighQuality();
	}

	pass Vertical
	{
		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_Blur_Vertical_HighQuality();
	}
};

technique Shadows_HullStencil
{
	pass Pass1
	{
		StencilEnable = True;
		StencilFunc = Always;
		StencilFail = Incr;

		VertexShader = compile vs_2_0 VS_Transform_VPT();
		PixelShader = compile ps_2_0 PS_White();
	}
};

technique PointLight
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Transform_VPCT();
		PixelShader = compile ps_2_0 PS_PointLight();
	}
};

technique Screen_Add
{
	pass Pass1
	{
		AlphaBlendEnable = True;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = One;

		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_ScreenCopy();
	}
};

technique Screen_Multiply
{
	pass Pass1
	{
		BlendOp = Add;
		SrcBlend = Zero;
		DestBlend = SrcColor;

		VertexShader = compile vs_2_0 VS_NoTransform();
		PixelShader = compile ps_2_0 PS_ScreenCopy();
	}
};