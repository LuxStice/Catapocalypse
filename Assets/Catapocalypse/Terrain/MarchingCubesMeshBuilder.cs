using Unity.Mathematics;
using UnityEngine;

namespace Catapocalypse.Terrain
{
    public static class MarchingCubesMeshBuilder
    {
        const int MaxVerts = 65536;

        public static Mesh GenerateMeshFromCompute(MarchingCubesSettings settings, float3 chunkOffset)
        {
            var cs = settings.marchingCubesShader;
            int kernel = cs.FindKernel("GenerateMesh");

            var vertexBuffer = new ComputeBuffer(MaxVerts, sizeof(float) * 3);
            var triangleBuffer = new ComputeBuffer(MaxVerts, sizeof(int));
            var vertexCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

            cs.SetFloat("_IsoLevel", settings.isoLevel);
            cs.SetFloat("_VoxelSize", settings.voxelSize);
            cs.SetInts("_ChunkSize", settings.chunkSize.x, settings.chunkSize.y, settings.chunkSize.z);
            cs.SetVector("_ChunkOffset", chunkOffset);

            int[] zero = new int[1] { 0 };
            vertexCountBuffer.SetData(zero);

            cs.SetBuffer(kernel, "Vertices", vertexBuffer);
            cs.SetBuffer(kernel, "Triangles", triangleBuffer);
            cs.SetBuffer(kernel, "VertexCount", vertexCountBuffer);

            int3 dispatch = (int3)Mathf.CeilToInt(settings.chunkSize.x / 4f);
            cs.Dispatch(kernel, dispatch.x, dispatch.y, dispatch.z);

            ComputeBuffer countReadback = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
            Graphics.CopyBuffer(vertexCountBuffer, 0, countReadback, 0, sizeof(int));
            int[] count = new int[1];
            countReadback.GetData(count);
            countReadback.Release();

            int vertCount = count[0];
            if (vertCount == 0)
            {
                vertexBuffer.Release();
                triangleBuffer.Release();
                vertexCountBuffer.Release();
                return null;
            }

            var verts = new Vector3[vertCount];
            var tris = new int[vertCount];
            vertexBuffer.GetData(verts, 0, 0, vertCount);
            triangleBuffer.GetData(tris, 0, 0, vertCount);

            vertexBuffer.Release();
            triangleBuffer.Release();
            vertexCountBuffer.Release();

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}