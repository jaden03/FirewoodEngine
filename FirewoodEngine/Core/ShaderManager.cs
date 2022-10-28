using System.Collections.Generic;

namespace FirewoodEngine.Core;

public static class ShaderManager
{
    public static List<Shader> shaders = new List<Shader>();
    
    public static void AddShader(Shader shader)
    {
        shaders.Add(shader);
    }
    
    public static void RemoveShader(Shader shader)
    {
        shaders.Remove(shader);
    }
}