namespace ExpressiveSharp.EntityFrameworkCore;

/// <summary>
/// Assembly-level attribute that registers an <see cref="IExpressivePlugin"/> implementation.
/// When <c>UseExpressives()</c> is called, all loaded assemblies are scanned for this attribute
/// and discovered plugins are activated automatically.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ExpressivePluginAttribute : Attribute
{
    /// <summary>
    /// The type implementing <see cref="IExpressivePlugin"/>.
    /// </summary>
    public Type PluginType { get; }

    public ExpressivePluginAttribute(Type pluginType)
    {
        if (!typeof(IExpressivePlugin).IsAssignableFrom(pluginType))
            throw new ArgumentException(
                $"Type '{pluginType.FullName}' does not implement {nameof(IExpressivePlugin)}.",
                nameof(pluginType));

        PluginType = pluginType;
    }
}
