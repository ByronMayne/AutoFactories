namespace Ninject.AutoFactories
{
    internal static class Iterator
    {
        public record struct Item<T>(T Value, bool IsFirst, bool IsLast)
        {
            public static implicit operator T(Item<T> item)
            {
                return item.Value;
            }
        }

        public static IEnumerable<Item<T>> Create<T>(IEnumerable<T> enumerable)
        {
            T[] collection = enumerable as T[] ?? enumerable.ToArray();
            int count = collection.Length;

            for (int i = 0; i < count; i++)
            {
                yield return new Item<T>(collection[i], i == 0, i + 1 >= count);
            }
        }
    }
}
