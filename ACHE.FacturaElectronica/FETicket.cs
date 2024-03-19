using System;

namespace ACHE.FacturaElectronica
{
    public struct FETicket
    {
        private long _cuit;
        private string _sign;
        private string _token;
        private DateTime _creado;
        private DateTime _vencimiento;
        private string _servicio;
        private uint _uniqueID;

        public FETicket(long cuit, string sign, string token, DateTime creado, DateTime vencimiento, string servicio, uint uniqueID)
        {
            _cuit = cuit;
            _sign = sign;
            _token = token;
            _vencimiento = vencimiento;
            _creado = creado;
            _servicio = servicio;
            _uniqueID = uniqueID;
        }

        public DateTime Creado
        {
            get { return _creado; }
            set { _creado = value; }
        }

        public string Servicio
        {
            get { return _servicio; }
            set { _servicio = value; }
        }

        public uint UniqueID
        {
            get { return _uniqueID; }
            set { _uniqueID = value; }
        }

        public long Cuit
        {
            get { return _cuit; }
            set { _cuit = value; }
        }

        public DateTime Vencimiento
        {
            get { return _vencimiento; }
            set { _vencimiento = value; }
        }

        public string Sign
        {
            get { return _sign; }
            set { _sign = value; }
        }

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
    }
}