using System;
using System.Collections.Generic;

namespace FirewoodEngine.Core;

public static class ComponentManager
{
    public static List<Component> components;

    public static void Initialize()
    {
        Console.WriteLine("Components Initialized");

        components = new List<Component>();
    }

    public static void AddComponent(Component component)
    {
        components.Add(component);
    }

    public static void RemoveComponent(Component component)
    {
        components.Remove(component);
    }
}