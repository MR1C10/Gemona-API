namespace Gemona.Application.Helpers
{
    public static class GeoHelper
    {
        /// <summary>
        /// Calcula a distância entre dois pontos geográficos usando a fórmula de Haversine
        /// </summary>
        /// <param name="lat1">Latitude do ponto 1</param>
        /// <param name="lon1">Longitude do ponto 1</param>
        /// <param name="lat2">Latitude do ponto 2</param>
        /// <param name="lon2">Longitude do ponto 2</param>
        /// <returns>Distância em quilômetros</returns>
        public static double CalcularDistancia(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371; // Raio da Terra em km

            var dLat = GrausParaRadianos((double)(lat2 - lat1));
            var dLon = GrausParaRadianos((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(GrausParaRadianos((double)lat1)) * 
                    Math.Cos(GrausParaRadianos((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double GrausParaRadianos(double graus)
        {
            return graus * Math.PI / 180;
        }
    }
}
