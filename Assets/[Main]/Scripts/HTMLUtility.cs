using System.IO;
using System.Net;
using System.Text;

public static class HTMLUtility
{
    static public string GetResponse(string uri)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    sb.Append(Encoding.Default.GetString(buf, 0, count));
                }
            }
            while (count > 0);

            return sb.ToString();
        }
        catch (System.Exception exception)
        {
            UnityEngine.Debug.LogError(exception);
        }

        return string.Empty;
    }
}