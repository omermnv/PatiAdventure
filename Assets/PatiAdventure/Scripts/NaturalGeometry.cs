using UnityEngine;
namespace PatiAdventure
{
    public static class NaturalGeometry
    {
        public static Mesh LeafyCanopy(int seed,int count=440)
        {
            var random=new System.Random(seed);var vertices=new Vector3[count*10];var uv=new Vector2[vertices.Length];
            var dark=new System.Collections.Generic.List<int>();var bright=new System.Collections.Generic.List<int>();
            Vector3[] leaf={new Vector3(-.18f,0,0),new Vector3(0,0,-.3f),new Vector3(.18f,0,0),new Vector3(0,.065f,0),new Vector3(0,0,.3f)};
            int[] face={1,0,3,1,3,2,0,4,3,3,4,2};
            for(int i=0;i<count;i++)
            {
                Vector3 centre;
                do{centre=new Vector3((float)random.NextDouble()*2-1,(float)random.NextDouble()*2-1,(float)random.NextDouble()*2-1);}while(centre.sqrMagnitude>1);
                centre=Vector3.Scale(centre,new Vector3(2.2f,1.3f,1.9f));
                var rotation=Quaternion.Euler((float)random.NextDouble()*75-35,(float)random.NextDouble()*360,(float)random.NextDouble()*60-30);float scale=.8f+(float)random.NextDouble()*.6f;
                int start=i*10;for(int v=0;v<10;v++){vertices[start+v]=centre+rotation*(leaf[v%5]*scale);uv[start+v]=new Vector2(leaf[v%5].x/.36f+.5f,leaf[v%5].z/.6f+.5f);}
                var triangles=i%4==0?bright:dark;
                for(int t=0;t<face.Length;t+=3){triangles.Add(start+face[t]);triangles.Add(start+face[t+1]);triangles.Add(start+face[t+2]);triangles.Add(start+5+face[t]);triangles.Add(start+5+face[t+2]);triangles.Add(start+5+face[t+1]);}
            }
            var mesh=new Mesh{name="Individual leaves canopy",vertices=vertices,uv=uv,subMeshCount=2};mesh.SetTriangles(dark,0);mesh.SetTriangles(bright,1);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        public static Mesh Rock(int seed,bool hill=false)
        {
            const int sides=12,rings=7;
            var points=new Vector3[sides*(rings+1)];
            for(int ring=0;ring<=rings;ring++)
            {
                float v=(float)ring/rings;
                float radius=hill?Mathf.Cos(v*Mathf.PI*.5f):Mathf.Sin(v*Mathf.PI);
                for(int s=0;s<sides;s++)
                {
                    float a=s*Mathf.PI*2/sides;
                    float noise=Mathf.PerlinNoise(seed*.17f+s*.61f,ring*.7f+seed*.09f);
                    float r=radius*(.78f+noise*.38f);
                    points[ring*sides+s]=new Vector3(Mathf.Cos(a)*r,hill?v: v-.5f,Mathf.Sin(a)*r);
                }
            }
            var vertices=new Vector3[sides*rings*6];var triangles=new int[vertices.Length];int t=0;
            for(int ring=0;ring<rings;ring++)for(int s=0;s<sides;s++)
            {
                int a=ring*sides+s,b=ring*sides+(s+1)%sides,c=a+sides,d=b+sides;
                vertices[t]=points[a];vertices[t+1]=points[c];vertices[t+2]=points[b];
                vertices[t+3]=points[b];vertices[t+4]=points[c];vertices[t+5]=points[d];t+=6;
            }
            var uv=new Vector2[vertices.Length];
            for(int i=0;i<vertices.Length;i++){triangles[i]=i;uv[i]=new Vector2(vertices[i].x,vertices[i].z);}
            var mesh=new Mesh{name=hill?"Sculpted ridge":"Weathered stone",vertices=vertices,triangles=triangles,uv=uv};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        public static Mesh Cone()
        {
            const int sides=12;var vertices=new Vector3[sides*3];var uv=new Vector2[vertices.Length];var triangles=new int[vertices.Length];
            for(int i=0;i<sides;i++)
            {
                float a=i*Mathf.PI*2/sides,b=(i+1)*Mathf.PI*2/sides;int k=i*3;
                vertices[k]=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));vertices[k+1]=Vector3.up;vertices[k+2]=new Vector3(Mathf.Cos(b),0,Mathf.Sin(b));
                uv[k]=new Vector2(0,0);uv[k+1]=new Vector2(.5f,1);uv[k+2]=new Vector2(1,0);
                triangles[k]=k;triangles[k+1]=k+1;triangles[k+2]=k+2;
            }
            var mesh=new Mesh{name="Pine crown",vertices=vertices,triangles=triangles,uv=uv};mesh.RecalculateNormals();return mesh;
        }
        public static Mesh Ridge(int seed)
        {
            // A closed radial height field keeps the shared distant mesh inexpensive.
            // Offset conical peaks create saddles and sharp summits instead of a dome.
            const int sides=48,rings=21;
            var random=new System.Random(seed);float turn=(float)random.NextDouble()*Mathf.PI*2;
            var peaks=new Vector3[4];
            Vector3[] layout={new Vector3(-.16f,1.28f,.03f),new Vector3(.38f,1.04f,.26f),new Vector3(-.4f,.91f,-.38f),new Vector3(.2f,.76f,-.44f)};
            for(int i=0;i<peaks.Length;i++)
            {
                var p=layout[i];p.x+=((float)random.NextDouble()-.5f)*.08f;p.z+=((float)random.NextDouble()-.5f)*.08f;
                p.y*=.95f+(float)random.NextDouble()*.05f;
                peaks[i]=new Vector3(p.x*Mathf.Cos(turn)-p.z*Mathf.Sin(turn),p.y,p.x*Mathf.Sin(turn)+p.z*Mathf.Cos(turn));
            }
            var vertices=new Vector3[sides*rings+2];var uv=new Vector2[vertices.Length];
            vertices[0]=new Vector3(0,RidgeHeight(0,0,0,seed,peaks),0);uv[0]=Vector2.one*.5f;
            for(int ring=1;ring<=rings;ring++)for(int side=0;side<sides;side++)
            {
                float radius=ring/(float)rings,angle=side*Mathf.PI*2/sides;
                float outline=.94f+.035f*Mathf.Sin(angle*3+turn)+.04f*Mathf.Cos(angle*5-turn);
                float x=Mathf.Cos(angle)*radius*outline,z=Mathf.Sin(angle)*radius*outline;
                int index=1+(ring-1)*sides+side;
                vertices[index]=new Vector3(x,RidgeHeight(x,z,radius,seed,peaks),z);uv[index]=new Vector2(x*.5f+.5f,z*.5f+.5f);
            }
            int bottom=vertices.Length-1;vertices[bottom]=Vector3.zero;uv[bottom]=Vector2.one*.5f;
            var triangles=new int[sides*rings*6];int t=0;
            for(int side=0;side<sides;side++)
            {
                int next=(side+1)%sides;
                triangles[t++]=0;triangles[t++]=1+next;triangles[t++]=1+side;
                for(int ring=1;ring<rings;ring++)
                {
                    int a=1+(ring-1)*sides+side,b=1+(ring-1)*sides+next,c=a+sides,d=b+sides;
                    triangles[t++]=a;triangles[t++]=b;triangles[t++]=c;
                    triangles[t++]=b;triangles[t++]=d;triangles[t++]=c;
                }
                triangles[t++]=bottom;triangles[t++]=1+(rings-1)*sides+side;triangles[t++]=1+(rings-1)*sides+next;
            }
            var mesh=new Mesh{name="Eroded multiple summit ridge",vertices=vertices,uv=uv,triangles=triangles};
            mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        private static float RidgeHeight(float x,float z,float radius,int seed,Vector3[] peaks)
        {
            float height=0;int summit=0;
            for(int i=0;i<peaks.Length;i++)
            {
                var p=peaks[i];float dx=x-p.x,dz=(z-p.z)*(i%2==0?1.05f:.92f);
                float candidate=p.y-Mathf.Sqrt(dx*dx+dz*dz)*(i==0?1.72f:1.85f);
                if(candidate>height){height=candidate;summit=i;}
            }
            var peak=peaks[summit];float angle=Mathf.Atan2(z-peak.z,x-peak.x),distance=new Vector2(x-peak.x,z-peak.z).magnitude;
            float gullies=(.5f+.5f*Mathf.Sin(angle*11+distance*4+seed))*(.5f+.5f*Mathf.Sin(angle*19-distance*3));
            float weather=Mathf.PerlinNoise(x*6+seed*.31f,z*6+seed*.19f)-.5f;
            height-=gullies*Mathf.Min(distance,.45f)*.19f;
            height+=weather*.045f*Mathf.Clamp01(distance*4);
            return Mathf.Max(0,height)*Mathf.SmoothStep(0,1,(1-radius)/.14f);
        }
        public static Texture2D Surface(int kind)
        {
            const int size=128;var pixels=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float n=Mathf.PerlinNoise(x*.14f+kind*31,y*.14f+kind*19);
                float grain=Mathf.PerlinNoise(x*.8f,y*.8f)*.12f;
                float value=kind==1?.62f+Mathf.PerlinNoise(x*.4f,y*.028f)*.38f:.7f+n*.28f+grain;
                if(kind>=3)value=.9f+n*.08f+grain*.15f;
                pixels[y*size+x]=new Color(value,value,value,1);
            }
            var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="Natural surface "+kind,wrapMode=TextureWrapMode.Repeat};texture.SetPixels(pixels);texture.Apply();return texture;
        }
    }
}
