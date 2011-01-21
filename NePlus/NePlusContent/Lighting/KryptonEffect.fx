// ------------------------------------------------------------------------------------------------ //
// ----- Copyright 2011 Christopher Harris --------------------- http://krypton.codeplex.com/ ----- //
// ----------------------------------------------------------------- mailto:xixonia@gmail.com ----- //
// ------------------------------------------------------------------------------------------------ //

// ------------------------------------------------------------------------------------------------ //
// ----- Parameters ------------------------------------------------------------------------------- //

float4x4 Matrix;

texture Texture0;
float2	LightPosition;

float2 TexelBias;

float4 AmbientColor;

float Bluriness = 1;
float BlurFactorU = 0;
float BlurFactorV = 0;

// ------------------------------------------------------------------------------------------------ //
// ----- Samplers --------------------------------------------------------------------------------- //

sampler2D tex0 = sampler_state
{
	Texture = <Texture0>;
	AddressU = Clamp;
	AddressV = Clamp;
};

// ------------------------------------------------------------------------------------------------ //
// ----- Structures ------------------------------------------------------------------------------- //

struct VertexPositionNormalTexture
{
	float4 Position	: POSITION0;
	float4 Normal	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
};

struct VertexPositionColorTexture
{
	float4 Position	: POSITION0;
	float4 Color	: COLOR0;
	float2 TexCoord	: TEXCOORD0;
};

struct VertexPositionTexture
{
	float4 Position	: POSITION0;
	float2 TexCoord : TEXCOORD0;
};

// ------------------------------------------------------------------------------------------------ //
// ----- Techniques ------------------------------------------------------------------------------- //

// ------------------------------------------------------------------------------------------------
// ----- Technique: TextureToTarget ---------------------------------------------------------------

VertexPositionTexture VS_ScreenCopy(VertexPositionTexture input)
{
	VertexPositionTexture output;
	
	output.Position = input.Position;
	output.TexCoord = input.TexCoord;
	
	return input;
};

float4 PS_ScreenCopy(VertexPositionTexture input) : COLOR0
{
	return tex2D(tex0, input.TexCoord + TexelBias);
};

technique TextureToTarget_Add
{
	pass Pass1
	{
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = One;

		AlphaBlendEnable = True;

		VertexShader = compile vs_2_0 VS_ScreenCopy();
		PixelShader = compile ps_2_0 PS_ScreenCopy();
	}
};

technique TextureToTarget_Multiply
{
	pass Pass1
	{
		BlendOp = Add;
		SrcBlend = Zero;
		DestBlend = SrcColor;

		VertexShader = compile vs_2_0 VS_ScreenCopy();
		PixelShader = compile ps_2_0 PS_ScreenCopy();
	}
};

// ------------------------------------------------------------------------------------------------
// ----- Technique: SimpleTexture -----------------------------------------------------------------

VertexPositionColorTexture VS_SimpleTexture(VertexPositionColorTexture input)
{
	VertexPositionColorTexture output;

	output.Position = mul(input.Position, Matrix);
	output.Color = input.Color;
	output.TexCoord = input.TexCoord;

	return output;
};

float4 PS_SimpleTexture(VertexPositionColorTexture input) : COLOR0
{
	return tex2D(tex0, input.TexCoord) * input.Color;
};

technique SimpleTexture
{
	pass Pass1
	{
		StencilEnable = false;
		VertexShader = compile vs_2_0 VS_SimpleTexture();
		PixelShader = compile ps_2_0 PS_SimpleTexture();
	}
};

// ------------------------------------------------------------------------------------------------
// ----- Technique: PointLight_Shadow -------------------------------------------------------------

VertexPositionTexture VS_PointLight_Shadow(VertexPositionNormalTexture input)
{
    float2 Direction = normalize(LightPosition.xy - input.Position.xy);
    
    if(dot(input.Normal.xy, Direction) < 0)
    {
		// Stretch backfacing vertices
		input.Position.xy -= Direction * 1000000;
    }
	
	VertexPositionTexture output;

	output.Position = mul(input.Position, Matrix);
	output.TexCoord = input.TexCoord;

	return output;
};

float4 PS_PointLight_Shadow(VertexPositionTexture input) : COLOR0
{
	return float4(0,0,0,1);
};

VertexPositionTexture VS_Shadow_HullIllumination(VertexPositionTexture input)
{
	VertexPositionTexture output;

	output.Position = mul(input.Position, Matrix);
	output.TexCoord = input.TexCoord;

	return output;
};

float4 PS_Shadow_HullIllumination(VertexPositionTexture input) : COLOR0
{
	return float4(1,1,1,1);
};

technique PointLight_Shadow
{
	pass Shadow
	{
		VertexShader = compile vs_2_0 VS_PointLight_Shadow();
		PixelShader = compile ps_2_0 PS_PointLight_Shadow();
	}
};

// ------------------------------------------------------------------------------------------------
// ----- Technique: PointLight_ShadowWithIllumination ---------------------------------------------

technique PointLight_ShadowWithIllumination
{
	pass Shadow
	{
		VertexShader = compile vs_2_0 VS_PointLight_Shadow();
		PixelShader = compile ps_2_0 PS_PointLight_Shadow();
	}

	pass Shadow_Illumination
	{
		VertexShader = compile vs_2_0 VS_Shadow_HullIllumination();
		PixelShader = compile ps_2_0 PS_Shadow_HullIllumination();
	}
};

// ------------------------------------------------------------------------------------------------
// ----- Technique: PointLight_ShadowWithOcclusion ------------------------------------------------

technique PointLight_ShadowWithOcclusion
{
	pass Shadow_HullStencil
	{
		// This outlines where our hulls are currently, so we don't draw shadows there
		StencilEnable = True;
		StencilFunc = Never;
		StencilFail = Incr;

		VertexShader = compile vs_2_0 VS_Shadow_HullIllumination();
		PixelShader = compile ps_2_0 PS_Shadow_HullIllumination();
	}

	pass Shadow
	{
		// This allows us to draw shadows on hulls behind other hulls
		StencilEnable = True;
		StencilFunc = NotEqual;
		StencilRef = 1;
		StencilPass = Incr;
		StencilFail = Incr;

		VertexShader = compile vs_2_0 VS_PointLight_Shadow();
		PixelShader = compile ps_2_0 PS_PointLight_Shadow();
	}
};

// ------------------------------------------------------------------------------------------------
// ----- Technique: DebugDraw ---------------------------------------------------------------------

technique DebugDraw
{
	pass Solid
	{
		VertexShader = compile vs_2_0 VS_Shadow_HullIllumination();
		PixelShader = compile ps_2_0 PS_Shadow_HullIllumination();
	}
};

// ------------------------------------------------------------------------------------------------
// ----- Technique: Blur --------------------------------------------------------------------------

float4 PS_BlurH(VertexPositionTexture input) : COLOR0
{
	float blurFactor = Bluriness * BlurFactorU / 4.0f;

	float center = tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y) + TexelBias);

	if(!all(center))
	{
		return center;
	}
	
	float4
	sum  = tex2D(tex0, float2(input.TexCoord.x - blurFactor * 4, input.TexCoord.y)	+ TexelBias) * 0.05f;
	sum += tex2D(tex0, float2(input.TexCoord.x - blurFactor * 3, input.TexCoord.y)	+ TexelBias) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x - blurFactor * 2, input.TexCoord.y)	+ TexelBias) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x - blurFactor,	 input.TexCoord.y)	+ TexelBias) * 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x,					 input.TexCoord.y)	+ TexelBias) * 0.18f;
	sum += tex2D(tex0, float2(input.TexCoord.x + blurFactor,	 input.TexCoord.y)	+ TexelBias) * 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x + blurFactor * 2, input.TexCoord.y)	+ TexelBias) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x + blurFactor * 3, input.TexCoord.y)	+ TexelBias) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x + blurFactor * 4, input.TexCoord.y)	+ TexelBias) * 0.05f;
	
	return sum;
}

float4 PS_BlurV(VertexPositionTexture input) : COLOR0
{
	float blurFactor = Bluriness * BlurFactorV / 4.0f;
	
	float center = tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y) + TexelBias);

	if(!all(center))
	{
		return center + AmbientColor;
	}
	float4
	sum  = tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y - blurFactor * 4)	+ TexelBias) * 0.05f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y - blurFactor * 3)	+ TexelBias) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y - blurFactor * 2)	+ TexelBias) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x,	input.TexCoord.y - blurFactor)		+ TexelBias) * 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y)					+ TexelBias) * 0.18f;
	sum += tex2D(tex0, float2(input.TexCoord.x,	input.TexCoord.y + blurFactor)		+ TexelBias) * 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y + blurFactor * 2)	+ TexelBias) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y + blurFactor * 3)	+ TexelBias) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y + blurFactor * 4)	+ TexelBias) * 0.05f;
	
	return sum + AmbientColor;
}

technique Blur
{
    pass HorizontalBlur
    {
		VertexShader = compile vs_2_0 VS_ScreenCopy();
        PixelShader = compile ps_2_0 PS_BlurH();
    }
    
    pass VerticalBlur
    {
		VertexShader = compile vs_2_0 VS_ScreenCopy();
        PixelShader = compile ps_2_0 PS_BlurV();
    }
}