using UnityEngine;
namespace PatiAdventure
{
    public sealed partial class AdventureDirector
    {
        private void BuildFoothillLandscape(float width,Material ground)
        {
            var mesh=LandscapeLookGeometry.Foothills(width,region+11);meshes.Add(mesh);
            MeshObject("Continuous foothill landscape",mesh,new Vector3(0,0,17),Vector3.one,ground,true);
        }
        private void LivingAlpineTree(Vector3 p,Mesh needles,Material wood,Material foliage,Material snow,int index)
        {
            Shape("Alpine pine trunk",PrimitiveType.Cylinder,p+Vector3.up*1.5f,new Vector3(.4f,1.5f,.4f),wood);
            var crown=MeshObject("Snowy pine boughs",needles,p+Vector3.up*1.35f,new Vector3(1.6f,1.25f,1.6f),foliage,false);
            crown.GetComponent<MeshRenderer>().sharedMaterials=new[]{foliage,snow};crown.transform.rotation=Quaternion.Euler(0,index*37,0);
        }
        private void LivingWater(string name,Vector3 centre,Vector2 size,Material rock,Material foliage)
        {
            var mesh=LandscapeLookGeometry.WaterDisc(region+21);meshes.Add(mesh);var texture=LandscapeLookGeometry.Ripples();textures.Add(texture);
            var water=ColorMaterial(region==2?new Color(.16f,.49f,.49f):new Color(.16f,.43f,.46f));water.SetTexture("_BaseMap",texture);water.SetTextureScale("_BaseMap",Vector2.one*3);water.SetFloat("_Smoothness",.78f);water.SetFloat("_Metallic",.15f);
            MeshObject(name,mesh,centre,new Vector3(size.x,1,size.y),water,false).AddComponent<WaterSurfaceMotion>();
            var pebbles=new DetailBatch();var reeds=new DetailBatch();
            for(int i=0;i<30;i++)
            {
                float a=i*Mathf.PI*2/30;var p=centre+new Vector3(Mathf.Cos(a)*size.x*.52f,-.025f,Mathf.Sin(a)*size.y*.52f);
                pebbles.Add(livingStone,p,new Vector3(.25f,.11f,.21f),Quaternion.Euler(0,i*53,0));
                if(i%3==0)for(int j=0;j<5;j++)reeds.Add(livingGrass,p+new Vector3(j*.045f,-.01f,0),new Vector3(1,1.35f+j*.15f,1),Quaternion.Euler(0,j*65+i*13,0));
            }
            pebbles.Bake(this,"Waterside pebbles",rock);reeds.Bake(this,"Waterside reeds",foliage);
        }
        private void DressLivingCabin(Vector3 origin,Material wood,Material roof,Material dark)
        {
            var trim=ColorMaterial(new Color(.55f,.34f,.17f));var glass=ColorMaterial(new Color(.55f,.69f,.67f));glass.SetFloat("_Smoothness",.6f);
            var warm=ColorMaterial(new Color(.95f,.71f,.33f));
            for(int s=-1;s<=1;s+=2)
            {
                Shape("Cottage window frame",PrimitiveType.Cube,origin+new Vector3(s*1.03f,1.3f,-1.52f),new Vector3(.66f,.72f,.12f),trim,false);
                Shape("Cottage window glass",PrimitiveType.Cube,origin+new Vector3(s*1.03f,1.3f,-1.59f),new Vector3(.52f,.58f,.025f),glass,false);
                Shape("Cottage window mullion",PrimitiveType.Cube,origin+new Vector3(s*1.03f,1.3f,-1.62f),new Vector3(.05f,.58f,.04f),wood,false);
                Shape("Cottage window sill",PrimitiveType.Cube,origin+new Vector3(s*1.03f,.9f,-1.63f),new Vector3(.8f,.08f,.3f),wood,false);
                for(int slat=0;slat<6;slat++)Shape("Cottage timber siding",PrimitiveType.Cube,origin+new Vector3(s*1.76f,.2f+slat*.31f,0),new Vector3(.025f,.018f,2.95f),dark,false);
            }
            Shape("Cottage door frame",PrimitiveType.Cube,origin+new Vector3(0,1.35f,-1.58f),new Vector3(1.08f,.12f,.18f),trim,false);
            Shape("Cottage door handle",PrimitiveType.Sphere,origin+new Vector3(.27f,.7f,-1.62f),Vector3.one*.08f,warm,false);
            Shape("Cottage chimney",PrimitiveType.Cube,origin+new Vector3(1.05f,2.65f,.6f),new Vector3(.46f,1.3f,.46f),roof,false);
            Shape("Cottage chimney cap",PrimitiveType.Cube,origin+new Vector3(1.05f,3.34f,.6f),new Vector3(.6f,.12f,.6f),roof,false);
        }
    }
}
