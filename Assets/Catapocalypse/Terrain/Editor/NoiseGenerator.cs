using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Catapocalypse.Terrain
{
    public class NoiseGenerator
    {
        public const int NOISE_IMAGE_SIZE = 1024;

        public readonly NoiseCollection NoiseCollection;

        public RenderTexture OutputTexture;

        private ComputeShader shader;

        public NoiseGenerator(ComputeShader shader, NoiseCollection noiseCollection)
        {
            NoiseCollection = noiseCollection;
            this.shader = shader;
        }

        public Texture2D GenerateNoise()
        {
            OutputTexture = new RenderTexture(NOISE_IMAGE_SIZE, NOISE_IMAGE_SIZE, 0, RenderTextureFormat.RFloat)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat,
            };
            OutputTexture.Create();
            
            var kernelHandle = shader.FindKernel("GenerateNoise");
            
            var layerBuffer = new ComputeBuffer(NoiseCollection.Layers.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(NoiseLayer.NoiseData)));

            var list = new List<NoiseLayer.NoiseData>();
            NoiseCollection.Layers.ForEach(layer => list.Add(layer.data));
            
            layerBuffer.SetData(list);
            
            shader.SetBuffer(kernelHandle, "layers", layerBuffer);
            shader.SetInt( "layerCount", layerBuffer.count);
            shader.SetInts("textureSize", NOISE_IMAGE_SIZE, NOISE_IMAGE_SIZE);
            shader.SetTexture(kernelHandle, "Result", OutputTexture);
            
            // Dispatch the compute shader
            int threadGroups = Mathf.CeilToInt(NOISE_IMAGE_SIZE / 8f);
            shader.Dispatch(kernelHandle, threadGroups, threadGroups, 1);
        
            // Release buffers
            layerBuffer.Release();

            var output = new Texture2D(NOISE_IMAGE_SIZE, NOISE_IMAGE_SIZE, TextureFormat.RFloat, false);
            
            Graphics.CopyTexture(OutputTexture, output);
            
            return output;
        }
    }
}