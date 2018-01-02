Texture xPixels;
sampler xPixelsSampler = sampler_state { texture = <xPixels>; AddressU = clamp; AddressV = clamp; magfilter = none; minfilter = none; mipfilter = none; };

Texture xPalette;
sampler xPaletteSampler = sampler_state { texture = <xPalette>; AddressU = clamp; AddressV = clamp; magfilter = none; minfilter = none; mipfilter = none; };

float4x4 xMatrix;
float4x4 xRotation;

float xBlendWeight;

int xFontColorIndex;
int xFontTotalColors;

bool xPalFx_Use;
//float4 xPalFx_Add;
//float4 xPalFx_Mul;
//bool xPalFx_Invert;
//float xPalFx_Color;
//float4 xPalFx_SinMath;

bool xAI_Use;
//bool xAI_Invert;
//float xAI_color;
//float4 xAI_preadd;
//float4 xAI_contrast;
//float4 xAI_postadd;
//float4 xAI_paladd;
//float4 xAI_palmul;
//int xAI_number;

//float4 xShadowColor;

struct VS_Output
{
    float4 Position : SV_Position;
    float2 TextureCoords: TEXCOORD;
    float4 Color: COLOR;
};

struct VS_Input
{
    float4 Position : SV_Position;
    float2 TextureCoords: TEXCOORD;
    float4 Color: COLOR;
};

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
    
    [unroll(25)]
    for(int i = 0; i <= xAI_number; ++i) output = (output + xAI_paladd) * xAI_palmul;
    
    return float4(output.rgb, inputcolor.a);
}

VS_Output DefaultVertexShader(VS_Input input)
{
    VS_Output Output;
    Output.Position = mul(mul(input.Position, xMatrix), xRotation);
    Output.TextureCoords = input.TextureCoords;
    Output.Color = input.Color;
    //Output.Color = float4(input.Color.b, input.Color.g, input.Color.r, input.Color.a);
    return Output;
}

float4 DefaultPixelShader(VS_Output input) : COLOR
{
    float color_index = tex2D(xPixelsSampler, input.TextureCoords).a;
    float4 output_color = tex1D(xPaletteSampler, color_index);
    
    if(xPalFx_Use == true) output_color = PalFx(output_color);
    if(xAI_Use == true) output_color = AfterImage(output_color);
    
    output_color *= input.Color;
            
    if(color_index == 0.0f)
    {
        return float4(0, 0, 0, 0);
    }
    else
    {
        //return float4(output_color.rgb, xBlendWeight);
        return float4(output_color.b, output_color.g, output_color.r, xBlendWeight);
    }
}

float4 PixelShaderOLD(VS_Output input) : COLOR
{
    float color_index = tex2D(xPixelsSampler, input.TextureCoords).a;
    float4 output_color = tex1D(xPaletteSampler, color_index);
    
    if(xPalFx_Use == true) output_color = PalFx(output_color);
    if(xAI_Use == true) output_color = AfterImageOLD(output_color);
    
    output_color *= input.Color;
    
    if(color_index == 0.0f)
    {
        return float4(0, 0, 0, 0);
    }
    else
    {
        return float4(output_color.rgb, xBlendWeight);
    }
}

float4 FontPixelShader(VS_Output input) : COLOR
{
    float color_index = tex2D(xPixelsSampler, input.TextureCoords).a;
    float per = 1.0 - float(xFontColorIndex) / float(xFontTotalColors);
    float4 output_color = tex1D(xPaletteSampler, color_index * per - 1.0/255.0);
    
    output_color *= input.Color;
    
    if(color_index == 0.0f)
    {
        return float4(0, 0, 0, 0);
    }
    else
    {
        return float4(output_color.rgb, xBlendWeight);
    }
}

float4 ShadowPixelShader(VS_Output input) : COLOR
{
    float color_index = tex2D(xPixelsSampler, input.TextureCoords).a;
    
    if(color_index == 0.0f)
    {
        return float4(0, 0, 0, 0);
    }
    else
    {
        return float4(xShadowColor.rgb, xBlendWeight);
    }
}

technique Draw
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 DefaultVertexShader();
        PixelShader = compile ps_3_0 DefaultPixelShader();
    }
}

technique DrawOLD
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 DefaultVertexShader();
        PixelShader = compile ps_3_0 PixelShaderOLD();
    }
}

technique FontDraw
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 DefaultVertexShader();
        PixelShader = compile ps_3_0 FontPixelShader();
    }
}

technique ShadowDraw
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 DefaultVertexShader();
        PixelShader = compile ps_3_0 ShadowPixelShader();
    }
}