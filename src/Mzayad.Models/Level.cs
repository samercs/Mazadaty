
namespace Mzayad.Models
{
    public class Level
    {
        public const int XpBase = 100;

        public string Name { get; set; }
        public int Index { get; set; }
        public int XpRequired { get; set; }
        public int TokensAwarded { get; set; }
    }
}
