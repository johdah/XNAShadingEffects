float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);
 
float4 TintColor = float4(1, 1, 1, 1);
float3 CameraPosition;

bool FogEnabled = true;
float4 FogColor = float4(0.39, 0.58, 0.93, 1);
float FogStart;
float FogEnd;
float FogDistance;
 
Texture ReflectedTexture; 
samplerCUBE SkyboxSampler = sampler_state 
{ 
   texture = <ReflectedTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};
texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

float BumpConstant = 1;
texture NormalMap;
sampler2D bumpSampler = sampler_state {
    Texture = (NormalMap);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInputBump
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float3 Tangent : TANGENT0;
    float3 Binormal : BINORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutputBump
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 Tangent : TEXCOORD2;
    float3 Binormal : TEXCOORD3;
};
 
struct VertexShaderInputReflection
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
};
 
struct VertexShaderOutputReflection
{
    float4 Position : POSITION0;
    float3 Reflection : TEXCOORD0;
};

VertexShaderOutputBump VertexShaderFunctionBump(VertexShaderInputBump input)
{
    VertexShaderOutputBump output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));
    output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose));
    output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));

    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}

float4 PixelShaderFunctionBump(VertexShaderOutputBump input) : COLOR0
{
    // Calculate the normal, including the information in the bump map
    float3 bump = BumpConstant * (tex2D(bumpSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
    float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
    bumpNormal = normalize(bumpNormal);

    // Calculate the diffuse light component with the bump map normal
    float diffuseIntensity = dot(normalize(DiffuseLightDirection), bumpNormal);
    if(diffuseIntensity < 0)
        diffuseIntensity = 0;

    // Calculate the specular light component with the bump map normal
    float3 light = normalize(DiffuseLightDirection);
    float3 r = normalize(2 * dot(light, bumpNormal) * bumpNormal - light);
    float3 v = normalize(mul(normalize(ViewVector), World));
    float dotProduct = dot(r, v);

    float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * diffuseIntensity;

    // Calculate the texture color
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1;

    // Combine all of these values into one (including the ambient light)
    return saturate(textureColor * (diffuseIntensity) + AmbientColor * AmbientIntensity + specular);
}

VertexShaderOutputReflection VertexShaderFunctionReflection(VertexShaderInputReflection input)
{
    VertexShaderOutputReflection output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    float3 ViewDirection = CameraPosition - VertexPosition;
 
    float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
    output.Reflection = reflect(-normalize(ViewDirection), normalize(Normal));
 
    return output;
}
 
float4 PixelShaderFunctionReflection(VertexShaderOutputReflection input) : COLOR0
{
	float affect;

	if(FogEnabled && FogDistance>FogStart){
		affect = (FogDistance-FogStart)/(FogEnd-FogStart);
		if(FogDistance>FogEnd)
			affect = 1;
	}
	else {
		 affect = 0;
	}
	return lerp(texCUBE(SkyboxSampler, normalize(input.Reflection)), FogColor, affect);	

}
 

technique BumpReflected
{
	pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunctionBump();
        PixelShader = compile ps_2_0 PixelShaderFunctionBump();
    }
 	pass Pass2
	{
        VertexShader = compile vs_2_0 VertexShaderFunctionReflection();
        PixelShader = compile ps_2_0 PixelShaderFunctionReflection();
	}


}
