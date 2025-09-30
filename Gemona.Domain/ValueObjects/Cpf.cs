namespace Gemona.Domain.ValueObjects
{
    public class Cpf
    {
        public string Valor { get; private set; }

        public Cpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF não pode ser vazio", nameof(cpf));
            }

            var cpfLimpo = RemoverMascara(cpf);

            if (!ValidarCpf(cpfLimpo))
            {
                throw new ArgumentException("CPF invalido", nameof(cpf));
            }

            Valor = cpfLimpo;
        }

        private static string RemoverMascara(string cpf)
        {
            return cpf.Replace(".", "").Replace("-", "").Trim();
        }

        private static bool ValidarCpf(string cpf)
        {
            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            {
                return false;
            }

            // Verifica se todos os digitos sao iguais
            if (cpf.All(c => c == cpf[0]))
            {
                return false;
            }

            // Validação primeiro digito verificador
            var soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != digito1)
            {
                return false;
            }

            // Validação segundo digito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cpf[10].ToString()) == digito2;
        }


        public string FormatarCpf()
        {
            return $"{Valor.Substring(0, 3)}.{Valor.Substring(3, 3)}.{Valor.Substring(6, 3)}-{Valor.Substring(9, 2)}";
        }

        public override string ToString() => Valor;

        public override bool Equals(object? obj) => obj is Cpf cpf && Valor == cpf.Valor;
        public override int GetHashCode() => Valor.GetHashCode();
    }
}