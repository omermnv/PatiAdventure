using UnityEngine;
using UnityEngine.Rendering;

namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        private void DressStorybookMeadow(Material foliage, Material gold)
        {
            if (region > 1) return;
            var stems = new DetailBatch();
            var centres = new DetailBatch();
            var petalMesh = OrganicGeometry.Sweep("Small flower petal", new[] {
                new Vector4(-.5f,.01f,.01f,0), new Vector4(-.25f,.4f,.3f,0),
                new Vector4(0,.5f,.4f,0), new Vector4(.3f,.35f,.25f,0), new Vector4(.5f,.01f,.01f,0) },6);
            var stemMesh = OrganicGeometry.Sweep("Small flower stem", new[] {
                new Vector4(0,.5f,.5f,0), new Vector4(1,.35f,.35f,0) },4);
            meshes.Add(petalMesh);
            meshes.Add(stemMesh);
            Color[] palette = { new Color(.72f,.40f,.78f), new Color(1f,.69f,.38f), new Color(.96f,.76f,.83f) };
            var flowers = new[] { new DetailBatch(), new DetailBatch(), new DetailBatch() };
            // Deterministic islands of colour along the meadow edges leave paths and spawn clear.
            for (int cluster = 0; cluster < 32; cluster++)
            {
                float angle = cluster * 2.39996f;
                float radius = 10f + cluster % 5 * 2.1f;
                var origin = new Vector3(Mathf.Sin(angle) * radius, -.035f, 17 + Mathf.Cos(angle) * radius);
                for (int flower = 0; flower < 7; flower++)
                {
                    float a = flower * 2.39996f + cluster;
                    var p = origin + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * (.25f + flower * .13f);
                    float height = .28f + (flower + cluster) % 4 * .09f;
                    stems.Add(stemMesh, p, new Vector3(.025f,.025f,height), Quaternion.Euler(-90,0,0));
                    stems.Add(livingLeaf, p + Vector3.up * height * .45f, new Vector3(.15f,.08f,.27f), Quaternion.Euler(0,flower*73,25));
                    var head = p + Vector3.up * height;
                    for (int petal = 0; petal < 5; petal++)
                    {
                        float turn = petal * Mathf.PI * 2 / 5 + a;
                        flowers[cluster % 3].Add(petalMesh, head + new Vector3(Mathf.Cos(turn),.06f,Mathf.Sin(turn))*.11f,
                            new Vector3(.18f,.055f,.105f), Quaternion.Euler(0,-turn*Mathf.Rad2Deg,0));
                    }
                    centres.Add(petalMesh, head + Vector3.up * .035f, new Vector3(.10f,.065f,.10f), Quaternion.identity);
                }
            }
            stems.Bake(this,"Wildflower garden stems",foliage);
            centres.Bake(this,"Wildflower garden hearts",gold);
            for (int i=0;i<flowers.Length;i++) flowers[i].Bake(this,"Wildflower garden petals " + i,ColorMaterial(palette[i]));
        }

        private void BuildRegionalMotes(Material snow, Material gold)
        {
            var obj = new GameObject(region == 3 ? "Drifting snowflakes" : region == 2 ? "Canyon dust motes" : "Sunlit floating pollen");
            obj.transform.SetParent(world,false);
            obj.transform.localPosition = new Vector3(0,region == 3 ? 8 : 1.8f,17);
            var particles = obj.AddComponent<ParticleSystem>();
            particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.useAutoRandomSeed = false;
            particles.randomSeed = (uint)(73 + region);
            var main = particles.main;
            main.maxParticles = 64;
            main.startLifetime = 12f;
            main.startSpeed = 0;
            main.startSize = new ParticleSystem.MinMaxCurve(.025f,region == 3 ? .085f : .055f);
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.prewarm = true;
            var emission = particles.emission;
            emission.rateOverTime = 5f;
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(38,region == 3 ? 7 : 3,44);
            var velocity = particles.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.x = new ParticleSystem.MinMaxCurve(.08f,.22f);
            velocity.y = new ParticleSystem.MinMaxCurve(region == 3 ? -.48f : .04f,region == 3 ? -.24f : .12f);
            velocity.z = new ParticleSystem.MinMaxCurve(-.08f,.08f);
            var size = particles.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1,new AnimationCurve(new Keyframe(0,0),new Keyframe(.15f,1),new Keyframe(.8f,1),new Keyframe(1,0)));
            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            renderer.mesh = livingRound;
            renderer.sharedMaterial = region == 3 ? snow : gold;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            particles.Play();
        }
    }
}
