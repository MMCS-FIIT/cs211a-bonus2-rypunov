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
    }
}
