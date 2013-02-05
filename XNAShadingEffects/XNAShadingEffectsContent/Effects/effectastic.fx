float4x4 World;
float4x4 View;
float4x4 Projection;

float Alpha = 1;

// Ambient
float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

// Diffuse
float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

// Specular
float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);

// Textured
texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Bump
bool BumpEnabled = true;
float BumpConstant = 1;
texture NormalMap;
sampler2D bumpSampler = sampler_state {
    Texture = (NormalMap);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

// Fog
bool FogEnabled = true;
float3 FogColor;
float FogEnd;
float FogStart;

// Reflection
bool ReflectionEnabled = true;
float4 TintColor = float4(1, 1, 1, 1);
float3 CameraPosition;

//Light
bool LightningEnabled = true;
bool DirectionalLightEnabled = true;
float3 DirectionalLightDirection = float3(0, -1, 0);
float3 DirectionalLightDiffuseColor = float3(1, 1, 1);
float3 DirectionalLightSpecularColor = float3(0, 0, 0);
 
Texture SkyboxTexture; 
samplerCUBE SkyboxSampler = sampler_state 
{ 
   texture = <SkyboxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror;
   AddressV = Mirror; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;    
    float4 Normal : NORMAL0; 
	// Bump
    float3 Tangent : TANGENT0;
    float3 Binormal : BINORMAL0;
	// Textured
    float2 TextureCoordinate : TEXCOORD0;  
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	// Textured
    float2 TextureCoordinate : TEXCOORD0;
	// Diffuse
    float3 Normal : TEXCOORD1;
	// Bump
    float3 Tangent : TEXCOORD2;
    float3 Binormal : TEXCOORD3;
	// Reflection
    float3 Reflection : TEXCOORD4;
	float  Interpolation : TEXCOORD5;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    float4 VertexPosition = mul(input.Position, World);
    float3 ViewDirection = CameraPosition - VertexPosition;

	// Bump
    float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
    output.Reflection = reflect(-normalize(ViewDirection), normalize(Normal));

	// Other
    output.Normal = Normal;
    output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose));
    output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose));

	// Textured
	output.TextureCoordinate = input.TextureCoordinate;

	// Fog
	output.Interpolation  = saturate((output.Position.z-FogStart)/(FogEnd-FogStart));

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Calculate the normal, including the information in the bump map
    float3 bump = BumpConstant * (tex2D(bumpSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
    float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
    bumpNormal = normalize(bumpNormal);

    // Calculate the diffuse light component with the bump map normal
	float diffuseIntensity = normalize(DiffuseLightDirection);
	if(BumpEnabled) {
		diffuseIntensity = dot(normalize(DiffuseLightDirection), bumpNormal);
	}
    if(diffuseIntensity < 0)
        diffuseIntensity = 0;

    // Calculate the specular light component with the bump map normal
    float3 light = normalize(DiffuseLightDirection);
    float3 r = normalize(2 * dot(light, bumpNormal) * bumpNormal - light);
    float3 v = normalize(mul(normalize(ViewVector), World));
    float dotProduct = dot(r, v);

    float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * diffuseIntensity;

    // Calculate the texture color
    //float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	float4 textureColor = tex2D(textureSampler, normalize(input.Reflection));
    textureColor.a = 1;

	float3 tempColor = saturate(diffuseIntensity + AmbientColor * AmbientIntensity + specular);

	if(ReflectionEnabled) {
		float4 reflectionColor = TintColor * texCUBE(SkyboxSampler, normalize(input.Reflection));
		reflectionColor.a = 1;

		tempColor = saturate(reflectionColor * (diffuseIntensity + AmbientColor * AmbientIntensity + specular));
		//tempColor = saturate(reflectionColor * (diffuseIntensity) + AmbientColor * AmbientIntensity + specular); // BUG??
	}

	if(FogEnabled) {
		return float4(lerp(tempColor,FogColor,input.Interpolation),1);
	} else {
		return float4(tempColor, 1);
	}
}

technique Effectastic
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}