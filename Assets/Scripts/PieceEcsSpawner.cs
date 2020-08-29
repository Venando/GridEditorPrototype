using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[Serializable]
public class PieceEcsSpawner
{
    public Renderer TestRender;
    public Material PieceMaterial;
    
    private EntityManager _entityManager;

    private EntityManager EntityManager =>
        _entityManager ?? (_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager);

    private readonly List<Entity> _pieceList = new List<Entity>();

    public void DeleteAll()
    {
        foreach (var entity in _pieceList)
            EntityManager.DestroyEntity(entity);
        _pieceList.Clear();
    }

    public void SetVisible(bool visible)
    {
        foreach (var entity in _pieceList)
        {
            _entityManager.SetEnabled(entity, visible);
        }
    }
    
    public void SpawnPiece(Vector3 position, Color color, Vector2Int size, float cellSize)
    {
        var entity = EntityManager.CreateEntity(typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderBounds));

        TestRender.material = PieceMaterial;
        TestRender.material.color = color;
        TestRender.material.mainTextureScale = new Vector2(size.x, size.y);
        
        EntityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = CreateMesh(size, cellSize),
            material = TestRender.material
        });
        EntityManager.SetComponentData(entity, new Translation
        {
            Value = new float3(position.x, position.y, position.z)
        });
        
        _pieceList.Add(entity);
    }

    private Mesh CreateMesh(Vector2Int size, float cellSize)
    {
        var vertices = new Vector3[4];
        var uv = new Vector2[4];
        var triangles = new int[6];
        
        var width = cellSize * size.x;
        var height = cellSize * size.y;
        vertices[0] = new Vector3(0f, 0f);
        vertices[1] = new Vector3(0f, height);
        vertices[2] = new Vector3(width, height);
        vertices[3] = new Vector3(width, 0f);
        
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        var mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };

        return mesh;
    }
}
