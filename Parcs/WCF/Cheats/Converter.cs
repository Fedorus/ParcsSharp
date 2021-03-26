using Parcs.WCF.DTO;

namespace Parcs.WCF.Cheats
{
    public static class Converter
    {
        public static ControlSpaceDTO ToDto(this ControlSpace item)
        {
            return new ControlSpaceDTO()
            {
                ID = item.ID,
                DaemonAddressees = item.DaemonAdresses,
                Name = item.Name
            };
        }
    }
}