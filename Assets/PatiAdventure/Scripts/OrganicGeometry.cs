using UnityEngine;
namespace PatiAdventure
{
    public static class OrganicGeometry
    {
        // Each section is z, horizontal radius, vertical radius, vertical offset.
        public static Mesh Sweep(string name,Vector4[] sections,int sides=20)
        {
            int rings=sections.Length;var vertices=new Vector3[rings*(sides+1)];var uv=new Vector2[vertices.Length];
            for(int ring=0;ring<rings;ring++)for(int side=0;side<=sides;side++)
            {
                float a=side*Mathf.PI*2/sides;var s=sections[ring];int i=ring*(sides+1)+side;
                vertices[i]=new Vector3(Mathf.Cos(a)*s.y,Mathf.Sin(a)*s.z+s.w,s.x);uv[i]=new Vector2((float)side/sides,(float)ring/(rings-1));
            }
            var triangles=new int[(rings-1)*sides*6];int t=0;
            for(int ring=0;ring<rings-1;ring++)for(int side=0;side<sides;side++)
            {
                int a=ring*(sides+1)+side,b=a+1,c=a+sides+1,d=c+1;
                triangles[t++]=a;triangles[t++]=b;triangles[t++]=c;triangles[t++]=b;triangles[t++]=d;triangles[t++]=c;
            }
            var mesh=new Mesh{name=name,vertices=vertices,triangles=triangles,uv=uv};mesh.RecalculateNormals();mesh.RecalculateBounds();
            // Close the UV seam without introducing a visible normal split.
            var normals=mesh.normals;for(int ring=0;ring<rings;ring++){int first=ring*(sides+1),last=first+sides;var n=(normals[first]+normals[last]).normalized;normals[first]=normals[last]=n;}mesh.normals=normals;
            return mesh;
        }
        public static Mesh Rounded()
        {
            var sections=new Vector4[13];for(int i=0;i<sections.Length;i++){float a=i*Mathf.PI/12;sections[i]=new Vector4(-Mathf.Cos(a)*.5f,Mathf.Max(.001f,Mathf.Sin(a)*.5f),Mathf.Max(.001f,Mathf.Sin(a)*.5f),0);}return Sweep("Smooth organic volume",sections);
        }
        public static Mesh Crystal()
        {return Sweep("Carved ice crystal",new[]{new Vector4(-.5f,.04f,.04f,0),new Vector4(-.34f,.4f,.4f,0),new Vector4(.28f,.5f,.45f,0),new Vector4(.5f,.1f,.1f,0)},6);}
        public static void SmoothNormals(Mesh mesh)
        {
            var sums=new System.Collections.Generic.Dictionary<Vector3,Vector3>();var points=mesh.vertices;var normals=mesh.normals;
            for(int i=0;i<points.Length;i++){sums.TryGetValue(points[i],out var sum);sums[points[i]]=sum+normals[i];}
            for(int i=0;i<points.Length;i++)normals[i]=sums[points[i]].normalized;mesh.normals=normals;
        }
        public static Vector4[] CurvedSections(Vector4[] controls)
        {
            var result=new Vector4[(controls.Length-1)*3+1];
            for(int i=0;i<controls.Length-1;i++)for(int step=0;step<3;step++)
            {
                float t=step/3f;Vector4 a=controls[Mathf.Max(0,i-1)],b=controls[i],c=controls[i+1],d=controls[Mathf.Min(controls.Length-1,i+2)];
                var value=.5f*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t*t+(-a+3*b-3*c+d)*t*t*t);
                value.y=Mathf.Max(.005f,value.y);value.z=Mathf.Max(.005f,value.z);result[i*3+step]=value;
            }
            result[result.Length-1]=controls[controls.Length-1];return result;
        }
        public static Mesh Ear()
        {
            var v=new[]{new Vector3(-.16f,0,-.08f),new Vector3(.16f,0,-.08f),new Vector3(0,.43f,.01f),new Vector3(-.13f,0,.065f),new Vector3(.13f,0,.065f),new Vector3(0,.39f,.07f)};
            var mesh=new Mesh{name="Triangular canine ear",vertices=v,triangles=new[]{0,2,1,3,4,5,0,3,5,0,5,2,1,2,5,1,5,4,0,1,4,0,4,3},uv=new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.zero,Vector2.right,Vector2.up}};mesh.RecalculateNormals();return mesh;
        }
        public static Mesh Bark(int seed)
        {
            var sections=new Vector4[9];for(int i=0;i<sections.Length;i++){float y=i/8f;float r=(1-y*.35f)*(1+Mathf.Sin(i*2.1f+seed)*.08f);sections[i]=new Vector4(y,r,r,Mathf.Sin(y*2+seed)*.08f);}
            return Sweep("Tapered bark trunk",sections,12);
        }
        public static Mesh Stone(int seed)
        {
            var sections=new Vector4[9];for(int i=0;i<sections.Length;i++){float a=i*Mathf.PI/8,r=Mathf.Max(.01f,Mathf.Sin(a))*(.48f+Mathf.Sin(i*1.7f+seed)*.06f);sections[i]=new Vector4(-Mathf.Cos(a)*.5f,r,r*.8f,Mathf.Sin(i*.5f+seed)*.04f);}return Sweep("Rounded weathered boulder",sections,14);
        }
    }
}
