using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Bonus_2
{
    internal class Parsing
    {
        /// <summary>
        /// Класс фильма
        /// </summary>
        class Film
        {
            public string Name { get; }
            public string Date { get; }
            public double Rating { get; }
            public string Genre { get; }
            public string Type { get; }
            public int TotalDuration { get; }
            public Film(string name, string date, double raiting, string genre, string duration, string type, string episodes)
            {
                Name = name;
                Date = date;
                Rating = raiting;
                Genre = genre;
                Type = type;
                if (type == "Film")
                    TotalDuration = int.Parse(duration);
                else
                    TotalDuration = int.Parse(episodes);
            }

            public override string ToString()
            {
                if (Type == "Film")
                    return "Название - " + Name + "\" \nГод выпуска - " + Date + "\nРейтинг - " + Rating + 
                        "\nЖанр - " + Genre + "\nТип - " + Type + "\nПродолжительность - " + TotalDuration + "\n";
                else
                    return "Название - " + Name + "\" \nГод выпуска - " + Date + "\nРейтинг - " + Rating +
                        "\nЖанр - " + Genre + "\nТип - " + Type + "\nКол-во эпизодов - " + TotalDuration +  "\n";
            }
        }

        /// <summary>
        /// Обрабатывает файл для хранения информации о фильмах
        /// </summary>
        public static void Parssing()
        {
            var lines_arr = System.IO.File.ReadAllLines("input_files/imdb.csv");
            var movie_database = new List<Film>();

            for (int i = 0; i < lines_arr.Count(); i++)
            {
                var reg = Regex.Matches(lines_arr[i], @""".+?""").ToArray();
                string[] el_film = lines_arr[i].Split(new char[] { ',' });
                var genre = "";
                if (reg.Count() < 5)
                    genre = el_film[4];
                else
                    genre = reg[3].ToString();

                if (el_film[2] != "No Rate")
                {
                    movie_database.Add(new Film(el_film[0], el_film[1],
                        double.Parse(el_film[2].Replace('.', ',')), genre,
                        el_film[^9], el_film[^8], el_film[^6]));
                }
            }
            //for (int i = 0; i < 50; i++)
            //    Console.WriteLine(movie_database[i]);
        }
    }
}
