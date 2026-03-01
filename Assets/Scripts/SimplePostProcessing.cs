using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SimplePostProcessing : MonoBehaviour
{
    [Range(0f, 2f)] public float threshold = 0.8f;
    [Range(0f, 5f)] public float intensity = 1.2f;
    [Range(0f, 2f)] public float saturation = 1.2f;
    [Range(0, 4)] public int iterations = 2;
    [Range(0, 2)] public int downsample = 1;

    private Material material;
    private Shader shader;

    void OnEnable()
    {
        shader = Shader.Find("Hidden/SimpleBloom");
        if (shader == null)
        {
            Debug.LogWarning("SimplePostProcessing: 'Hidden/SimpleBloom' shader not found.");
            enabled = false;
            return;
        }
        material = new Material(shader);
    }

    void OnDisable()
    {
        if (material) DestroyImmediate(material);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        material.SetFloat("_Threshold", threshold);
        material.SetFloat("_Intensity", intensity);
        material.SetFloat("_Saturation", saturation);

        int width = src.width >> downsample;
        int height = src.height >> downsample;
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);

        RenderTexture current = RenderTexture.GetTemporary(width, height, 0, src.format);
        // Prefilter (threshold)
        Graphics.Blit(src, current, material, 0);

        for (int i = 0; i < iterations; i++)
        {
            // Horizontal blur
            RenderTexture temp = RenderTexture.GetTemporary(width, height, 0, src.format);
            material.SetVector("_BlurDirection", new Vector2(1f, 0f));
            Graphics.Blit(current, temp, material, 1);
            RenderTexture.ReleaseTemporary(current);
            current = temp;

            // Vertical blur
            temp = RenderTexture.GetTemporary(width, height, 0, src.format);
            material.SetVector("_BlurDirection", new Vector2(0f, 1f));
            Graphics.Blit(current, temp, material, 1);
            RenderTexture.ReleaseTemporary(current);
            current = temp;
        }

        material.SetTexture("_BloomTex", current);
        Graphics.Blit(src, dest, material, 2);

        RenderTexture.ReleaseTemporary(current);
    }
}
