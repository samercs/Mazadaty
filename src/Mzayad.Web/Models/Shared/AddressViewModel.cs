using Mzayad.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Mzayad.Web.Models.Shared
{
    public class AddressViewModel : Address
    {
        public IEnumerable<SelectListItem> CountriesList { get; set; }

        public AddressViewModel()
        {

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

        public AddressViewModel Hydrate()
        {
            //CountriesList = OrangeJetpack.Regionalization.Countries.GetAllCountries()
            //    .Select(i => new SelectListItem
            //    {
            //        Text = i.Name,
            //        Value = i.IsoCode,
            //        Selected = i.IsoCode.Equals(CountryCode)
            //    })
            //    .ToList();

            CountriesList = new[]
            {
                new SelectListItem
                {
                    Text = Resources.Global.Kuwait,
                    Value = "KW"
                }
            };

            return this;
        }

        public IEnumerable<string> KuwaitAreas
        {
            get
            {
                return new[]
                {
                    "أبرق خيطان",
                    "أبو الحصانية",
                    "أبو حليفة",
                    "أبو فطيرة",
                    "اشبلية",
                    "الأحمدي",
                    "الأحمدي",
                    "الأندلس",
                    "البدع",
                    "الجابرية",
                    "الجهراء",
                    "الجهراء",
                    "الخالدية",
                    "الدسمة",
                    "الدسمه",
                    "الدعية",
                    "الدوحة",
                    "الرابية",
                    "الرحاب",
                    "الرقة",
                    "الرقعي",
                    "الرميثية",
                    "الروضة",
                    "الري ",
                    "الزهراء",
                    "السالمية",
                    "السرة",
                    "السلام",
                    "الشامية",
                    "الشرق",
                    "الشعب",
                    "الشهداء",
                    "الشويخ",
                    "الصالحية",
                    "الصباحية",
                    "الصديق",
                    "الصليبخات",
                    "الصليبية",
                    "الضجيج",
                    "الضهر",
                    "العارظية",
                    "العارضية الصناعية",
                    "العاصمه",
                    "العدان",
                    "العديلية",
                    "العقيلة",
                    "العمرية",
                    "العيون",
                    "الفحيحيل",
                    "الفردوس",
                    "الفروانية",
                    "الفروانية",
                    "الفنطاس",
                    "الفنيطيس",
                    "الفيحاء",
                    "القبلة",
                    "القرين",
                    "القصر",
                    "القصور",
                    "القيروان - جنوب الدوحة",
                    "المسايل",
                    "المسيلة",
                    "المقوع ",
                    "المنصورية",
                    "المنقف",
                    "المهبولة",
                    "النزهة",
                    "النسيم",
                    "النعيم",
                    "النهضة",
                    "الهدية",
                    "الواحة",
                    "الوسطى",
                    "الوفرة السكنية",
                    "اليرموك",
                    "بنيد القار",
                    "بيان",
                    "تيماء",
                    "جابر الأحمد",
                    "جابر العلي",
                    "جليب الشيوخ",
                    "جنوب الوسطى",
                    "حطين",
                    "حولي",
                    "حولي",
                    "خيطان",
                    "دسمان",
                    "سعد العبدالله",
                    "سلوى",
                    "صباح الأحمد السكنية",
                    "صباح السالم",
                    "صباح الناصر",
                    "عباسية",
                    "عبدالله السالم",
                    "عبدالله المبارك - غرب الجليب",
                    "علي صباح السالم",
                    "غرناطة",
                    "فهد الأحمد",
                    "قادسية",
                    "قرطبة",
                    "كيفان",
                    "مبارك العبدالله",
                    "مبارك الكبير",
                    "مبارك الكبير",
                    "مدينة الكويت",
                    "مرقاب",
                    "مشرف",
                    "ميدان حولي",
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
    }
}