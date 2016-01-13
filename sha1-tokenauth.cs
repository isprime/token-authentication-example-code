using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CdnHash
{
   public class CdnHasher
   {
      public static string compute_cdn_hash(string url, string key, 
                              int gen = 0, 
                              string hash_valid_start_name = "stime", 
                              string hash_valid_end_name = "etime", 
                              int hash_valid_lifetime = 60, /* in minutes */ 
                              bool inject_client_ip = false, 
                              string client_ip = "127.0.0.1", 
                              string time_format = "YmdHis" /* or U */) 
      {
         DateTime endTimeDt =  DateTime.Now.AddMinutes(hash_valid_lifetime);
         DateTime startTimeDt = DateTime.Now.AddMinutes(-5);

         string startTime, endTime;
         if (time_format == "YmdHis") { 
            startTime = startTimeDt.ToString("yyyyMMddHHmmss");
            endTime = endTimeDt.ToString("yyyyMMddHHmmss");
         } else if (time_format == "U") { 
            startTime = startTimeDt.ToString("U");
            endTime = endTimeDt.ToString("U");
         } else {
            startTime = "";
            endTime = "";
         }
         
         string client_ip_string;
         if (inject_client_ip) { 
           client_ip_string = string.Format("clientip={0}", client_ip); 
         }  else {  
           client_ip_string =  "";
         } 

         var parts = new Uri(url);
         string scheme = parts.Scheme, 
                host = parts.Host, 
                path = parts.AbsolutePath, 
                query = parts.Query;
         string sign_string;
         if (!string.IsNullOrEmpty(query)) { 
            string new_query = string.Format("{0}&{1}={2}&{3}={4}", 
                                             query, 
                                             hash_valid_start_name, 
                                             startTime, 
                                             hash_valid_end_name, 
                                             endTime);
            sign_string = string.Format("{0}{1}{2}", path, client_ip_string, new_query);
         } else { 
            string new_query = string.Format("{0}={1}&{2}={3}", 
                                             hash_valid_start_name, 
                                             startTime, 
                                             hash_valid_end_name, 
                                             endTime);
            sign_string = string.Format("{0}?{1}{2}", path, client_ip_string, new_query);
         }

         // http://stackoverflow.com/questions/6067751/how-to-generate-hmac-sha1-in-c
         byte[] keyByte = Encoding.ASCII.GetBytes(key);
         HMACSHA1 myhmacsha1 = new HMACSHA1(keyByte);
         byte[] byteArray = Encoding.ASCII.GetBytes(sign_string);
         MemoryStream stream = new MemoryStream(byteArray);
         var hmac = myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}",e), s => s );
         var thmac =  hmac.Length <= 20 ? hmac : hmac.Substring(0, 20); 

         string signed_url = string.Format("{0}://{1}{2}&hash={3}{4}", scheme, host, sign_string, gen, thmac);
         return signed_url;
      }       

      static void Main(string[] args)
      {
         string hashed_url = compute_cdn_hash("http://domain.com/path/to/image.jpg", "secretkey");
         Console.WriteLine("{0}", hashed_url);
      }
   }
}
