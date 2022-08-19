using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Portal.CacheManager
{
	/// <summary>
	/// Управление КЭШем
	/// </summary>
	public class CacheManager : ICacheManager
	{
		private IMemoryCache _cache;

		public CacheManager(IMemoryCache cache)
		{
			_cache = cache;
		}
		/// <summary>
		/// Генерация уникального ключа для объекта
		/// </summary>
		/// <param name="obj">Объект</param>
		/// <param name="keyField">Название поля идентификатор объекта</param>
		/// <returns></returns>
		public string GenKey(object obj, string keyField)
		{
			if (obj == null)
			{
				throw new ArgumentException($"Object is null!!!");
			}
			var type = obj.GetType();
			var field = type.GetProperty(keyField);
			if (field == null)
			{
				throw new ArgumentException($"Object doesn't contain {keyField} field!!!");
			}
			var id = field.GetValue(obj);
			return (type.Name + id);
		}

		/// <summary>
		/// Устанавливет кэш объекта (obj), где obj гарантированно имеет ключевое поле "Id"
		/// </summary>
		/// <param name="obj">Объект</param>
		/// <param name="key">Ключ по которому кэшируем</param>
		public void Set(object? obj, string key)
		{
			_cache.Set(key, obj);
		}

        /// <summary>
        /// Получить запись
        /// </summary>
        /// <param name="key">Ключ по которому кэшируем</param>
        /// <returns></returns>
        public object Get(string key)
		{
			return _cache.Get(key);
		}

        /// <summary>
        /// Удалить
        /// </summary>
        /// <param name="key">Ключ</param>
        public void Remove(string key)
		{
			_cache.Remove(key);
		}

		/// <summary>
		/// Очистить
		/// </summary>
		public void Clear()
		{
			_cache.Dispose();
			_cache = new MemoryCache(new MemoryCacheOptions());
		}
	}
}