
namespace FileShareConnectivity.Models;

public class NearbyDevice
{
    public string Name { get; }
    public string Address { get; }
    public bool IsConnected { get; }

    public NearbyDevice(string name, string address, bool isConnected = false)
    {
        Name = name;
        Address = address;
        IsConnected = isConnected;
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

    public override string ToString()
    {
        string connectedStatus = IsConnected ? "Connected" : "Not Connected";

        return $"NearbyDevice: Name: {Name}, Address: {Address}, Status: {connectedStatus}";
    }
}
