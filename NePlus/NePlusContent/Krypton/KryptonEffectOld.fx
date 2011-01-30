float4x4 Matrix;
float2	 LightPosition;
float2	 LightPositionUV;
float	 LightRange;

float UFactor;
float VFactor;

float Bluriness;

texture LightTexture;

sampler2D tex0 = sampler_state
{
	Texture = <LightTexture>;
	AddressU = Clamp;
	AddressV = Clamp;
};

// Input Structs ----------------------------------------------------------------------------------

struct PS_2DInput
{
    float2 TexCoord : TEXCOORD0;
    float4 Color	: COLOR0;
};

struct VS_LightInput
{
	float4 Position : POSITION0;
	float4 Color	: COLOR0;
	float2 TexCoord : TEXCOORD0;
};

struct PS_LightInput
{
	float4 Position : POSITION0;
	float4 Color	: COLOR0;
	float2 TexCoord : TEXCOORD0;
};

struct VS_ShadowInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct PS_ShadowInput
{
    float4 Position : POSITION0;
};

// LightTechnique Shaders -------------------------------------------------------------------------

PS_LightInput VS_Light(VS_LightInput input)
{
	PS_LightInput output;
	
	output.Position = input.Position;
	output.Color = input.Color;
	
	// Scale and Position Texture
	output.TexCoord.x = (input.TexCoord.x - LightPositionUV.x) / (LightRange * 2 * UFactor) + 0.5f;
	output.TexCoord.y = (input.TexCoord.y - LightPositionUV.y) / (LightRange * 2 * VFactor) + 0.5f;
	
	return output;
}

float4 PS_Light(PS_2DInput input) : COLOR0
{
	return tex2D(tex0, input.TexCoord) * input.Color;
}

technique LightTechnique
{
    pass Pass1
    {
		CullMode = None;
        AlphaBlendEnable = true;
        BlendOp = Add;
		SrcBlend = Zero;
		DestBlend = SrcColor; 

        VertexShader = compile vs_2_0 VS_Light();
        PixelShader = compile ps_2_0 PS_Light();
    }
}

// ShadowTechnique Shaders ------------------------------------------------------------------------

PS_ShadowInput VS_Shadow(VS_ShadowInput input)
{
    PS_ShadowInput output;
    
    float2 Direction = normalize(LightPosition.xy - input.Position.xy);
    
    if(dot(input.Normal.xy, Direction) < 0)
    {
		// Stretch backfacing vertices
		input.Position.xy -= Direction * 1000000;
    }

    output.Position = mul(input.Position, Matrix);

    return output;
}

float4 PS_Shadow(PS_ShadowInput input) : COLOR0
{
    return float4(0.0f, 0.0f, 0.0f, 1.0f);
}

technique ShadowTechnique
{
    pass Pass1
    {
		StencilEnable = true;
        StencilFunc = Always;
        StencilPass = Incr;

        VertexShader = compile vs_2_0 VS_Shadow();
        PixelShader = compile ps_2_0 PS_Shadow();
    }
}

// IlluminationTechnique Shaders ------------------------------------------------------------------

PS_ShadowInput VS_Illumination(VS_ShadowInput input)
{
    PS_ShadowInput output;
    
    output.Position = mul(input.Position, Matrix);

    return output;
}

float4 PS_Illumination(PS_ShadowInput input) : COLOR0
{
	return float4(1, 1, 1, 1);
}

technique IlluminationTechnique
{
    pass Pass1
    {
        // Stencil Func options:
        // Equal - Objects will draw occlusions on other objects (and on themselves, if concave)
        // Always - Objects will not draw occlusions on other objects
		StencilEnable = true;
        StencilRef = 1;
        StencilFunc = Equal;

        VertexShader = compile vs_2_0 VS_Illumination();
        PixelShader = compile ps_2_0 PS_Illumination();
    }
}

// BlurTechnique Shaders --------------------------------------------------------------------------

float4 PS_BlurH(PS_2DInput input) : COLOR0
{
	float BlurFactor = Bluriness * UFactor / 4.0f;
	
	float4
	sum  = tex2D(tex0, float2(input.TexCoord.x - BlurFactor * 4, input.TexCoord.y)) * 0.05f;
	sum += tex2D(tex0, float2(input.TexCoord.x - BlurFactor * 3, input.TexCoord.y)) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x - BlurFactor * 2, input.TexCoord.y)) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x - BlurFactor,	 input.TexCoord.y)) * 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x,					 input.TexCoord.y)) * 0.18f;
	sum += tex2D(tex0, float2(input.TexCoord.x + BlurFactor,	 input.TexCoord.y)) * 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x + BlurFactor * 2, input.TexCoord.y)) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x + BlurFactor * 3, input.TexCoord.y)) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x + BlurFactor * 4, input.TexCoord.y)) * 0.05f;
	
	return sum;
}

float4 PS_BlurV(PS_2DInput input) : COLOR0
{
	float BlurFactor = Bluriness * VFactor / 4.0f;
	
	float4
	sum  = tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y - BlurFactor * 4)) * 0.05f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y - BlurFactor * 3)) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y - BlurFactor * 2)) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x,	input.TexCoord.y - BlurFactor))		* 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y))					* 0.18f;
	sum += tex2D(tex0, float2(input.TexCoord.x,	input.TexCoord.y + BlurFactor))		* 0.15f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y + BlurFactor * 2)) * 0.12f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y + BlurFactor * 3)) * 0.09f;
	sum += tex2D(tex0, float2(input.TexCoord.x, input.TexCoord.y + BlurFactor * 4)) * 0.05f;
	
	return sum;
}

technique BlurTechnique
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PS_BlurH();
    }
    
    pass Pass2
    {
        PixelShader = compile ps_2_0 PS_BlurV();
    }
}