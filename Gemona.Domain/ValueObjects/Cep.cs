namespace Gemona.Domain.ValueObjects
{
    public class Cep
    {
        public string Valor { get; private set; }

        public Cep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                throw new ArgumentException("CEP não pode ser vazio", nameof(cep));
            }

            var cepLimpo = RemoverMascara(cep);

            if (!ValidarCep(cepLimpo))
            {
                throw new ArgumentException("CEP inválido", nameof(cep));
            }
            Valor = cepLimpo;
        }

        private static string RemoverMascara(string cep)
        {
            return cep.Replace("-", "").Trim();
        }

        private static bool ValidarCep(string cep)
        {
            if (cep.Length != 8)
            {
                return false;
            }

            if (!cep.All(char.IsDigit))
            {
                return false;
            }

            if (cep.All(c => c == '0'))
            {
                return false;
            }

            return true;
        }

        public string FormatarCep()
        {
            return $"{Valor.Substring(0, 5)}-{Valor.Substring(5, 3)}";
        }

        public override string ToString() => Valor;

        public override bool Equals(object? obj) => obj is Cep cep && Valor == cep.Valor;
        public override int GetHashCode() => Valor.GetHashCode();
    }
}