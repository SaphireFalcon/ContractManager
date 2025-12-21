using KSA;
using StarMap.API;

namespace ContractManager
{
    [StarMapMod]
    public class ContractManager
    {
        [StarMapImmediateLoad]
        public void Init(Mod definingMod)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
