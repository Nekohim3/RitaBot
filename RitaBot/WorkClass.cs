using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace RitaBot
{
    internal class WorkClass : NotificationObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                RaisePropertyChanged(() => Address);
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                RaisePropertyChanged(() => City);
            }
        }

        private string _shopCode;
        public string ShopCode
        {
            get => _shopCode;
            set
            {
                _shopCode = value;
                RaisePropertyChanged(() => ShopCode);
            }
        }

        private string _manager;
        public string Manager
        {
            get => _manager;
            set
            {
                _manager = value;
                RaisePropertyChanged(() => Manager);
            }
        }

        private List<string> _position;
        public List<string> Position
        {
            get => _position;
            set
            {
                _position = value;
                RaisePropertyChanged(() => Position);
            }
        }

        private DateTime _dateFrom;
        public DateTime DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                RaisePropertyChanged(() => DateFrom);
            }
        }

        private DateTime _dateTo;
        public DateTime DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                RaisePropertyChanged(() => DateTo);
            }
        }

        private bool _canRegister;
        public bool CanRegister
        {
            get => _canRegister;
            set
            {
                _canRegister = value;
                RaisePropertyChanged(() => CanRegister);
            }
        }
        
        public WorkClass(dynamic data)
        {
            Id       = data.Id;
            Address  = data.Shop.Address;
            City     = data.Shop.City.Name;
            ShopCode = data.Shop.Code;
            Manager  = data.Shop.ManagerName;
            Position = new List<string>(); //data.RequiredPositions)
            foreach (var x in data.RequiredPositions)
                Position.Add((string)x.Position.Name);

            DateFrom    = DateTime.Parse(data.TimeFrom.ToString().Replace('-', '.').Replace('T', ' '));
            DateTo      = DateTime.Parse(data.TimeTo.ToString().Replace('-', '.').Replace('T', ' '));
            CanRegister = !((bool)data.RegistrationAvailability.AlreadyRegistered);
        }

        public override string ToString()
        {
            return Position.Aggregate($"\n{Address}\n{DateFrom.ToShortDateString()} ({DateFrom.ToShortTimeString()} - {DateTo.ToShortTimeString()})\n", (current, x) => current + $"\t{x}\nhttps://auction.tdera.ru/#/registration/{Id}\n");
        }
    }
}
