using UnityEngine;
namespace PatiAdventure
{
    public static class LandscapeLookGeometry
    {
        public static Mesh PineBoughs()
        {
            const int levels=7,spokes=9;var points=new Vector3[levels*spokes*10];var uv=new Vector2[points.Length];
            var green=new System.Collections.Generic.List<int>();var snow=new System.Collections.Generic.List<int>();int[] fan={0,1,4,1,2,4,2,3,4,3,0,4};
            for(int level=0;level<levels;level++)for(int spoke=0;spoke<spokes;spoke++)
            {
                float a=spoke*Mathf.PI*2/spokes+level*.36f,reach=1.3f*(1-level/8f);var forward=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));var side=Vector3.Cross(Vector3.up,forward);var root=Vector3.up*(.3f+level*.43f);
                Vector3[] frond={root+forward*.06f,root+forward*reach*.5f+side*reach*.23f-Vector3.up*.12f,root+forward*reach-Vector3.up*.3f,root+forward*reach*.5f-side*reach*.23f-Vector3.up*.12f,root+forward*reach*.53f-Vector3.up*.045f};
                int start=(level*spokes+spoke)*10;for(int i=0;i<10;i++){points[start+i]=frond[i%5];uv[start+i]=new Vector2(i%5*.25f,level/6f);}
                var top=level==levels-1||spoke%3==0?snow:green;
                for(int t=0;t<fan.Length;t+=3)
                {
                    int p=fan[t],q=fan[t+1],r=fan[t+2];if(Vector3.Cross(frond[q]-frond[p],frond[r]-frond[p]).y<0){int swap=q;q=r;r=swap;}
                    top.Add(start+p);top.Add(start+q);top.Add(start+r);green.Add(start+5+p);green.Add(start+5+r);green.Add(start+5+q);
                }
            }
            var mesh=new Mesh{name="Snow laden alpine boughs",vertices=points,uv=uv,subMeshCount=2};mesh.SetTriangles(green,0);mesh.SetTriangles(snow,1);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        public static Mesh Foothills(float width,int seed)
        {
            const int cells=40;int row=cells+1;var points=new Vector3[row*row];var uv=new Vector2[points.Length];var triangles=new int[cells*cells*6];
            for(int z=0;z<=cells;z++)for(int x=0;x<=cells;x++)
            {
                float px=(x/(float)cells-.5f)*width*3,pz=(z/(float)cells-.5f)*width*3;
                float outside=Mathf.Max(Mathf.Abs(px),Mathf.Abs(pz))-(width*.5f+1);
                float blend=Mathf.SmoothStep(0,1,Mathf.Clamp01(outside/(width*.35f)));
                float hills=.7f+Mathf.PerlinNoise(px*.035f+seed*3.7f,pz*.035f+12)*2.8f;
                int i=z*row+x;points[i]=new Vector3(px,-.085f+blend*hills,pz);uv[i]=new Vector2(px/width+.5f,pz/width+.5f);
            }
            int t=0;for(int z=0;z<cells;z++)for(int x=0;x<cells;x++)
            {int a=z*row+x,b=a+1,c=a+row,d=c+1;triangles[t++]=a;triangles[t++]=c;triangles[t++]=b;triangles[t++]=b;triangles[t++]=c;triangles[t++]=d;}
            var mesh=new Mesh{name="Continuous foothills terrain",vertices=points,uv=uv,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        public static Mesh WaterDisc(int seed)
        {
            const int sides=64;var points=new Vector3[sides+2];var uv=new Vector2[points.Length];var triangles=new int[sides*3];uv[0]=Vector2.one*.5f;
            for(int i=0;i<=sides;i++)
            {float a=i*Mathf.PI*2/sides,r=.5f*(1+Mathf.Sin(a*3+seed)*.035f+Mathf.Cos(a*5)*.02f);var p=new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r);points[i+1]=p;uv[i+1]=new Vector2(p.x+.5f,p.z+.5f);if(i<sides){triangles[i*3]=0;triangles[i*3+1]=i+2;triangles[i*3+2]=i+1;}}
            var mesh=new Mesh{name="Natural shoreline water",vertices=points,uv=uv,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        public static Texture2D Ripples()
        {
            const int size=128;var pixels=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {float u=x/(float)(size-1)*Mathf.PI*2,v=y/(float)(size-1)*Mathf.PI*2;float wave=Mathf.Sin(u*4+Mathf.Sin(v*3)*.4f)*Mathf.Cos(v*5+Mathf.Sin(u*2)*.2f);float light=.85f+wave*.08f;pixels[y*size+x]=new Color(light*.83f,light*.97f,light,1);}
            var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="Fine flowing water ripples",wrapMode=TextureWrapMode.Repeat};texture.SetPixels(pixels);texture.Apply();return texture;
        }
    }
}
