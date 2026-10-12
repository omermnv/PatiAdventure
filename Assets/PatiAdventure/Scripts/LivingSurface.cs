using UnityEngine;
namespace PatiAdventure
{
    public static class LivingSurface
    {
        public static Texture2D Leaf()
        {
            const int width=128,height=256;var pixels=new Color[width*height];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)
            {
                float across=Mathf.Abs(x/(float)(width-1)*2-1),along=y/(float)(height-1);
                float midrib=Mathf.Exp(-across*80),phase=Mathf.Repeat(along*8-across*1.75f,1);
                float veins=Mathf.Exp(-Mathf.Min(phase,1-phase)*65);
                float value=.64f+Mathf.PerlinNoise(x*.035f,y*.035f)*.19f+Mathf.PerlinNoise(x*.35f,y*.35f)*.08f+midrib*.12f+veins*.09f;
                pixels[y*width+x]=new Color(value,value,value,1);
            }
            var texture=new Texture2D(width,height,TextureFormat.RGB24,true){name="Leaf veins and grain",wrapMode=TextureWrapMode.Clamp};texture.SetPixels(pixels);texture.Apply();return texture;
        }
        public static Texture2D Fur()
        {
            const int size=256;var colors=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float strands=Mathf.PerlinNoise(x*.65f,y*.045f),grain=Mathf.PerlinNoise(x*.24f,y*.2f);
                float value=.9f+strands*.07f+grain*.03f;colors[y*size+x]=new Color(value,value,value,1);
            }
            var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="Fine fur strands",wrapMode=TextureWrapMode.Repeat};texture.SetPixels(colors);texture.Apply();return texture;
        }
        public static Texture2D Ground(int kind)
        {
            const int size=256;var colors=new Color[size*size];
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float u=x/(float)(size-1),v=y/(float)(size-1);
                float macro=GroundNoise(u,v,3,kind*17+11),detail=GroundNoise(u,v,12,kind*19+53),grain=GroundNoise(u,v,70,kind*23+97);
                float value;
                if(kind==0)value=.79f+macro*.1f+detail*.06f+grain*.05f;
                else if(kind==1)
                {
                    float ripples=.5f+.5f*Mathf.Sin(Mathf.PI*2*(v*9+u*2)+macro*4);
                    value=.82f+macro*.08f+grain*.06f+ripples*.025f;
                }
                else
                {
                    float drift=.5f+.5f*Mathf.Sin(Mathf.PI*2*(u*3-v*2)+detail*2);
                    value=.91f+macro*.045f+grain*.025f+drift*.015f;
                }
                colors[y*size+x]=new Color(value,value,value,1);
            }
            // Small blades, grit and ice grains give close views readable detail.
            // Stamp through the repeat boundary, then duplicate the edge texels.
            var random=new System.Random(713+kind*47);int count=kind==0?520:kind==1?440:180;
            for(int i=0;i<count;i++)
            {
                int x=random.Next(size-1),y=random.Next(size-1),length=kind==0?random.Next(2,6):1;
                float angle=(float)random.NextDouble()*Mathf.PI*2;
                float value=kind==0?.64f+(float)random.NextDouble()*.2f:kind==1?.72f+(float)random.NextDouble()*.19f:.97f+(float)random.NextDouble()*.025f;
                for(int j=0;j<length;j++)
                {
                    int px=(x+Mathf.RoundToInt(Mathf.Cos(angle)*j)+size-1)%(size-1),py=(y+Mathf.RoundToInt(Mathf.Sin(angle)*j)+size-1)%(size-1);
                    colors[py*size+px]=new Color(value,value,value,1);
                }
            }
            for(int y=0;y<size-1;y++)colors[y*size+size-1]=colors[y*size];
            for(int x=0;x<size;x++)colors[(size-1)*size+x]=colors[x];
            var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="Fine seamless landscape soil "+kind,wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Trilinear};texture.SetPixels(colors);texture.Apply();return texture;
        }
        private static float GroundNoise(float u,float v,float frequency,float seed)
        {
            float x=u*frequency+seed,y=v*frequency+seed*.63f;
            float across=Mathf.SmoothStep(0,1,u),along=Mathf.SmoothStep(0,1,v);
            return Mathf.Lerp(Mathf.Lerp(Mathf.PerlinNoise(x,y),Mathf.PerlinNoise(x-frequency,y),across),Mathf.Lerp(Mathf.PerlinNoise(x,y-frequency),Mathf.PerlinNoise(x-frequency,y-frequency),across),along);
        }
    }
}
