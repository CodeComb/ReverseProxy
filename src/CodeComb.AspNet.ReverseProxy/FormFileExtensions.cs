using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Http;


namespace CodeComb.AspNet.ReverseProxy.Extensions
{
    public static class FormFileExtentions
    {
        public static string PopFrontMatch(this string self, string str)
        {
            if (str.Length > self.Length)
                return self;
            else if (self.IndexOf(str) == 0)
                return self.Substring(str.Length);
            else
                return self;
        }

        public static string PopBackMatch(this string self, string str)
        {
            if (str.Length > self.Length)
                return self;
            else if (self.LastIndexOf(str) == self.Length - str.Length)
                return self.Substring(0, self.Length - str.Length);
            else
                return self;
        }

        public static string PopFront(this string self, int count = 1)
        {
            if (count > self.Length || count < 0)
                throw new IndexOutOfRangeException();
            return self.Substring(count);
        }

        public static string PopBack(this string self, int count = 1)
        {
            if (count > self.Length || count < 0)
                throw new IndexOutOfRangeException();
            return self.Substring(0, self.Length - count);
        }

        public static byte[] ReadAllBytes(this IFormFile self)
        {
            using (var reader = new BinaryReader(self.OpenReadStream()))
            {
                return reader.ReadBytes(Convert.ToInt32(self.Length));
            }
        }

        public static Task<byte[]> ReadAllBytesAsync(this IFormFile self)
        {
            return Task.Factory.StartNew<byte[]>(() => {
                using (var reader = new BinaryReader(self.OpenReadStream()))
                {
                    return reader.ReadBytes(Convert.ToInt32(self.Length));
                }
            });
        }

        public static string GetFormFieldName(this IFormFile self)
        {
            try
            {
                var tmp = self.ContentDisposition.Split(';');
                foreach (var str in tmp)
                {
                    var tmp2 = str.Trim(' ');
                    var tmp3 = tmp2.Split('=');
                    if (tmp3.Count() == 2 && tmp3[0].ToLower() == "name")
                        return tmp3[1].PopFrontMatch("\"").PopBackMatch("\"");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string GetFileName(this IFormFile self)
        {
            try
            {
                var tmp = self.ContentDisposition.Split(';');
                foreach (var str in tmp)
                {
                    var tmp2 = str.Trim(' ');
                    var tmp3 = tmp2.Split('=');
                    if (tmp3.Count() == 2 && tmp3[0].ToLower() == "filename")
                        return tmp3[1].PopFrontMatch("\"").PopBackMatch("\"");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}