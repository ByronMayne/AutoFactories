using Ninject.AutoFactories;
using static Ninject.AutoFactories.AccessModifier;

namespace AutoFactories.Tests
{
    public class AccessModiferTests
    {

        [Theory]
        [MemberData(nameof(MostRestrictiveTestData))]
        public void MostRestrictive(AccessModifier left, AccessModifier right, AccessModifier expected)
        {
            Assert.Equal(expected, AccessModifier.MostRestrictive(left, right));
        }



        public static TheoryData<AccessModifier, AccessModifier, AccessModifier> MostRestrictiveTestData()
        {
            return new TheoryData<AccessModifier, AccessModifier, AccessModifier>
            {
                { Public, Internal, Internal },
                { Internal, Protected, Protected },
                { Protected, ProtectedAndInternal, ProtectedAndInternal },
                { ProtectedAndInternal, Private, Private }
            };
        }
    }
}
