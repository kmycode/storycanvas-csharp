using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace StoryCanvas.Shared.Models.IO
{
    public static class SerializationUtil
    {
        /// <summary>
        /// 指定したオブジェクトに対してシリアライズを行う
        /// </summary>
        /// <param name="stream">ストリーム</param>
        /// <param name="obj">シリアライズを行うオブジェクト</param>
        private static void XMLSerialize(Stream stream, object obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            using (var xmlStream = XmlWriter.Create(stream))
            {
                try
                {
                    serializer.WriteObject(xmlStream, obj);
                }
                catch (InvalidDataContractException e)
                {
                    throw e;
                }
                catch (SerializationException e)
                {
                    throw e;
                }
            }
        }

        private static T XMLDeserialize<T>(Stream stream)
        {
            T model = default(T);
            var serializer = new DataContractSerializer(typeof(T));
            using (var xmlStream = XmlReader.Create(stream))
            {
                var obj = serializer.ReadObject(xmlStream);
                if (obj.GetType() == typeof(T))
                {
                    model = (T)obj;
                }
            }
            return model;
        }

        /// <summary>
        /// GZIPを利用して、指定したオブジェクトに対してシリアライズを行う
        /// </summary>
        /// <param name="stream">ストリーム</param>
        /// <param name="obj">シリアライズを行うオブジェクト</param>
        private static void GzipSerialize(Stream stream, object obj)
        {
            using (var gzipStream = new GZipStream(stream, CompressionLevel.Fastest))
            {
                XMLSerialize(gzipStream, obj);
            }
        }

        private static T GzipDeserialize<T>(Stream stream)
        {
            T model = default(T);
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                model = XMLDeserialize<T>(gzipStream);
            }
            return model;
        }

        public static void Serialize(Stream stream, object obj)
        {
            GzipSerialize(stream, obj);
        }
        public static T Deserialize<T>(Stream stream)
        {
            return GzipDeserialize<T>(stream);
        }
    }

    [Obsolete("SerializationUtilに置き換え")]
    public class SerializationModel
	{
		/// <summary>
		/// 指定したオブジェクトに対してシリアライズを行う
		/// </summary>
		/// <param name="stream">ストリーム</param>
		/// <param name="obj">シリアライズを行うオブジェクト</param>
		private void XMLSerialize(Stream stream, object obj)
		{
			var serializer = new DataContractSerializer(obj.GetType());
			using (var xmlStream = XmlWriter.Create(stream))
			{
				try
				{
					serializer.WriteObject(xmlStream, obj);
				}
				catch (InvalidDataContractException e)
				{
					throw e;
				}
				catch (SerializationException e)
				{
					throw e;
				}
			}
		}

		private T XMLDeserialize<T>(Stream stream)
		{
			T model = default(T);
			var serializer = new DataContractSerializer(typeof(T));
			using (var xmlStream = XmlReader.Create(stream))
			{
				var obj = serializer.ReadObject(xmlStream);
				if (obj.GetType() == typeof(T)) {
					model = (T)obj;
				}
			}
			return model;
		}

		/// <summary>
		/// GZIPを利用して、指定したオブジェクトに対してシリアライズを行う
		/// </summary>
		/// <param name="stream">ストリーム</param>
		/// <param name="obj">シリアライズを行うオブジェクト</param>
		private void GzipSerialize(Stream stream, object obj)
		{
			using (var gzipStream = new GZipStream(stream, CompressionLevel.Fastest))
			{
				this.XMLSerialize(gzipStream, obj);
			}
		}

		private T GzipDeserialize<T>(Stream stream)
		{
			T model = default(T);
			using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
			{
				model = this.XMLDeserialize<T>(gzipStream);
			}
			return model;
		}

		public void Serialize(Stream stream, object obj)
		{
			this.GzipSerialize(stream, obj);
		}
		public T Deserialize<T>(Stream stream)
		{
			return this.GzipDeserialize<T>(stream);
		}
	}
}
