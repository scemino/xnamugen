Texture xTexture;
sampler xTextureSampler = sampler_state { texture = <xTexture>; AddressU = clamp; AddressV = clamp; magfilter = none; minfilter = none; mipfilter = none; };

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

float4 BaseColor(float4 inputcolor, float base)
{
	if(base == 0.0f)
	{	
		float color = (0.299f * inputcolor.r) + (0.587f * inputcolor.g) + (0.114f * inputcolor.b);
		return float4(color, color, color, inputcolor.a);
	}
	else
	{
		return float4(inputcolor.rgb * base, inputcolor.a);
	}
}

float4 PalFx(float4 inputcolor)
{
	float4 output = BaseColor(inputcolor, xPalFx_Color);
	
	if(xPalFx_Invert == true) output = float4(1 - output.rgba);
		
	output = (output + xPalFx_Add + xPalFx_SinMath) * xPalFx_Mul;
	
	return float4(output.rgb, inputcolor.a);
}

float4 AfterImageOLD(float4 inputcolor)
{
	float4 output = BaseColor(inputcolor, xAI_color);
	
	if(xAI_Invert == true) output = float4(1 - output.rgba);
	
	output += xAI_preadd;
	output *= xAI_contrast;
	output += xAI_postadd;
	
	return float4(output.rgb, inputcolor.a);
}

float4 AfterImage(float4 inputcolor)
{
	float4 output = BaseColor(inputcolor, xAI_color);
	
	if(xAI_Invert == true) output = float4(1 - output.rgba);
	
	output += xAI_preadd;
	output *= xAI_contrast;
	output += xAI_postadd;
	
	for(int i = 0; i <= xAI_number; ++i) output = (output + xAI_paladd) * xAI_palmul;
	
	return float4(output.rgb, inputcolor.a);
}

struct VS_Output
{
	float4 Position : POSITION;
	float2 TextureCoords: TEXCOORD;
	float4 Color: COLOR;
};

VS_Output VertexShader(float4 inPos : POSITION, float2 inTexCoords: TEXCOORD, float4 inColor : COLOR)
{
	VS_Output Output = (VS_Output)0;
	Output.Position = mul(mul(inPos, xMatrix), xRotation);
	Output.TextureCoords = inTexCoords;
	Output.Color = inColor;
	return Output;
}

float4 DefaultPixelShader(float4 color : COLOR, float2 texCoord : TEXCOORD) : COLOR
{
	float4 output_color = tex2D(xTextureSampler, texCoord);
	
	if(xPalFx_Use == true) output_color = PalFx(output_color);
	if(xAI_Use == true) output_color = AfterImage(output_color);
	
	output_color *= color;
	
	if(output_color.a == 0.0f)
	{
		return output_color;
	}
	else
	{
		return float4(output_color.rgb, xBlendWeight);
	}
}

float4 PixelShaderOLD(float4 color : COLOR, float2 texCoord : TEXCOORD) : COLOR
{
	float4 output_color = tex2D(xTextureSampler, texCoord);
	
	if(xPalFx_Use == true) output_color = PalFx(output_color);
	if(xAI_Use == true) output_color = AfterImageOLD(output_color);
	
	output_color *= color;
	
	if(output_color.a == 0.0f)
	{
		return output_color;
	}
	else
	{
		return float4(output_color.rgb, xBlendWeight);
	}
}

float4 FontPixelShader(float4 color : COLOR, float2 texCoord : TEXCOORD) : COLOR
{
	float4 output_color = tex2D(xTextureSampler, texCoord);		
	output_color *= color;
	
	if(output_color.a == 0.0f)
	{
		return output_color;
	}
	else
	{
		return float4(output_color.rgb, xBlendWeight);
	}
}

technique Draw
{
	pass Pass0
	{
		VertexShader = compile vs_1_1 VertexShader();
		PixelShader = compile ps_3_0 DefaultPixelShader();
	}
}

technique DrawOLD
{
	pass Pass0
	{
		VertexShader = compile vs_1_1 VertexShader();
		PixelShader = compile ps_2_0 PixelShaderOLD();
	}
}

technique FontDraw
{
	pass Pass0
	{
		VertexShader = compile vs_1_1 VertexShader();
		PixelShader = compile ps_2_0 FontPixelShader();
	}
}