using Mazadaty.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mazadaty.Web.Models.Shared
{
    public class AddressViewModel : Address
    {
        public static IEnumerable<SelectListItem> AllCountries => new[]
        {
            new SelectListItem
            {
                Text = Resources.Global.Kuwait,
                Value = "KW"
            },
            new SelectListItem
            {
                Text = Resources.Global.SaudiArabia,
                Value = "SA"
            },
            new SelectListItem
            {
                Text = Resources.Global.UAE,
                Value = "AE"
            },
            new SelectListItem
            {
                Text = Resources.Global.Bahrain,
                Value = "BH"
            },
            new SelectListItem
            {
                Text = Resources.Global.Qatar,
                Value = "QA"
            },
            new SelectListItem
            {
                Text = Resources.Global.Oman,
                Value = "OM"
            }
        };

        public IEnumerable<SelectListItem> CountriesList
        {
            get
            {
                foreach (var country in AllCountries)
                {
                    if (country.Value == CountryCode)
                    {
                        country.Selected = true;
                    }

                    yield return country;
                }
            }
        }

        public AddressViewModel()
        {
            CountryCode = "KW";
        }

        public AddressViewModel(Address address)
        {
            AddressId = address.AddressId;
            AddressLine1 = address.AddressLine1;
            AddressLine2 = address.AddressLine2;
            AddressLine3 = address.AddressLine3;
            AddressLine4 = address.AddressLine4;
            CityArea = address.CityArea;
            StateProvince = address.StateProvince;
            PostalCode = address.PostalCode;
            CountryCode = address.CountryCode;
            Floor = address.Floor;
            FlatNumber = address.FlatNumber;
        }

        public IEnumerable<string> KuwaitAreas => new[]
        {
            "√»—ﬁ ŒÌÿ«‰",
            "√»Ê «·Õ’«‰Ì…",
            "√»Ê Õ·Ì›…",
            "√»Ê ›ÿÌ—…",
            "«‘»·Ì…",
            "«·√Õ„œÌ",
            "«·√Õ„œÌ",
            "«·√‰œ·”",
            "«·»œ⁄",
            "«·Ã«»—Ì…",
            "«·ÃÂ—«¡",
            "«·ÃÂ—«¡",
            "«·Œ«·œÌ…",
            "«·œ”„…",
            "«·œ”„Â",
            "«·œ⁄Ì…",
            "«·œÊÕ…",
            "«·—«»Ì…",
            "«·—Õ«»",
            "«·—ﬁ…",
            "«·—ﬁ⁄Ì",
            "«·—„ÌÀÌ…",
            "«·—Ê÷…",
            "«·—Ì ",
            "«·“Â—«¡",
            "«·”«·„Ì…",
            "«·”—…",
            "«·”·«„",
            "«·‘«„Ì…",
            "«·‘—ﬁ",
            "«·‘⁄»",
            "«·‘Âœ«¡",
            "«·‘ÊÌŒ",
            "«·’«·ÕÌ…",
            "«·’»«ÕÌ…",
            "«·’œÌﬁ",
            "«·’·Ì»Œ« ",
            "«·’·Ì»Ì…",
            "«·÷ÃÌÃ",
            "«·÷Â—",
            "«·⁄«—ŸÌ…",
            "«·⁄«—÷Ì… «·’‰«⁄Ì…",
            "«·⁄«’„Â",
            "«·⁄œ«‰",
            "«·⁄œÌ·Ì…",
            "«·⁄ﬁÌ·…",
            "«·⁄„—Ì…",
            "«·⁄ÌÊ‰",
            "«·›ÕÌÕÌ·",
            "«·›—œÊ”",
            "«·›—Ê«‰Ì…",
            "«·›—Ê«‰Ì…",
            "«·›‰ÿ«”",
            "«·›‰ÌÿÌ”",
            "«·›ÌÕ«¡",
            "«·ﬁ»·…",
            "«·ﬁ—Ì‰",
            "«·ﬁ’—",
            "«·ﬁ’Ê—",
            "«·ﬁÌ—Ê«‰ - Ã‰Ê» «·œÊÕ…",
            "«·„”«Ì·",
            "«·„”Ì·…",
            "«·„ﬁÊ⁄ ",
            "«·„‰’Ê—Ì…",
            "«·„‰ﬁ›",
            "«·„Â»Ê·…",
            "«·‰“Â…",
            "«·‰”Ì„",
            "«·‰⁄Ì„",
            "«·‰Â÷…",
            "«·ÂœÌ…",
            "«·Ê«Õ…",
            "«·Ê”ÿÏ",
            "«·Ê›—… «·”ﬂ‰Ì…",
            "«·Ì—„Êﬂ",
            "»‰Ìœ «·ﬁ«—",
            "»Ì«‰",
            " Ì„«¡",
            "Ã«»— «·√Õ„œ",
            "Ã«»— «·⁄·Ì",
            "Ã·Ì» «·‘ÌÊŒ",
            "Ã‰Ê» «·Ê”ÿÏ",
            "ÕÿÌ‰",
            "ÕÊ·Ì",
            "ÕÊ·Ì",
            "ŒÌÿ«‰",
            "œ”„«‰",
            "”⁄œ «·⁄»œ«··Â",
            "”·ÊÏ",
            "’»«Õ «·√Õ„œ «·”ﬂ‰Ì…",
            "’»«Õ «·”«·„",
            "’»«Õ «·‰«’—",
            "⁄»«”Ì…",
            "⁄»œ«··Â «·”«·„",
            "⁄»œ«··Â «·„»«—ﬂ - €—» «·Ã·Ì»",
            "⁄·Ì ’»«Õ «·”«·„",
            "€—‰«ÿ…",
            "›Âœ «·√Õ„œ",
            "ﬁ«œ”Ì…",
            "ﬁ—ÿ»…",
            "ﬂÌ›«‰",
            "„»«—ﬂ «·⁄»œ«··Â",
            "„»«—ﬂ «·ﬂ»Ì—",
            "„»«—ﬂ «·ﬂ»Ì—",
            "„œÌ‰… «·ﬂÊÌ ",
            "„—ﬁ«»",
            "„‘—›",
            "„Ìœ«‰ ÕÊ·Ì",
            "Abbasiya",
            "Abdullah Al-Mubarak - West Jleeb",
            "Abdullah Al-Salem",
            "Abraq Khaitan",
            "Abu Ftaira",
            "Abu Halifa",
            "Abu Hasaniya",
            "Adailiya",
            "Adan",
            "Ahmadi",
            "Al Dasmah",
            "Al Masayel",
            "Al Naeem",
            "Al-Ahmadi",
            "Al-Bedae",
            "Al-Qurain",
            "Al-Qusour",
            "Ali Sabah Al-Salem",
            "Andalous",
            "Ardhiya",
            "Ardhiya Industrial",
            "Ashbeliah",
            "Bayan",
            "Bnaid Al-Gar",
            "Daiya",
            "Dasma",
            "Dasman",
            "Dhaher",
            "Dhajeej",
            "Doha",
            "Egaila",
            "Fahad Al Ahmed",
            "Fahaheel",
            "Faiha",
            "Farwaniya",
            "Farwaniya",
            "Ferdous",
            "Fintas",
            "Fnaitees",
            "Ghornata",
            "Hadiya",
            "Hawally",
            "Hawally",
            "Hitteen",
            "Jaber Al Ahmed",
            "Jaber Al-Ali",
            "Jabriya",
            "Jahra",
            "Jahra",
            "Jleeb Al-Shiyoukh",
            "Kaifan",
            "Khaitan",
            "Khaldiya",
            "Kuwait City",
            "Kuwait City (Capital)",
            "Magwa",
            "Mahboula",
            "Maidan Hawally",
            "Mangaf",
            "Mansouriya",
            "Messila",
            "Mirqab",
            "Mishrif",
            "Mubarak Al-Abdullah",
            "Mubarak Al-kabir",
            "Mubarak Al-Kabir",
            "Nahda",
            "Nasseem",
            "Nuzha",
            "Omariya",
            "Oyoun",
            "Qadsiya",
            "Qairawan - South Doha",
            "Qasr",
            "Qibla",
            "Qortuba",
            "Rabiya",
            "Rai",
            "Rawda",
            "Reggai",
            "Rehab",
            "Riqqa",
            "Rumaithiya",
            "Saad Al Abdullah",
            "Sabah AL Ahmad Residential",
            "Sabah Al-Nasser",
            "Sabah Al-Salem",
            "Sabahiya",
            "Salam",
            "Salhiya",
            "Salmiya",
            "Salwa",
            "Shaab",
            "Shamiya",
            "Sharq",
            "Shuhada",
            "Shuwaikh",
            "Siddiq",
            "South Wista",
            "Sulaibikhat",
            "Sulaibiya",
            "Surra",
            "Taima",
            "Wafra Residential",
            "Waha",
            "Wista",
            "Yarmouk",
            "Zahra"
        };
    }
}
