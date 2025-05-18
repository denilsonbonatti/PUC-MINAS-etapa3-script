using System;
using System.Collections.Generic;
using System.IO;

namespace AtendimentoChamado
{
    class Program
    {
        static List<string> log = new List<string>();
        static bool modoAutomatico = false;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("== Atendimento de Chamado ==");
            Console.WriteLine("** O arquivo com o LOG do atendimento será gerado na área de trabalho **");
            Console.WriteLine("Deseja executar em modo manual (interativo) ou automático (aleatório)?");
            Console.Write("Digite 'm' para manual ou 'a' para automático: ");

            while (true)
            {
                string escolha = Console.ReadLine()?.Trim().ToLower();
                if (escolha == "a")
                {
                    modoAutomatico = true;
                    log.Add("Modo: Automático");
                    break;
                }
                else if (escolha == "m")
                {
                    modoAutomatico = false;
                    log.Add("Modo: Manual");
                    break;
                }
                else
                {
                    Console.Write("Opção inválida. Digite 'm' para manual ou 'a' para automático: ");
                }
            }

            log.Add("== INÍCIO DO ATENDIMENTO ==");
            log.Add("Data/Hora: " + DateTime.Now);

            Console.WriteLine("\nChamado recebido via: WhatsApp, E-mail ou Telefone");
            log.Add("Chamado recebido via canal do usuário.");

            if (Perguntar("Foi encontrada resposta na Base de Conhecimento?"))
            {
                log.Add("Solução encontrada na base de conhecimento. Chamado encerrado.");
                FinalizarChamado();
                return;
            }

            CriarRequisicao();

            if (Perguntar("IA conseguiu resolver o chamado?"))
            {
                log.Add("IA solucionou o chamado.");
                NotificarUsuario("IA");
                FinalizarChamado();
                return;
            }

            if (TratarNivel(1))
            {
                NotificarUsuario("N1");
                FinalizarChamado();
                return;
            }

            if (TratarNivel(2))
            {
                NotificarUsuario("N2");
                FinalizarChamado();
                return;
            }

            if (TratarNivel(3))
            {
                NotificarUsuario("N3");
                FinalizarChamado();
                return;
            }

            log.Add("Encaminhado para o fabricante do produto.");
            if (Perguntar("Fabricante conseguiu entregar uma solução ou workaround?"))
            {
                log.Add("Fabricante entregou solução ou workaround.");
                NotificarUsuario("Fabricante");
            }
            else
            {
                log.Add("Fabricante não conseguiu resolver o problema.");
                log.Add("Chamado encerrado sem solução.");
            }

            FinalizarChamado();
        }

        static void CriarRequisicao()
        {
            Console.WriteLine("\nAtendente cria requisição e classifica o chamado.");
            log.Add("Requisição criada e classificada pelo atendente.");
        }

        static bool TratarNivel(int nivel)
        {
            Console.WriteLine($"\n== Encaminhado para o Nível {nivel} ==");
            log.Add($"Encaminhado para atendimento de Nível {nivel}.");
            return Perguntar($"Atendimento N{nivel} conseguiu resolver?");
        }

        static void NotificarUsuario(string origem)
        {
            log.Add($"Usuário foi notificado da solução ({origem}).");
            if (Perguntar("Usuário aceitou a solução e deu feedback?"))
            {
                log.Add("Usuário aceitou a solução. Chamado encerrado com sucesso.");
            }
            else
            {
                log.Add("Usuário não aceitou a solução.");
            }
        }

        static bool Perguntar(string pergunta)
        {
            if (modoAutomatico)
            {
                bool resultado = rnd.Next(0, 2) == 1;
                Console.WriteLine($"{pergunta} (automático): {(resultado ? "Sim" : "Não")}");
                log.Add($"{pergunta}: {(resultado ? "Sim" : "Não")} (automático)");
                return resultado;
            }
            else
            {
                while (true)
                {
                    Console.Write($"{pergunta} (s/n): ");
                    string resposta = Console.ReadLine()?.Trim().ToLower();
                    if (resposta == "s")
                    {
                        log.Add($"{pergunta}: Sim");
                        return true;
                    }
                    else if (resposta == "n")
                    {
                        log.Add($"{pergunta}: Não");
                        return false;
                    }
                    Console.WriteLine("Resposta inválida. Digite 's' para sim ou 'n' para não.");
                }
            }
        }

        static void FinalizarChamado()
        {
            log.Add("Data/Hora de encerramento: " + DateTime.Now);
            log.Add("== FIM DO ATENDIMENTO ==");

            // Define caminho da Área de Trabalho
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string nomeArquivo = $"log_chamado_{DateTime.Now:yyyyMMdd_HHmmss}.dat";
            string caminhoCompleto = Path.Combine(desktopPath, nomeArquivo);

            using (StreamWriter writer = new StreamWriter(caminhoCompleto))
            {
                foreach (string linha in log)
                {
                    writer.WriteLine(linha);
                }
            }

            Console.WriteLine($"\nLog do chamado salvo como: {caminhoCompleto}");

            if (modoAutomatico)
            {
                Console.WriteLine("\n== Caminho percorrido no modo automático ==");
                foreach (var linha in log)
                {
                    if (!linha.StartsWith("Data/Hora") && !linha.Contains(".dat"))
                    {
                        Console.WriteLine(linha);
                    }
                }
            }
        }
    }
}
