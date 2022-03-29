sampler input : register(s0);
sampler mask : register(S1);
float2 imageSize;
float2 imageSize2;
float4 frame;
float2 offset;
float pixelation = 100.0;

float4 main(float4 color : COLOR0, float2 uv : TEXCOORD0) : COLOR
{ 
	float4 initialColor = tex2D(input, uv);

	float2 maskCoords = (uv * imageSize - frame.xy) / imageSize2;
	
	float4 maskClr = color;
	
	float2 brickSize = 1.0 / pixelation;
	
	color *= tex2D( input , uv.xy);
	
	float2 brickNum = floor(maskCoords / brickSize);
	float2 centerOfBrick = brickNum * brickSize + brickSize /2;
	
	float3 hieroColor = float3(94.0f / 255.0f, 236.0f / 255.0f, 39.0f / 255.0f);
	
	maskClr *= tex2D(mask, centerOfBrick + offset);
	
	if(all(initialColor == hieroColor))
    {
		return maskClr;
    }

	return color; 
}

technique technique0
{
    pass p0
    {
		CullMode = None;
        PixelShader = compile ps_2_0 main();
    }
}