namespace Gemona.Domain.ValueObjects
{
    public class Cnpj
    {
        public string Valor { get; private set; }

        public Cnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new ArgumentException("CNPJ não pode ser vazio", nameof(cnpj));
            }

            var cnpjLimpo = RemoverMascara(cnpj);

            if (!ValidarCnpj(cnpjLimpo))
            {
                throw new ArgumentException("CNPJ invalido", nameof(cnpj));
            }

            Valor = cnpjLimpo;
        }

        private static string RemoverMascara(string cnpj)
        {
            return cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim(); //00.000.000/0000-00
        }

        private static bool ValidarCnpj(string cnpj)
        {
            if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
            {
                return false;
            }

            if (cnpj.All(c => c == cnpj[0]))
            {
                return false;
            }

            // primeiro digito verificador
            var mutiplicadores1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var soma = 0;

            for (int i = 0; i < 12; i++)
            {
                soma += int.Parse(cnpj[i].ToString()) * mutiplicadores1[i];
            }

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cnpj[12].ToString()) != digito1)
            {
                return false;
            }

            // segundo digito verificador
            var mutiplicadores2 = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;

            for (int i = 0; i < 13; i++)
            {
                soma += int.Parse(cnpj[i].ToString()) * mutiplicadores2[i];
            }

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cnpj[13].ToString()) == digito2;
        }

        public string FormatarCnpj()
        {
            return $"{Valor.Substring(0, 2)}.{Valor.Substring(2, 3)}.{Valor.Substring(5, 3)}/{Valor.Substring(8, 4)}-{Valor.Substring(12, 2)}";
        }

        public override string ToString() => Valor;

        public override bool Equals(object? obj) => obj is Cnpj cnpj && Valor == cnpj.Valor;
        public override int GetHashCode() => Valor.GetHashCode();
    }
}