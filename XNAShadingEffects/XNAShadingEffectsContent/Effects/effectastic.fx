float4x4 World;
float4x4 View;
float4x4 Projection;

float Alpha = 1;

// Ambient
float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 1;

// Diffuse
float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

// Specular
float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);

// Textured
bool TextureEnabled = false;
float4 TextureColorDefault = float4(1, 1, 1, 1);
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
float BumpConstant = 2.0;
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
float3 FogColor = float3(1, 1, 1);
float FogEnd = 20;
float FogStart = 10;

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
 
Texture ReflectionTexture; 
samplerCUBE ReflectionSampler = sampler_state 
{ 
   texture = <ReflectionTexture>; 
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
	// Textured
    float2 TextureCoordinate : TEXCOORD0;  
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	// Diffuse
    float3 Normal : TEXCOORD0;
	// Bump
    float3 Tangent : TEXCOORD1;
	// Textured
    float2 TextureCoordinate : TEXCOORD2;
	// Reflection
	float3 ViewDirection : TEXCOORD3;
	float Depth : TEXCOORD4;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    //float4 VertexPosition = mul(input.Position, World);
    //float3 ViewDirection = CameraPosition - VertexPosition;

	// Bump
    //output.Reflection = reflect(-normalize(ViewDirection), normalize(Normal));

	// Other
    output.Normal = normalize(mul(input.Normal, World));
    output.Tangent = normalize(mul(input.Tangent, World));

	// Textured
	output.TextureCoordinate = input.TextureCoordinate;

	output.Depth = output.Position.z;
	output.ViewDirection = CameraPosition - worldPosition;

    return output;
}

float3 GetBumpNormal(float2 TexCoord, float3 Normal, float3 Tangent) : COLOR0 {
	// Get the tangent frame normal from the normal map.
	float3 tN = 2.0 * tex2D(bumpSampler, TexCoord) - 1;

	// Compute the unit vectors of the tangent frame in the world frame.
	float3 wN = normalize(Normal);
	float3 wT = normalize(Tangent);
	float3 wB = cross(wN, wT);

	// Transform the tangent frame normal to the world frame (Double check!)
	return normalize(tN.x*wT + tN.y*wB + tN.z*wN);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Calculate the normal, including the information in the bump map
	float3 bump = GetBumpNormal(input.TextureCoordinate, input.Normal, input.Tangent);

    //// Calculate the diffuse light component with the bump map normal
	float diffuseIntensity = normalize(DiffuseLightDirection);
	//float3 r;
	float3 r2;
	float3 light = normalize(DiffuseLightDirection);
	float3 v = normalize(mul(normalize(ViewVector), World));

    if(diffuseIntensity < 0)
        diffuseIntensity = 0;

	if(BumpEnabled) {
		diffuseIntensity = dot(normalize(DiffuseLightDirection), bump);
		//r = reflect(normalize(input.Reflection), normalize(bumpNormal));
		r2 = normalize(2 * dot(light, bump) * bump - light);
	} else {
		//r = reflect(normalize(input.Reflection), 0);
		r2 = normalize(2 * dot(light, 0) * light);
	}

	float dotProduct = dot(r2, v);
    float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * diffuseIntensity;
	
    // Calculate the texture color
    float4 textureColor = TextureColorDefault;

	float4 reflectionColor2;
	if(ReflectionEnabled) {
		float3 reflection = reflect(-normalize(input.ViewDirection), normalize(input.Normal));
		if(BumpEnabled) {
			reflection = reflect(-normalize(input.ViewDirection), bump);
		}

		//float4 reflectionColor = saturate(reflectionColor * (diffuseIntensity + AmbientColor * AmbientIntensity + specular));
		float4 reflectionColor = float4(texCUBE(ReflectionSampler, normalize(reflection)).xyz, 0);
		reflectionColor2 = TintColor * reflectionColor;
	} else {
		
	}

	if(ReflectionEnabled) {
		if(FogEnabled) {
			float l = saturate((input.Depth - FogStart) / (FogEnd - FogStart));
			return float4(lerp(reflectionColor2,FogColor,l),1);
		} else {
			return reflectionColor2;
		}
	}

	return saturate(textureColor * (diffuseIntensity + AmbientColor * AmbientIntensity + specular));
}

technique Effectastic
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}