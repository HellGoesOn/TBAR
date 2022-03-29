sampler input : register(s0);
sampler mask : register(S1);
float2 imageSize;
float2 imageSize2;
float4 frame;
float2 offset;

float4 main(float2 uv : TEXCOORD0) : COLOR 
{ 
	float2 maskCoords = (uv * imageSize - frame.xy) / imageSize2;
	
	float4 color; 
	float4 maskClr;
	
	color= tex2D( input , uv.xy); 
	maskClr = tex2D(mask, maskCoords + offset); 
	
	float3 hieroColor = float3(94.0f / 255.0f, 236.0f / 255.0f, 39.0f / 255.0f);
	
	if(all(color == hieroColor))
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