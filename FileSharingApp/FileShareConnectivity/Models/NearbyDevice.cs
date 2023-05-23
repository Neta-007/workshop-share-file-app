
namespace FileShareConnectivity.Models;

public class NearbyDevice
{
    public string Name { get; }
    public string Address { get; }

    public NearbyDevice(string name, string address)
    {
        Name = name;
        Address = address;
    }

    public override bool Equals(object obj)
    {
        NearbyDevice other = obj as NearbyDevice;

        return other != null && this.Name == other.Name && this.Address == other.Address;
    }

    public static bool operator ==(NearbyDevice device1, NearbyDevice device2)
    {
        if (ReferenceEquals(device1, device2))
        {
            return true;
        }

        if (device1 is null || device2 is null)
        {
            return false;
        }

        return device1.Equals(device2);
    }

    public static bool operator !=(NearbyDevice device1, NearbyDevice device2)
    {
        return !(device1 == device2);
    }

    public override int GetHashCode()
    {
        return this.Address.GetHashCode();
    }
}
