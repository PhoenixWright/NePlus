/*********************************************************************NVMH3****
$Revision$

Copyright NVIDIA Corporation 2007
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.


To learn more about shading, shaders, and to bounce ideas off other shader
    authors and users, visit the NVIDIA Shader Library Forums at:

    http://developer.nvidia.com/forums/

******************************************************************************/

// Relative filter weights indexed by distance (in texels) from "home" texel
//   (WT_0 is the "home" or center of the filter, WT_4 is four texels away)
// Try changing these around for different filter patterns....
#define WT_0 1.0
#define WT_1 0.8
#define WT_2 0.6
#define WT_3 0.4
#define WT_4 0.2
// these ones are based on the above....
#define WT_NORMALIZE (WT_0+2.0*(WT_1+WT_2+WT_3+WT_4))
#define KW_0 (WT_0/WT_NORMALIZE)
#define KW_1 (WT_1/WT_NORMALIZE)
#define KW_2 (WT_2/WT_NORMALIZE)
#define KW_3 (WT_3/WT_NORMALIZE)
#define KW_4 (WT_4/WT_NORMALIZE)

float Script : STANDARDSGLOBAL <
    string UIWidget = "none";
    string ScriptClass = "scene";
    string ScriptOrder = "postprocess";
    string ScriptOutput = "color";
    string Script = "Technique=Glow?Main:Main10;";
> = 0.8;

// Standard full-screen imaging value
float4 ClearColor <
    string UIWidget = "color";
    string UIName = "Clear (Bg) Color";
> = {0,0,0,1.0};

float ClearDepth <
    string UIWidget = "None";
> = 1.0;

float2 ViewportSize : VIEWPORTPIXELSIZE <
    string UIName="Screen Size";
    string UIWidget="None";
>;

static float2 ViewportOffset = (float2(0.5,0.5)/ViewportSize);

///////////////////////////////////////////////////////////
/////////////////////////////////////// Tweakables ////////
///////////////////////////////////////////////////////////

float Glowness <
    string UIName = "Glow Strength";
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.05f;
> = 0.7f;

float Sceneness <
    string UIName = "Scene Strength";
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.05f;
> = 0.3f;

float GlowSpan <
    string UIName = "Glow Step Size (Texels)";
    string UIWidget = "slider";
    float UIMin = 0.2f;
    float UIMax = 8.0f;
    float UIStep = 0.5f;
> = 2.5f;

///////////////////////////////////////////////////////////
///////////////////////////// Render-to-Texture Targets ///
///////////////////////////////////////////////////////////

texture2D ScnMap : RENDERCOLORTARGET <
    float2 ViewPortRatio = {1.0,1.0};
    int MipLevels = 1;
    string Format = "X8R8G8B8" ;
    string UIWidget = "None";
>;

sampler2D ScnSamp = sampler_state {
    texture = <ScnMap>;
    AddressU  = CLAMP;
    AddressV = CLAMP;
    FILTER = MIN_MAG_LINEAR_MIP_POINT;
};

texture2D GlowMap1 : RENDERCOLORTARGET <
    float2 ViewPortRatio = {1.0,1.0};
    int MipLevels = 1;
    string Format = "X8R8G8B8" ;
    string UIWidget = "None";
>;

sampler2D GlowSamp1 = sampler_state {
    texture = <GlowMap1>;
    AddressU  = CLAMP;
    AddressV = CLAMP;
    FILTER = MIN_MAG_LINEAR_MIP_POINT;
};

texture2D GlowMap2 : RENDERCOLORTARGET <
    float2 ViewPortRatio = {1.0,1.0};
    int MipLevels = 1;
    string Format = "X8R8G8B8" ;
    string UIWidget = "None";
>;

sampler2D GlowSamp2 = sampler_state {
    texture = <GlowMap2>;
    AddressU  = CLAMP;
    AddressV = CLAMP;
    FILTER = MIN_MAG_LINEAR_MIP_POINT;
};

texture2D DepthBuffer : RENDERDEPTHSTENCILTARGET <
    float2 ViewPortRatio = {1.0,1.0};
    string Format = "D24S8";
    string UIWidget = "None";
>;

///////////////////////////////////////////////////////////
///////////////////////////// Connector Data Struct ///////
///////////////////////////////////////////////////////////

struct OneTexelVertex {
    float4 Position	: POSITION;
    float2 UV		: TEXCOORD0;
};

// nine texcoords, to sample nine in-line texels
struct NineTexelVertex
{
    float4 Position   : POSITION;
    float2 UV    : TEXCOORD0;
    float4 UV1   : TEXCOORD1; // xy AND zw used as UV coords
    float4 UV2   : TEXCOORD2; // xy AND zw used as UV coords
    float4 UV3   : TEXCOORD3; // xy AND zw used as UV coords
    float4 UV4   : TEXCOORD4; // xy AND zw used as UV coords
};

///////////////////////////////////////////////////////////
/////////////////////////////////// Vertex Shaders ////////
///////////////////////////////////////////////////////////

// vertex shader to align blur samples vertically
NineTexelVertex vert9BlurVS(
		float3 Position : POSITION, 
		float2 UV : TEXCOORD0
) {
    NineTexelVertex OUT = (NineTexelVertex)0;
    OUT.Position = float4(Position, 1);
    float TexelIncrement = GlowSpan/ViewportSize.y;
    float2 Coord = float2(UV.xy + ViewportOffset);
    OUT.UV = Coord;
    OUT.UV1 = float4(Coord.x, Coord.y + TexelIncrement,
		     Coord.x, Coord.y - TexelIncrement);
    OUT.UV2 = float4(Coord.x, Coord.y + TexelIncrement*2,
		     Coord.x, Coord.y - TexelIncrement*2);
    OUT.UV3 = float4(Coord.x, Coord.y + TexelIncrement*3,
		     Coord.x, Coord.y - TexelIncrement*3);
    OUT.UV4 = float4(Coord.x, Coord.y + TexelIncrement*4,
		     Coord.x, Coord.y - TexelIncrement*4);
    return OUT;
}

// vertex shader to align blur samples horizontally
NineTexelVertex horiz9BlurVS(
		float3 Position : POSITION, 
		float2 UV : TEXCOORD0
) {
    NineTexelVertex OUT = (NineTexelVertex)0;
    OUT.Position = float4(Position, 1);
    float TexelIncrement = GlowSpan/ViewportSize.x;
    float2 Coord = float2(UV.xy + ViewportOffset);
    OUT.UV = Coord;
    OUT.UV1 = float4(Coord.x + TexelIncrement, Coord.y,
		     Coord.x - TexelIncrement, Coord.y);
    OUT.UV2 = float4(Coord.x + TexelIncrement*2, Coord.y,
		     Coord.x - TexelIncrement*2, Coord.y);
    OUT.UV3 = float4(Coord.x + TexelIncrement*3, Coord.y,
		     Coord.x - TexelIncrement*3, Coord.y);
    OUT.UV4 = float4(Coord.x + TexelIncrement*4, Coord.y,
		     Coord.x - TexelIncrement*4, Coord.y);
    return OUT;
}

OneTexelVertex ScreenQuadVS(
		float3 Position : POSITION, 
		float2 UV	: TEXCOORD0
) {
    OneTexelVertex OUT = (OneTexelVertex)0;
    OUT.Position = float4(Position, 1);
    OUT.UV = float2(UV.xy + ViewportOffset);
    return OUT;
}

///////////////////////////////////////////////////////////
/////////////////////////////////// Pixel Shaders /////////
///////////////////////////////////////////////////////////

float4 blur9PS(NineTexelVertex IN,
		uniform sampler2D SrcSamp) : COLOR
{   
    float4 OutCol = tex2D(SrcSamp, IN.UV4.zw) * KW_4;
    OutCol += tex2D(SrcSamp, IN.UV3.zw) * KW_3;
    OutCol += tex2D(SrcSamp, IN.UV2.zw) * KW_2;
    OutCol += tex2D(SrcSamp, IN.UV1.zw) * KW_1;
    OutCol += tex2D(SrcSamp, IN.UV) * KW_0;
    OutCol += tex2D(SrcSamp, IN.UV1.xy) * KW_1;
    OutCol += tex2D(SrcSamp, IN.UV2.xy) * KW_2;
    OutCol += tex2D(SrcSamp, IN.UV3.xy) * KW_3;
    OutCol += tex2D(SrcSamp, IN.UV4.xy) * KW_4;
    return OutCol;
} 

// add glow on top of model

float4 PS_GlowPass(OneTexelVertex IN) : COLOR
{   
    float4 scn = Sceneness * tex2D(ScnSamp, IN.UV);
    float3 glow = Glowness * tex2D(GlowSamp2, IN.UV).xyz;
    return float4(scn.xyz+glow,scn.w);
}  

////////////////////////////////////////////////////////////
/////////////////////////////////////// techniques /////////
////////////////////////////////////////////////////////////
RasterizerState DisableCulling
{
    CullMode = NONE;
};

DepthStencilState DepthEnabling
{
	DepthEnable = FALSE;
	DepthWriteMask = ZERO;
};

BlendState DisableBlend
{
	BlendEnable[0] = FALSE;
};

technique10 Main10 <
	string Script =
	"RenderColorTarget0=ScnMap;"
	"RenderDepthStencilTarget=DepthBuffer;"
		"ClearSetColor=ClearColor;"
		"ClearSetDepth=ClearDepth;"
		"Clear=Color;"
		"Clear=Depth;"
	    "ScriptExternal=color;"
	"Pass=BlurGlowBuffer_Horz;"
	"Pass=BlurGlowBuffer_Vert;"
	"Pass=GlowPass;";
> {
    pass BlurGlowBuffer_Horz <
	string Script = "RenderColorTarget=GlowMap1;"
			"RenderDepthStencilTarget=DepthBuffer;"
			"Draw=Buffer;";
    > 
	{
        SetVertexShader( CompileShader( vs_4_0, horiz9BlurVS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, blur9PS(ScnSamp) ) );
                
        SetRasterizerState(DisableCulling);       
		SetDepthStencilState(DepthEnabling, 0);
		SetBlendState(DisableBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
    }
	
    pass BlurGlowBuffer_Vert <
	string Script = "RenderColorTarget=GlowMap2;"
			"RenderDepthStencilTarget=DepthBuffer;"
			"Draw=Buffer;";
    > 
	{
        SetVertexShader( CompileShader( vs_4_0, vert9BlurVS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, blur9PS(GlowSamp1) ) );
                
        SetRasterizerState(DisableCulling);       
		SetDepthStencilState(DepthEnabling, 0);
		SetBlendState(DisableBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
    }
	
    pass GlowPass <
       	string Script= "RenderColorTarget0=;"
			"RenderDepthStencilTarget=;"
			"Draw=Buffer;";        	
    >
    {
        SetVertexShader( CompileShader( vs_4_0, ScreenQuadVS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_GlowPass() ) );
                
        SetRasterizerState(DisableCulling);       
		SetDepthStencilState(DepthEnabling, 0);
		SetBlendState(DisableBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF);
    }	
	
}


technique Main <
	string Script =
	"RenderColorTarget0=ScnMap;"
	"RenderDepthStencilTarget=DepthBuffer;"
		"ClearSetColor=ClearColor;"
		"ClearSetDepth=ClearDepth;"
		"Clear=Color;"
		"Clear=Depth;"
	    "ScriptExternal=color;"
	"Pass=BlurGlowBuffer_Horz;"
	"Pass=BlurGlowBuffer_Vert;"
	"Pass=GlowPass;";
> {
    pass BlurGlowBuffer_Horz <
	string Script = "RenderColorTarget=GlowMap1;"
			"RenderDepthStencilTarget=DepthBuffer;"
			"Draw=Buffer;";
    > {
	VertexShader = compile vs_3_0 horiz9BlurVS();
	cullmode = none;
	ZEnable = false;
	ZWriteEnable = false;
	AlphaBlendEnable = false;
	PixelShader = compile ps_3_0 blur9PS(ScnSamp);
    }
    pass BlurGlowBuffer_Vert <
	string Script = "RenderColorTarget=GlowMap2;"
			"RenderDepthStencilTarget=DepthBuffer;"
			"Draw=Buffer;";
    > {
	VertexShader = compile vs_3_0 vert9BlurVS();
	cullmode = none;
	ZEnable = false;
	ZWriteEnable = false;
	AlphaBlendEnable = false;
	PixelShader = compile ps_3_0 blur9PS(GlowSamp1);
    }
    pass GlowPass <
       	string Script= "RenderColorTarget0=;"
			"RenderDepthStencilTarget=;"
			"Draw=Buffer;";        	
    >
    {
	cullmode = none;
	ZEnable = false;
	ZWriteEnable = false;
	AlphaBlendEnable = false;
	VertexShader = compile vs_3_0 ScreenQuadVS();
	PixelShader = compile ps_3_0 PS_GlowPass();	
    }
}

////////////// eof ///
