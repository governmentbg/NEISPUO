namespace SB.ApiAbstractions;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.HttpOverrides;

public class IPNetworkTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s)
        {
            if (!s.Contains('/'))
            {
                var ipAddress = IPAddress.Parse(s);

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    // an IPv4 network with prefix length 32 represents a single IP address
                    return new IPNetwork(ipAddress, 32);
                }
                else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // an IPv6 network with prefix length 128 represents a single IP address
                    return new IPNetwork(ipAddress, 128);
                }
            }
            else
            {
                var parts = s.Split('/');

                return new IPNetwork(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
            }
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) &&
            value is IPNetwork net)
        {
            return net.PrefixLength != 32 ? $"{net.Prefix}/{net.PrefixLength}" : net.Prefix.ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
