using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pokedex.Model.Wrappers;

namespace Pokedex.Model
{
    public class Pokemon : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate
        {
        };
        void RaiseProperty([CallerMemberName]string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public Pokemon()
        {

        }

        private int _nationalDex;
        public int NationalDex
        {
            get { return _nationalDex; }
            set
            {
                if (value != _nationalDex)
                {
                    _nationalDex = value;
                    RaiseProperty();
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseName;
        public string JapaneseName
        {
            get { return _japaneseName; }
            set
            {
                if (value != _japaneseName)
                {
                    _japaneseName = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseTransliteration;
        public string JapaneseTransliteration
        {
            get { return _japaneseTransliteration; }
            set
            {
                if (value != _japaneseTransliteration)
                {
                    _japaneseTransliteration = value;
                    RaiseProperty();
                }
            }
        }

        private string _japaneseRomanizedName;
        public string JapaneseRomanizedName
        {
            get { return _japaneseRomanizedName; }
            set
            {
                if (value != _japaneseRomanizedName)
                {
                    _japaneseRomanizedName = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<Form> _forms;
        public ObservableCollection<Form> Forms
        {
            get { return _forms; }
            set
            {
                if (value != _forms)
                {
                    _forms = value;
                    RaiseProperty();
                }
            }
        }
        public Form PrimaryForm { get { return Forms.ElementAt(0); } }

        private bool _hasMega;
        public bool HasMega
        {
            get { return _hasMega; }
            set
            {
                if (value != _hasMega)
                {
                    _hasMega = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<MegaStonePicture> _megaStones;
        public ObservableCollection<MegaStonePicture> MegaStones
        {
            get { return _megaStones; }
            set
            {
                if (value != _megaStones)
                {
                    _megaStones = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<RegionNumber> _regionsAndNumbers;
        public ObservableCollection<RegionNumber> RegionsAndNumbers
        {
            get { return _regionsAndNumbers; }
            set
            {
                if (value != _regionsAndNumbers)
                {
                    _regionsAndNumbers = value;
                    RaiseProperty();
                }
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<string> _eggGroups;
        public ObservableCollection<string> EggGroups
        {
            get { return _eggGroups; }
            set
            {
                if (value != _eggGroups)
                {
                    _eggGroups = value;
                    RaiseProperty();
                }
            }
        }

        private int _hatchTime;
        public int HatchTime
        {
            get { return _hatchTime; }
            set
            {
                if (value != _hatchTime)
                {
                    _hatchTime = value;
                    RaiseProperty();
                }
            }
        }

        private int _experienceYield;
        public int ExperienceYield
        {
            get { return _experienceYield; }
            set
            {
                if (value != _experienceYield)
                {
                    _experienceYield = value;
                    RaiseProperty();
                }
            }
        }

        private double _genderCode;
        public double GenderCode
        {
            get { return _genderCode; }
            set
            {
                if (value != _genderCode)
                {
                    _genderCode = value;
                    RaiseProperty();
                }
            }
        }

        private int _catchRate;
        public int CatchRate
        {
            get { return _catchRate; }
            set
            {
                if (value != _catchRate)
                {
                    _catchRate = value;
                    RaiseProperty();
                }
            }
        }

        private Generation _introducedGeneration;
        public Generation IntroducedGeneration
        {
            get { return _introducedGeneration; }
            set
            {
                if (value != _introducedGeneration)
                {
                    _introducedGeneration = value;
                    RaiseProperty();
                }
            }
        }

        private int _baseFriendship;
        public int BaseFriendship
        {
            get { return _baseFriendship; }
            set
            {
                if (value != _baseFriendship)
                {
                    _baseFriendship = value;
                    RaiseProperty();
                }
            }
        }

        private ObservableCollection<EvolutionLink> _evolutions;
        public ObservableCollection<EvolutionLink> Evolutions
        {
            get { return _evolutions; }
            set
            {
                if (value != _evolutions)
                {
                    _evolutions = value;
                    RaiseProperty();
                }
            }
        }

        private string url;
        public string URL
        {
            get { return url; }
            set
            {
                if (value != url)
                {
                    url = value;
                    RaiseProperty();
                }
            }
        }

    }
}
