using System;
using System.Collections.Generic;

namespace Portal.CacheManager
{
	/// <summary>
	/// Управление КЭШем
	/// </summary>
	public interface ICacheManager
	{

		/// <summary>
		/// Генерация уникального ключа для объекта
		/// </summary>
		/// <param name="obj">Объект</param>
		/// <param name="keyField">Название поля идентификатор объекта</param>
		/// <returns></returns>
		public string GenKey(object obj, string keyField);

		/// <summary>
		/// Устанавливет кэш объекта (obj), где obj гарантированно имеет ключевое поле "Id"
		/// </summary>
		/// <param name="obj">Объект</param>
		/// <param name="key">Ключ по которому кэшируем</param>
		public void Set(object obj, string key);

		/// <summary>
		/// Получить запись
		/// </summary>
		/// <param name="key">Ключ по которому кэшируем</param>
		/// <returns></returns>
		public object Get(string key);

		/// <summary>
		/// Удалить
		/// </summary>
		/// <param name="key">Ключ</param>
		public void Remove(string key);

		/// <summary>
		/// Очистить
		/// </summary>
		void Clear();
	}
}