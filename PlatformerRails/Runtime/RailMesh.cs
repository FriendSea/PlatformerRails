using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlatformerRails
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RailMesh : MonoBehaviour
    {
        [SerializeField]
        RailBehaviour railBehaviour;
        [SerializeField]
        Mesh original;

        [SerializeField, Space]
        Vector3 Scale = Vector3.one;
        [SerializeField]
        Vector3 OffSet;
        [SerializeField]
        Vector3 Span;
        [SerializeField]
        int Loop;

        /*
        [SerializeField, Space]
        bool InvertYZ;
        */

        void Start()
        {
            Generate();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
            Generate();
        }

        void Reset()
        {
            transform.position = Vector3.zero;
        }
#endif

        void Generate()
        {
            IRail Rail = railBehaviour as IRail;
            if (Rail == null) Rail = RailManager.instance;
            Mesh mesh = new Mesh();
            Vector3[] originalverts = original.vertices.ToArray();
            Vector3[] originalnorms = original.normals.ToArray();
            /*
            if (InvertYZ)
            {
                originalverts = originalverts.Select((vec) => { return new Vector3(-vec.x, vec.z, vec.y); }).ToArray();
                originalnorms = originalnorms.Select((vec) => { return new Vector3(-vec.x, vec.z, vec.y); }).ToArray();
            }
            */
            List<Vector3> verts = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();

            for (int i = 0; i < Loop; i++)
            {
                verts.AddRange(originalverts.Select((Vector3 vec) =>
                {
                    vec.Scale(Scale);
                    vec += OffSet + Span * i;
                    vec.y += Rail.Height(vec.z);
                    return Rail.Local2World(vec);
                }));
                triangles.AddRange(original.triangles.Select((int x) => { return x + originalverts.Length * i; }));
                uvs.AddRange(original.uv.ToList());
                normals.AddRange(originalnorms.Select((norm, index) =>
                {
                    Quaternion qt = Rail.Rotation(originalverts[index].z + OffSet.z + Span.z * i);
                    return qt * norm;
                }));
                colors.AddRange(original.colors.ToList());
            }
            mesh.vertices = verts.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateBounds();
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            MeshCollider coll = GetComponent<MeshCollider>();
            if (coll != null) coll.sharedMesh = mesh;
        }
    }
}

