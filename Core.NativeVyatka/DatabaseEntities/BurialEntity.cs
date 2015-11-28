using System;
using SQLite;

namespace NativeVyatkaCore
{
    [Table(BurialEntity.TableName)]
    public class BurialEntity
    {      
        public const string TableName = "BurialTable";
        /// <summary>
        /// Id в базе на устройстве
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Уникальный глобальный идентификатор записи, генерируется на устройстве в момент съемки
        /// </summary>
        public string HashId { get; set;}
        /// <summary>
        /// Название могилы, имя человека
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
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
        /// путь к файлу фотографии на устройстве
        /// </summary>
        public string PicturePath { get; set; }
        /// <summary>
        /// Синхранизирована ли запись
        /// </summary>
        public bool IsSended { get; set; }
    } 
}
