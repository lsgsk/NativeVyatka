using System;

namespace Core
{
    public class BurialRequest
    {
        /// <summary>
        /// Уникальный глобальный идентификатор записи, генерируется на устройстве в момент съемки
        /// </summary>
        public string HashId { get; set;}
        /// <summary>
        /// Название могилы, имя человека
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        /// <value>The death time.</value>
        public DateTime BirthTime { get; set; }
        /// <summary>
        /// Дата смерти
        /// </summary>
        public DateTime DeathTime { get; set; }
        /// <summary>
        /// Дополнительное описание
        /// </summary>
        public string Desctiption { get; set; }
        /// <summary>
        /// Время создания записи
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// Широта места захоронения
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Долгот а места захоронения
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// закодированное изображение захоронения
        /// </summary>
        public byte[] Picture { get; set; }
    }
}

