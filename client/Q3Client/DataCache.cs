﻿using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Q3Client
{
    static class DataCache
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static T Load<T>(string name = null) where T : class
        {
            var store = GetIsolatedStorageFile();
            var fileName = name ?? (typeof (T)).Name;

            if (store.FileExists(fileName))
            {
                try
                {
                    using (var stream = store.OpenFile(fileName, FileMode.Open))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(T));
                        return (T)serializer.ReadObject(stream);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to deserialize " + fileName);
                    File.Delete(fileName);
                }
            }

            return null;
        }

        public static void Save<T>(T data, string name = null)
        {
            var store = GetIsolatedStorageFile();
            var fileName = name ?? (typeof(T)).Name;

            using (var stream = store.OpenFile(fileName, FileMode.Create))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, data);
            }
            
        }

        private static IsolatedStorageFile GetIsolatedStorageFile()
        {
            try
            {
                return IsolatedStorageFile.GetUserStoreForApplication();
            }
            catch (Exception)
            {
                return IsolatedStorageFile.GetUserStoreForAssembly();
            }
        }

        [Serializable]
        public class ListContainer<T>
        {
            public ListContainer(IEnumerable<T> items)
            {
                this.items = items.ToList();
            }

            public List<T> items;
        }
    }
}
