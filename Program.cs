using System;
using System.Collections.Generic;
using System.Linq;

namespace FalloutHacker {
    class Program {

        class PossiblePassword {
            public string Password { get; set; }
            public int? CorrectCount { get; set; }
        }
        
        static void Main(string[] args) {
            var possiblePasswords = new List<PossiblePassword>();
            while (true) {
                Console.Write("Enter word: ");
                var word = Console.ReadLine();
                if (word == "!!") {
                    return;
                } else if (word == "") {
                    break;
                } else {
                    possiblePasswords.Add(new PossiblePassword {Password = word});
                }
            }

            if (possiblePasswords.Any(pp => pp.Password is null)) {
                throw new Exception("All possible passwords must have a value");
            }

            if (possiblePasswords.Select(pp => pp.Password.Length).Distinct().Count() > 1) {
                throw new Exception("Not all passwords have the same length");
            }

            ICollection<PossiblePassword> matches;
            while ((matches = FindMatchingPasswords(possiblePasswords)).Count > 0) {
                foreach (var possibleMatch in matches) {
                    Console.WriteLine($"{possibleMatch.Password}: {CalculateHelpfulness(matches, possibleMatch)}");
                }                
                Console.WriteLine();

                var firstMatch = matches.First();
                Console.Write(firstMatch.Password + ": ");
                int correctCount;
                while (!int.TryParse(Console.ReadLine(), out correctCount)) ;
                firstMatch.CorrectCount = correctCount;
            }
        }

        private static double CalculateHelpfulness(IEnumerable<PossiblePassword> possiblePasswords, PossiblePassword possiblePassword) {
            var avg = Enumerable.Range(1, possiblePassword.Password.Length).Select(i => {
                possiblePassword.CorrectCount = i;
                var matches = FindMatchingPasswords(possiblePasswords.ToArray());
                possiblePassword.CorrectCount = null;
                return matches.Length;
            }).Average();
            return avg;
        }

        private static PossiblePassword[] FindMatchingPasswords(ICollection<PossiblePassword> possiblePasswords) {
            var triedPasswords = possiblePasswords.Where(pp => pp.CorrectCount != null).ToArray();
            var possibleMatches = possiblePasswords
                .Where(pp => triedPasswords.All(tp => GetMatches(pp, tp) == tp.CorrectCount)).ToArray();
            return possibleMatches;
        }

        private static int GetMatches(PossiblePassword pp, PossiblePassword tp) {
            return pp.Password.Zip(tp.Password, (ppc, tpc) => ppc == tpc).Count(isMatch => isMatch);
        }
    }
}