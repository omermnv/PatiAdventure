using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace PatiAdventure.Tests
{
    public sealed class NaturalLandscapeTests
    {
        [TestCase(83)]
        [TestCase(51)]
        [TestCase(7)]
        public void RidgeIsClosedBoundedAndHasValidColliderFaces(int seed)
        {
            var mesh=NaturalGeometry.Ridge(seed);
            try
            {
                Assert.Less(mesh.vertexCount,2000,"Shared mountain meshes must remain suitable for mobile.");
                Assert.Greater(mesh.bounds.max.y,.95f);
                Assert.LessOrEqual(mesh.bounds.max.y,1.3f);
                Assert.AreEqual(0,mesh.bounds.min.y,.00001f);
                Assert.LessOrEqual(mesh.bounds.size.x,2.1f);
                Assert.LessOrEqual(mesh.bounds.size.z,2.1f);
                var points=mesh.vertices;var faces=mesh.triangles;var edges=new Dictionary<ulong,int>();
                Assert.AreEqual(points.Length,mesh.uv.Length);
                Assert.AreEqual(points.Length,mesh.normals.Length);
                for(int i=0;i<faces.Length;i+=3)
                {
                    int a=faces[i],b=faces[i+1],c=faces[i+2];
                    Assert.That(a,Is.InRange(0,points.Length-1));Assert.That(b,Is.InRange(0,points.Length-1));Assert.That(c,Is.InRange(0,points.Length-1));
                    Assert.Greater(Vector3.Cross(points[b]-points[a],points[c]-points[a]).sqrMagnitude,1e-12f,"A collider face cannot have zero area.");
                    CountEdge(edges,a,b);CountEdge(edges,b,c);CountEdge(edges,c,a);
                }
                foreach(var count in edges.Values)Assert.AreEqual(2,count,"Every edge must have two faces, including the ground cap.");
                foreach(var normal in mesh.normals)
                {
                    Assert.IsFalse(float.IsNaN(normal.x)||float.IsNaN(normal.y)||float.IsNaN(normal.z));
                    Assert.Greater(normal.sqrMagnitude,.5f);
                }
            }
            finally{Object.DestroyImmediate(mesh);}
        }

        [Test]
        public void RidgeSeedsAreStableAndGiveDifferentSilhouettes()
        {
            var first=NaturalGeometry.Ridge(83);var repeat=NaturalGeometry.Ridge(83);var other=NaturalGeometry.Ridge(51);
            try
            {
                CollectionAssert.AreEqual(first.vertices,repeat.vertices);
                CollectionAssert.AreEqual(first.triangles,repeat.triangles);
                CollectionAssert.AreNotEqual(first.vertices,other.vertices);
            }
            finally{Object.DestroyImmediate(first);Object.DestroyImmediate(repeat);Object.DestroyImmediate(other);}
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void LandscapeTextureRepeatsWithoutASeamAndKeepsBiomeMaterialColour(int kind)
        {
            var texture=LivingSurface.Ground(kind);
            try
            {
                Assert.That(texture.width,Is.LessThanOrEqualTo(512));Assert.AreEqual(texture.width,texture.height);
                Assert.AreEqual(TextureFormat.RGB24,texture.format);Assert.AreEqual(TextureWrapMode.Repeat,texture.wrapMode);
                Assert.Greater(texture.mipmapCount,1);
                var pixels=texture.GetPixels32();int size=texture.width;byte lowest=255,highest=0;
                for(int i=0;i<size;i++)
                {
                    Assert.AreEqual(pixels[i*size],pixels[i*size+size-1],"Horizontal repeat edge.");
                    Assert.AreEqual(pixels[i],pixels[(size-1)*size+i],"Vertical repeat edge.");
                }
                foreach(var pixel in pixels)
                {
                    Assert.AreEqual(pixel.r,pixel.g);Assert.AreEqual(pixel.g,pixel.b);
                    if(pixel.r<lowest)lowest=pixel.r;if(pixel.r>highest)highest=pixel.r;
                }
                Assert.Greater(highest-lowest,4,"The texture must contain visible fine detail.");
            }
            finally{Object.DestroyImmediate(texture);}
        }

        private static void CountEdge(Dictionary<ulong,int> edges,int a,int b)
        {
            uint low=(uint)Mathf.Min(a,b),high=(uint)Mathf.Max(a,b);ulong key=((ulong)low<<32)|high;
            edges.TryGetValue(key,out int count);edges[key]=count+1;
        }
    }
}
