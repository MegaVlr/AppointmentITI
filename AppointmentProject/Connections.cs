using System;

namespace cfapp
{
    internal static class Connections
    {
        public const string sqlConstr = @"
            Server= o6ocyRVFE;
            Database= AppointmentDb;
            Integrated Security=True;
            TrustServerCertificate= True;
        ";
    }
}
