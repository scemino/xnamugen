#ifdef SM4

// Macros for targetting shader model 4.0 (DX11)
#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_4_0_level_9_1 vsname (); PixelShader = compile ps_4_0_level_9_3 psname(); } }

#define DECLARE_TEXTURE(Name) \
    Texture2D<float4> Name; \
    sampler Name##Sampler

#define SAMPLE_TEXTURE(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)

#else

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_3_0 vsname (); PixelShader = compile ps_3_0 psname(); } }

#define DECLARE_TEXTURE(Name) \
    sampler2D Name

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name, texCoord)

#endif

DECLARE_TEXTURE(xPixels) = sampler_state { texture = <xPixels>; AddressU = clamp; AddressV = clamp; magfilter = point; minfilter = point; mipfilter = point; };
DECLARE_TEXTURE(xPalette) = sampler_state { texture = <xPalette>; AddressU = clamp; AddressV = clamp; magfilter = none; minfilter = none; mipfilter = none; };

float4x4 xMatrix;
float4x4 xRotation;

float xBlendWeight;

int xFontColorIndex;
int xFontTotalColors;

bool xPalFx_Use;
float4 xPalFx_Add;
float4 xPalFx_Mul;
bool xPalFx_Invert;
float xPalFx_Color;
float4 xPalFx_SinMath;

bool xAI_Use;
bool xAI_Invert;
float xAI_color;
float4 xAI_preadd;
float4 xAI_contrast;
float4 xAI_postadd;
float4 xAI_paladd;
float4 xAI_palmul;
int xAI_number;

float4 xShadowColor;

struct VS_Output
{
    float4 Position : SV_Position;
    float2 TextureCoords: TEXCOORD0;
    float4 Color: COLOR0;
};

struct VS_Input
{
    float4 Position : POSITION0;
    float2 TextureCoords: TEXCOORD0;
    float4 Color: COLOR0;
};

float4 BaseColor(float4 inputcolor, float base)
{
	if(base == 0.0f)
	{	
		float color = (0.299f * inputcolor.r) + (0.587f * inputcolor.g) + (0.114f * inputcolor.b);
		return float4(color, color, color, inputcolor.a);
	}
    return float4(inputcolor.rgb * base, inputcolor.a);
}

float4 PalFx(float4 inputcolor)
{
    float4 output = BaseColor(inputcolor, xPalFx_Color);
    
    if(xPalFx_Invert == true) output = float4(1 - output.rgba);
        
    output = (output + xPalFx_Add + xPalFx_SinMath) * xPalFx_Mul;
    
    return float4(output.bgr, inputcolor.a);
}

float4 AfterImageOLD(float4 inputcolor)
{
    float4 output = BaseColor(inputcolor, xAI_color);
    
    if(xAI_Invert == true) output = float4(1 - output.rgba);
    
    output += xAI_preadd;
    output *= xAI_contrast;
    output += xAI_postadd;
    
    return float4(output.bgr, inputcolor.a);
}

float4 AfterImage(float4 inputcolor)
{
    float4 output = BaseColor(inputcolor, xAI_color);
    
    if(xAI_Invert == true) output = float4(1 - output.rgba);
    
    output += xAI_preadd;
    output *= xAI_contrast;
    output += xAI_postadd;
    
    [unroll(25)]
    for(int i = 0; i <= xAI_number; ++i) output = (output + xAI_paladd) * xAI_palmul;
    
    return float4(output.bgr, inputcolor.a);
}

VS_Output DefaultVertexShader(VS_Input input)
{
    VS_Output Output;
    Output.Position = mul(mul(input.Position, xMatrix), xRotation);
    Output.TextureCoords = input.TextureCoords;
    Output.Color = input.Color;
    return Output;
}

float4 DefaultPixelShader(VS_Output input) : COLOR0
{
    float color_index = SAMPLE_TEXTURE(xPixels, input.TextureCoords).r;
    if(color_index == 0.0f) return float4(0, 0, 0, 0);

    float4 output_color = SAMPLE_TEXTURE(xPalette, color_index);
    
    if(xPalFx_Use == true) output_color = PalFx(output_color);
    if(xAI_Use == true) output_color = AfterImage(output_color);
    
    output_color *= input.Color;

    return float4(output_color.bgr, xBlendWeight);
}

float4 PixelShaderOLD(VS_Output input) : COLOR
{
    float color_index = SAMPLE_TEXTURE(xPixels, input.TextureCoords).r;
    if(color_index == 0.0f) return float4(0, 0, 0, 0);

    float4 output_color = SAMPLE_TEXTURE(xPalette, color_index);
    
    if(xPalFx_Use == true) output_color = PalFx(output_color);
    if(xAI_Use == true) output_color = AfterImageOLD(output_color);
    
    output_color *= input.Color;
    
    return float4(output_color.bgr, xBlendWeight);
}

float4 FontPixelShader(VS_Output input) : COLOR
{
    float color_index = SAMPLE_TEXTURE(xPixels, input.TextureCoords).r;
    if(color_index == 0.0f) return float4(0, 0, 0, 0);

    float per = 1.0 - float(xFontColorIndex) / float(xFontTotalColors);
    float4 output_color = SAMPLE_TEXTURE(xPalette, color_index * per - 1.0/255.0);
    
    output_color *= input.Color;
    
    return float4(output_color.bgr, xBlendWeight);
}

float4 ShadowPixelShader(VS_Output input) : COLOR0
{
    float color_index = SAMPLE_TEXTURE(xPixels, input.TextureCoords).r;
    
    if(color_index == 0.0f)
    {
        return float4(0, 0, 0, 0);
    }
    return float4(xShadowColor.bgr, xBlendWeight);
}

TECHNIQUE(Draw,DefaultVertexShader,DefaultPixelShader)
TECHNIQUE(DrawOLD,DefaultVertexShader,PixelShaderOLD)
TECHNIQUE(FontDraw,DefaultVertexShader,FontPixelShader)
TECHNIQUE(ShadowDraw,DefaultVertexShader,ShadowPixelShader)
