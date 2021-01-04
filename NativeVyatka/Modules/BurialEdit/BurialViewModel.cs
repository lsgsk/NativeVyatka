#nullable enable
namespace NativeVyatka
{
    public struct BurialViewModel
    {
        public readonly struct DateViewModel
        {
            public string Day { get; init; }
            public string Month { get; init; }
            public string Year { get; init; }

            public DateViewModel(string day, string month, string year) {
                this.Day = day;
                this.Month = month;
                this.Year = year;
            }
        }

        public readonly struct ImageViewModel
        {
            public string? UrlPicture { get; init; }
            public Java.IO.File? LocalPicture { get; init; }

            public ImageViewModel(string picturePath) {
                if (picturePath.StartsWith("http")) {
                    UrlPicture = picturePath;
                    LocalPicture = null;
                }
                else {
                    UrlPicture = null;
                    LocalPicture = new Java.IO.File(picturePath);
                }
            }
        }

        public string Title { get; init; }
        public ImageViewModel Image { get; init; }
        public string Surname { get; init; }
        public string Name { get; init; }
        public string Patronymic { get; init; }
        public string Description { get; init; }
        public string RecordTime { get; init; }
        public DateViewModel BirthDay { get; init; }
        public DateViewModel DeathDay { get; init; }
        public bool IsFavorite { get; init; }
        public BurialViewModel(
            string title,
            ImageViewModel image,
            string surname,
            string name,
            string patronymic,
            string description,
            string recordTime,
            DateViewModel birthDay,
            DateViewModel deathDay,
            bool isFavorite) {
            this.Title = title;
            this.Image = image;
            this.Surname = surname;
            this.Name = name;
            this.Patronymic = patronymic;
            this.Description = description;
            this.RecordTime = recordTime;
            this.BirthDay = birthDay;
            this.DeathDay = deathDay;
            this.IsFavorite = isFavorite;
        }
    }
}

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}
#nullable restore