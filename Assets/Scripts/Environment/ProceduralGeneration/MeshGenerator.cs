using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GenerateGreedyMesh(HashSet<Vector3Int> positions, float tileSize, int gridSize)
    {
        // Находим мин/макс границы для оптимизации
        int minX = int.MaxValue, maxX = int.MinValue;
        int minZ = int.MaxValue, maxZ = int.MinValue; // Используем Z вместо Y, если это 3D, но в вашем коде Y=0
        foreach (var pos in positions)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minZ = Mathf.Min(minZ, pos.z);
            maxZ = Mathf.Max(maxZ, pos.z);
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); // Опционально для текстур

        // Greedy: проходим по строкам (Z) и объединяем горизонтальные полосы
        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (!positions.Contains(new Vector3Int(x, 0, z))) continue;

                // Находим длину горизонтальной полосы
                int width = 0;
                while (x + width <= maxX && positions.Contains(new Vector3Int(x + width, 0, z)))
                {
                    width += gridSize;
                }

                // Находим высоту (вертикальную полосу вниз по Z, но для простоты — только горизонтально)
                int height = 0;
                bool canMergeDown = true;
                while (canMergeDown && z + height <= maxZ)
                {
                    for (int w = 0; w < width; w += gridSize)
                    {
                        if (!positions.Contains(new Vector3Int(x + w, 0, z + height)))
                        {
                            canMergeDown = false;
                            break;
                        }
                    }
                    if (canMergeDown) height += gridSize;
                }

                // Добавляем квад (quad) для этой полосы
                int vertIndex = vertices.Count;
                vertices.Add(new Vector3(x * tileSize, 0, (z + height) * tileSize)); // Top-left
                vertices.Add(new Vector3((x + width) * tileSize, 0, (z + height) * tileSize)); // Top-right
                vertices.Add(new Vector3((x + width) * tileSize, 0, z * tileSize)); // Bottom-right
                vertices.Add(new Vector3(x * tileSize, 0, z * tileSize)); // Bottom-left

                // Треугольники (два на quad)
                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 3);

                // UV (опционально, для текстур)
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(width, 0));
                uvs.Add(new Vector2(width, height));
                uvs.Add(new Vector2(0, height));

                // Удаляем обработанные позиции, чтобы не повторять
                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        positions.Remove(new Vector3Int(x + w, 0, z + h));
                    }
                }

                x += width - 1; // Пропускаем обработанную полосу
            }
        }

        // Создаем mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}