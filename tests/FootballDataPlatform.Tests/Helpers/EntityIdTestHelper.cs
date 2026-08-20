using System.Reflection;

namespace FootballDataPlatform.Tests.Helpers;

internal static class EntityIdTestHelper
{
    public static T WithId<T>(this T entity, long id)
    {
        var property = typeof(T).GetProperty(
            "Id",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        property?.SetValue(entity, id);
        return entity;
    }
}
