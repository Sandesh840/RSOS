using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Utilities;

public static class ExtensionMethods
{
    private static string[] _maliciousInputPattern =
    [
        "<script>",
        "<img src=\"javascript:",
        "<a href=\"javascript:",
        "<iframe>",
        @"\.\./",
        @"\.\.\\",
        @"\.(exe|dll|bat)$",
        @"<!ENTITY",
        @"<!DOCTYPE",
        @"(&|\(|\*)"
    ];
    
    public static string SetUniqueFileName(this string fileExtension)
    {
        var renamedFileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString();
        
        return renamedFileName + fileExtension;
    }
    
    public static string GetIpAddress()
    {
        var hostName = Dns.GetHostName();
        
        var ipEntry = Dns.GetHostEntry(hostName);
        
        var addr = ipEntry.AddressList;
        
        return Convert.ToString(addr[^1]) ?? "";
    }
    
    public static string GetMacAddress()
    {
        var macAddress = string.Empty;
        
        var networks = NetworkInterface.GetAllNetworkInterfaces();
        
        foreach (var adapter in networks)
        {
            if (macAddress != string.Empty) continue;

            macAddress = Convert.ToString(adapter.GetPhysicalAddress());
        }
        
        return macAddress ?? "";
    }

    public static bool IsMaliciousInput(string input)
    {
        var encodedInput = HttpUtility.HtmlDecode(input);
        
        if (input.Split('"').Length - 1 >= 2)
        {
            return true;
        }
        if (encodedInput.StartsWith($";"))
        {
            return true;
        }
        if (encodedInput.Contains("<script>") || encodedInput.Contains("<img src=\"javascript:") || encodedInput.Contains("<a href=\"javascript:") || encodedInput.Contains("<iframe>"))
        {
            return true;
        }
        if (encodedInput.Contains("../") || encodedInput.Contains("..\\"))
        {
            return true;
        }
        if (encodedInput.EndsWith(".exe") || encodedInput.EndsWith(".dll") || encodedInput.EndsWith(".bat"))
        {
            return true;
        }
        if (encodedInput.Contains("<!ENTITY") || encodedInput.Contains("<!DOCTYPE"))
        {
            return true; 
        }
        if (encodedInput.Contains("(&") || encodedInput.Contains("(|") || encodedInput.Contains("*)"))
        {
            return true;
        }
        
        return false;
    }
}