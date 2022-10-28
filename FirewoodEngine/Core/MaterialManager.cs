using System.Collections.Generic;
using FirewoodEngine.Core;
namespace FirewoodEngine.Core;

public static class MaterialManager
{
    public static List<Material> materials = new List<Material>();
    
    public static void AddMaterial(Material material)
    {
        materials.Add(material);
    }
    
    public static void RemoveMaterial(Material material)
    {
        materials.Remove(material);
    }
}