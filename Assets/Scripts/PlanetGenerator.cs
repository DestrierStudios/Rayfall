using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [Tooltip("The radius of the planet in Unity units.")]
    [Range(1, 100)]
    public float radius = 20f;

    [Header("Procedural Texture Settings")]
    [Tooltip("The resolution of the generated texture.")]
    public int textureWidth = 1024;
    public int textureHeight = 512;

    [Tooltip("A random seed for the noise generator. 0 means a new random seed every time.")]
    public int seed;

    [Header("Noise Configuration")]
    [Tooltip("Controls the 'zoom' level of the noise features.")]
    public float noiseScale = 15f;
    [Tooltip("Number of noise layers to combine for more detail.")]
    public int octaves = 5;
    [Tooltip("How much each successive octave contributes to the overall shape (0-1).")]
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    [Tooltip("How much the frequency increases for each successive octave.")]
    public float lacunarity = 2f;

    [Header("Planet Colors")]
    [Tooltip("Defines the colors of the planet based on the generated noise value.")]
    public Gradient colorGradient;

    private GameObject planetGameObject;

    void Start()
    {
        GeneratePlanet();
    }

    /// <summary>
    /// Creates the planet GameObject and applies a procedurally generated material.
    /// </summary>
    public void GeneratePlanet()
    {
        if (planetGameObject != null)
        {
            Destroy(planetGameObject);
        }

        // 1. Create the planet GameObject from a sphere primitive
        planetGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planetGameObject.transform.SetParent(this.transform, false);
        // A primitive sphere has a radius of 0.5, so we scale it by radius * 2
        planetGameObject.transform.localScale = Vector3.one * radius * 2; 
        planetGameObject.name = "Procedural Planet";

        // 2. Generate and apply the procedural material
        Renderer planetRenderer = planetGameObject.GetComponent<Renderer>();
        // Get the renderer's material instance. This avoids issues with Shader.Find() in URP/HDRP
        // and automatically uses the correct default lit shader for the project's render pipeline.
        Material planetMaterial = planetRenderer.material;
        planetMaterial.mainTexture = GenerateTexture();
    }

    /// <summary>
    /// Generates a 2D texture using multiple layers of Perlin noise.
    /// </summary>
    /// <returns>A procedurally generated Texture2D.</returns>
    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        // Use a random seed if the provided one is 0, for variety
        if (seed == 0)
        {
            seed = Random.Range(0, 100000);
        }
        var noiseOffset = new Vector2(seed, seed);

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // Generate a noise value for each pixel using Fractal Brownian Motion (fBm)
                float noiseValue = 0;
                float frequency = 1;
                float amplitude = 1;
                float maxAmplitude = 0;

                for (int i = 0; i < octaves; i++)
                {
                    // Sample Perlin noise at different frequencies
                    float sampleX = x / noiseScale * frequency + noiseOffset.x;
                    float sampleY = y / noiseScale * frequency + noiseOffset.y;
                    
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseValue += perlinValue * amplitude;

                    maxAmplitude += amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                
                // Normalize the noise value to be between 0 and 1
                float normalizedNoise = noiseValue / maxAmplitude;

                // Use the noise value to pick a color from the gradient
                Color pixelColor = colorGradient.Evaluate(normalizedNoise);
                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return texture;
    }
}
